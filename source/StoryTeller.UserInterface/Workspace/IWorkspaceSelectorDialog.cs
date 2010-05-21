using System;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceSelectorDialog
    {
        void Clear();
        void Add(string workspaceName, bool selected);
        Action OnSelection { set; }
        string[] GetSelections();
    }
}