using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.AiServices.HuggingFace
{
    public class HuggingFaceSetting
    {
        public const string DefaultSectionName = "HuggingFace";
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api-inference.huggingface.co";
        public int MaxTokens { get; set; } = 1000;
        public double Temperature { get; set; } = 0.7;
        public string Model { get; set; } = "text-embedding-ada-002";
    }
}
