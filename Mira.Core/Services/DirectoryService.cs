using System;
using System.IO;

namespace Mira.Core.Services
{
    /// <summary>
    /// Service for handling directory operations
    /// </summary>
    public interface IDirectoryService
    {
        /// <summary>
        /// Ensures that required directories exist, creating them if necessary
        /// </summary>
        void EnsureDirectoriesExist(string comparisonId);
    }

    /// <summary>
    /// Implementation of directory service
    /// </summary>
    public class DirectoryService : IDirectoryService
    {
        /// <summary>
        /// Ensures that required directories exist, creating them if necessary
        /// </summary>
        public void EnsureDirectoriesExist(string comparisonId)
        {
            if (!Directory.Exists(Paths.DataDirectory))
            {
                Directory.CreateDirectory(Paths.DataDirectory);
            }

            string comparisonDirectory = Path.Combine(Paths.ReportsDirectory, comparisonId);
            if (!Directory.Exists(comparisonDirectory))
            {
                Directory.CreateDirectory(comparisonDirectory);
            }
        }
    }
}
