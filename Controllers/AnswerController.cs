using KnowledgeAssistant.Api.Models;
using KnowledgeAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace KnowledgeAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswerController(
    EmbeddingService embeddingService,
    VectorStoreService vectorStore,
    Kernel kernel) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Answer([FromBody] SearchRequest request)
    {
        var queryEmbedding = await embeddingService.GenerateEmbeddingAsync(request.Query);
        var results = await vectorStore.SearchAcrossCollectionsAsync(queryEmbedding, request.Limit);

        var context = string.Join(
            "\n",
            results.Select(r =>
                r.Payload["title"].StringValue + ":\n" +
                r.Payload["content"].StringValue[..Math.Min(1000, r.Payload["content"].StringValue.Length)])
        );

        var prompt = $"""
        Answer the question using only the information below.
        Do not mention file paths or metadata.

        Context:
        {context}

        Question:
        {request.Query}
        """;

        var response = await kernel.InvokePromptAsync(prompt);

        return Ok(response.ToString());
    }
}
