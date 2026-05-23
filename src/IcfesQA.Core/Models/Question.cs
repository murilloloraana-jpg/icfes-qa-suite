namespace IcfesQA.Core.Models;

/// <summary>
/// Represents a single ICFES exam question with its answer options.
/// </summary>
public class Question
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int CorrectAnswerIndex { get; set; }
    public string Difficulty { get; set; } = string.Empty; // "easy" | "medium" | "hard"
}
