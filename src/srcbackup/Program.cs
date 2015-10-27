using System;
using System.Collections.Generic;
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
        private static FileSystem FileSystem = new FileSystem();
        private static GitIgnoreCompiler GitIgnoreCompiler = new GitIgnoreCompiler(FileSystem);

        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: srcbackup.exe <source-directory> <output-zip-file>");
                return;
            }

            var rootPath = args[0].TrimEnd('\\', '/') + '\\';
            var zipPath = args[1];
            var files = GetFiles(rootPath, null);

            var outputDirectory = Path.GetDirectoryName(zipPath);
            if(!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var n = CreateZipArchive(files, rootPath, zipPath);

            Console.WriteLine("{0} files written to {1}.", n, zipPath);
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
                : Directory.GetFiles(directory).Where(f => !gitIgnore.Ignores(f));

            foreach (var file in files)
            {
                yield return file;
            }

            var directories = gitIgnore == null
                ? Directory.GetDirectories(directory)
                : Directory.GetDirectories(directory).Where(d => !d.EndsWith(".git") && !gitIgnore.Ignores(d));

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

                    var entry = archive.CreateEntry(file.Replace(rootPath, ""));
                    using (var stream = entry.Open())
                    {
                        using (var dataStream = File.OpenRead(file))
                        {
                            dataStream.CopyTo(stream);
                        }
                    }

                    n++;
                }
            }

            return n;
        }
    }
}
