using System;
using System.Windows.Controls;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IWorkspaceSelectorDialog
    {
        
    }

    public class WorkspaceItem : StackPanel
    {
        private CheckBox _checkbox;
        private string _name;

        public WorkspaceItem(string name, bool selected)
        {
            this.Horizontal();
            _checkbox = this.Add<CheckBox>();
            this.Add<Label>(l => l.Content = name);
            _name = name;

            _checkbox.IsChecked = selected;
        }

        public string WorkspaceName
        {
            get { return _name; }
        }

        public bool Selected
        {
            get
            {
                return _checkbox.IsChecked.GetValueOrDefault(false);
            }
        }
    }
}