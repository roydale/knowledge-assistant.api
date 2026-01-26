namespace KnowledgeAssistant.Api.Models;

public record SearchRequest(string Query, int Limit = 3);