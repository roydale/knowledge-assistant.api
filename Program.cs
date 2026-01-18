using KnowledgeAssistant.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DocumentLoaderService>();
builder.Services.AddHttpClient<EmbeddingService>();
builder.Services.AddSingleton<VectorStoreService>();
builder.Services.AddSingleton<VectorIndexingService>();
builder.Services.AddSingleton<IKnowledgeRepository, KnowledgeRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var indexer = scope.ServiceProvider.GetRequiredService<VectorIndexingService>();
    await indexer.IndexAllAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
