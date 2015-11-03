using System.Text.RegularExpressions;

namespace GitIgnorer
{
    internal class GitIgnoreCompiledPattern
    {
        public Regex Regex { get; private set; }
        public PatternTarget Target { get; private set; }

        public GitIgnoreCompiledPattern(Regex regex, PatternTarget target)
        {
            Regex = regex;
            Target = target;
        }
    }
}
