using Codecaine.Common.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Authentication.Events
{
    public class UserLogedInProviderEvent : IEvent
    {
        public UserLogedInProviderEvent(string userName, bool isAuthenticated, string ip)
        {
            UserName = userName;
            IsAuthenticated = isAuthenticated;
            Ip = ip;
        }

        public string UserName { get; }
        public bool IsAuthenticated { get; }
        public string Ip { get; }
    }
}
