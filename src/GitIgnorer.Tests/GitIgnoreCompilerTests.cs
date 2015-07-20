using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace GitIgnorer.Tests
{
    public class GitIgnoreCompilerTests
    {
        [Fact]
        public void ShouldParseAndCompileExistingFiles()
        {
            // Arrange
            var compiler = new GitIgnoreCompiler(new MockFileSystem().WithEmptyGitIgnore());

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ShouldThrowOnNonExistingFiles()
        {
            // Arrange
            var compiler = new GitIgnoreCompiler(new MockFileSystem());

            // Act / Assert
            Assert.Throws<FileNotFoundException>(() =>
            {
                compiler.Compile(".gitignore");
            });
        }

        [Fact]
        public void ShouldCompileNegatedPatterns()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("!foo.txt")
                .WithFile("/foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.False(result.Ignores("/foo.txt"));
        }

        [Fact]
        public void ShouldCompileRootedPatterns()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("/foo.txt")
                .WithFiles("/bar/foo.txt", "/foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/foo.txt"));
            Assert.False(result.Ignores("/bar/foo.txt"));
        }

        [Fact]
        public void ShouldIgnoreMatchedFiles()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/foo.txt"));
            Assert.True(result.Ignores("/bar/foo.txt"));
        }

        [Fact]
        public void ShouldIgnoreMatchedDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("foo/")
                .WithDirectory("/foo")
                .WithFile("/bar/foo");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/foo"));
            Assert.False(result.Ignores("/bar/foo"));
        }

        [Fact]
        public void ShouldIgnoreAllMatchingRelativePathsIfPatternDoesNotContainSlash()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("foo")
                .WithFiles("/bar/baz/foo/barf.txt", "/bar/foo.txt", "/foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/bar/baz/foo/barf.txt"));
            Assert.True(result.Ignores("/bar/foo.txt"));
            Assert.True(result.Ignores("/foo.txt"));
        }

        [Fact]
        public void ShouldIgnoreMatchesInAllDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("**/foo/bar")
                .WithFiles("/baz/foo/bar", "/foo/bar");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/baz/foo/bar"));
            Assert.True(result.Ignores("/foo/bar"));
        }

        [Fact]
        public void ShouldIgnoreEverythingInMatchedDirectory()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("abc/**")
                .WithFiles("/abc/foo/bar.txt", "/abc/barf.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/abc/foo/bar.txt"));
            Assert.True(result.Ignores("/abc/barf.txt"));
        }

        [Fact]
        public void ShouldCompilePatternsFlaggedMatchZeroOrMoreDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("a/**/b")
                .WithFiles("/a/b", "/a/x/b", "/a/x/y/b");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/a/b"));
            Assert.True(result.Ignores("/a/x/b"));
            Assert.True(result.Ignores("/a/x/y/b"));
        }

        [Fact]
        public void ShouldApplyWildcardOnlyToFilenameIfPatternContainsSlash()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("Documentation/*.html")
                .WithFiles("/Documentation/git.html", "/Documentation/ppc/ppc.html", "/tools/perf/Documentation/perf.html");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/Documentation/git.html"));
            Assert.False(result.Ignores("/Documentation/ppc/ppc.html"));
            Assert.False(result.Ignores("/tools/perf/Documentation/perf.html"));
        }

        [Fact]
        public void ShouldMatchPatternsContainingAsteriskWildcard()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("*.txt")
                .WithFiles("/foo/bar/baz.txt", "/foo/barf.txt", "/foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(".gitignore");

            // Assert
            Assert.True(result.Ignores("/foo/bar/baz.txt"));
            Assert.True(result.Ignores("/foo/barf.txt"));
            Assert.True(result.Ignores("/foo.txt"));
        }
    }
}
