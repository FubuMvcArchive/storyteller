using System.Windows.Controls;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public class OutlineView : TreeView, IOutlineView
    {

        //public void SetTestNode(ITestNode node)
        //{
        //    Items.Clear();
        //    Items.Add(node);
        //    MinWidth = 200;

        //    select(node);
        //}

        private void select(object node)
        {
            node.As<TreeViewItem>().IsSelected = true;
        }
    }
}