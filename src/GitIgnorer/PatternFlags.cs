using System;

namespace GitIgnorer
{
    [Flags]
    internal enum PatternFlags
    {
        None = 0,
        Negated = 1,
        Rooted = 2,
        MatchFile = 4,
        MatchDirectory = 8,
        MatchInAllDirectories = 16,
        MatchInsideDirectory = 32,
        MatchZeroOrMoreDirectories = 64,
        WildcardsDoNotMatchSlashes = 128,
    }
}