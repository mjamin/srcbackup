using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GitIgnorer
{
    internal class GitIgnoreParser
    {
        private const string RegexPatternTrimWhitespace = @"^\s+|((?<!\\)\s)+$";

        public IEnumerable<GitIgnorePattern> Parse(string contents)
        {
            return contents.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseLine)
                .Where(pattern => pattern != null);
        }

        private static GitIgnorePattern ParseLine(string line)
        {
            var flags = PatternFlags.MatchFile | PatternFlags.MatchDirectory;

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
                flags = flags & ~PatternFlags.MatchFile;
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
            else if (line.Contains("/"))
            {
                flags = flags | PatternFlags.WildcardsDoNotMatchSlashes;
            }

            return new GitIgnorePattern(line, flags);
        }
    }
}
