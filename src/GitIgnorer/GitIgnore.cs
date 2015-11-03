using System.IO.Abstractions;
using System.Linq;
using System.Collections.Generic;
using System;

namespace GitIgnorer
{
    public class GitIgnore
    {
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>> _excludes;
        private readonly Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>> _includes;

        internal GitIgnore(IFileSystem fileSystem, Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>> excludes, Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>> includes)
        {
            _fileSystem = fileSystem;
            _excludes = excludes;
            _includes = includes;
        }

        public bool Ignores(string path)
        {
            if (!_excludes.Any())
                return false;

            foreach(var exclude in _excludes)
            {
                if (exclude.Value.Any(e => Matches(e, exclude.Key, path)))
                {
                    foreach(var include in _includes)
                    {
                        if (include.Value.Any(i => Matches(i, include.Key, path)))
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public static GitIgnore operator +(GitIgnore c1, GitIgnore c2)
        {
            var excludes = new Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>>(c1._excludes);
            foreach(var e in c2._excludes)
            {
                excludes.Add(e.Key, e.Value);
            }

            var includes = new Dictionary<string, IEnumerable<GitIgnoreCompiledPattern>>(c1._includes);
            foreach (var e in c2._includes)
            {
                includes.Add(e.Key, e.Value);
            }

            if(c1._fileSystem != c2._fileSystem)
            {
                throw new NotSupportedException();
            }

            return new GitIgnore(c1._fileSystem, excludes, includes);
        }

        private bool Matches(GitIgnoreCompiledPattern pattern, string rootPath, string path)
        {
            for (var i = rootPath.Length; i <= path.Length - 1; i++)
            {
                var currentPath = path.Substring(rootPath.Length, i - rootPath.Length + 1);

                if ((i == path.Length - 1 || (path[i] == '\\' || path[i] == '/')) && pattern.Regex.Match(currentPath).Success)
                {
                    if (pattern.Target == PatternTarget.Directory)
                    {
                        if (_fileSystem.Directory.Exists(_fileSystem.Path.Combine(rootPath, currentPath)))
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
