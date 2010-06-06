using System.Windows;
using System.Windows.Controls;

namespace StoryTeller.UserInterface.Workspace
{
    public interface IStartupActionSelector
    {
        bool IsSelected();
        string ActionName { get; }
    }

    public class StartupActionSelector : StackPanel, IStartupActionSelector
    {
        private readonly string _actionName;
        private readonly CheckBox _checkbox;

        public StartupActionSelector(string actionName, bool selected)
        {
            _actionName = actionName;

            Height = 25;
            this.Horizontal();
            _checkbox = this.Add<CheckBox>();
            _checkbox.Padding = new Thickness(0, 5, 0, 0);
            _checkbox.VerticalAlignment = VerticalAlignment.Top;
            _checkbox.IsChecked = selected;

            this.Add<System.Windows.Controls.Label>(l =>
            {
                l.Content = actionName;
                l.VerticalAlignment = VerticalAlignment.Top;
            });
        }

        public bool IsSelected()
        {
            return _checkbox.IsChecked.GetValueOrDefault(false);
        }

        public string ActionName
        {
            get { return _actionName; }
        }
    }
}