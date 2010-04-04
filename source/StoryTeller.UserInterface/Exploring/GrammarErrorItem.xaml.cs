using System.Windows.Controls;
using FubuCore;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Exploring
{
    /// <summary>
    /// Interaction logic for GrammarErrorItem.xaml
    /// </summary>
    public partial class GrammarErrorItem : UserControl
    {
        public GrammarErrorItem()
        {
            InitializeComponent();
        }

        public GrammarErrorItem(GrammarError error)
            : this()
        {
            label.Content = error.Message;

            if (error.ErrorText.IsEmpty())
            {
                errorText.Hide();
            }

            errorText.Text = error.ErrorText;
        }
    }
}