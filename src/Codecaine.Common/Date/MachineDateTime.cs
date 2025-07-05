using Codecaine.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Date
{
    /// <summary>
    /// MachineDateTime provides the current date and time based on the system clock.
    /// </summary>
    public class MachineDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Now =>DateTime.Now;        

        public DateTime LocalTime => TimeZoneInfo.ConvertTimeFromUtc(UtcNow, GetTimeZone());

        public string TimeZone => "Asia/Kuala_Lumpur";

        private static TimeZoneInfo GetTimeZone()
        {
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "Singapore Standard Time"
        : "Asia/Kuala_Lumpur";

            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
    }
}
