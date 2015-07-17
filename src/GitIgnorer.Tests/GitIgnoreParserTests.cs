using System;
using Xunit;

namespace GitIgnorer.Tests
{
    public class GitIgnoreParserTests
    {
        [Fact]
        void ShouldIgnoreBlankLines()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldIgnoreLinesStartingWithHash()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldNotIgnoreLinesStartingWithEscapedHash()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldNotTrimEscapedTrailingSpaces()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldNegatePatternsStartingWithExclamationMark()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldNegatePatternsStartingWithEscapedExclamationMark()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldOnlyMatchDirectoriesIfPatternEndsWithSlash()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldCheckForMatchInPathRelativeToGitIgnoreIfPatternDoesNotContainSlash()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldNotMatchSlashesThroughWildcardsIfPatternContainsSlash()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldTreatLeadingSlashAsBeginningOfPath()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldTreatLeadingDoubleAsteriskFollowedBySlashAsMatchForAllDirectories()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldTreatTrailingSlashFollowedByDoubleAsteriskAsMatchForAllFilesWithin()
        {
            throw new NotImplementedException();
        }

        [Fact]
        void ShouldTreatSlashFollowedByByDoubleAsteriskFollowedBySlashAsMatchForZeroOrMoreDirectories()
        {
            throw new NotImplementedException();
        }
    }
}
