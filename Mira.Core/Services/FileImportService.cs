using System;
using System.IO;
using System.Windows.Forms;

namespace Mira.Core.Services
{
    /// <summary>
    /// Service for handling file import operations
    /// </summary>
    public interface IFileImportService
    {
        /// <summary>
        /// Imports a file from the user's file system to the reports directory
        /// </summary>
        /// <param name="reportType">The type of report being imported</param>
        /// <param name="destinationDirectory">The directory where the file should be copied</param>
        /// <returns>The name of the imported file, or null if the operation was cancelled</returns>
        string? ImportFile(Enums.ReportType reportType, string destinationDirectory);
    }

    /// <summary>
    /// Implementation of file import service
    /// </summary>
    public class FileImportService : IFileImportService
    {
        /// <summary>
        /// Imports a file from the user's file system to the specified directory
        /// </summary>
        /// <param name="reportType">The type of report being imported</param>
        /// <param name="destinationDirectory">The directory where the file should be copied</param>
        /// <returns>The name of the imported file, or null if the operation was cancelled</returns>
        public string? ImportFile(Enums.ReportType reportType, string destinationDirectory)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = Constants.SELECT_REPORT_FILE_TITLE;
                openFileDialog.Filter = Constants.PDF_FILTER;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return CopyFileToDestination(openFileDialog.FileName, reportType, destinationDirectory);
                }
            }

            return null;
        }

        /// <summary>
        /// Copies a file to the destination directory with a unique name
        /// </summary>
        private string CopyFileToDestination(string sourceFilePath, Enums.ReportType reportType, string destinationDirectory)
        {
            string fileExtension = Path.GetExtension(sourceFilePath);
            string timeStamp = DateTime.Now.ToString(Constants.TIMESTAMP_FORMAT);
            string destFileName = $"{reportType}_Report_{timeStamp}{fileExtension}";
            string destFilePath = Path.Combine(destinationDirectory, destFileName);

            File.Copy(sourceFilePath, destFilePath, overwrite: false);

            return destFileName;
        }
    }
}
