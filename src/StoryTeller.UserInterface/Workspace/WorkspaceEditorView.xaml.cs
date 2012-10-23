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
using StoryTeller.Model;
using StoryTeller.UserInterface.Controls;

namespace StoryTeller.UserInterface.Workspace
{
    /// <summary>
    /// Interaction logic for WorkspaceEditorView.xaml
    /// </summary>
    public partial class WorkspaceEditorView : UserControl, IWorkspaceEditorView
    {
        public WorkspaceEditorView()
        {
            InitializeComponent();
        }

        public void ShowFixtureNamespaces(IEnumerable<IFixtureSelector> selectors)
        {
            fixtureSelectors.Children.Clear();
            selectors.Each(x => fixtureSelectors.Children.Add((UIElement)x));
        }

        public void ShowFixtureUsage(IEnumerable<FixtureGraph> fixtures)
        {
            fixtureUsages.Children.Clear();
            fixtures.Each(x =>
            {
                var link = new Link
                {
                    ToolTip = "{0} ({1})".ToFormat(x.FixtureClassName, x.Label),
                    Padding = new Thickness(0, 0, 0, 5)
                };

                link.WireUp(x.Name, () => new OpenItemMessage(x));
                fixtureUsages.Children.Add(link);
            });
        }
    }
}
