namespace Codecaine.Common.Primitives.Errors
{
    public class InfrastructureError : Error
    {
    
        public InfrastructureError(string code, Exception message) : base($"InfrastructureError.{code}", message.ToJsonString())
        {
        }
    }
}
