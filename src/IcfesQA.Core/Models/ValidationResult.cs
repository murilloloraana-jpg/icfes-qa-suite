namespace IcfesQA.Core.Models;

/// <summary>
/// Holds the outcome of validating a single question.
/// </summary>
public class ValidationResult
{
    public int QuestionId { get; set; }
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ValidationResult Pass(int questionId) =>
        new() { QuestionId = questionId, IsValid = true };

    public static ValidationResult Fail(int questionId, IEnumerable<string> errors) =>
        new() { QuestionId = questionId, IsValid = false, Errors = errors.ToList() };
}
