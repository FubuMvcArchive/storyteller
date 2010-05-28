using System.Collections.Generic;

namespace StoryTeller.Domain
{
    public interface INamedItem
    {
        string Name { get; }
        TPath GetPath();
        IEnumerable<Test> AllTests { get; }
    }
}