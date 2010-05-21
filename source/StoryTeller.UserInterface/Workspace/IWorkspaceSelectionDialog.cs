using System;
using StoryTeller.UserInterface.Dialogs;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceSelectionDialog : ICommandDialog
    {
        void Clear();
        void Add(string workspaceName, bool selected);
        Action OnSelection { set; }
        string[] GetSelections();
        void SelectAll();
    }
}