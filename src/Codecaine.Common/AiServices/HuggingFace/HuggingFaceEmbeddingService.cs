using Codecaine.Common.AiServices.Interfaces;
using Codecaine.Common.Exceptions;
using Codecaine.Common.HttpServices;
using Codecaine.Common.OpenAiServices.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Codecaine.Common.AiServices.HuggingFace
{
    public class HuggingFaceEmbeddingService : IEmbeddingService
    {
        private readonly HuggingFaceSetting _settings;
        private readonly IHttpService _httpService;

        public HuggingFaceEmbeddingService(IOptions<HuggingFaceSetting> settings, IHttpService httpService)
        {
            _settings = settings.Value;
            _httpService = httpService;
        }
        public async Task<List<float>> GetVectorAsync(string input)
        {
            var tokenCount = TokenCounter.CountTokens(input);

            if (tokenCount > _settings.MaxTokens)
            {
                throw new CommonLibraryException(new Primitives.Errors.Error("InputTooLong", $"Input exceeds the maximum token limit of {_settings.MaxTokens}. Current token count: {tokenCount}."));
            }

            var request = new
            {
                inputs = input
            };

            var openAiUrl = $"{_settings.BaseUrl}/pipeline/feature-extraction/sentence-transformers/{_settings.Model}";


            // Send a POST request to the token endpoint with the prepared request content
            var response = await _httpService.PostJsonAsync(openAiUrl, request, _settings.ApiKey);
            response.EnsureSuccessStatusCode(); // Make sure 200 OK



            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<JsonDocument>(json);

            if (result == null || !result.RootElement.TryGetProperty("data", out var data) || data.GetArrayLength() == 0)
            {
                throw new CommonLibraryException(new Primitives.Errors.Error("FailedToRetrieveEmbbedingFromHuggingFace", "Failed to retrieve embedding from HuggingFace API."));
            }

            var embedding = result.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(e => e.GetSingle())
                .ToList();

            return embedding;
        }
    }
}
