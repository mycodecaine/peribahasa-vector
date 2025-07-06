using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Authentication
{
    public interface IJwtService
    {
        Guid GetSubId(string token);
    }
}
