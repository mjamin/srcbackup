using System.Collections.Generic;

namespace GitIgnorer
{
    public class GitIgnoreParseResult
    {
        public IEnumerable<GitIgnorePattern> Patterns { get; private set; }

        internal GitIgnoreParseResult()
        {
        }
    }
}
