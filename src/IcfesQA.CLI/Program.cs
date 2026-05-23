using IcfesQA.Core.Services;

// ─── Entry point ────────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║   ICFES QA Automation Suite v1.0    ║");
Console.WriteLine("╚══════════════════════════════════════╝");
Console.ResetColor();
Console.WriteLine();

var filePath = args.Length > 0
    ? args[0]
    : Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "data", "questions.json");

Console.WriteLine($"Loading question bank: {filePath}");
Console.WriteLine();

try
{
    var loader    = new QuestionLoader();
    var generator = new ReportGenerator();

    var questions = loader.LoadFromFile(filePath);
    Console.WriteLine($"  Loaded {questions.Count} questions.");
    Console.WriteLine();

    var report = generator.Generate(questions);

    // ─── Summary ──────────────────────────────────────────────────────────
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("── Validation Report ───────────────────");
    Console.ResetColor();
    Console.WriteLine($"  Generated at : {report.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
    Console.WriteLine($"  Total        : {report.TotalQuestions}");

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  ✓ PASS       : {report.PassCount}  ({report.PassRate}%)");
    Console.ResetColor();

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  ✗ FAIL       : {report.FailCount}");
    Console.ResetColor();
    Console.WriteLine();

    // ─── Failures detail ──────────────────────────────────────────────────
    if (report.FailCount > 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("── Failed Questions ────────────────────");
        Console.ResetColor();

        foreach (var failure in report.Failures)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"  [Q{failure.QuestionId}] ");
            Console.ResetColor();
            Console.WriteLine($"FAIL — {failure.Errors.Count} error(s):");
            foreach (var error in failure.Errors)
                Console.WriteLine($"         • {error}");
        }

        Console.WriteLine();
    }

    Console.ForegroundColor = report.FailCount == 0 ? ConsoleColor.Green : ConsoleColor.Yellow;
    Console.WriteLine(report.FailCount == 0
        ? "All questions passed. Question bank is ready ✓"
        : $"Question bank has {report.FailCount} issue(s) to fix before deployment.");
    Console.ResetColor();

    return report.FailCount == 0 ? 0 : 1; // exit code for CI pipelines
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.ResetColor();
    return 2;
}
