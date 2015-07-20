using System;
using System.IO;
using System.IO.Abstractions;

namespace GitIgnorer
{
    public class GitIgnoreCompiler : IGitIgnoreCompiler
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGitIgnoreParser _parser;

        public GitIgnoreCompiler(IFileSystem fileSystem, IGitIgnoreParser parser)
        {
            _fileSystem = fileSystem;
            _parser = parser;
        }

        public GitIgnore Compile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
