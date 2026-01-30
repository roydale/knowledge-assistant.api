using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;

namespace KnowledgeAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(Kernel kernel) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Run([FromBody] string message)
    {
        var prompt = """
        You are a grounded knowledge assistant.

        You have access to application tools.
        When a question cannot be answered without accessing stored documents or deterministic code, you MUST use the appropriate tool.

        Do not claim lack of access when tools are available.

        Tool usage rules:
        - Use tools only when required to answer the user's question.
        - Do not invent research plans or multi-step workflows.
        - Do not broaden or reinterpret the user's question.
        - Use the minimum number of tool calls necessary.
        - If a single tool call is sufficient, make exactly one call.

        Available tools:
        - search.SearchDocuments(query)
        Use when the user asks about documents, stored knowledge, or the contents of the knowledge base.

        - summarize.Summarize(text)
        Use only when the user explicitly asks for summarization or shortening.

        Never explain how you would use a tool.
        Either use it or answer directly.

        Question:
        """ + message;

        var result = await kernel.InvokePromptAsync(prompt);

        return Ok(result.ToString());
    }
}
