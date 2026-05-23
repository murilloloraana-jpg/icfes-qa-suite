namespace IcfesQA.Core.Models;

/// <summary>
/// Aggregated validation report for a full question bank run.
/// </summary>
public class ValidationReport
{
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
    public int TotalQuestions { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public List<ValidationResult> Results { get; set; } = new();

    public double PassRate => TotalQuestions == 0
        ? 0
        : Math.Round((double)PassCount / TotalQuestions * 100, 2);

    public IEnumerable<ValidationResult> Failures =>
        Results.Where(r => !r.IsValid);
}
