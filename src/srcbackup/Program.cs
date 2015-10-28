using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using GitIgnorer;
using SevenZip;

namespace srcbackup
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetDllDirectory(int bufsize, StringBuilder buf);

        private static readonly FileSystem FileSystem = new FileSystem();
        private static readonly GitIgnoreCompiler GitIgnoreCompiler = new GitIgnoreCompiler(FileSystem);

        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: srcbackup.exe <source-directory> <output-zip-file>");
                return;
            }

            InitializeSevenZip();

            var rootPath = args[0].TrimEnd('\\', '/') + '\\';
            var zipPath = args[1];
            var files = GetFiles(rootPath, null);

            var outputDirectory = Path.GetDirectoryName(zipPath);
            if(!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            Console.CursorVisible = false;
            var n = CreateZipArchive(files, rootPath, zipPath);
            Console.CursorVisible = true;

            WriteFullLine("{0} files written to {1}.", n, zipPath);
        }

        private static IEnumerable<string> GetFiles(string directory, GitIgnore gitIgnore)
        {
            var gitIgnorePath = Path.Combine(directory, ".gitignore");
            if (File.Exists(gitIgnorePath))
            {
                gitIgnore = gitIgnore == null
                    ? GitIgnoreCompiler.Compile(gitIgnorePath)
                    : gitIgnore + GitIgnoreCompiler.Compile(gitIgnorePath);
            }

            var files = gitIgnore == null
                ? Directory.GetFiles(directory)
                : Directory.GetFiles(directory).AsParallel().Where(f => !gitIgnore.Ignores(f)).ToArray();

            foreach (var file in files)
            {
                yield return file;
            }

            var directories = gitIgnore == null
                ? Directory.GetDirectories(directory)
                : Directory.GetDirectories(directory).AsParallel().Where(d => !d.EndsWith(".git") && !gitIgnore.Ignores(d)).ToArray();

            foreach (var file in directories.SelectMany(d => GetFiles(d, gitIgnore)))
            {
                yield return file;
            }
        }

        private static int CreateZipArchive(IEnumerable<string> files, string rootPath, string outputPath)
        {
            var n = 0;
            var compressor = new SevenZipCompressor { ArchiveFormat = OutArchiveFormat.SevenZip, CompressionLevel = CompressionLevel.Fast };
            compressor.FileCompressionStarted += (s, e) =>
            {
                WriteFullLine("[{0}%] {1}", e.PercentDone, e.FileName);
                n++;
            };
            compressor.CompressFiles(outputPath, rootPath.Length, files.ToArray());

            return n;
        }

        private static void InitializeSevenZip()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("7zLocation"))
                return;

            var path = new StringBuilder(255);
            GetDllDirectory(path.Capacity, path);
            ConfigurationManager.AppSettings["7zLocation"] = Path.Combine(path.ToString(), IntPtr.Size == 8 ? "64" : "32", "7z.dll");
        }

        private static void WriteFullLine(string format, params object[] args)
        {
            var cursorPosition = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("{0,-" + Console.WindowWidth + "}", String.Format(format, args));
            Console.SetCursorPosition(0, cursorPosition);
        }
    }
}
