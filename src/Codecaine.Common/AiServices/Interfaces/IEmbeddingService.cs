namespace Codecaine.Common.AiServices.Interfaces
{
    public interface IEmbeddingService
    {
        Task<List<float>> GetVectorAsync(string input);
    }
}
