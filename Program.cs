using KnowledgeAssistant.Api.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DocumentLoaderService>();
builder.Services.AddHttpClient<EmbeddingService>();
builder.Services.AddSingleton<VectorStoreService>();
builder.Services.AddSingleton<VectorIndexingService>();
builder.Services.AddSingleton<IKnowledgeRepository, KnowledgeRepository>();
builder.Services.AddSingleton(_ => KernelFactory.CreateKernel());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var indexer = scope.ServiceProvider.GetRequiredService<VectorIndexingService>();
    await indexer.IndexAllAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
