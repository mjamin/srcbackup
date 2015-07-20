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
        MatchPathNameRelative = 16,
        MatchInAllDirectories = 32,
        MatchInsideDirectory = 64,
        MatchZeroOrMoreDirectories = 128,
        WildcardsDoNotMatchSlashes = 256,
    }
}