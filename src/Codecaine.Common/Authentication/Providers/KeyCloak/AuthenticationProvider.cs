using Codecaine.Common.Authentication.Providers.KeyCloak.Setting;
using Codecaine.Common.Errors;
using Codecaine.Common.HttpServices;
using Codecaine.Common.Primitives.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Codecaine.Common.Authentication.Providers.KeyCloak
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly AuthenticationSetting _authenticationSetting;
        private readonly IHttpService _httpService;
        private readonly ILogger<AuthenticationProvider> _logger;

        public AuthenticationProvider(IOptions<AuthenticationSetting> authenticationSetting, IHttpService httpService, ILogger<AuthenticationProvider> logger)
        {
            _authenticationSetting = authenticationSetting.Value;
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<Result<string>> CreateUser(string username, string email, string firstName, string lastName, string password)
        {
            _logger.LogInformation("Creating user with username: {Username}", username);
            var userEndpoint = $"{_authenticationSetting.BaseUrl}/admin/realms/{_authenticationSetting.RealmName}/users";
            var accesstoken = await GetAccessToken();
            if (accesstoken.IsFailure)
            {
                return Result.Failure<string>(AuthenticationErrors.InvalidAdminToken);
            }
            var admintoken = accesstoken.Value;

            var user = new
            {
                username = username,
                email = email,
                firstName = firstName,
                lastName = lastName,
                enabled = true,
                credentials = new List<object>
            {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false
                }
            }
            };
            var response = await _httpService.PostJsonAsync(userEndpoint, user, admintoken);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                return await Login(username, password);
            }

            return Result.Failure<string>(AuthenticationErrors.InvalidUserNameOrPassword);
        }

        public async Task<Result<string>> GetAdminAccessToken()
        {
            _logger.LogInformation("Getting admin access token");
            var adminToken = await Login(_authenticationSetting.Admin, _authenticationSetting.Password);
            return adminToken;
        }

        private async Task<Result<string>> GetAccessToken()
        {
            _logger.LogInformation("Getting admin access token");
            var adminToken = await Login(_authenticationSetting.Admin, _authenticationSetting.Password);
            JObject content = JObject.Parse(adminToken.Value);
            return content["access_token"]?.ToString();
        }

        public async Task<Result<string>> GetIdByUserName(string userName)
        {
            var userEndpoint = $"{_authenticationSetting.BaseUrl}/admin/realms/{_authenticationSetting.RealmName}/users?username={userName}";
            var accesstoken = await GetAccessToken();
            var response = await _httpService.GetAsync(userEndpoint, accesstoken.Value);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var users = JArray.Parse(responseContent);

                if (users.Count == 1)
                {
                    string userId = users[0]?["id"]?.ToString() ?? string.Empty;

                    if(string.IsNullOrEmpty(userId))
                    {
                        return Result.Failure<string>(AuthenticationErrors.UserNameNotExist);
                    }

                    return Result.Success<string>(userId);

                }
            }

            return Result.Failure<string>(AuthenticationErrors.UserNameNotExist);
        }

        public Task<Result<string>> GetRoleIdByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> IsUserNameExist(string userName)
        {
            var getId = await GetIdByUserName(userName);
            if (getId.IsSuccess)
            {
                return Result.Success<bool>(true);
            }

            return Result.Success<bool>(false);
        }

        public async Task<Result<string>> Login(string username, string password)
        {
            // Specify the token endpoint URL
            var tokenEndpoint = _authenticationSetting.TokenEndPoint;
            var formData = new List<KeyValuePair<string, string>>();

            formData.Add(new KeyValuePair<string, string>("client_id", _authenticationSetting.ClientId));
            formData.Add(new KeyValuePair<string, string>("client_secret", _authenticationSetting.ClientSecret));

            formData.Add(new KeyValuePair<string, string>("grant_type", "password"));
            formData.Add(new KeyValuePair<string, string>("username", username));
            formData.Add(new KeyValuePair<string, string>("password", password));

            // Send a POST request to the token endpoint with the prepared request content
            var response = await _httpService.PostAsync(tokenEndpoint, formData);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the access token from the response content
                var token = await response.Content.ReadAsStringAsync();

                return Result.Success<string>(token);
            }

            return Result.Failure<string>(AuthenticationErrors.InvalidUserNameOrPassword);
        }

        public async Task<Result<string>> Login(string refreshToken)
        {
            // Specify the token endpoint URL
            var tokenEndpoint = _authenticationSetting.TokenEndPoint;
            var formData = new List<KeyValuePair<string, string>>();

            formData.Add(new KeyValuePair<string, string>("client_id", _authenticationSetting.ClientId));
            formData.Add(new KeyValuePair<string, string>("client_secret", _authenticationSetting.ClientSecret));

            formData.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            formData.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));

            // Send a POST request to the token endpoint with the prepared request content
            var response = await _httpService.PostAsync(tokenEndpoint, formData);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the access token from the response content
                var token = await response.Content.ReadAsStringAsync();

                return Result.Success<string>(token);
            }

            return Result.Failure<string>(AuthenticationErrors.InvalidUserNameOrPassword);
        }

        public async Task<Result<bool>> ResetPassword(string userName, string password)
        {
            var userId = await GetIdByUserName(userName.Trim());

            if (userId.IsFailure)
            {
                return Result.Failure<bool>(AuthenticationErrors.UserNameNotExist);
            }

            var userEndpoint = $"{_authenticationSetting.BaseUrl}/admin/realms/{_authenticationSetting.RealmName}/users/{userId.Value}/reset-password";

            var requestData = new
            {
                type = "password",
                temporary = false,
                value = password
            };

            var response = await _httpService.PutJsonAsync(userEndpoint, requestData);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                return Result.Success<bool>(true);
            }
            return Result.Success<bool>(false);
        }
    }
}
