using System.Text.Json;
using IcfesQA.Core.Models;

namespace IcfesQA.Core.Services;

/// <summary>
/// Loads a question bank from a JSON file on disk.
/// </summary>
public class QuestionLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public List<Question> LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Question bank not found: {filePath}");

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Question>>(json, Options)
               ?? throw new InvalidDataException("JSON file is empty or malformed.");
    }
}
