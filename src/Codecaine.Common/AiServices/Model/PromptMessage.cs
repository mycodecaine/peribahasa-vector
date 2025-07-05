using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.AiServices.Model
{
    public class PromptMessage
    {
        public string Role { get;  } = "user"; // Default role is "user"
        public string Content { get;  } = string.Empty;

        public DateTime TimeStamp { get; }
        public PromptMessage() { }
        public PromptMessage(string role, string content)
        {
            Role = role;
            Content = content;
            TimeStamp = DateTime.UtcNow; // Set the timestamp to the current UTC time
        }
        public override string ToString()
        {
            return $"{Role}: {Content}";
        }
    }
}
