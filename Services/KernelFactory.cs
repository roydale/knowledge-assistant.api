using Microsoft.SemanticKernel;

namespace KnowledgeAssistant.Api.Services;

public static class KernelFactory
{
    public static Kernel CreateKernel()
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

        return builder.Build();
    }
}
