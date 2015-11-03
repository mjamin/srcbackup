using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace GitIgnorer
{
    public class GitIgnoreCompiler : IGitIgnoreCompiler
    {
        private readonly IFileSystem _fileSystem;
        private readonly GitIgnoreParser _parser;

        public GitIgnoreCompiler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _parser = new GitIgnoreParser();
        }

        public GitIgnore Compile(string filePath)
        {
            var parsedPatterns = _parser.Parse(_fileSystem.File.ReadAllText(filePath));
            var excludes = new List<GitIgnoreCompiledPattern>();
            var includes = new List<GitIgnoreCompiledPattern>();

            foreach (var p in parsedPatterns)
            {
                var regex = PrepareRegex(p);
                var target = GetTarget(p);

                if (p.Flags.HasFlag(PatternFlags.MatchInAllDirectories))
                {
                    regex = ".*?" + regex;
                }

                if (p.Flags.HasFlag(PatternFlags.Rooted) || p.Flags.HasFlag(PatternFlags.WildcardsDoNotMatchSlashes))
                {
                    regex = "^" + regex;
                }

                regex = regex.Replace("\\*", p.Flags.HasFlag(PatternFlags.WildcardsDoNotMatchSlashes) ? "[^\\\\/]*?" : ".*?");

                var compiledPattern = new GitIgnoreCompiledPattern(new Regex(regex, RegexOptions.Compiled), target);
                if (p.Flags.HasFlag(PatternFlags.Negated))
                    includes.Add(compiledPattern);
                else
                    excludes.Add(compiledPattern);
            }

            var rootPath = _fileSystem.Path.GetDirectoryName(filePath).TrimEnd('\\', '/') + '\\';

            return new GitIgnore(_fileSystem,
                new Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>> { { rootPath, excludes } },
                new Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>> { { rootPath, includes } });
        }

        private static string PrepareRegex(GitIgnorePattern pattern)
        {
            return Regex.Escape(pattern.Pattern)
                .Replace("\\[", "[")
                .Replace("/\\*\\*/", "((/.*?/)|/)")
                .Replace("\\\\", "(\\\\|/)")
                .Replace("/", "(\\\\|/)")
                .Replace("\\?", ".");
        }

        private static PatternTarget GetTarget(GitIgnorePattern pattern)
        {
            return pattern.Flags.HasFlag(PatternFlags.MatchFile) && pattern.Flags.HasFlag(PatternFlags.MatchDirectory)
                    ? PatternTarget.FileOrDirectory
                    : pattern.Flags.HasFlag(PatternFlags.MatchFile) ? PatternTarget.File : PatternTarget.Directory;
        }
    }
}
