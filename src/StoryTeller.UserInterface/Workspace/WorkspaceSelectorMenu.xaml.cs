using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FubuCore;

namespace StoryTeller.UserInterface.Workspace
{
    /// <summary>
    /// Interaction logic for WorkspaceSelectorMenu.xaml
    /// </summary>
    public partial class WorkspaceSelectorMenu : UserControl, IWorkspaceSelectorMenu
    {
        public WorkspaceSelectorMenu()
        {
            InitializeComponent();
        }

        public void ShowAllWorkspacesAreAvailable()
        {
            button.Content = "Workspaces";
            button.ToolTip = new ToolTip() { Content = "Click to hide/show workspaces for the current project" };
            button.Visibility = System.Windows.Visibility.Visible;
        }

        public void Hide()
        {

        }

        public void ShowThatWorkspacesAreHidden(string[] workspaceNames)
        {
            button.Content = "Workspaces ({0} hidden)".ToFormat(workspaceNames.Length);
            button.ToolTip = new ToolTip() { Content = "Click to hide/show workspaces for the current project\n{0} are hidden.".ToFormat(workspaceNames.Join(", ")) };
            button.Visibility = System.Windows.Visibility.Visible;
        }

        public Action OnSelection
        {
            set { button.Click += value.ToRoutedHandler(); }
        }
    }
}
