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
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: srcbackup.exe <source-directory> <output-zip-file>");
                return;
            }

            var rootPath = args[0].TrimEnd('\\', '/') + '\\';
            var zipPath = args[1];

            var gitIgnore = new GitIgnoreCompiler(new FileSystem()).Compile(Path.Combine(rootPath, ".gitignore"));
            var files = GetFiles(rootPath, f => !gitIgnore.Ignores(f), d => !d.EndsWith(".git") && !gitIgnore.Ignores(d));

            CreateZipArchive(files, rootPath, zipPath);
        }

        private static IEnumerable<string> GetFiles(string path, Func<string, bool> filePredicate, Func<string, bool> directoryPredicate)
        {
            foreach (var file in Directory.GetFiles(path)
                                          .Where(filePredicate))
            {
                yield return file;
            }

            foreach (var file in Directory.GetDirectories(path)
                                          .Where(directoryPredicate)
                                          .SelectMany(directory => GetFiles(directory, filePredicate, directoryPredicate)))
            {
                yield return file;
            }
        }

        private static void CreateZipArchive(IEnumerable<string> files, string rootPath, string outputPath)
        {
            using (var archive = new ZipArchive(new FileStream(outputPath, FileMode.Create), ZipArchiveMode.Create, false, Encoding.GetEncoding(850)))
            {
                foreach (var file in files)
                {
                    var entry = archive.CreateEntry(file.Replace(rootPath, ""));
                    using (var stream = entry.Open())
                    {
                        using (var dataStream = File.OpenRead(file))
                        {
                            dataStream.CopyTo(stream);
                        }
                    }
                }
            }
        }
    }
}
