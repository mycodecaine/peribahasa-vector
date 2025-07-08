using Codecaine.Common.Abstractions;

namespace Codecaine.PeribahasaVector.Presentation.WebApi.Context
{
    /// <summary>
    /// Provides a mock implementation of the <see cref="IRequestContext"/> interface for testing purposes.
    /// </summary>
    /// <remarks>This class simulates a request context by generating a new user identifier and returning a
    /// fixed user name. It is intended for use in unit tests or development environments where a real request context
    /// is not available.</remarks>
    public class MockRequestContext : IRequestContext
    {


        public MockRequestContext()
        {

        }

        public Guid UserId
        {
            get
            {

                return Guid.NewGuid();
            }
        }

        public string UserName
        {
            get
            {
                return "TestUser@codecaine.my";
            }
        }
    }
}
