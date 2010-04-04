using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Testing.UserInterface.Screens;
using StoryTeller.UserInterface.Exploring;
using StoryTeller.UserInterface.Screens;

namespace StoryTeller.Testing.UserInterface.Exploring
{
    [TestFixture]
    public class GrammarErrorsSubjectTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void create_screen_should_build_the_GrammarErrorsView()
        {
            var factory = MockRepository.GenerateMock<IScreenFactory>();
            var theView = new GrammarErrorsView();
            factory.Expect(x => x.Build<GrammarErrorsView>()).Return(theView);

            var subject = new GrammarErrorsSubject();
            subject.CreateScreen(factory).ShouldBeTheSameAs(theView);
        }

        [Test]
        public void only_matches_a_grammar_errors_view()
        {
            var subject = new GrammarErrorsSubject();
            subject.Matches(new StubScreen()).ShouldBeFalse();

            subject.Matches(new GrammarErrorsView());
        }
    }
}