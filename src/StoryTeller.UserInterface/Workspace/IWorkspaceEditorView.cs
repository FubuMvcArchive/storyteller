using System.Collections.Generic;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceEditorView
    {
        void ShowFixtureNamespaces(IEnumerable<IFixtureSelector> selectors);
        void ShowFixtureUsage(IEnumerable<FixtureGraph> fixtures);
    }
}