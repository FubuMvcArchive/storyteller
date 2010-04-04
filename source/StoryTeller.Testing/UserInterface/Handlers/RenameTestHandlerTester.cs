using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Domain;
using StoryTeller.UserInterface;
using StoryTeller.UserInterface.Handlers;
using StoryTeller.UserInterface.Screens;

namespace StoryTeller.Testing.UserInterface.Handlers
{
    [TestFixture]
    public class RenameTestHandlerTester : InteractionContext<RenameTestHandler>
    {
        private Test test;
        private ScreenLocator<Test> _locator;
        private IScreen theScreen;

        protected override void beforeEach()
        {
            test = new Test("the test");

            _locator = new ScreenLocator<Test>(test);
            MockFor<IScreenObjectLocator>().Expect(x => x.BuildSubject(test)).Return(_locator);

            theScreen = MockFor<IScreen>();
            MockFor<IScreenFinder>().Expect(x => x.Find(_locator)).Return(theScreen);

            ClassUnderTest.HandleMessage(new TestRenamed
            {
                Test = test
            });
        }

        [Test]
        public void should_rename_the_tab_to_the_new_test_name()
        {
            MockFor<IScreenCollection>().AssertWasCalled(x => x.RenameTab(theScreen, test.Name));
        }
    }
}