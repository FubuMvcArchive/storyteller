using System;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceSelectorMenu
    {
        void ShowAllWorkspacesAreAvailable();
        void Hide();
        void ShowThatWorkspacesAreHidden(string[] workspaceNames);
        Action OnSelection { set; }
    }
}