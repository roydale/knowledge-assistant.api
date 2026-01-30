using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace KnowledgeAssistant.Api.Tools;

/// <summary>
/// Tool that performs deterministic text summarization.
/// This demonstrates non-retrieval, non-infrastructure tool usage.
/// </summary>
public class SummarizeTextTool
{
    /// <summary>
    /// Summarizes the provided text into a shorter form.
    /// </summary>
    [KernelFunction]
    public string Summarize(
        [Description("The text to summarize")] string text,
        [Description("Maximum length of the summary")] int maxLength = 300)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "No text provided to summarize.";

        if (text.Length <= maxLength)
            return text;

        return text[..maxLength] + "...";
    }
}
