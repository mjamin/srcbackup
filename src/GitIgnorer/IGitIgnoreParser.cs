namespace GitIgnorer
{
    public interface IGitIgnoreParser
    {
        GitIgnoreParseResult Parse(string contents);
    }
}
