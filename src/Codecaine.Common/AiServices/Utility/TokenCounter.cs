using SharpToken;

namespace Codecaine.Common.OpenAiServices.Utility
{
    public static class TokenCounter
    {
        private const string encodingName = "cl100k_base";
        public static int CountTokens(string text)
        {
            var encoding = GptEncoding.GetEncoding(encodingName);
            var tokens = encoding.Encode(text);
            return tokens.Count;
        }

        public static int CountTokens(List<string> texts)
        {
            var encoding = GptEncoding.GetEncoding(encodingName);
            int total = 0;
            foreach (var text in texts)
            {
                total += encoding.Encode(text).Count;
            }
            return total;
        }
    }
}
