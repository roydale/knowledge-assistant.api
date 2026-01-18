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
    private readonly QdrantClient _client;

    public VectorStoreService()
    {
        _client = new QdrantClient("localhost", port: 6334, https: false);
    }

    /// <summary>
    /// Ensure a collection exists with the specified vector size
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="vectorSize"></param>
    /// <returns></returns>
    public async Task EnsureCollectionExistsAsync(string collectionName, int vectorSize)
    {
        var collections = await _client.ListCollectionsAsync();
        if (collections.Any(c => c == collectionName))
            return;

        await _client.CreateCollectionAsync(
            collectionName: collectionName,
            vectorsConfig: new VectorParams
            {
                Size = (uint)vectorSize,
                Distance = Distance.Cosine
            });
    }

    /// <summary>
    /// Upsert a vector into a specific collection
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="id"></param>
    /// <param name="vector"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public async Task UpsertAsync(string collectionName, Guid id, float[] vector, Dictionary<string, string> metadata)
    {
        var point = new PointStruct { Id = id, Vectors = vector };

        foreach (var kvp in metadata)
        {
            point.Payload.Add(kvp.Key, kvp.Value);
        }

        await _client.UpsertAsync(collectionName, [point]);
    }

    /// <summary>
    /// Search in a specific collection
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="vector"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ScoredPoint>> SearchInCollectionAsync(string collectionName, float[] vector, int limit)
    {
        return await _client.SearchAsync(
            collectionName: collectionName,
            vector: vector,
            limit: (uint)limit);
    }

    /// <summary>
    /// Search across all collections
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<ScoredPoint>> SearchAcrossCollectionsAsync(float[] vector, int limit)
    {
        var collections = await _client.ListCollectionsAsync();
        var allResults = new List<ScoredPoint>();

        foreach (var collectionName in collections)
        {
            try
            {
                var results = await _client.SearchAsync(
                    collectionName: collectionName,
                    vector: vector,
                    limit: (uint)limit);

                allResults.AddRange(results);
            }
            catch (Exception)
            {
                // Skip collections that might have different vector dimensions or other issues
                continue;
            }
        }

        // Sort by score and take top N results
        return [.. allResults
            .OrderByDescending(r => r.Score)
            .Take(limit)];
    }

    /// <summary>
    /// Get all collection names
    /// </summary>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<string>> GetCollectionNamesAsync()
    {
        return await _client.ListCollectionsAsync();
    }
}