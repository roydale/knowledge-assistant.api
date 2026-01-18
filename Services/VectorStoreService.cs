using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Service to manage vector storage and retrieval using Qdrant.
/// It creates the vector collection, stores embeddings, and performs similarity search.
/// It does not know about documents or embeddings generation.
/// </summary>
public class VectorStoreService
{
    private const string CollectionName = "knowledge-documents";
    private readonly QdrantClient _client;

    public VectorStoreService()
    {
        _client = new QdrantClient("localhost", port: 6334, https: false);
    }

    public async Task EnsureCollectionExistsAsync(int vectorSize)
    {
        var collections = await _client.ListCollectionsAsync();
        if (collections.Any(c => c == CollectionName))
            return;

        await _client.CreateCollectionAsync(
            collectionName: CollectionName,
            vectorsConfig: new VectorParams
            {
                Size = (uint)vectorSize,
                Distance = Distance.Cosine
            });
    }

    public async Task UpsertAsync(Guid id, float[] vector, Dictionary<string, string> metadata)
    {
        var point = new PointStruct { Id = id, Vectors = vector };

        foreach (var kvp in metadata)
        {
            point.Payload.Add(kvp.Key, kvp.Value);
        }

        await _client.UpsertAsync(CollectionName, [point]);
    }

    public async Task<IReadOnlyCollection<ScoredPoint>> SearchAsync(float[] vector, int limit)
    {
        return await _client.SearchAsync(
            collectionName: CollectionName,
            vector: vector,
            limit: (uint)limit);
    }
}
