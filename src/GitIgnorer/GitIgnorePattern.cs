namespace GitIgnorer
{
    public class GitIgnorePattern
    {
        public string Pattern { get; private set; }
        public PatternFlags Flags { get; private set; }

        public GitIgnorePattern(string pattern, PatternFlags flags)
        {
            Pattern = pattern;
            Flags = flags;
        }
    }
}
