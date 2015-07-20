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
            var result = compiler.Compile(@"C:\.gitignore");

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
                compiler.Compile(@"C:\.gitignore");
            });
        }

        [Fact]
        public void ShouldCompileNegatedPatterns()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("!foo.txt")
                .WithFile(@"C:\foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.False(result.Ignores("C:\foo.txt"));
        }

        [Fact]
        public void ShouldCompileRootedPatterns()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("/foo.txt")
                .WithFiles(@"C:\bar\foo.txt", @"C:\foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\foo.txt"));
            Assert.False(result.Ignores(@"C:\bar\foo.txt"));
        }

        [Fact]
        public void ShouldIgnoreMatchedFiles()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("foo.txt")
                .WithFiles(@"C:\foo.txt", @"C:\bar\foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\foo.txt"));
            Assert.True(result.Ignores(@"C:\bar\foo.txt"));
        }

        [Fact]
        public void ShouldIgnoreMatchedDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("foo/")
                .WithDirectory(@"C:\foo")
                .WithFile(@"C:\bar\foo");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\foo"));
            Assert.False(result.Ignores(@"C:\bar\foo"));
        }

        [Fact]
        public void ShouldIgnoreAllMatchingRelativePathsIfPatternDoesNotContainSlash()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("foo")
                .WithFiles(@"C:\bar\baz\foo\barf.txt", @"C:\bar\foo.txt", @"C:\foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\bar\baz\foo\barf.txt"));
            Assert.True(result.Ignores(@"C:\bar\foo.txt"));
            Assert.True(result.Ignores(@"C:\foo.txt"));
        }

        [Fact]
        public void ShouldIgnoreMatchesInAllDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("**/foo/bar")
                .WithFiles(@"C:\baz\foo\bar", @"C:\foo\bar");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\baz\foo\bar"));
            Assert.True(result.Ignores(@"C:\foo\bar"));
        }

        [Fact]
        public void ShouldIgnoreEverythingInMatchedDirectory()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("abc/**")
                .WithFiles(@"C:\abc\foo\bar.txt", @"C:\abc\barf.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\abc\foo\bar.txt"));
            Assert.True(result.Ignores(@"C:\abc\barf.txt"));
        }

        [Fact]
        public void ShouldCompilePatternsFlaggedMatchZeroOrMoreDirectories()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("a/**/b")
                .WithFiles(@"C:\a\b", @"C:\a\x\b", @"C:\a\x\y\b");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\a\b"));
            Assert.True(result.Ignores(@"C:\a\x\b"));
            Assert.True(result.Ignores(@"C:\a\x\y\b"));
        }

        [Fact]
        public void ShouldApplyWildcardOnlyToFilenameIfPatternContainsSlash()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("Documentation/*.html")
                .WithFiles(@"C:\Documentation\git.html", @"C:\Documentation\ppc\ppc.html", @"C:\tools\perf\Documentation\perf.html");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\Documentation\git.html"));
            Assert.False(result.Ignores(@"C:\Documentation\ppc\ppc.html"));
            Assert.False(result.Ignores(@"C:\tools\perf\Documentation\perf.html"));
        }

        [Fact]
        public void ShouldMatchPatternsContainingAsteriskWildcard()
        {
            // Arrange
            var fileSystem = new MockFileSystem()
                .WithGitIgnoreContaining("*.txt")
                .WithFiles(@"C:\foo\bar\baz.txt", @"C:\foo\barf.txt", @"C:\foo.txt");

            var compiler = new GitIgnoreCompiler(fileSystem);

            // Act
            var result = compiler.Compile(@"C:\.gitignore");

            // Assert
            Assert.True(result.Ignores(@"C:\foo\bar\baz.txt"));
            Assert.True(result.Ignores(@"C:\foo\barf.txt"));
            Assert.True(result.Ignores(@"C:\foo.txt"));
        }
    }
}
