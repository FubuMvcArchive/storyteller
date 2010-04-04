using System.Windows.Controls;
using StoryTeller.Execution;

namespace StoryTeller.UserInterface.Examples
{
    /// <summary>
    /// Interaction logic for FixtureNodeView.xaml
    /// </summary>
    public partial class FixtureNodeView : UserControl, IFixtureNodeView
    {
        public FixtureNodeView()
        {
            InitializeComponent();
        }

        #region IFixtureNodeView Members

        public void Display(Example example)
        {
            description.Content = example.Description;
            xml.Text = example.Xml;
            browser.NavigateToString(example.Html);
        }

        #endregion
    }
}