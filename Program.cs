using System;
using System.Windows.Forms;

namespace ResourceExplorer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ArchiveViewer());
        }

        public static string ExtractPath(string folder)
        {
            string pathExtract = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Extracted");

            if (!string.IsNullOrEmpty(folder))
            {
                pathExtract = System.IO.Path.Combine(pathExtract, System.IO.Path.GetFileName(folder).Replace(".", "_"));
            }

            if (!System.IO.Directory.Exists(pathExtract))
            {
                System.IO.Directory.CreateDirectory(pathExtract);
            }

            return pathExtract;
        }

        public static string BinaryPath
        {
            get { return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Binary"); }
        }

        private static string _filePath;
        public static string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
    }
}
