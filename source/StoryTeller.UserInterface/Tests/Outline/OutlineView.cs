using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public class OutlineView : TreeView, IOutlineView
    {
        public OutlineView()
        {
            Focusable = true;
        }

        public void ResetTree(OutlineNode node)
        {
            Items.Clear();
            Items.Add(node);
            node.IsSelected = true;
        }

        public void FocusOnTop()
        {
            Focus();
            //Keyboard.Focus(this);
            //CaptureMouse();
            

            var outlineNode = Items[0].As<OutlineNode>();
            outlineNode.BringIntoView();
            outlineNode.IsSelected = true;
            outlineNode.Focus();

        }
    }
}