using System.Collections.Generic;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceEditorView
    {
        void ShowFixtureNamespaces(IEnumerable<IFixtureSelector> selectors);
        void ShowActionSelectors(IEnumerable<IStartupActionSelector> selectors);
        void ShowFixtureUsage(IEnumerable<FixtureGraph> fixtures);
    }
}