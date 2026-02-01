using KnowledgeAssistant.Api.Tools;
using Microsoft.SemanticKernel;

namespace KnowledgeAssistant.Api.Services;

/// <summary>
/// Factory for creating and configuring Semantic Kernel instances.
/// </summary>
public static class KernelFactory
{
    public static Kernel CreateKernel(IServiceProvider services)
    {
        var builder = Kernel.CreateBuilder();

        // Configure Ollama client with extended timeout
        builder.Services.ConfigureHttpClientDefaults(c =>
        {
            c.ConfigureHttpClient(client =>
                client.Timeout = TimeSpan.FromMinutes(5));
        });

        // Add Ollama chat completion service
        builder.AddOllamaChatCompletion(
            modelId: "llama3.2",
            endpoint: new Uri("http://localhost:11434")
        );

        // Build kernel
        var kernel = builder.Build();

        // Register tools
        kernel.Plugins.AddFromObject(
            services.GetRequiredService<SearchDocumentsTool>(),
            "search");

        kernel.Plugins.AddFromObject(
            services.GetRequiredService<SummarizeTextTool>(),
            "summarize");

        return kernel;
    }
}
