using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            selected.Click += (o, y) =>
            {
                IEnumerable<IFixtureSelector> fixtureSelectors = children;
                fixtureSelectors.Each(x => x.Enable(!IsSelected()));
            };

        }

        public NamespaceSelector(string @namespace) : this()
        {
            _ns = @namespace;
            label.WireUp(@namespace, () =>
            {
                Visibility visibility = container.Visibility == Visibility.Visible
                                            ? Visibility.Collapsed
                                            : Visibility.Visible;

                container.Visibility = visibility;
            });

            label.ToolTip =
                selected.ToolTip =
                new ToolTip {Content = "All fixtures in namespace " + @namespace + "\nClick to show/hide fixtures"};
        }

        private IEnumerable<IFixtureSelector> children
        {
            get { return container.Children.Cast<IFixtureSelector>(); }
        }

        public CheckBox Selected
        {
            get { return selected; }
        }

        #region IFixtureSelector Members

        public IEnumerable<FixtureFilter> GetFilters()
        {
            if (IsSelected())
            {
                yield return FixtureFilter.Namespace(_ns);
            }
            else
            {
                foreach (FixtureFilter fixtureFilter in children.SelectMany(x => x.GetFilters()))
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

        public void SetInitialState()
        {
            count.Text = "(" + FixtureCount + ")";

            if (IsSelected())
            {
                Close();
            }
            else if (GetFilters().Any())
            {
                Open();
            }
            else
            {
                Close();
            }

            children.Each(x => x.SetInitialState());
        }

        public int FixtureCount
        {
            get { return children.Sum(x => x.FixtureCount); }
        }

        #endregion

        public void Open()
        {
            container.Visibility = Visibility.Visible;
        }

        public void Close()
        {
            container.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void Add(IFixtureSelector selector)
        {
            container.Children.Add((UIElement) selector);
        }

        public void Select(bool isSelected)
        {
            selected.IsChecked = isSelected;
        }
    }
}