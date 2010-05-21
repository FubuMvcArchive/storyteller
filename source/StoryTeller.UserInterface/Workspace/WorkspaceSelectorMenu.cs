using System;
using System.Windows;
using System.Windows.Controls;
using FubuCore;
using System.Collections.Generic;

namespace StoryTeller.UserInterface.Workspace
{
    public class WorkspaceSelectorMenu : Button, IWorkspaceSelectorMenu
    {
        private Label _label;

        public WorkspaceSelectorMenu()
        {
            //Hide();
            Height = 30;
            MinWidth = 100;
            Content = "Workspaces";
        }

        public void ShowAllWorkspacesAreAvailable()
        {
            Content = "Workspaces";
            ToolTip = new ToolTip() {Content = "Click to hide/show workspaces for the current project"};
            Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {

        }

        public void ShowThatWorkspacesAreHidden(string[] workspaceNames)
        {
            Content = "Workspaces ({0} hidden)".ToFormat(workspaceNames.Length);
            ToolTip = new ToolTip() { Content = "Click to hide/show workspaces for the current project\n{0} are hidden.".ToFormat(workspaceNames.Join(", ")) };
            Visibility = System.Windows.Visibility.Visible;
        }

        public Action OnSelection
        {
            set { Click += value.ToRoutedHandler(); }
        }
    }
}