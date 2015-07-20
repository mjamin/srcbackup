using System.IO.Abstractions.TestingHelpers;

namespace GitIgnorer.Tests
{
    public static class MockFileSystemExtensions
    {
        public static MockFileSystem WithEmptyGitIgnore(this MockFileSystem mockFileSystem)
        {
            mockFileSystem.AddFile(@"C:\.gitignore", new MockFileData(""));
            return mockFileSystem;
        }

        public static MockFileSystem WithGitIgnoreContaining(this MockFileSystem mockFileSystem, string contents)
        {
            mockFileSystem.AddFile(@"C:\.gitignore", new MockFileData(contents));
            return mockFileSystem;
        }

        public static MockFileSystem WithDirectory(this MockFileSystem mockFileSystem, string path)
        {
            mockFileSystem.AddDirectory(mockFileSystem.Path.Combine(mockFileSystem.Directory.GetCurrentDirectory(), path));
            return mockFileSystem;
        }

        public static MockFileSystem WithFile(this MockFileSystem mockFileSystem, string path)
        {
            mockFileSystem.AddFile(mockFileSystem.Path.Combine(mockFileSystem.Directory.GetCurrentDirectory(), path), new MockFileData(""));
            return mockFileSystem;
        }

        public static MockFileSystem WithFiles(this MockFileSystem mockFileSystem, params string[] paths)
        {
            foreach (var path in paths)
            {
                mockFileSystem.AddFile(mockFileSystem.Path.Combine(mockFileSystem.Directory.GetCurrentDirectory(), path), new MockFileData(""));
            }

            return mockFileSystem;
        }
    }
}
