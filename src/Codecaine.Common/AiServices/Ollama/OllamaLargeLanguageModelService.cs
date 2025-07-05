using Codecaine.Common.AiServices.Interfaces;
using Codecaine.Common.AiServices.Model;
using Codecaine.Common.Exceptions;
using Codecaine.Common.HttpServices;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Codecaine.Common.AiServices.Ollama
{
    public class OllamaLargeLanguageModelService : ILargeLanguageModelService
    {
        private readonly OllamaSetting _ollamaSetting;
        private readonly IHttpService _httpService;

        public OllamaLargeLanguageModelService(IOptions<OllamaSetting> ollamaSetting, IHttpService httpService)
        {
            _ollamaSetting = ollamaSetting.Value;
            _httpService = httpService;
        }

        public async Task<string> GenerateTextAsync(List<PromptMessage> messages)
        {
            var maxMessage = _ollamaSetting.MaxMessages;

            messages = messages.OrderBy(m => m.TimeStamp).ToList();

            if (messages.Count > maxMessage)
            {
                messages = messages.Take(maxMessage).ToList();
            }
            var request = new
            {
                model = _ollamaSetting.LargeLanguageModel,
                messages = messages.Select(m => new
                {
                    role = m.Role.ToString().ToLower(),
                    content = m.Content
                }).ToList(),
                stream = false // Ollama does not support streaming responses
            };
            var url = $"{_ollamaSetting.BaseUrl}/chat";
            // Send a POST request to the token endpoint with the prepared request content
            var response = await _httpService.PostJsonAsync(url, request);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonDocument>(json);
            if (result == null || !result.RootElement.TryGetProperty("message", out var messageElement))
            {
                throw new CommonLibraryException(new Primitives.Errors.Error("FailedToGenerateTextFromOllama", "Failed to generate text from Ollama."));
            }
            var message = messageElement.GetProperty("content").GetString();
            return message ?? "";
        }
    }
}
