using StoryTeller.Domain;

namespace StoryTeller.UserInterface.Tests
{
    public class ResultsMode : TestMode
    {
        private readonly Test _test;
        private readonly IHtmlView _view;

        public ResultsMode(IHtmlView view, Test test)
            : base(Mode.Results)
        {
            _view = view;
            _test = test;
        }

        public override object ContentView { get { return _view; } }

        public override void Refresh()
        {
            _view.Html = _test.LastResult.Html;
        }

        public override bool IsEnabled(Test test)
        {
            return test.HasResult();
        }
    }
}