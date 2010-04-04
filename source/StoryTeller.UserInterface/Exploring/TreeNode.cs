using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Commands;

namespace StoryTeller.UserInterface.Exploring
{
    public class TreeNode : TreeViewItem
    {
        private readonly INamedItem _subject;
        private Func<TreeNode, IList<ActionMenuItem>> _buildItems = x => new ActionMenuItem[0];
        private bool _contextMenuInitialized;
        private Icon _icon;
        private Image _image;
        private Label _label;
        private Action<TreeNode> _onSelection = node => { };
        private Action<TreeNode> _onRunNodeGesture = node => { };
        private ICommand _runTest;

        public TreeNode(INamedItem item)
        {
            setupHeader(item);

            _subject = item;
            _runTest = new ActionCommand(runTest);
            ContextMenu = new ContextMenu();
            ContextMenuOpening += TreeNode_ContextMenuOpening;

            MouseDoubleClick += (s, args) => Top().OnSelection(this);
            
            //TODO: Yuck.  Need to find a better way to do this.
            InputBindings.Add(new InputBinding(_runTest, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control)));
        }

        public string Text { get { return (string) _label.Content; } }

        public Icon Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                _image.SetIcon(_icon);

                updateParentIcon();
            }
        }

        private void runTest()
        {
            Top().OnRunNodeGesture(this);
        }

        public Action<TreeNode> OnSelection { get { return _onSelection; } set { _onSelection = value; } }
        public Action<TreeNode> OnRunNodeGesture { get { return _onRunNodeGesture; } set { _onRunNodeGesture = value; } }

        public Func<TreeNode, IList<ActionMenuItem>> BuildItems { get { return _buildItems; } set { _buildItems = value; } }


        public INamedItem Subject { get { return _subject; } }

        public INamedItem[] Children
        {
            get
            {
                var list = new List<INamedItem>();
                foreach (TreeNode item in Items)
                {
                    list.Add(item.Subject);
                }

                return list.ToArray();
            }
        }

        private void setupHeader(INamedItem item)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            _image = new Image();
            stack.Children.Add(_image);
            _label = new Label
            {
                Content = item.Name,
            };
            stack.Children.Add(_label);
            Header = stack;

            Icon = Icon.Unknown;
        }

        private void updateParentIcon()
        {
            var parent = Parent as TreeNode;
            if (parent != null)
            {
                parent.updateIcon();
            }
        }

        private void TreeNode_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!_contextMenuInitialized)
            {
                InitializeContextMenu();
            }

            foreach (ActionMenuItem item in ContextMenu.Items)
            {
                item.Update();
            }
        }

        public void InitializeContextMenu()
        {
            Top().BuildItems(this).Each(item => ContextMenu.Items.Add(item));
            _contextMenuInitialized = true;
        }

        public bool Equals(TreeNode obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return base.Equals(obj) && Equals(obj._subject, _subject);
        }

        public override string ToString()
        {
            return string.Format("TreeNode: {0}", _subject);
        }


        public void Add(TreeNode node)
        {
            node.ClearParent();

            Items.Add(node);
        }

        public void ClearParent()
        {
            Parent.CallOn<TreeNode>(x => x.Items.Remove(this));
        }

        public void ExpandFirstLevel()
        {
            IsExpanded = true;
            foreach (TreeNode child in Items)
            {
                child.IsExpanded = true;
            }
        }

        public TreeNode Top()
        {
            var parent = Parent as TreeNode;
            return parent == null ? this : parent.Top();
        }

        public TreeNode NodeAt(int index)
        {
            return Items[index].As<TreeNode>();
        }

        private void updateIcon()
        {
            if (Items.Count == 0) return;

            var nodes = new TreeNode[Items.Count];
            Items.CopyTo(nodes, 0);

            Icon = nodes.Select(x => x.As<TreeNode>().Icon).OrderBy(x => x).First();
            updateParentIcon();
        }

        public void CollapseAll()
        {
            IsExpanded = false;
            foreach (TreeNode child in Items)
            {
                child.CollapseAll();
            }
        }

        public void ExpandAll()
        {
            IsExpanded = true;
            foreach (TreeNode child in Items)
            {
                child.ExpandAll();
            }
        }

        public void Remove(TreeNode node)
        {
            Items.Remove(node);
        }

        public void ResetText()
        {
            _label.Content = _subject.Name;
            _label.Background = new SolidColorBrush(Colors.SeaGreen);
            InvalidateVisual();
        }

        public void Reorder()
        {
            List<TreeNode> list = Items.Cast<TreeNode>().ToList();

            Items.Clear();

            list.OrderBy(x => x.Subject.Name).Each(x => Items.Add(x));
        }
    }
}