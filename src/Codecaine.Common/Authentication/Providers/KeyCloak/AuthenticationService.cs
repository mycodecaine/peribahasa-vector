using Codecaine.Common.Authentication.Events;
using Codecaine.Common.Caching;
using Codecaine.Common.Errors;
using Codecaine.Common.Exceptions;
using Codecaine.Common.Primitives.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly.Timeout;

namespace Codecaine.Common.Authentication.Providers.KeyCloak
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ICacheService _cachingService;
        private readonly IMediator _mediator;
        private const string Authenticate = "authenticate-admin";
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IJwtService _jwtService;

        public AuthenticationService(IAuthenticationProvider authenticationProvider, ICacheService cachingService, IMediator mediator, ILogger<AuthenticationService> logger, IJwtService jwtService)
        {

            _authenticationProvider = authenticationProvider;
            _cachingService = cachingService;
            _mediator = mediator;
            _logger = logger;
            _jwtService = jwtService;
        }
        /// <summary>
        /// Get the admin access token
        /// </summary>
        /// <returns>Token</returns>
        protected async Task<Result<string>> GetAdminAccessToken()
        {
            var key = Authenticate;
            try
            {
                if (!_cachingService.CacheItemExists(key))
                {
                    var data = await _authenticationProvider.GetAdminAccessToken();
                    if (data.IsFailure)
                    {
                        return Result.Failure<string>(AuthenticationErrors.InvalidAdminToken);
                    }
                    _cachingService.SetCacheItem(key, data.Value);
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(data.Value);
                    if (tokenResponse == null)
                    {
                        return Result.Failure<string>(AuthenticationErrors.InvalidAdminToken);
                    }
                    return Result.Success<string>(tokenResponse.Token);
                }

                var adminTokenJson = _cachingService.GetCacheItem(key);
                var token = JsonConvert.DeserializeObject<TokenResponse>(adminTokenJson);

                if (token == null)
                {
                    return Result.Failure<string>(AuthenticationErrors.InvalidAdminToken);
                }

                if (DateTime.UtcNow < token.ExpiredIn)
                {

                    return Result.Success<string>(token.Token);
                }

                var newAdminToken = await _authenticationProvider.GetAdminAccessToken();

                if (newAdminToken.IsFailure)
                {
                    return Result.Failure<string>(AuthenticationErrors.InvalidAdminToken);
                }

                _cachingService.SetCacheItem(key, newAdminToken.Value);
                var newtoken = JsonConvert.DeserializeObject<TokenResponse>(newAdminToken.Value);

                if (newtoken == null)
                {
                    return Result.Failure<string>(AuthenticationErrors.InvalidAdminToken);
                }

                return Result.Success<string>(newtoken.Token);
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "GetAdminAccessTokenTimeoutRejectedException");

                throw new CommonLibraryException(new Primitives.Errors.Error("GetAdminAccessTokenTimeoutRejectedException", $"An error occurred while GetAdminAccessToken.TimeOut: {ex.Message}"));

            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "GetAdminAccessTokenException");
                throw new CommonLibraryException(new Primitives.Errors.Error("GetAdminAccessTokenException", $"An error occurred while GetAdminAccessToken: {ex.Message}"));

            }
        }
        /// <summary>
        /// Get the role id by role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        protected async Task<Result<string>> GetRoleIdByNameAsync(string roleName)
        {

            return await _authenticationProvider.GetRoleIdByNameAsync(roleName);
        }
        /// <summary>
        /// Base login method
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="refreshtoken"></param>
        /// <returns></returns>
        virtual protected async Task<Result<TokenResponse>> BaseLogin(string username, string password, string refreshtoken = "")
        {

            var token = "";
            if (string.IsNullOrEmpty(refreshtoken))
            {
                var loginWithUserName = await _authenticationProvider.Login(username, password);

                if (loginWithUserName.IsFailure)
                {
                    await _mediator.Publish(new UserLogedInProviderEvent(username, false, ""));
                    return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidUserNameOrPassword);
                }
                token = loginWithUserName.Value;
            }

            if (!string.IsNullOrEmpty(refreshtoken))
            {
                var loginWithRefreshToken = await _authenticationProvider.Login(refreshtoken);
                if (loginWithRefreshToken.IsFailure)
                {
                    await _mediator.Publish(new UserLogedInProviderEvent(username, false, ""));
                    return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidUserNameOrPassword);
                }
                token = loginWithRefreshToken.Value;

            }

            JObject content = JObject.Parse(token);

            if (content == null)
            {
                await _mediator.Publish(new UserLogedInProviderEvent(username, false, ""));
                return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidUserNameOrPassword);
            }

            var expiresIn = int.Parse(content["expires_in"]?.ToString());
            var refreshexpiresIn = int.Parse(content["refresh_expires_in"]?.ToString());
            var tokenExpired = DateTime.UtcNow.AddSeconds(expiresIn);
            var refreshtokenExpired = DateTime.UtcNow.AddSeconds(refreshexpiresIn);
            var accessToken = content["access_token"]?.ToString();
            var sub = _jwtService.GetSubId(accessToken);
            if (sub == Guid.Empty)
            {
                await _mediator.Publish(new UserLogedInProviderEvent(username, false, ""));
                return Result.Failure<TokenResponse>(AuthenticationErrors.SubIdNotExist);
            }
            var newtoken = new TokenResponse(accessToken, content["refresh_token"]?.ToString(), tokenExpired, refreshtokenExpired, sub);
            await _mediator.Publish(new UserLogedInProviderEvent(username, true, ""));
            return Result.Success<TokenResponse>(newtoken);
        }

        public async Task<Result<TokenResponse>> Login(string username, string password)
        {
            return await BaseLogin(username, password);
        }

        public async Task<Result<TokenResponse>> LoginFromRefreshToken(string refreshToken)
        {
            return await BaseLogin("", "", refreshToken);
        }

        public async Task<Result<TokenResponse>> CreateUser(string username, string email, string firstName, string lastName, string password)
        {

            var userNameVerify = await _authenticationProvider.IsUserNameExist(username);
            if (userNameVerify.IsSuccess && userNameVerify.Value)
            {
                return Result.Failure<TokenResponse>(AuthenticationErrors.UserNameAlreadyExist);
            }
            var accesstoken = await GetAdminAccessToken();
            if (accesstoken.IsFailure)
            {
                return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidAdminToken);
            }
           
           
            var response = await _authenticationProvider.CreateUser(username, email, firstName, lastName, password);

            if (response.IsSuccess)
            {
                return await Login(username, password);

            }
            return Result.Failure<TokenResponse>(AuthenticationErrors.InvalidUserNameOrPassword);
        }
        public async Task<Result<string>> GetIdByUserName(string userName)
        {

            try
            {
                var response = await _authenticationProvider.GetIdByUserName(userName);
                return response;
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "GetIdByUserNameTimeoutRejecte");             
                throw new CommonLibraryException(new Primitives.Errors.Error("GetIdByUserNameTimeoutRejectedException", $"An error occurred while GetIdByUserName.TimeOut: {ex.Message}"));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetIdByUserNameException");
                throw new CommonLibraryException(new Primitives.Errors.Error("GetIdByUserNameException", $"An error occurred while GetIdByUserName.Exception: {ex.Message}"));
            }
        }
        public async Task<Result<bool>> ResetPassword(string userName, string password)
        {            

            var response = await _authenticationProvider.ResetPassword(userName, password);
            return response;
        }
       
    }
}
