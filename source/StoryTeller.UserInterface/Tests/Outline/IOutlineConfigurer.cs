using System;
using StoryTeller.Domain;
using StoryTeller.Model;
using System.Collections.Generic;

namespace StoryTeller.UserInterface.Tests.Outline
{
    public interface IOutlineConfigurer
    {
        // Dunno how this is going to work just yet
        // Column selector
        void ConfigureTableColumnSelector(OutlineNode node, Table table, IStep step);

        // Remove, MoveUp, MoveDown
        void ConfigureRearrangeCommands(OutlineNode node, IPartHolder holder, ITestPart part);
        
        // ALT-INS opens the add step / comment dialog -- just let it stay open until closed
        void ConfigurePartAdders(OutlineNode node, FixtureGraph fixture, IPartHolder holder);
        void WriteSentenceText(OutlineNode node, Sentence sentence, IStep step);
    }

    public class OutlineConfigurer : IOutlineConfigurer
    {
        private readonly IOutlineController _controller;

        public OutlineConfigurer(IOutlineController controller)
        {
            _controller = controller;
        }

        public void ConfigureTableColumnSelector(OutlineNode node, Table table, IStep step)
        {
            // no-op
        }

        public void ConfigureRearrangeCommands(OutlineNode node, IPartHolder holder, ITestPart part)
        {

        }

        public void ConfigurePartAdders(OutlineNode node, FixtureGraph fixture, IPartHolder holder)
        {

        }

        public void WriteSentenceText(OutlineNode node, Sentence sentence, IStep step)
        {
            var writer = new SentenceWriter(node, step);
            sentence.Parts.Each(x => x.AcceptVisitor(writer));
        }

        public class SentenceWriter : ISentenceVisitor
        {
            private readonly OutlineNode _node;
            private readonly IStep _step;

            public SentenceWriter(OutlineNode node, IStep step)
            {
                _node = node;
                _step = step;
            }

            public void Label(Label label)
            {
                _node.AddText(label.Text);
            }

            public void Input(TextInput input)
            {
                var key = input.Cell.Key;
                string value = _step.Has(key) ? _step.Get(key) : input.Cell.DefaultValue ?? "{" + key + "}";

                _node.AddItalicizedText(value);
            }
        }
    }

    public interface IOutlineController
    {
        void AddComment(IPartHolder holder);
        void AddSection(string section);
        void AddStep(string grammarKey, IPartHolder holder);
        void Remove(ITestPart part, IPartHolder holder);
        void MoveUp(ITestPart part, IPartHolder holder);
        void MoveDown(ITestPart part, IPartHolder holder);
    }

    public class OutlineController : ITestStateListener, IOutlineController
    {
        private readonly Test _test;
        private readonly IOutlineView _view;
        private readonly ITestStateManager _stateManager;
        private readonly IOutlineTreeService _treeService;

        public OutlineController(Test test, IOutlineView view, ITestStateManager stateManager, IOutlineTreeService treeService)
        {
            _test = test;
            _view = view;
            _stateManager = stateManager;
            _treeService = treeService;
            stateManager.RegisterListener(this);
        }

        public void AddComment(IPartHolder holder)
        {

        }

        public void AddSection(string section)
        {

        }

        public void AddStep(string grammarKey, IPartHolder holder)
        {
            
        }

        public void Remove(ITestPart part, IPartHolder holder)
        {
            
        }

        public void MoveUp(ITestPart part, IPartHolder holder)
        {
            
        }

        public void MoveDown(ITestPart part, IPartHolder holder)
        {
            
        }

        public void Update(object source)
        {

        }
    }
}