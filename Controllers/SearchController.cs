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

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(request.Query);
        var results = await _vectorStore.SearchAsync(queryEmbedding, request.Limit);

        var response = results.Select(r => new
        {
            r.Score,
            Title = r.Payload["title"].StringValue,
            Source = r.Payload["source"].StringValue
        });

        return Ok(response);
    }
}

public record SearchRequest(string Query, int Limit = 3);
