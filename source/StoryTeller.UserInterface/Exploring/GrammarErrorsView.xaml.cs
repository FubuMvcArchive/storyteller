using System.Windows.Controls;
using StoryTeller.Execution;
using StoryTeller.Model;
using StoryTeller.UserInterface.Actions;
using StoryTeller.UserInterface.Screens;

namespace StoryTeller.UserInterface.Exploring
{
    public partial class GrammarErrorsView : UserControl, IScreen, IListener<BinaryRecycleFinished>
    {
        public GrammarErrorsView()
        {
            InitializeComponent();
        }

        public GrammarErrorsView(FixtureLibrary library)
            : this()
        {
            reset(library);
        }

        #region IListener<BinaryRecycleFinished> Members

        public void Handle(BinaryRecycleFinished message)
        {
            reset(message.Library);
        }

        #endregion

        #region IScreen Members

        public object View { get { return this; } }

        public string Title { get { return "Grammar Errors"; } }

        public void Activate(IScreenObjectRegistry screenObjects)
        {
        }

        public bool CanClose()
        {
            return true;
        }

        public void Dispose()
        {
        }

        #endregion

        private void reset(FixtureLibrary library)
        {
            main.Children.Clear();
            foreach (GrammarError error in library.AllErrors())
            {
                var item = new GrammarErrorItem(error);
                main.Children.Add(item);
            }
        }
    }
}