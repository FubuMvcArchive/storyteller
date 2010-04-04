using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StoryTeller.Domain;
using StoryTeller.Model;
using Label=System.Windows.Controls.Label;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public abstract class OutlineNode : TreeViewItem
    {
        private Icon _icon;
        private Image _image;

        protected OutlineNode()
        {
            IsExpanded = true;

            Header = new StackPanel().Horizontal().Configure(x =>
            {
                _image = x.Add<Image>();
            });
        }

        public ITestPart TestPart { get; set; }
        public GrammarStructure Grammar { get; set; }

        protected void addText(string text)
        {
            var label = Header.As<StackPanel>().Add<Label>();
            label.Content = text;
        }

        protected void addItalicizedText(string text)
        {
            var label = Header.As<StackPanel>().Add<Label>();
            label.Content = text;
            label.FontStyle = FontStyles.Italic;
        }

        public Icon Icon
        {
            get { return _icon; }
            set
            {
                _image.SetIcon(value);
                _icon = value;
            }
        }

        public IEnumerable<OutlineNode> Descendents
        {
            get
            {
                foreach (OutlineNode item in Items)
                {
                    yield return item;

                    foreach (OutlineNode node in item.Descendents)
                    {
                        yield return node;
                    }
                }
            }
        }


        //protected T findDescendent<T>(Predicate<T> filter) where T : class
        //{
        //    return Descendents.First(filter);
        //}

        private void OutlineNode_Selected(object sender, RoutedEventArgs e)
        {
            //Select();
            //e.Handled = true;
        }
    }
}