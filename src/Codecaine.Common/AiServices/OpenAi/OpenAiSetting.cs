namespace Codecaine.Common.AiServices.OpenAi
{
    public class OpenAiSetting
    {
        public const string DefaultSectionName = "OpenAi";
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
        public int MaxTokens { get; set; } = 1000;
        public int MaxMessages { get; set; } = 30; // Maximum number of messages to keep in context
        public double Temperature { get; set; } = 0.7;
        public string EmbeddingModel { get; set; } = "text-embedding-ada-002";
        public string LargeLanguageModel { get; set; } = "gpt-3.5-turbo"; // Default model for chat completions
    }
}
