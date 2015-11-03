using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Text;
using GitIgnorer;

namespace srcbackup
{
    public class Program
    {
        private static readonly FileSystem FileSystem = new FileSystem();
        private static readonly GitIgnoreCompiler GitIgnoreCompiler = new GitIgnoreCompiler(FileSystem);

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: srcbackup.exe <source-directory> <output-zip-file> [-t]");
                return;
            }

            var rootPath = args[0].TrimEnd('\\', '/') + '\\';
            var zipPath = args[1];

            if (args.Length == 3 && args[2].Equals("-t", StringComparison.InvariantCultureIgnoreCase))
            {
                zipPath = zipPath.Insert(zipPath.Length - 4, DateTime.Now.ToString("_yyyyMMddHHmmss", CultureInfo.InvariantCulture));
            }

            var outputDirectory = Path.GetDirectoryName(zipPath);
            if(!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            Console.Write("Creating archive for all files in \"{0}\" not ignored by GIT", rootPath);

            Console.CursorVisible = false;
            var sw = Stopwatch.StartNew();
            var n = CreateZipArchive(GetFiles(rootPath, null), rootPath, zipPath);
            sw.Stop();
            Console.CursorVisible = true;

            Console.WriteLine("{0} files written to {1} after {2}.", n, zipPath, sw.Elapsed.ToString(@"mm\mss\s"));
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

            using (var archive = new ZipArchive(new FileStream(outputPath, FileMode.Create), ZipArchiveMode.Create, false, Encoding.GetEncoding(850)))
            {
                foreach (var file in files)
                {
                    if (file == outputPath)
                        continue;

                    var entry = archive.CreateEntry(file.Replace(rootPath, ""), CompressionLevel.Fastest);
                    using (var stream = entry.Open())
                    {
                        using (var dataStream = File.OpenRead(file))
                        {
                            dataStream.CopyTo(stream);
                        }
                    }

                    n++;

                    if (n % 1000 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine();

            return n;
        }
    }
}
