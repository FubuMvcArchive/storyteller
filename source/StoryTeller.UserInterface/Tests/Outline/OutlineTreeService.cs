using System;
using System.Windows;
using System.Windows.Input;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public class OutlineTreeService : IOutlineTreeService
    {
        private readonly ProjectContext _context;
        private readonly Window _window;
        private OutlineNode _topNode;

        public OutlineTreeService(ProjectContext context, Window window)
        {
            _context = context;
            _window = window;
            _window.Focusable = true;
        }

        public OutlineNode BuildNode(Test test, IOutlineController controller)
        {
            var builder = this.builder(controller, test);

            _topNode = builder.Build();
            return _topNode;
        }

        private OutlineTreeBuilder builder(IOutlineController controller, Test test)
        {
            var configurer = new OutlineConfigurer(controller);
            return new OutlineTreeBuilder(test, _context.Library, configurer);
        }

        public void RedrawNode(Test test, IOutlineController controller)
        {
            builder(controller, test).Rebuild(_topNode);
            moveNext();
        }

        private void moveNext()
        {
            Keyboard.FocusedElement.As<UIElement>().MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public void SelectNodeFor(ITestPart part)
        {
            //_topNode.Parent.CallOn<UIElement>(e => e.Focus());
            //moveNext();
            //moveNext();
            //moveNext();
            //moveNext();
            //moveNext();


            _window.Focus();
            
            _topNode.IsSelected = false;
            var parent = _topNode.Parent as UIElement;
            parent.Focus();
            parent.InvalidateVisual();
            OutlineNode node = _topNode.Find(n => n.Part == part);
            node.BringIntoView();
            node.IsSelected = true;
            node.Focus();
            //node.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //moveNext();
            return;
            

            //_topNode.Parent.CallOn<UIElement>(e => e.Focus());
            
            node.Focus();
            
            //parent.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //Keyboard.FocusedElement.As<UIElement>().MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //Keyboard.FocusedElement.As<UIElement>().MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));

            //node.IsSelected = true;
            //node.Focus();
            //_topNode.Parent.CallOn<UIElement>(e => e.Focus());

            
        }
    }
}