using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mira.Core.Services;

namespace Mira.Core.DTO
{
    /// <summary>
    /// Data Transfer Object for Comparison data
    /// This class is responsible only for holding comparison data, not for file operations or UI logic
    /// </summary>
    public class ComparisonDto
    {
        private readonly string _id;
        private readonly string _baseReportDirectory;

        public string Id => _id;
        public string BaseReportDirectory => _baseReportDirectory;

        public string ProjectName { get; set; } = string.Empty;
        public string ResponsiblePerson { get; set; } = string.Empty;
        public DateTime ComparisonDate { get; set; }
        public string EmpPlanReference { get; set; } = string.Empty;
        public string ClientPlanReference { get; set; } = string.Empty;
        public string ClientPlantPath { get; set; } = string.Empty;
        public string EmpPlanPath { get; set; } = string.Empty;
        public bool ClientPlanLoaded { get; set; }
        public bool EmpPlanLoaded { get; set; }

        /// <summary>
        /// Creates a new ComparisonDto instance
        /// </summary>
        public ComparisonDto()
        {
            // Generate ID once and reuse it for both the ID and directory path
            _id = Utils.GetNextComparisonId();
            _baseReportDirectory = Path.Combine(Paths.ReportsDirectory, _id);

            // Initialize directory structure
            var logger = new LoggerService();
            var directoryService = new DirectoryService(logger);
            directoryService.EnsureDirectoriesExist(_id);
        }
    }
}
