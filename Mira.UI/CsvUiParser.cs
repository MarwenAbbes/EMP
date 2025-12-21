using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mira.UI;

public static class CsvUiParser
{
    // Parses the CSV produced by the exporter into sections mapping to rows [Client,Fournisseur,Statut]
    public static Dictionary<string, List<string[]>> ParseCsvToSections(string csvPath)
    {
        if (string.IsNullOrWhiteSpace(csvPath)) throw new ArgumentNullException(nameof(csvPath));
        if (!File.Exists(csvPath)) throw new FileNotFoundException("CSV file not found", csvPath);

        var lines = File.ReadAllLines(csvPath, Encoding.UTF8);
        if (lines.Length <= 1) return new Dictionary<string, List<string[]>>();

        var dataBySection = new Dictionary<string, List<string[]>>();

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var fields = ParseCsvLine(line);
            if (fields.Length < 4) continue;
            var section = fields[0];
            if (!dataBySection.TryGetValue(section, out var list))
            {
                list = new List<string[]>();
                dataBySection[section] = list;
            }
            list.Add(new[] { fields[1], fields[2], fields[3] });
        }

        return dataBySection;
    }

    // Simple CSV parser handling quoted fields and escaped quotes
    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++; // skip escaped quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        fields.Add(sb.ToString());
        return fields.ToArray();
    }
}
