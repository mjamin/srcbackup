namespace GitIgnorer
{
    public interface IGitIgnoreCompiler
    {
        GitIgnore Compile(string fileName);
    }
}
