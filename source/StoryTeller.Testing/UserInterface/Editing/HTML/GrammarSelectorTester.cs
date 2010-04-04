using NUnit.Framework;
using StoryTeller.Model;
using StoryTeller.UserInterface.Editing.HTML;

namespace StoryTeller.Testing.UserInterface.Editing.HTML
{
    [TestFixture]
    public class GrammarSelectorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void close_should_appear_in_the_html_for_a_multiple_selection_fixture()
        {
            var fixture = new FixtureGraph("Math");
            fixture.Policies.SelectionMode = SelectionMode.Any;

            new GrammarSelector(fixture).Build().ToString().ShouldContain(GrammarConstants.CLOSE);
        }

        [Test]
        public void close_should_not_appear_in_the_html_for_a_single_selection_fixture()
        {
            var fixture = new FixtureGraph("Math");
            fixture.Policies.SelectionMode = SelectionMode.Single;

            new GrammarSelector(fixture).Build().ToString().ShouldNotContain(GrammarConstants.CLOSE);
        }

        [Test]
        public void required_should_appear_in_the_html_for_a_single_selection_fixture()
        {
            var fixture = new FixtureGraph("Math");
            fixture.Policies.SelectionMode = SelectionMode.Single;

            new GrammarSelector(fixture).Build().ToString().ShouldContain(GrammarConstants.REQUIRED);
        }

        [Test]
        public void required_should_not_appear_in_the_html_for_a_multiple_selection_fixture()
        {
            var fixture = new FixtureGraph("Math");
            fixture.Policies.SelectionMode = SelectionMode.Any;

            new GrammarSelector(fixture).Build().ToString().ShouldNotContain(GrammarConstants.REQUIRED);
        }
    }
}