using Codecaine.Common.AiServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Domain
{
    public abstract class AggregateVectorRoot : AggregateRoot
    {
        protected AggregateVectorRoot()
        {
        }
        protected AggregateVectorRoot(Guid id) : base(id)
        {
        }

        public string Content { get; protected set; } = "";
        protected readonly List<float> _embedding = [];
        public IReadOnlyCollection<float> Embedding => _embedding.AsReadOnly();

        public async Task UpdateEmbeddingAsync(IEmbeddingService embeddingService)
        {
            if (string.IsNullOrWhiteSpace(Content))
                throw new InvalidOperationException("Content must not be empty to generate embedding.");

            var vector = await embeddingService.GetVectorAsync(Content);

            _embedding.Clear();
            _embedding.AddRange(vector);
        }

        /*
          
         Example SQL table creation for storing documents with embeddings:
         
        CREATE TABLE Documents (
                
                id UUID PRIMARY KEY,
                content TEXT,
                embedding VECTOR(1536)  -- OpenAI embedding size
            );
          
         
         */


    }
}
