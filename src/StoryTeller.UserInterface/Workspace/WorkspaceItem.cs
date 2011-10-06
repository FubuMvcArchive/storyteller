using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StoryTeller.UserInterface.Workspace
{
    public class WorkspaceItem : StackPanel
    {
        private readonly CheckBox _checkbox;
        private readonly string _name;

        public WorkspaceItem(string name, bool selected)
        {
            Height = 25;
            this.Horizontal();
            _checkbox = this.Add<CheckBox>();
            _checkbox.Padding = new Thickness(0, 5, 0, 0);
            _checkbox.VerticalAlignment = VerticalAlignment.Center;

            this.Add<Label>(l =>
            {
                l.Content = name;
                l.VerticalAlignment = VerticalAlignment.Top;
            });
            _name = name;

            _checkbox.IsChecked = selected;
        }

        public string WorkspaceName
        {
            get { return _name; }
        }

        public bool Selected
        {
            get { return _checkbox.IsChecked.GetValueOrDefault(false); }
            set { _checkbox.IsChecked = value; }
        }
    }
}