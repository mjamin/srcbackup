using System.Collections.Generic;

namespace GitIgnorer
{
    internal class GitIgnoreParseResult
    {
        public IList<GitIgnorePattern> Patterns { get; private set; }

        internal GitIgnoreParseResult()
        {
            Patterns = new List<GitIgnorePattern>();
        }
    }
}
