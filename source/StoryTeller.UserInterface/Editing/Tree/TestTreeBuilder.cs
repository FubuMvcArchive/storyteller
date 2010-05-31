using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using StoryTeller.Domain;
using StoryTeller.Model;
using StoryTeller.UserInterface.Tests;

namespace StoryTeller.UserInterface.Editing.Tree
{

    public class TestTreeNode : TreeViewItem, IEnumerable<TestTreeNode>
    {


        public ITestPart Part { get; set; }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TestTreeNode> GetEnumerator()
        {
            foreach (TestTreeNode child in Items)
            {
                yield return child;

                foreach (TestTreeNode grandchild in child)
                {
                    yield return grandchild;
                }
            }
        }
    }

    public interface ITestTreeController : ITestStateListener
    {
        
    }


    public interface ITreeNodeBuilder
    {
        TestTreeNode ForComment(Comment comment);
        TestTreeNode ForSentence(Sentence sentence, IStep step);
        TestTreeNode ForStep(string title, IStep step);

        // Dunno how this is going to work just yet
        // Column selector
        void ConfigureTableColumnSelector(TestTreeNode node, Table table, IStep step);

        // Remove, MoveUp, MoveDown
        void ConfigureRearrangeCommands(TestTreeNode node, IPartHolder holder, ITestPart part);
        
        // ALT-INS opens the add step / comment dialog -- just let it stay open until closed
        void ConfigurePartAdders(TestTreeNode node, FixtureGraph fixture, IPartHolder holder);
    }


    public class TestTreeBuilder
    {
        private FixtureLibrary _library;
        private Test _test;

        private readonly TreeViewItem _top;

        public TestTreeBuilder(Test test, FixtureLibrary library)
        {
            var workspace = test.GetWorkspace();
            _library = library.Filter(workspace.CreateFixtureFilter().Matches);
            _test = test;

            _top = new TreeViewItem();
        }

        public void Rebuild()
        {
            throw new NotImplementedException();
        }

        public TreeViewItem Top
        {
            get { return _top; }
        }
    }

    
}