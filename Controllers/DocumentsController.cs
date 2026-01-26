using KnowledgeAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeAssistant.Api.Controllers;

/// <summary>
/// Controller to manage knowledge documents.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DocumentsController(IKnowledgeRepository repository) : ControllerBase
{
    private readonly IKnowledgeRepository _repository = repository;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_repository.GetAll());
    }
}
