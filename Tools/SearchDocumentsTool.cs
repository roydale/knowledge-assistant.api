using KnowledgeAssistant.Api.Services;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text;

namespace KnowledgeAssistant.Api.Tools;

/// <summary>
/// Tool exposed to the LLM that allows semantic document retrieval.
/// This is a controlled, read-only capability.
/// </summary>
public class SearchDocumentsTool(EmbeddingService embeddingService, VectorStoreService vectorStore)
{
    private readonly EmbeddingService _embeddingService = embeddingService;
    private readonly VectorStoreService _vectorStore = vectorStore;

    /// <summary>
    /// Perform semantic search across all collections.
    /// The LLM may call this when it needs factual grounding.
    /// </summary>
    [KernelFunction]
    public async Task<string> SearchDocumentsAsync(
        [Description("The natural language search query")] string query,
        [Description("Maximum number of results to return")] int limit = 3)
    {
        Console.WriteLine($"[TOOL] SearchDocuments called with query: {query}");

        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query);
        var results = await _vectorStore.SearchAcrossCollectionsAsync(queryEmbedding, limit);

        if (!results.Any())
        {
            return "No relevant documents were found.";
        }

        var sb = new StringBuilder();

        foreach (var result in results)
        {
            var title = result.Payload["title"].StringValue;
            var source = result.Payload["source"].StringValue;

            sb.AppendLine($"Title: {title}");
            sb.AppendLine($"Source: {source}");
            sb.AppendLine($"Relevance Score: {result.Score:F3}");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
