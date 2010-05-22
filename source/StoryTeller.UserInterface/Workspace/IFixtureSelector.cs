using System.Collections.Generic;
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IFixtureSelector
    {
        IEnumerable<FixtureFilter> GetFilters();
        bool IsSelected();
        void Enable(bool enabled);
        
    }
}