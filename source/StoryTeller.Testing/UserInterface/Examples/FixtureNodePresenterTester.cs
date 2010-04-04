using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Examples;
using StoryTeller.Execution;
using StoryTeller.Model;
using StoryTeller.UserInterface.Examples;

namespace StoryTeller.Testing.UserInterface.Examples
{
    [TestFixture]
    public class when_activating_the_fixture_node_presenter : InteractionContext<FixtureNodePresenter>
    {
        private Example example;
        private FixtureGraph fixture;

        protected override void beforeEach()
        {
            fixture = new FixtureGraph("Math");
            Services.Inject<IFixtureNode>(fixture);


            example = new Example
            {
                Description = "some description",
                Html = "html",
                Title = "the title",
                Xml = "xml"
            };
            MockFor<IExampleSource>().Expect(x => x.BuildExample(fixture.GetPath())).Return(example);

            ThePresenter.Activate(null);
        }

        public FixtureNodePresenter ThePresenter { get { return ClassUnderTest; } }

        [Test]
        public void activate_should_pass_the_example_to_the_view()
        {
            MockFor<IFixtureNodeView>().AssertWasCalled(x => x.Display(example));
        }

        [Test]
        public void the_presenter_should_create_a_new_example_object()
        {
            MockFor<IExampleSource>().VerifyAllExpectations();
        }

        [Test]
        public void the_title_is_the_example_title()
        {
            ThePresenter.Title.ShouldEqual(example.Title);
        }
    }


    [TestFixture]
    public class when_handling_the_fixture_library_loaded_message : InteractionContext<FixtureNodePresenter>
    {
        private Example example;
        private FixtureGraph fixture;

        protected override void beforeEach()
        {
            fixture = new FixtureGraph("Math");
            Services.Inject<IFixtureNode>(fixture);


            example = new Example
            {
                Description = "some description",
                Html = "html",
                Title = "the title",
                Xml = "xml"
            };
            MockFor<IExampleSource>().Expect(x => x.BuildExample(fixture.GetPath())).Return(example);

            ThePresenter.Activate(null);
        }

        public FixtureNodePresenter ThePresenter { get { return ClassUnderTest; } }

        [Test]
        public void activate_should_pass_the_example_to_the_view()
        {
            MockFor<IFixtureNodeView>().AssertWasCalled(x => x.Display(example));
        }

        [Test]
        public void the_presenter_should_create_a_new_example_object()
        {
            MockFor<IExampleSource>().VerifyAllExpectations();
        }

        [Test]
        public void the_title_is_the_example_title()
        {
            ThePresenter.Title.ShouldEqual(example.Title);
        }
    }
}