using System;
using System.Linq;
using Xunit;

namespace GitIgnorer.Tests
{
    public class GitIgnoreParserTests
    {
        [Fact]
        void ShouldIgnoreBlankLines()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse(Environment.NewLine);

            // Assert
            Assert.True(!parsedPatterns.Any());
        }

        [Fact]
        void ShouldIgnoreLinesStartingWithHash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("#comment");

            // Assert
            Assert.True(!parsedPatterns.Any());
        }

        [Fact]
        void ShouldNotIgnoreLinesStartingWithEscapedHash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("\\#nocomment");

            // Assert
            Assert.Equal(parsedPatterns.Count(), 1);
            Assert.Equal("#nocomment", parsedPatterns.Single().Pattern);
        }

        [Fact]
        void ShouldTrimLines()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse(" foo ");

            // Assert
            Assert.Equal("foo", parsedPatterns.Single().Pattern);
        }

        [Fact]
        void ShouldNotTrimEscapedTrailingSpaces()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("foo\\ ");

            // Assert
            Assert.Equal("foo ", parsedPatterns.Single().Pattern);
        }

        [Fact]
        void ShouldNegatePatternsStartingWithExclamationMark()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("!foo");

            // Assert
            Assert.Equal(parsedPatterns.Single().Pattern, "foo");
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.Negated));
        }

        [Fact]
        void ShouldNotNegatePatternsStartingWithEscapedExclamationMark()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("\\!important!.txt");

            // Assert
            Assert.Equal("!important!.txt", parsedPatterns.Single().Pattern);
        }

        [Fact]
        void ShouldOnlyMatchDirectoriesIfPatternEndsWithSlash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("foo/");

            // Assert
            Assert.Equal("foo", parsedPatterns.Single().Pattern);
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.MatchDirectory));
            Assert.False(parsedPatterns.Single().Flags.HasFlag(PatternFlags.MatchFile));
        }

        [Fact]
        void ShouldNotMatchSlashesThroughWildcardsIfPatternContainsSlash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("foo/*bar");

            // Assert
            Assert.Equal("foo/*bar", parsedPatterns.Single().Pattern);
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.WildcardsDoNotMatchSlashes));
        }

        [Fact]
        void ShouldTreatLeadingSlashAsBeginningOfPath()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("/*.c");

            // Assert
            Assert.Equal("*.c", parsedPatterns.Single().Pattern);
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.Rooted));
        }

        [Fact]
        void ShouldTreatLeadingDoubleAsteriskFollowedBySlashAsMatchForAllDirectories()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("**/foo");

            // Assert
            Assert.Equal("foo", parsedPatterns.Single().Pattern);
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.MatchInAllDirectories));
        }

        [Fact]
        void ShouldTreatTrailingSlashFollowedByDoubleAsteriskAsMatchForAllFilesWithin()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("abc/**");

            // Assert
            Assert.Equal("abc", parsedPatterns.Single().Pattern);
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.MatchInsideDirectory));
        }

        [Fact]
        void ShouldTreatSlashFollowedByDoubleAsteriskFollowedBySlashAsMatchForZeroOrMoreDirectories()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var parsedPatterns = parser.Parse("a/**/b");

            // Assert
            Assert.Equal("a/**/b", parsedPatterns.Single().Pattern);
            Assert.True(parsedPatterns.Single().Flags.HasFlag(PatternFlags.MatchZeroOrMoreDirectories));
        }
    }
}
