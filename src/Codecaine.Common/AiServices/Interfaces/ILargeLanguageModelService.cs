using Codecaine.Common.AiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.AiServices.Interfaces
{
    public interface ILargeLanguageModelService
    {
        Task<string> GenerateTextAsync(List<PromptMessage> messages );
    }
}
