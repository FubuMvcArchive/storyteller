using System.Windows;
using StoryTeller.Domain;
using StoryTeller.UserInterface.Tests.Editing;

namespace WpfSpike
{
    /// <summary>
    /// Interaction logic for CommentPlaying.xaml
    /// </summary>
    public partial class CommentPlaying : Window
    {
        public CommentPlaying()
        {
            InitializeComponent();

            Loaded += CommentPlaying_Loaded;
        }

        private void CommentPlaying_Loaded(object sender, RoutedEventArgs e)
        {
            var comment =
                new Comment(
                    "I ran into a scenario I needed - namely I had another layer of abstract class between the concrete type and the base generic abstract type.  Updated patch attached.");
            var section = new Section("Math");

            var node = new CommentEditorNode(null, section, comment);
            panel.Children.Add(node);
        }
    }
}