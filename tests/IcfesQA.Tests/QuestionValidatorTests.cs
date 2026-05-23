using IcfesQA.Core.Models;
using IcfesQA.Core.Validators;
using NUnit.Framework;

namespace IcfesQA.Tests;

/// <summary>
/// Unit tests for QuestionValidator.
/// Each test covers exactly ONE rule to keep failures pinpointed.
/// </summary>
[TestFixture]
public class QuestionValidatorTests
{
    private QuestionValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new QuestionValidator();

    // ── Helper ──────────────────────────────────────────────────────────────
    private static Question ValidQuestion(int id = 1) => new()
    {
        Id = id,
        Subject = "Math",
        Text = "What is 2 + 2?",
        Options = new List<string> { "3", "4", "5", "6" },
        CorrectAnswerIndex = 1,
        Difficulty = "easy"
    };

    // ── Happy path ──────────────────────────────────────────────────────────

    [Test]
    public void Valid_question_passes_all_rules()
    {
        var result = _validator.Validate(ValidQuestion());
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    [TestCase("Math")]
    [TestCase("Spanish")]
    [TestCase("Science")]
    [TestCase("Social Studies")]
    [TestCase("English")]
    public void All_five_valid_subjects_are_accepted(string subject)
    {
        var q = ValidQuestion();
        q.Subject = subject;
        Assert.That(_validator.Validate(q).IsValid, Is.True);
    }

    [Test]
    [TestCase("easy")]
    [TestCase("medium")]
    [TestCase("hard")]
    public void All_three_valid_difficulties_are_accepted(string difficulty)
    {
        var q = ValidQuestion();
        q.Difficulty = difficulty;
        Assert.That(_validator.Validate(q).IsValid, Is.True);
    }

    // ── Rule 1: Question text ────────────────────────────────────────────────

    [Test]
    public void Empty_text_produces_error()
    {
        var q = ValidQuestion();
        q.Text = "";
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("empty or whitespace"));
    }

    [Test]
    public void Whitespace_only_text_produces_error()
    {
        var q = ValidQuestion();
        q.Text = "   ";
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
    }

    // ── Rule 2: Minimum options ──────────────────────────────────────────────

    [Test]
    public void Single_option_produces_error()
    {
        var q = ValidQuestion();
        q.Options = new List<string> { "Only one" };
        q.CorrectAnswerIndex = 0;
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("at least 2 options"));
    }

    [Test]
    public void Null_options_produces_error()
    {
        var q = ValidQuestion();
        q.Options = null!;
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void Two_options_is_the_minimum_and_passes()
    {
        var q = ValidQuestion();
        q.Options = new List<string> { "Yes", "No" };
        q.CorrectAnswerIndex = 0;
        Assert.That(_validator.Validate(q).IsValid, Is.True);
    }

    // ── Rule 3: CorrectAnswerIndex bounds ────────────────────────────────────

    [Test]
    public void Negative_correct_index_produces_error()
    {
        var q = ValidQuestion();
        q.CorrectAnswerIndex = -1;
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("out of range"));
    }

    [Test]
    public void Index_equal_to_options_count_produces_error()
    {
        var q = ValidQuestion();
        q.CorrectAnswerIndex = q.Options.Count; // one past end
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("out of range"));
    }

    [Test]
    public void Last_valid_index_passes()
    {
        var q = ValidQuestion();
        q.CorrectAnswerIndex = q.Options.Count - 1;
        Assert.That(_validator.Validate(q).IsValid, Is.True);
    }

    // ── Rule 4: Duplicate options ────────────────────────────────────────────

    [Test]
    public void Duplicate_options_produce_error()
    {
        var q = ValidQuestion();
        q.Options = new List<string> { "Paris", "London", "Paris", "Berlin" };
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("Duplicate options"));
    }

    [Test]
    public void Case_insensitive_duplicates_are_detected()
    {
        var q = ValidQuestion();
        q.Options = new List<string> { "paris", "PARIS", "London" };
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
    }

    // ── Rule 5: Subject validation ───────────────────────────────────────────

    [Test]
    public void Unknown_subject_produces_error()
    {
        var q = ValidQuestion();
        q.Subject = "History";
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("Unknown subject"));
    }

    [Test]
    public void Empty_subject_produces_error()
    {
        var q = ValidQuestion();
        q.Subject = "";
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
    }

    // ── Rule 6: Difficulty validation ────────────────────────────────────────

    [Test]
    public void Unknown_difficulty_produces_error()
    {
        var q = ValidQuestion();
        q.Difficulty = "expert";
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Some.Contains("Unknown difficulty"));
    }

    // ── Multiple errors in one question ──────────────────────────────────────

    [Test]
    public void Multiple_violations_all_reported_at_once()
    {
        var q = new Question
        {
            Id = 99,
            Subject = "Unknown",
            Text = "",
            Options = new List<string> { "Only one" },
            CorrectAnswerIndex = 5,
            Difficulty = "godmode"
        };
        var result = _validator.Validate(q);
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Count, Is.GreaterThanOrEqualTo(4),
            "Should report text, options, index, subject, and difficulty errors.");
    }
}
