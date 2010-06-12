using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Execution;
using StoryTeller.Model;
using StoryTeller.UserInterface.Examples;

namespace StoryTeller.Testing.UserInterface.Examples
{
    [TestFixture]
    public class when_activating_the_fixture_node_presenter : InteractionContext<FixtureNodePresenter>
    {
        private FixtureGraph fixture;

        protected override void beforeEach()
        {
            fixture = new FixtureGraph("Math");
            Services.Inject<IFixtureNode>(fixture);


            ThePresenter.Activate(null);
        }

        public FixtureNodePresenter ThePresenter { get { return ClassUnderTest; } }


    }

}