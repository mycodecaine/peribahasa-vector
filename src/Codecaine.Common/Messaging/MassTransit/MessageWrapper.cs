using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Messaging.MassTransit
{
    ///<summary>
    /// A wrapper class for messages to be published in a message queue using MassTransit.
    /// </summary>
    public class MessageWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWrapper"/> class.
        /// </summary>
        /// <param name="message"></param>
        public MessageWrapper(string message) { Message = message; }

        public string Message { get; private set; }
    }
}
