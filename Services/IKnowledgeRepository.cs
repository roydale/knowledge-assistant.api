using KnowledgeAssistant.Api.Domain;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Defines methods for accessing knowledge documents.
/// </summary>
public interface IKnowledgeRepository
{
    IReadOnlyCollection<KnowledgeDocument> GetAll();
}
