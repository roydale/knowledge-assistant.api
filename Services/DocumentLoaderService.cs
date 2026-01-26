using System.Security.Cryptography;
using System.Text;
using KnowledgeAssistant.Api.Domain;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Service responsible for reading raw files from disk
/// and turning them into KnowledgeDocument objects.
/// </summary>
public class DocumentLoaderService(IWebHostEnvironment env)
{
    private readonly string _documentsPath = Path.Combine(env.ContentRootPath, "Data", "Documents");

    public IReadOnlyCollection<KnowledgeDocument> LoadDocuments()
    {
        if (!Directory.Exists(_documentsPath))
            return [];

        // Recursively get all .md files in subfolders
        var files = Directory.GetFiles(_documentsPath, "*.md", SearchOption.AllDirectories);

        return [.. files.Select(file => new KnowledgeDocument
            {
                Id = GenerateId(file),
                Title = Path.GetFileNameWithoutExtension(file),
                Content = File.ReadAllText(file),
                Source = file,
                CollectionName = GetCollectionName(file)
            })];
    }

    private static string GetCollectionName(string filePath)
    {
        // Get the parent directory of the file
        var parentDir = Directory.GetParent(filePath);

        // Return the parent directory name, or a default if null
        return parentDir?.Name ?? "default-collection";
    }

    public static Guid GenerateId(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid([.. hash.Take(16)]);
    }
}