using IcfesQA.Core.Models;
using IcfesQA.Core.Validators;

namespace IcfesQA.Core.Services;

/// <summary>
/// Orchestrates validation over a full question bank and produces a report.
/// </summary>
public class ReportGenerator
{
    private readonly QuestionValidator _validator = new();
    private readonly DuplicateDetector _duplicateDetector = new();

    public ValidationReport Generate(IEnumerable<Question> questions)
    {
        var questionList = questions.ToList();
        var report = new ValidationReport { TotalQuestions = questionList.Count };

        // Per-question structural validation
        foreach (var question in questionList)
        {
            var result = _validator.Validate(question);
            report.Results.Add(result);

            if (result.IsValid) report.PassCount++;
            else report.FailCount++;
        }

        // Cross-question duplicate check — flag duplicates as additional failures
        var duplicateGroups = _duplicateDetector.FindDuplicates(questionList).ToList();
        foreach (var group in duplicateGroups)
        {
            foreach (var q in group)
            {
                var existing = report.Results.First(r => r.QuestionId == q.Id);
                var dupError = $"Duplicate question text shared with IDs: {string.Join(", ", group.Where(x => x.Id != q.Id).Select(x => x.Id))}.";

                if (existing.IsValid)
                {
                    // Was passing — now fails due to duplicate
                    report.PassCount--;
                    report.FailCount++;
                    existing.IsValid = false;
                }

                if (!existing.Errors.Contains(dupError))
                    existing.Errors.Add(dupError);
            }
        }

        return report;
    }
}
