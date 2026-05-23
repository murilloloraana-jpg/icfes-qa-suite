# ICFES QA Automation Suite

A C# test automation framework that validates ICFES exam question banks before deployment — catching structural errors, invalid metadata, and duplicate questions automatically.

Built as a portfolio project demonstrating **C# OOP**, **NUnit test automation**, and **quality engineering** practices aligned with game QA workflows.

---

## What it does

Given a JSON question bank, the suite:

| Check | Description |
|---|---|
| **Text validation** | Flags empty or whitespace-only question text |
| **Option count** | Requires at least 2 answer options |
| **Answer index bounds** | Verifies `correctAnswerIndex` points to a real option |
| **Duplicate options** | Detects repeated choices within one question (case-insensitive) |
| **Subject validation** | Enforces the 5 official ICFES subject areas |
| **Difficulty validation** | Accepts only `easy`, `medium`, or `hard` |
| **Cross-bank duplicates** | Detects questions with identical text across the whole bank |

Exit code `0` = all clear (CI-friendly). Exit code `1` = failures found.

---

## Project structure

```
IcfesQA/
├── src/
│   ├── IcfesQA.Core/          # Domain logic (models, validators, services)
│   │   ├── Models/            # Question, ValidationResult, ValidationReport
│   │   ├── Validators/        # QuestionValidator
│   │   └── Services/          # QuestionLoader, DuplicateDetector, ReportGenerator
│   └── IcfesQA.CLI/           # Console runner — accepts a JSON path argument
├── tests/
│   └── IcfesQA.Tests/         # NUnit test suites (25+ test cases)
├── data/
│   └── questions.json         # Sample bank (valid + intentionally broken questions)
└── .github/workflows/ci.yml   # GitHub Actions — build, test, CLI smoke test
```

---

## Getting started

**Requirements:** [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
# Clone
git clone https://github.com/<your-username>/icfes-qa-suite.git
cd icfes-qa-suite

# Run tests
dotnet test

# Validate the sample question bank
dotnet run --project src/IcfesQA.CLI -- data/questions.json

# Validate your own file
dotnet run --project src/IcfesQA.CLI -- path/to/your/questions.json
```

---

## Sample output

```
╔══════════════════════════════════════╗
║   ICFES QA Automation Suite v1.0    ║
╚══════════════════════════════════════╝

Loading question bank: data/questions.json
  Loaded 15 questions.

── Validation Report ───────────────────
  Generated at : 2025-06-15 14:32:01 UTC
  Total        : 15
  ✓ PASS       : 10  (66.67%)
  ✗ FAIL       : 5

── Failed Questions ────────────────────
  [Q9]  FAIL — 1 error(s):
         • Duplicate question text shared with IDs: 8.
  [Q10] FAIL — 1 error(s):
         • Unknown subject "Geography". Valid: Math, Spanish, Science, Social Studies, English.
  ...
```

---

## Architecture decisions

- **Separated concerns:** `QuestionValidator` handles per-question rules; `DuplicateDetector` handles cross-question analysis; `ReportGenerator` orchestrates both. Each class is independently testable.
- **Immutable result objects:** `ValidationResult` uses factory methods (`Pass`/`Fail`) to make state explicit.
- **CI-ready exit codes:** The CLI returns `0`/`1`/`2` so it integrates directly into GitHub Actions pipelines without extra tooling.
- **NUnit parameterized tests:** `[TestCase]` attributes reduce boilerplate while increasing coverage across all valid subjects and difficulties.

---

## Adding new validation rules

1. Add the check inside `QuestionValidator.Validate()` and append to `errors`.
2. Write at least one passing and one failing test in `QuestionValidatorTests.cs`.
3. Run `dotnet test` — CI will catch regressions automatically.

---

## Author

**Ana María Murillo Lora** — [github.com/murilloloraana-jpg](https://github.com/murilloloraana-jpg)  
Full Stack Developer · B.Eng. Multimedia Engineering (UAO, Cali)
