using IcfesQA.Core.Models;

namespace IcfesQA.Core.Validators;

/// <summary>
/// Validates individual questions against ICFES quality rules.
/// Rules:
///   1. Text must not be null or whitespace.
///   2. Must have at least 2 answer options.
///   3. CorrectAnswerIndex must point to a valid option.
///   4. No two options may be identical (case-insensitive).
///   5. Subject must be one of the 5 official ICFES areas.
///   6. Difficulty must be "easy", "medium", or "hard".
/// </summary>
public class QuestionValidator
{
    private static readonly HashSet<string> ValidSubjects = new(StringComparer.OrdinalIgnoreCase)
    {
        "Math", "Spanish", "Science", "Social Studies", "English"
    };

    private static readonly HashSet<string> ValidDifficulties = new(StringComparer.OrdinalIgnoreCase)
    {
        "easy", "medium", "hard"
    };

    public ValidationResult Validate(Question question)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(question.Text))
            errors.Add("Question text is empty or whitespace.");

        if (question.Options == null || question.Options.Count < 2)
            errors.Add($"Must have at least 2 options. Found: {question.Options?.Count ?? 0}.");

        if (question.Options != null)
        {
            if (question.CorrectAnswerIndex < 0 || question.CorrectAnswerIndex >= question.Options.Count)
                errors.Add($"CorrectAnswerIndex {question.CorrectAnswerIndex} is out of range (options: {question.Options.Count}).");

            var duplicates = question.Options
                .GroupBy(o => o.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
                errors.Add($"Duplicate options found: {string.Join(", ", duplicates.Select(d => $"\"{d}\""))}.");
        }

        if (!ValidSubjects.Contains(question.Subject))
            errors.Add($"Unknown subject \"{question.Subject}\". Valid: {string.Join(", ", ValidSubjects)}.");

        if (!ValidDifficulties.Contains(question.Difficulty))
            errors.Add($"Unknown difficulty \"{question.Difficulty}\". Valid: easy, medium, hard.");

        return errors.Count == 0
            ? ValidationResult.Pass(question.Id)
            : ValidationResult.Fail(question.Id, errors);
    }
}
