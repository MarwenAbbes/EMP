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

    internal class Constants
    {
    }
}
