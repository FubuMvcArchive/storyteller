using System.Collections.Generic;
using StoryTeller.Model;
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IFixtureSelectorOrganizer
    {
        IEnumerable<IFixtureSelector> Organize(FixtureLibrary library, WorkspaceFilter workspace);
    }
}