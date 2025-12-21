using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mira.Core.Services;

public class CsvChatGptResponseExporter : IChatGptResponseExporter
{
    private readonly ILoggerService _logger;

    public CsvChatGptResponseExporter(ILoggerService logger)
    {
        _logger = logger;
    }

    public string SaveAsCsv(string chatResponseText, string fileNameWithoutExtension, string? outputDirectory)
    {
        var csv = ConvertChatGptMarkdownTablesToCsv(chatResponseText);
        var dir = string.IsNullOrWhiteSpace(outputDirectory) ? Directory.GetCurrentDirectory() : outputDirectory!;
        Directory.CreateDirectory(dir);
        var filePath = Path.Combine(dir, $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        File.WriteAllText(filePath, csv, Encoding.UTF8);
        _logger.LogInfo($"ChatGPT response saved as CSV: {filePath}");
        return filePath;
    }

    private string ConvertChatGptMarkdownTablesToCsv(string chatResponseText)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Section,Client,Fournisseur,Statut");

        using var reader = new StringReader(chatResponseText);
        string? line;
        string currentSection = string.Empty;
        bool inTable = false;
        bool headerSeen = false;

        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var sectionMatch = Regex.Match(line, "^\\s*(\\d+)\\)\\s*(.+)$");
            if (sectionMatch.Success)
            {
                currentSection = sectionMatch.Groups[2].Value.Trim();
                inTable = false;
                headerSeen = false;
                continue;
            }

            var trimmed = line.Trim();
            if (!trimmed.StartsWith("|"))
            {
                continue;
            }

            var parts = trimmed.Split('|').Select(p => p.Trim()).ToList();

            var isSeparator = parts.Any(p => Regex.IsMatch(p, "^-{3,}$"));
            if (isSeparator)
            {
                headerSeen = true;
                inTable = true;
                continue;
            }

            if (!headerSeen && parts.Count >= 4 && parts[1].IndexOf("Client", StringComparison.OrdinalIgnoreCase) >= 0 && parts[2].IndexOf("Fournisseur", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                continue;
            }

            if (inTable || headerSeen)
            {
                string client = parts.Count > 1 ? parts[1] : string.Empty;
                string fournisseur = parts.Count > 2 ? parts[2] : string.Empty;
                string statut = parts.Count > 3 ? parts[3] : string.Empty;

                if (string.IsNullOrWhiteSpace(client) && string.IsNullOrWhiteSpace(fournisseur) && string.IsNullOrWhiteSpace(statut))
                    continue;

                sb.AppendLine($"{EscapeCsv(currentSection)},{EscapeCsv(client)},{EscapeCsv(fournisseur)},{EscapeCsv(statut)}");
            }
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string field)
    {
        if (field == null) return string.Empty;
        var needsQuotes = field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.StartsWith(" ") || field.EndsWith(" ");
        var escaped = field.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{escaped}\"" : escaped;
    }
}
