using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mira.Core
{
    public class Utils
    {
        public static string GetNextComparisonId()
        {
            // Scan the baseDirectory/reports for existing folders starting with the comparison prefix and determine the next available ID
            
            // Get the reports directory
            if (!Directory.Exists(Paths.ReportsDirectory))
            {
                // If directory doesn't exist, return ID 1
                return $"{Constants.COMPARISON_PREFIX}-0001";
            }

            // Get all subdirectories in the reports directory
            var directories = Directory.GetDirectories(Paths.ReportsDirectory);

            // Filter directories that start with the comparison prefix
            var comparisonDirs = directories
                .Select(d => Path.GetFileName(d))
                .Where(name => name.StartsWith(Constants.COMPARISON_PREFIX))
                .ToList();

            // If no existing comparison directories, return ID 1
            if (comparisonDirs.Count == 0)
            {
                return $"{Constants.COMPARISON_PREFIX}-0001";
            }

            // Extract the numeric ID from each folder name and find the maximum
            var maxId = comparisonDirs
                .Select(name =>
                {
                    // Expected format: "COMP-0001"
                    var parts = name.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int id))
                    {
                        return id;
                    }
                    return 0;
                })
                .Max();

            // Return the next available ID
            return $"{Constants.COMPARISON_PREFIX}-{maxId + 1:D4}";
        }
    }
}
