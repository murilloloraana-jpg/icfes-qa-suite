using IcfesQA.Core.Models;

namespace IcfesQA.Core.Services;

/// <summary>
/// Detects questions whose text is identical or near-identical across the bank.
/// Uses exact match (normalized) and reports duplicate groups.
/// </summary>
public class DuplicateDetector
{
    /// <summary>
    /// Returns groups of questions that share the same normalized text.
    /// </summary>
    public IEnumerable<IGrouping<string, Question>> FindDuplicates(IEnumerable<Question> questions)
    {
        return questions
            .GroupBy(q => Normalize(q.Text))
            .Where(g => g.Count() > 1);
    }

    private static string Normalize(string text) =>
        text.Trim().ToLowerInvariant();
}
