using Codecaine.Common.AiServices.Interfaces;
using Codecaine.Common.AiServices.Model;
using Codecaine.Common.Exceptions;
using Codecaine.Common.HttpServices;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Codecaine.Common.AiServices.OpenAi
{
    public class OpenAiLargeLanguageModelService : ILargeLanguageModelService
    {
        private readonly OpenAiSetting _openAiSettings;
        private readonly IHttpService _httpService;

        public OpenAiLargeLanguageModelService(IOptions<OpenAiSetting> openAiSettings, IHttpService httpService)
        {
            _openAiSettings = openAiSettings.Value;
            _httpService = httpService;
        }

        public async Task<string> GenerateTextAsync(List<PromptMessage> messages)
        {
            var maxMessage = _openAiSettings.MaxMessages;

            messages = messages.OrderBy(m => m.TimeStamp).ToList();

            if (messages.Count > maxMessage)
            {
                messages = messages.Take(maxMessage).ToList();
            }
            var request = new
            {
                model = _openAiSettings.LargeLanguageModel,
                messages = messages.Select(m => new
                {
                    role = m.Role.ToString().ToLower(),
                    content = m.Content
                }).ToList()
            };
            var openAiUrl = $"{_openAiSettings.BaseUrl}/chat/completions";
            // Send a POST request to the token endpoint with the prepared request content
            var response = await _httpService.PostJsonAsync(openAiUrl, request, _openAiSettings.ApiKey);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonDocument>(json);
            if (result == null || !result.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                throw new CommonLibraryException(new Primitives.Errors.Error("FailedToGenerateTextFromOpenAI", "Failed to generate text from OpenAI API."));
            }
            var message = result.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return message??"";
        }
    }
}
