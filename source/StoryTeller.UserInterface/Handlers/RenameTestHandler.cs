using StoryTeller.Domain;
using StoryTeller.UserInterface.Screens;

namespace StoryTeller.UserInterface.Handlers
{
    // Marker

    public class RenameTestHandler : IListener<TestRenamed>
    {
        private readonly IScreenFinder _finder;
        private readonly IScreenCollection _screens;

        public RenameTestHandler(IScreenFinder finder, IScreenCollection screens)
        {
            _finder = finder;
            _screens = screens;
        }

        #region IListener<TestRenamed> Members

        public void Handle(TestRenamed message)
        {
            IScreenLocator _locator = new ScreenLocator<Test>(message.Test);
            IScreen screen = _finder.Find(_locator);
            if (screen == null) return;
            _screens.RenameTab(screen, message.Test.Name);
        }

        #endregion
    }
}