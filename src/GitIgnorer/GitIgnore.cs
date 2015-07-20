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
            for (var i = _rootPath.Length; i <= path.Length - 1; i++)
            {
                var currentPath = path.Substring(_rootPath.Length, i - _rootPath.Length + 1);

                if ((i == path.Length - 1 || (path[i] == '\\' || path[i] == '/')) && pattern.Regex.Match(currentPath).Success)
                {
                    if (pattern.Target == PatternTarget.Directory)
                    {
                        if (_fileSystem.Directory.Exists(_fileSystem.Path.Combine(_rootPath, currentPath)))
                            return true;
                    }
                    else
                    {
                        return true;
                    }
                }

            }

            return false;
        }
    }
}
