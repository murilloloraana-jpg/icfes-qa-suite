using IcfesQA.Core.Models;
using IcfesQA.Core.Services;
using NUnit.Framework;

namespace IcfesQA.Tests;

[TestFixture]
public class ReportGeneratorTests
{
    private ReportGenerator _generator = null!;

    [SetUp]
    public void SetUp() => _generator = new ReportGenerator();

    private static Question Valid(int id, string text = "Valid question text?") => new()
    {
        Id = id, Subject = "Math", Text = text,
        Options = new List<string> { "A", "B", "C", "D" },
        CorrectAnswerIndex = 0, Difficulty = "medium"
    };

    [Test]
    public void All_valid_questions_produce_100_percent_pass_rate()
    {
        var questions = new List<Question> { Valid(1), Valid(2), Valid(3) };
        var report = _generator.Generate(questions);

        Assert.That(report.PassRate, Is.EqualTo(100.0));
        Assert.That(report.FailCount, Is.EqualTo(0));
    }

    [Test]
    public void Failed_question_reduces_pass_count()
    {
        var bad = Valid(2);
        bad.Text = "";

        var questions = new List<Question> { Valid(1), bad, Valid(3) };
        var report = _generator.Generate(questions);

        Assert.That(report.PassCount, Is.EqualTo(2));
        Assert.That(report.FailCount, Is.EqualTo(1));
    }

    [Test]
    public void Report_total_matches_input_count()
    {
        var questions = Enumerable.Range(1, 10).Select(i => Valid(i)).ToList();
        var report = _generator.Generate(questions);
        Assert.That(report.TotalQuestions, Is.EqualTo(10));
    }

    [Test]
    public void Duplicate_questions_are_flagged_in_report()
    {
        var questions = new List<Question>
        {
            Valid(1, "Duplicate text"),
            Valid(2, "Duplicate text"),
            Valid(3, "Unique text")
        };
        var report = _generator.Generate(questions);

        Assert.That(report.FailCount, Is.EqualTo(2), "Both duplicates should fail.");
        Assert.That(report.PassCount, Is.EqualTo(1), "Unique question should still pass.");
    }

    [Test]
    public void Empty_bank_produces_zero_totals()
    {
        var report = _generator.Generate(new List<Question>());
        Assert.That(report.TotalQuestions, Is.EqualTo(0));
        Assert.That(report.PassRate, Is.EqualTo(0));
    }
}
