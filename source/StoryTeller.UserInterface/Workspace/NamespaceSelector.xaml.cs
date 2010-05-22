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
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Workspace
{
    /// <summary>
    /// Interaction logic for NamespaceSelection.xaml
    /// </summary>
    public partial class NamespaceSelector : UserControl, IFixtureSelector
    {
        private readonly string _ns;

        public NamespaceSelector()
        {
            InitializeComponent();

            selected.Checked += (o, y) =>
            {
                var fixtureSelectors = children;
                fixtureSelectors.Each(x => x.Enable(!IsSelected()));
            };
        }

        private IEnumerable<IFixtureSelector> children
        {
            get { return container.Children.Cast<IFixtureSelector>(); }
        }

        public NamespaceSelector(string @namespace) : this()
        {
            _ns = @namespace;
            label.Content = @namespace;

            label.ToolTip = selected.ToolTip = new ToolTip() {Content = "All fixtures in namespace " + @namespace};
        }

        public CheckBox Selected
        {
            get { return selected; }
        }

        public void Add(IFixtureSelector selector)
        {
            container.Children.Add((UIElement)selector);
        }


        public IEnumerable<FixtureFilter> GetFilters()
        {
            if (IsSelected())
            {
                yield return FixtureFilter.Namespace(_ns);
            }
            else
            {
                foreach (var fixtureFilter in children.SelectMany(x => x.GetFilters()))
                {
                    yield return fixtureFilter;
                }

            }
            
        }

        public bool IsSelected()
        {
            return selected.IsChecked.GetValueOrDefault(false);
        }

        public void Enable(bool enabled)
        {
            IsEnabled = enabled;

            bool childrenShouldBeEnabled = enabled && !IsSelected();

            children.Each(x => x.Enable(childrenShouldBeEnabled));
        }
    }
}
