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

namespace StoryTeller.UserInterface.Workspace
{
    /// <summary>
    /// Interaction logic for WorkspaceSelectionDialog.xaml
    /// </summary>
    public partial class WorkspaceSelectionDialog : UserControl, IWorkspaceSelectionDialog
    {
        public WorkspaceSelectionDialog()
        {
            InitializeComponent();

            cancel.Click += (x, y) => close();
            all.Click += (x, y) => items.Each(i => i.Selected = true);
            none.Click += (x, y) => items.Each(i => i.Selected = false);
        }

        private void close()
        {
            Parent.As<Window>().Close();
        }

        public void Clear()
        {
            container.Children.Clear();
        }

        public void Add(string workspaceName, bool selected)
        {
            var item = new WorkspaceItem(workspaceName, selected);
            container.Children.Add(item);
        }

        public Action OnSelection
        {
            set 
            { 
                select.Click += (x, y) =>
                {
                    value();
                    close();
                };
            }
        }

        private IEnumerable<WorkspaceItem> items
        {
            get
            {
                return container.Children.Cast<WorkspaceItem>();
            }
        }

        public string[] GetSelections()
        {
            return items.Where(x => x.Selected).Select(x => x.Name).ToArray();
        }

        public void SelectAll()
        {
            items.Each(i => i.Selected = true);
        }

        public string Title
        {
            get { return "Select Workspaces"; }
        }
    }
}
