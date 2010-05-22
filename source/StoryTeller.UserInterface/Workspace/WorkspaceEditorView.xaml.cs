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
    }
}
