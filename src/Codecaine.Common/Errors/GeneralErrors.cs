using Codecaine.Common.Primitives.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Errors
{
    public static class GeneralErrors
    {
        /// <summary>
        /// Represents an error indicating that the server could not process the request.
        /// </summary>        
        public static Error UnProcessableRequest => new Error(
            "General.UnProcessableRequest",
            "The server could not process the request.");

        /// <summary>
        /// Represents an error indicating that the server encountered an unrecoverable error.
        /// </summary>        
        public static Error ServerError => new Error(
            "General.ServerError",
            "The server encountered an unrecoverable error.");
    }
   
}
