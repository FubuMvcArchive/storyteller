using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Testing;
using StoryTeller.UserInterface.Controls;
using StoryTeller.UserInterface.Exploring;
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Testing.UI.Exploring
{
    [TestFixture]
    public class TreeExplorer_responding_to_TestListener_events
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            hierarchy =
                StoryTeller.Testing.DataMother.BuildHierarchy(
                    @"
t1,Success
t2,Failure
t3,Success
s1/t4,Success
s1/t5,Success
s1/t6,Failure
s1/s2/t7,Success
s1/s2/t8,Failure
s1/s2/s3/t9,Success
s1/s2/s3/t10,Success
s1/s2/s3/s4/t11,Success
s5/t12,Success
s5/s6/t13,Success
s5/s6/s7/t14,Success
s5/s6/s7/s8/t15,Success
s9/t16,Success
s9/t17,Success
s9/t18,Success
");

            view = new StubExplorerView();
            events = MockRepository.GenerateMock<IEventAggregator>();

            explorer = new TestExplorer(view, events, new TestFilterBar(), new TestFilter(), null);
            explorer.Handle(hierarchy);
        }

        #endregion

        private Hierarchy hierarchy;
        private StubExplorerView view;
        private TestExplorer explorer;
        private IEventAggregator events;

        private Test unknownTest()
        {
            Test test = hierarchy.FindTest("t1");
            test.Reset();
            return test;
        }


        private Test successfulTest()
        {
            Test test = hierarchy.FindTest("t1");
            test.LastResult = new TestResult();

            test.LastResult.Counts.IncrementRights();
            return test;
        }

        private Test failedTest()
        {
            Test test = hierarchy.FindTest("t1");
            test.Reset();

            test.LastResult = new TestResult();
            test.LastResult.Counts.IncrementWrongs();
            return test;
        }

        private void sentIconChanged(Test test, Icon icon)
        {
            events.AssertWasCalled(x => x.SendMessage(new TestIconChanged(test, icon)));
        }

        [Test]
        public void cancel_a_test_that_has_no_prior_result()
        {
            Test test = unknownTest();

            explorer.Handle(new TestRunEvent(test, TestState.NotQueued));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Unknown);
        }

        [Test]
        public void cancel_a_test_that_previously_failed()
        {
            Test test = failedTest();

            explorer.Handle(new TestRunEvent(test, TestState.NotQueued));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Failed);
        }

        [Test]
        public void cancel_a_test_that_previously_succeeded()
        {
            Test test = successfulTest();

            explorer.Handle(new TestRunEvent(test, TestState.NotQueued));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Success);
        }

        [Test]
        public void cancelling_a_test_sends_the_icon_changed_message()
        {
            Test test = unknownTest();

            explorer.Handle(new TestRunEvent(test, TestState.NotQueued));

            sentIconChanged(test, Icon.Unknown);
        }

        [Test]
        public void finish_a_test_with_failure()
        {
            Test test = failedTest();


            explorer.Handle(new TestRunEvent(test, TestState.NotQueued)
            {
                Completed = true
            });

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Failed);
        }

        [Test]
        public void finish_a_test_with_success()
        {
            Test test = successfulTest();

            explorer.Handle(new TestRunEvent(test, TestState.NotQueued)
            {
                Completed = true
            });

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Success);
        }

        [Test]
        public void finishing_a_test_sends_the_icon_changed_message()
        {
            Test test = failedTest();

            explorer.Handle(new TestRunEvent(test, TestState.NotQueued)
            {
                Completed = true
            });

            sentIconChanged(test, Icon.Failed);
        }

        [Test]
        public void queue_a_test_that_has_no_prior_result()
        {
            Test test = unknownTest();


            explorer.Handle(new TestRunEvent(test, TestState.Queued));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Pending);
        }

        [Test]
        public void queueing_a_test_sends_the_icon_changed_message()
        {
            Test test = unknownTest();

            explorer.Handle(new TestRunEvent(test, TestState.Queued));

            sentIconChanged(test, Icon.Pending);
        }

        [Test]
        public void start_a_test_that_has_no_prior_result()
        {
            Test test = unknownTest();


            explorer.Handle(new TestRunEvent(test, TestState.Executing));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.Pending);

            sentIconChanged(test, Icon.Pending);
        }


        [Test]
        public void start_a_test_that_has_previously_failed()
        {
            Test test = failedTest();

            explorer.Handle(new TestRunEvent(test, TestState.Executing));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.RunningFailure);
        }


        [Test]
        public void start_a_test_that_has_previously_succeeded()
        {
            Test test = successfulTest();

            explorer.Handle(new TestRunEvent(test, TestState.Executing));

            explorer.NodeFor(test).Icon.ShouldEqual(Icon.RunningSuccess);
        }
    }

    public class when_test_explorer_responds_to_workflow_filters_changed : InteractionContext<TestExplorer>
    {
        private string[] workspaces;

        protected override void beforeEach()
        {
            workspaces = new string[] {"a", "b", "c"};
            MockFor<IProject>().Expect(x => x.SelectedWorkspaceNames).Return(workspaces);

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.ResetFilter());

            ClassUnderTest.HandleMessage(new WorkflowFiltersChanged(MockFor<IProject>()));
        }

        [Test]
        public void should_apply_the_workspaces_selected_from_the_project_to_the_test_filter()
        {
            MockFor<ITestFilter>().AssertWasCalled(x => x.Workspaces = workspaces);
        }

        [Test]
        public void should_reset_the_filter()
        {
            ClassUnderTest.AssertWasCalled(x => x.ResetFilter());
        }
    }

    [TestFixture]
    public class when_test_explorer_responds_to_TestAdded_event
    {
    }
}