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
        private readonly ILoggerService _logger;

        public DirectoryService(ILoggerService logger)
        {
            _logger = logger;
            _logger.LogInfo("Directory service initialized");
        }

        /// <summary>
        /// Ensures that required directories exist, creating them if necessary
        /// </summary>
        public void EnsureDirectoriesExist(string comparisonId)
        {
            _logger.LogInfo($"Ensuring directories exist for comparison: {comparisonId}");

            if (!Directory.Exists(Paths.DataDirectory))
            {
                _logger.LogInfo($"Creating data directory: {Paths.DataDirectory}");
                Directory.CreateDirectory(Paths.DataDirectory);
            }

            string comparisonDirectory = Path.Combine(Paths.ReportsDirectory, comparisonId);
            if (!Directory.Exists(comparisonDirectory))
            {
                _logger.LogInfo($"Creating comparison directory: {comparisonDirectory}");
                Directory.CreateDirectory(comparisonDirectory);
            }
            else
            {
                _logger.LogDebug($"Comparison directory already exists: {comparisonDirectory}");
            }
        }
    }
}
