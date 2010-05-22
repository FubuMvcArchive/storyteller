using System.Collections.Generic;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceEditorView
    {
        void ShowFixtureNamespaces(IEnumerable<IFixtureSelector> selectors);
    }
}