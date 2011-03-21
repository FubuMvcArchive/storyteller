using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Testing;
using StoryTeller.UserInterface.Commands;
using StoryTeller.UserInterface.Dialogs;
using Rhino.Mocks;

namespace StoryTeller.UserInterface.Testing.UI.Commands
{
    [TestFixture]
    public class AddWorkspaceCommandTester : InteractionContext<AddWorkspaceCommand>
    {
        private Hierarchy hierarchy;

        protected override void beforeEach()
        {
            hierarchy = new Hierarchy("some name");
            Services.Inject(hierarchy);
        }

        [Test]
        public void when_executed_open_dialog()
        {
            ClassUnderTest.Execute();

            MockFor<IDialogLauncher>().AssertWasCalled(x => x.LaunchForCommand<IAddWorkspaceCommand>(ClassUnderTest));
        }

        
    }

    [TestFixture]
    public class when_creating_a_new_workspace : InteractionContext<AddWorkspaceCommand>
    {
        private Hierarchy hierarchy;

        protected override void beforeEach()
        {
            hierarchy = new Hierarchy("some name");
            Services.Inject(hierarchy);

            ClassUnderTest.CreateWorkspace("workspace1");
        }

        [Test]
        public void should_have_added_a_new_workspace_suite_to_the_hierarchy()
        {
            hierarchy.FindSuite("workspace1").ShouldBeOfType<WorkspaceSuite>().Name.ShouldEqual("workspace1");
        }

        [Test]
        public void should_broadcast_a_suite_added_message()
        {
            MockFor<IEventAggregator>().AssertWasCalled(x => x.SendMessage(new SuiteAddedMessage()
            {
                NewSuite = hierarchy.FindSuite("workspace1")
            }));
        }
    }
}