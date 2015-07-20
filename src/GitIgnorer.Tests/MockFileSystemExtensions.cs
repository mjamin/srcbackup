using System.IO.Abstractions.TestingHelpers;

namespace GitIgnorer.Tests
{
    public static class MockFileSystemExtensions
    {
        public static MockFileSystem WithEmptyGitIgnore(this MockFileSystem mockFileSystem)
        {
            mockFileSystem.AddFile(".gitignore", new MockFileData(""));
            return mockFileSystem;
        }

        public static MockFileSystem WithGitIgnoreContaining(this MockFileSystem mockFileSystem, string contents)
        {
            mockFileSystem.AddFile(".gitignore", new MockFileData(contents));
            return mockFileSystem;
        }

        public static MockFileSystem WithDirectory(this MockFileSystem mockFileSystem, string path)
        {
            mockFileSystem.AddDirectory(path);
            return mockFileSystem;
        }

        public static MockFileSystem WithFile(this MockFileSystem mockFileSystem, string path)
        {
            mockFileSystem.AddFile(path, new MockFileData(""));
            return mockFileSystem;
        }

        public static MockFileSystem WithFiles(this MockFileSystem mockFileSystem, params string[] paths)
        {
            foreach (var path in paths)
            {
                mockFileSystem.AddFile(path, new MockFileData(""));
            }

            return mockFileSystem;
        }
    }
}
