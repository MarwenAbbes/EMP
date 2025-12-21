using System;

namespace Mira.Core.Services;

public interface IChatGptResponseExporter
{
    /// <summary>
    /// Convert the raw ChatGPT response text into CSV and save to disk.
    /// Returns the full path to the saved file.
    /// </summary>
    string SaveAsCsv(string chatResponseText, string fileNameWithoutExtension, string? outputDirectory);
}
