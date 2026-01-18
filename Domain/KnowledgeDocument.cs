namespace KnowledgeAssistant.Api.Domain;

/// <summary>
/// Represents a knowledge document within the system.
/// The "shape" of information your system understands.
/// It has no logic and no awareness of files, databases, or AI.
/// </summary>
public class KnowledgeDocument
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Title { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string CollectionName { get; set; } = string.Empty;
}
