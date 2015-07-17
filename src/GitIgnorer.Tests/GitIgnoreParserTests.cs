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
            var result = parser.Parse("\n");

            // Assert
            Assert.True(!result.Patterns.Any());
        }

        [Fact]
        void ShouldIgnoreLinesStartingWithHash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("#comment\n");

            // Assert
            Assert.True(!result.Patterns.Any());
        }

        [Fact]
        void ShouldNotIgnoreLinesStartingWithEscapedHash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("\\#nocomment\n");

            // Assert
            Assert.Equal(result.Patterns.Count(), 1);
        }

        [Fact]
        void ShouldTrimLines()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse(" foo ");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo");
        }

        [Fact]
        void ShouldNotTrimEscapedTrailingSpaces()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("foo\\ ");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo ");
        }

        [Fact]
        void ShouldNegatePatternsStartingWithExclamationMark()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("!foo");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.Negated));
        }

        [Fact]
        void ShouldNotNegatePatternsStartingWithEscapedExclamationMark()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("\\!important!.txt");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "!important!.txt");
        }

        [Fact]
        void ShouldOnlyMatchDirectoriesIfPatternEndsWithSlash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("foo/");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.MatchDirectory));
            Assert.False(result.Patterns.Single().Flags.HasFlag(PatternFlags.MatchFile));
        }

        [Fact]
        void ShouldCheckForMatchInPathRelativeToGitIgnoreIfPatternDoesNotContainSlash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("foo");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.MatchPathNameRelative));
        }

        [Fact]
        void ShouldNotMatchSlashesThroughWildcardsIfPatternContainsSlash()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("foo/*bar");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo/*bar");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.WildcardsDoNotMatchSlashes));
        }

        [Fact]
        void ShouldTreatLeadingSlashAsBeginningOfPath()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("/*.c");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "*.c");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.Rooted));
        }

        [Fact]
        void ShouldTreatLeadingDoubleAsteriskFollowedBySlashAsMatchForAllDirectories()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("**/foo");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "foo");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.MatchInAllDirectories));
        }

        [Fact]
        void ShouldTreatTrailingSlashFollowedByDoubleAsteriskAsMatchForAllFilesWithin()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("abc/**");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "abc");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.MatchInsideDirectory));
        }

        [Fact]
        void ShouldTreatSlashFollowedByDoubleAsteriskFollowedBySlashAsMatchForZeroOrMoreDirectories()
        {
            // Arrange
            var parser = new GitIgnoreParser();

            // Act
            var result = parser.Parse("a/**/b");

            // Assert
            Assert.Equal(result.Patterns.Single().Pattern, "a/**/b");
            Assert.True(result.Patterns.Single().Flags.HasFlag(PatternFlags.MatchZeroOrMoreDirectories));
        }
    }
}
