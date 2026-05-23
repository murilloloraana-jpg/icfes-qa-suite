using IcfesQA.Core.Models;
using IcfesQA.Core.Services;
using NUnit.Framework;

namespace IcfesQA.Tests;

[TestFixture]
public class DuplicateDetectorTests
{
    private DuplicateDetector _detector = null!;

    [SetUp]
    public void SetUp() => _detector = new DuplicateDetector();

    private static Question Make(int id, string text) => new()
    {
        Id = id, Text = text,
        Subject = "Math", Difficulty = "easy",
        Options = new List<string> { "A", "B" }, CorrectAnswerIndex = 0
    };

    [Test]
    public void No_duplicates_returns_empty()
    {
        var questions = new List<Question>
        {
            Make(1, "What is 1+1?"),
            Make(2, "What is 2+2?"),
            Make(3, "What is 3+3?")
        };
        Assert.That(_detector.FindDuplicates(questions), Is.Empty);
    }

    [Test]
    public void Exact_duplicate_texts_are_detected()
    {
        var questions = new List<Question>
        {
            Make(1, "What is 2+2?"),
            Make(2, "What is 2+2?"),
            Make(3, "What is 3+3?")
        };
        var groups = _detector.FindDuplicates(questions).ToList();
        Assert.That(groups, Has.Count.EqualTo(1));
        Assert.That(groups[0].Count(), Is.EqualTo(2));
    }

    [Test]
    public void Duplicates_are_case_and_whitespace_insensitive()
    {
        var questions = new List<Question>
        {
            Make(1, "  What is 2+2?  "),
            Make(2, "what is 2+2?")
        };
        Assert.That(_detector.FindDuplicates(questions).Count(), Is.EqualTo(1));
    }

    [Test]
    public void Multiple_duplicate_groups_are_all_reported()
    {
        var questions = new List<Question>
        {
            Make(1, "What is 2+2?"),
            Make(2, "What is 2+2?"),
            Make(3, "Capital of Colombia?"),
            Make(4, "Capital of Colombia?")
        };
        Assert.That(_detector.FindDuplicates(questions).Count(), Is.EqualTo(2));
    }
}
