using System;
using System.Windows.Controls;
using System.Windows.Input;
using StoryTeller.UserInterface.Actions;

namespace StoryTeller.UserInterface
{
    public static class TreeViewItemExtensions
    {
        public static TreeViewItemBindingExpression Bind(this TreeViewItem item, ModifierKeys modifiers, Key key)
        {
            return new TreeViewItemBindingExpression(item, new KeyGesture(key, modifiers));    
        }

        public static TreeViewItemBindingExpression Bind(this TreeViewItem item, ModifierKeys modifiers, MouseAction action)
        {
            return new TreeViewItemBindingExpression(item, new MouseGesture(action, modifiers));
        }

        public static TreeViewItemBindingExpression Bind(this TreeViewItem item, MouseAction action)
        {
            return new TreeViewItemBindingExpression(item, new MouseGesture(action));
        }

        public static TreeViewItemBindingExpression BindControlAnd(this TreeViewItem item, Key key)
        {
            return item.Bind(ModifierKeys.Control, key);
        }

        public static TreeViewItemBindingExpression BindControlAnd(this TreeViewItem item, MouseAction action)
        {
            return item.Bind(ModifierKeys.Control, action);
        }
    }

    public class TreeViewItemBindingExpression
    {
        private readonly TreeViewItem _item;
        private InputGesture _gesture;

        public TreeViewItemBindingExpression(TreeViewItem item, InputGesture gesture)
        {
            _item = item;
            _gesture = gesture;
        }

        public void To(Action action)
        {
            To(new ActionCommand(action));
        }

        public void To(ICommand command)
        {
            var binding = new InputBinding(command, _gesture);
            _item.InputBindings.Add(binding);
        }

        public void ToMessage<T>(T message)
        {
            To(() =>
            {
                var args = new MessageRequestArgs(events => events.SendMessage(message));
                _item.RaiseEvent(args);
            });
        }

        public void ToMessage<T>(Func<T> toMessage)
        {
            To(() =>
            {
                var args = new MessageRequestArgs(events => events.SendMessage(toMessage()));
                _item.RaiseEvent(args);
            });
        }
    }
}