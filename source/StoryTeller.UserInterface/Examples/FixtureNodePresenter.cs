using StoryTeller.Examples;
using StoryTeller.Execution;
using StoryTeller.Model;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Screens;

namespace StoryTeller.UserInterface.Examples
{
    public class FixtureNodePresenter : IScreen<IFixtureNode>, IListener<BinaryRecycleFinished>
    {
        private readonly IExampleSource _examples;
        private readonly IFixtureNode _subject;
        private readonly IFixtureNodeView _view;
        private Example _example;

        public FixtureNodePresenter(IFixtureNodeView view, IFixtureNode subject, IExampleSource examples)
        {
            _view = view;
            _subject = subject;
            _examples = examples;
        }


        public IExampleSource Examples { get; set; }

        public Example Example { get { return _example; } }

        #region IListener<BinaryRecycleFinished> Members

        public void Handle(BinaryRecycleFinished message)
        {
            createExample();
        }

        #endregion

        #region IScreen<IFixtureNode> Members

        public IFixtureNode Subject { get { return _subject; } }

        public object View { get { return _view; } }

        public string Title { get { return _example.Title; } }

        public void Activate(IScreenObjectRegistry screenObjects)
        {
            createExample();
        }

        public bool CanClose()
        {
            return true;
        }

        public void Dispose()
        {
        }

        #endregion

        private void createExample()
        {
            _example = _examples.BuildExample(_subject.GetPath());
            _view.Display(_example);
        }
    }
}