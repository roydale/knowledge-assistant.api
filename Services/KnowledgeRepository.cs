using KnowledgeAssistant.Api.Domain;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Implementation of IKnowledgeRepository to access knowledge documents.
/// The bridge between your domain and your data source.
/// </summary>
public class KnowledgeRepository(DocumentLoaderService loader) : IKnowledgeRepository
{
    private readonly IReadOnlyCollection<KnowledgeDocument> _documents = loader.LoadDocuments();

    public IReadOnlyCollection<KnowledgeDocument> GetAll()
    {
        return _documents;
    }
}
