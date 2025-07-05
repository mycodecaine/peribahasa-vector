using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Persistence.MongoDB
{
    public class MongoDbSetting
    {
        public string ConnectionString { get; set; } 
        public string DatabaseName { get; set; }
        public bool IsStandalone { get; set; } 
    }
}
