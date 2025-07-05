using Codecaine.Common.Exceptions;
using Codecaine.Common.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.HttpServices
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpService> _logger;

        public HttpService(IHttpClientFactory httpClientFactory, ILogger<HttpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> PostAsync(string url, List<KeyValuePair<string, string>> content, string authToken = "")
        {
            var client = CreateClient(authToken);
            var requestContent = new FormUrlEncodedContent(content);
            return await client.PostAsync(url, requestContent);
        }

        public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T content, string authToken = "")
        {
            var client = CreateClient(authToken);
            return await client.PostAsJsonAsync(url, content);
        }

        public async Task<HttpResponseMessage> GetAsync(string url, string authToken = "")
        {
            var client = CreateClient(authToken);
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PutJsonAsync<T>(string url, T content, string authToken = "")
        {
            var client = CreateClient(authToken);
            return await client.PutAsJsonAsync(url, content);
        }

        /// <summary>
        /// Creates an HttpClient with the specified authorization token.
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        private HttpClient CreateClient(string authToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClientWithPolicy();
                if (!string.IsNullOrEmpty(authToken))
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
                }
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                throw new CommonLibraryException(new Primitives.Errors.Error("CreateHttpClientFail", $"An error occurred while creating the HTTP client: {ex.Message}"));

            }
        }
    }
}
