using KnowledgeAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeAssistant.Api.Controllers;

/// <summary>
/// Controller to handle search requests.
/// It embeds the user query, searches the vector store, and returns similar documents.
/// </summary>
[ApiController]
[Route("search")]
public class SearchController : ControllerBase
{
    private readonly EmbeddingService _embeddingService;
    private readonly VectorStoreService _vectorStore;

    public SearchController(
        EmbeddingService embeddingService,
        VectorStoreService vectorStore)
    {
        _embeddingService = embeddingService;
        _vectorStore = vectorStore;
    }

    /// <summary>
    /// Search across all collections
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(request.Query);
        var results = await _vectorStore.SearchAcrossCollectionsAsync(queryEmbedding, request.Limit);

        var response = results.Select(r => new
        {
            r.Score,
            Title = r.Payload["title"].StringValue,
            Source = r.Payload["source"].StringValue
        });

        return Ok(response);
    }

    /// <summary>
    /// Search within a specific collection
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("{collectionName}")]
    public async Task<IActionResult> SearchByCollection(string collectionName, [FromBody] SearchRequest request)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(request.Query);
        var results = await _vectorStore.SearchInCollectionAsync(collectionName, queryEmbedding, request.Limit);

        var response = results.Select(r => new
        {
            r.Score,
            Title = r.Payload["title"].StringValue,
            Source = r.Payload["source"].StringValue,
            Collection = collectionName
        });

        return Ok(response);
    }

    /// <summary>
    /// Get list of all available collections
    /// </summary>
    /// <returns></returns>
    [HttpGet("collections")]
    public async Task<IActionResult> GetCollections()
    {
        var collections = await _vectorStore.GetCollectionNamesAsync();
        return Ok(collections);
    }
}

public record SearchRequest(string Query, int Limit = 3);