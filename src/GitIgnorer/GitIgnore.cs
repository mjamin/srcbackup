using System.IO.Abstractions;
using System.Linq;
using System.Collections.Generic;

namespace GitIgnorer
{
    public class GitIgnore
    {
        private readonly string _rootPath;
        private readonly IFileSystem _fileSystem;
        private readonly IEnumerable<GitIgnoreCompiledPattern> _excludes;
        private readonly IEnumerable<GitIgnoreCompiledPattern> _includes;

        internal GitIgnore(string rootPath, IFileSystem fileSystem, IEnumerable<GitIgnoreCompiledPattern> excludes, IEnumerable<GitIgnoreCompiledPattern> includes)
        {
            _rootPath = rootPath;
            _fileSystem = fileSystem;
            _excludes = excludes;
            _includes = includes;
        }

        public bool Ignores(string path)
        {
            if (!_excludes.Any())
                return false;

            return _excludes.Any(r => Matches(r, path)) && !_includes.Any(r => Matches(r, path));
        }

        private bool Matches(GitIgnoreCompiledPattern pattern, string path)
        {
            var relativePath = path.Replace(_rootPath, "");

            if (!pattern.Regex.Match(relativePath).Success)
                return false;

            return pattern.Target == PatternTarget.FileOrDirectory
                || pattern.Target == PatternTarget.File && _fileSystem.File.Exists(path)
                || pattern.Target == PatternTarget.Directory && _fileSystem.Directory.Exists(path);
        }
    }
}
