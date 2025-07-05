using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Domain.Extensions
{
    public static class EnumExtension
    {
        public static bool IsValidEnumValue<T>(this T value)
            where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}
