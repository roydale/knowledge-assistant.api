using KnowledgeAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeAssistant.Api.Controllers;

/// <summary>
/// Controller to manage knowledge documents.
/// </summary>
[ApiController]
[Route("documents")]
public class DocumentsController : ControllerBase
{
    private readonly IKnowledgeRepository _repository;

    public DocumentsController(IKnowledgeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repository.GetAll());
    }
}
