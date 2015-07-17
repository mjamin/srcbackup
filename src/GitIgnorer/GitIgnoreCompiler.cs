using System;

namespace GitIgnorer
{
    public class GitIgnoreCompiler : IGitIgnoreCompiler
    {
        private readonly IGitIgnoreParser _parser;

        public GitIgnoreCompiler(IGitIgnoreParser parser)
        {
            _parser = parser;
        }

        public GitIgnore Compile(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
