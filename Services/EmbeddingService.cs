using System.Net.Http.Json;
using System.Text.Json;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Service to generate text embeddings using an external API.
/// It does one thing only: text → numbers.
/// No documents.No Qdrant.No search.
/// </summary>
public class EmbeddingService
{
    private readonly HttpClient _httpClient;

    public EmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/embeddings",
            new
            {
                model = "nomic-embed-text",
                prompt = text
            });

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToArray();
    }
}
