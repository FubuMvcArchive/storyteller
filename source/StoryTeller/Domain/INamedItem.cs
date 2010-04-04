namespace StoryTeller.Domain
{
    public interface INamedItem
    {
        string Name { get; }
        TPath GetPath();
    }
}