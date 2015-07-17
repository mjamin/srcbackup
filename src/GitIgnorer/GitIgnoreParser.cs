using System;
using System.Text.RegularExpressions;

namespace GitIgnorer
{
    public class GitIgnoreParser : IGitIgnoreParser
    {
        private const string RegexPatternTrimWhitespace = @"^\s+|((?<!\\)\s)+$";

        public GitIgnoreParseResult Parse(string contents)
        {
            var result = new GitIgnoreParseResult();

            foreach (var line in contents.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                var pattern = ParseLine(line);
                if (pattern != null)
                {
                    result.Patterns.Add(pattern);
                }
            }

            return result;
        }

        private GitIgnorePattern ParseLine(string line)
        {
            var flags = PatternFlags.None;

            if (String.IsNullOrEmpty(line))
            {
                return null;
            }

            line = Regex.Replace(line, RegexPatternTrimWhitespace, "")
                        .Replace("\\ ", " ");

            if (line[0] == '#')
            {
                return null;
            }
            
            if (line[0] == '\\' && line[1] == '#')
            {
                line = line.Substring(1, line.Length - 1);
            }
            else if (line[0] == '!')
            {
                flags = flags | PatternFlags.Negated;
                line = line.Substring(1, line.Length - 1);
            }
            else if (line[0] == '\\' && line[1] == '!')
            {
                line = line.Substring(1, line.Length - 1);
            }
            else if (line[0] == '/')
            {
                flags = flags | PatternFlags.Rooted;
                line = line.Substring(1, line.Length - 1);
            }
            else if (line.StartsWith("**/"))
            {
                flags = flags | PatternFlags.MatchInAllDirectories;
                line = line.Substring(3, line.Length - 3);
            }

            if (line[line.Length - 1] == '/')
            {
                flags = flags | PatternFlags.MatchDirectory;
                line = line.TrimEnd('/');
            }
            else if (line.EndsWith("/**"))
            {
                flags = flags | PatternFlags.MatchInsideDirectory;
                line = line.Substring(0, line.Length - 3);
            }
            else if (line.Contains("/**/"))
            {
                flags = flags | PatternFlags.MatchZeroOrMoreDirectories;
            }
            else if (!line.Contains("/"))
            {
                flags = flags | PatternFlags.MatchPathNameRelative;
            }
            else
            {
                flags = flags | PatternFlags.WildcardsDoNotMatchSlashes;
            }

            return new GitIgnorePattern(line, flags);
        }
    }
}
