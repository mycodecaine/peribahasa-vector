using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Caching.Settings
{
    /// <summary>
    /// CachingSetting represents the configuration settings for caching.
    /// </summary>
    public class CachingSetting
    {
        public const string DefaultSectionName = "Caching";
        public string ConnectionString { get; set; }
    }
}
