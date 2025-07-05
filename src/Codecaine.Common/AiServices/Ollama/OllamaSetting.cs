using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.AiServices.Ollama
{
    public class OllamaSetting
    {
        public const string DefaultSectionName = "Ollama";       
        public string BaseUrl { get; set; } = "http://localhost:11434/api";
        public int MaxTokens { get; set; } = 1000;
        public double Temperature { get; set; } = 0.7;
        public string EmbeddingModel { get; set; } = "nomic-embed-text";
        public string LargeLanguageModel { get; set; } = "llama3-70b-chat"; // Default model for chat completions
        public int MaxMessages { get; set; } = 30; // Maximum number of messages to keep in context
    }
}
