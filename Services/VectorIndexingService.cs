using KnowledgeAssistant.Api.Domain;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Service to index documents into the vector store.
/// It gets documents, generates embeddings, and stores them in Qdrant.
/// The only place where documents, embeddings, and vector storage meet.
/// </summary>
public class VectorIndexingService(
    IKnowledgeRepository repository,
    EmbeddingService embeddingService,
    VectorStoreService vectorStore)
{
    private readonly IKnowledgeRepository _repository = repository;
    private readonly EmbeddingService _embeddingService = embeddingService;
    private readonly VectorStoreService _vectorStore = vectorStore;

    public async Task IndexAllAsync()
    {
        var documents = _repository.GetAll();

        // Group documents by collection name
        var documentsByCollection = documents.GroupBy(d => d.CollectionName);

        foreach (var collectionGroup in documentsByCollection)
        {
            var collectionName = collectionGroup.Key;
            var collectionDocs = collectionGroup.ToList();

            // Ensure collection exists with proper vector size
            var firstEmbedding = await _embeddingService.GenerateEmbeddingAsync(collectionDocs.First().Content);
            await _vectorStore.EnsureCollectionExistsAsync(collectionName, firstEmbedding.Length);

            // Index all documents in this collection
            foreach (var doc in collectionDocs)
            {
                var embedding = await _embeddingService.GenerateEmbeddingAsync(doc.Content);

                await _vectorStore.UpsertAsync(
                    collectionName,
                    doc.Id,
                    embedding,
                    new Dictionary<string, string>
                    {
                        ["title"] = doc.Title,
                        ["source"] = doc.Source
                    });
            }
        }
    }
}