using KnowledgeAssistant.Api.Tools;
using Microsoft.SemanticKernel;

namespace KnowledgeAssistant.Api.Services;

public static class KernelFactory
{
    public static Kernel CreateKernel(IServiceProvider services)
    {
        var builder = Kernel.CreateBuilder();

        // Configure timeout before adding Ollama
        builder.Services.ConfigureHttpClientDefaults(c =>
        {
            c.ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(5));
        });

        builder.AddOllamaChatCompletion(
            modelId: "phi3",
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
