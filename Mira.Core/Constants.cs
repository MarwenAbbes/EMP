using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mira.Core
{
    public abstract class Paths
    {
        public static string DataDirectory = Path.Combine(Environment.CurrentDirectory,"Data");
        public static string ReportsDirectory = Path.Combine(DataDirectory, "Reports");
    }

    public class Constants
    {
        public const string COMPARISON_PREFIX = "COMP";
        public const string TIMESTAMP_FORMAT = "yyyyMMdd_HHmmss";
        public const string PDF_FILTER = "PDF Files (*.pdf)|*.pdf";
        public const string SELECT_REPORT_FILE_TITLE = "Select Report File";
        public const string REPORT_IMPORT_SUCCESS_MESSAGE = "{0} report imported successfully!";
        public const string REPORT_IMPORT_SUCCESS_TITLE = "Success";
    }
}
