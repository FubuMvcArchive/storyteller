using System.Windows;
using StoryTeller.Domain;
using StoryTeller.Model;
using StoryTeller.UserInterface.Tests.Editing.Tables;

namespace WpfSpike
{
    /// <summary>
    /// Interaction logic for GridWindow.xaml
    /// </summary>
    public partial class GridWindow : Window
    {
        public GridWindow()
        {
            InitializeComponent();

            Loaded += GridWindow_Loaded;
        }

        private void GridWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var fixture = new TableFixture();
            var table = (ITable) fixture["Add"].ToStructure(null);

            var leaf = new StepLeaf();
            leaf.AddParts(
                new Step().With("w:1,x:2,y:3"),
                new Step().With("w:2,x:3,y:4"),
                new Step().With("w:5,x:6,y:7"),
                new Step().With("w:6,x:7,y:8"),
                new Step().With("w:7,x:8,y:9")
                );

            var controller = new TableController(grid);
            controller.Start(table, leaf);
        }
    }
}