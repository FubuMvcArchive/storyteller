using System;
using System.Windows.Controls;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public class OutlineView : TreeView, IOutlineView
    {
        public void ResetTree(OutlineNode node)
        {
            Items.Clear();
            Items.Add(node);
            node.IsSelected = true;
        }
    }
}