using Codecaine.Common.Primitives.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Exceptions
{
    public class CommonLibraryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfrastructureException"/> class with a specified <see cref="Error"/>.
        /// </summary>
        /// <param name="error">The infrastructure-specific error associated with the exception.</param>
        public CommonLibraryException(Error error)
            : base(error.Message)
            => Error = error;

        /// <summary>
        /// Gets the associated <see cref="Error"/> that describes the infrastructure failure.
        /// </summary>
        public Error Error { get; }
    }
}
