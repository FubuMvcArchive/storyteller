using System;
using NUnit.Framework;
using StoryTeller.UserInterface.Dialogs;
using StoryTeller.UserInterface.Workspace;
using StoryTeller.Workspace;
using System.Linq;
using Rhino.Mocks;

namespace StoryTeller.Testing.UserInterface.Workspace
{
    [TestFixture]
    public class when_starting_up : InteractionContext<WorkspaceSelector>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.Start();
        }

        [Test]
        public void should_hide_the_workspace_selector_menu()
        {
            MockFor<IWorkspaceSelectorMenu>().AssertWasCalled(x => x.Hide());
        }
    }

    [TestFixture]
    public class when_responding_to_project_loaded : InteractionContext<WorkspaceSelector>
    {
        protected override void beforeEach()
        {
            var project = new Project();
            project.SelectedWorkspaces.Any().ShouldBeFalse();

            ClassUnderTest.HandleMessage(new ProjectLoaded(project));
        }

        [Test]
        public void should_direct_the_menu_to_hide()
        {
            MockFor<IWorkspaceSelectorMenu>().AssertWasCalled(x => x.Hide());
        }

    }

    [TestFixture]
    public class when_responding_to_hierarchy_loaded_for_project_with_no_prior_selected_workspaces : InteractionContext<WorkspaceSelector>
    {
        protected override void beforeEach()
        {
            var project = new Project();
            project.SelectedWorkspaces.Any().ShouldBeFalse();

            ClassUnderTest.Project = project;

            var hierarchy =
    DataMother.BuildHierarchy(
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
s5/t12,Failure
s5/s6/t13,Success
s5/s6/s7/t14,Success
s5/s6/s7/s8/t15,Success
s9/t16,Success
s9/t17,Success
s9/t18,Failure
");

            ClassUnderTest.HandleMessage(hierarchy);
        }

        [Test]
        public void should_direct_the_menu_show_that_all_workspaces_are_available()
        {
            MockFor<IWorkspaceSelectorMenu>().AssertWasCalled(x => x.ShowAllWorkspacesAreAvailable());
        }
    }

    [TestFixture]
    public class when_responding_to_hierarchy_loaded_for_project_with_prior_selected_workspaces : InteractionContext<WorkspaceSelector>
    {
        protected override void beforeEach()
        {
            var project = new Project();
            project.SelectWorkspaces(new string[]{ "s1", "s5"});

            ClassUnderTest.Project = project;

            var hierarchy =
    DataMother.BuildHierarchy(
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
s5/t12,Failure
s5/s6/t13,Success
s5/s6/s7/t14,Success
s5/s6/s7/s8/t15,Success
s9/t16,Success
s9/t17,Success
s9/t18,Failure
");

            ClassUnderTest.HandleMessage(hierarchy);
        }

        [Test]
        public void should_direct_the_menu_to_show_that_there_are_hidden_workspaces()
        {
            MockFor<IWorkspaceSelectorMenu>().AssertWasCalled(x => x.ShowThatWorkspacesAreHidden(new string[]{"s9"}));
        }
    }

    [TestFixture]
    public class when_starting_to_select_workspaces : InteractionContext<WorkspaceSelector>
    {
        protected override void beforeEach()
        {
            var project = new Project();
            project.SelectWorkspaces(new string[] { "s1", "s5" });

            ClassUnderTest.Project = project;

            var hierarchy =
    DataMother.BuildHierarchy(
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
s5/t12,Failure
s5/s6/t13,Success
s5/s6/s7/t14,Success
s5/s6/s7/s8/t15,Success
s9/t16,Success
s9/t17,Success
s9/t18,Failure
");
            ClassUnderTest.Hierarchy = hierarchy;


            ClassUnderTest.OpenWorkspaceSelection();
        }

        [Test]
        public void should_clear_out_the_dialog_first()
        {
            MockFor<IWorkspaceSelectorDialog>().AssertWasCalled(x => x.Clear());
        }

        [Test]
        public void should_register_a_control_for_each_workspace()
        {
            MockFor<IWorkspaceSelectorDialog>().AssertWasCalled(x => x.Add("s1", true));
            MockFor<IWorkspaceSelectorDialog>().AssertWasCalled(x => x.Add("s5", true));
            MockFor<IWorkspaceSelectorDialog>().AssertWasCalled(x => x.Add("s9", false));
        }

        [Test]
        public void should_show_the_dialog()
        {
            MockFor<IDialogLauncher>().AssertWasCalled(x => x.Launch(MockFor<IWorkspaceSelectorDialog>()));
        }
    }

    [TestFixture]
    public class when_selecting_workspaces : InteractionContext<WorkspaceSelector>
    {
        private string[] selections;

        protected override void beforeEach()
        {
            var project = new Project();
            project.SelectWorkspaces(new string[] { "s1", "s5" });

            Services.PartialMockTheClassUnderTest();
            ClassUnderTest.Expect(x => x.UpdateSelectionMenu());

            ClassUnderTest.Project = project;

            var hierarchy =
    DataMother.BuildHierarchy(
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
s5/t12,Failure
s5/s6/t13,Success
s5/s6/s7/t14,Success
s5/s6/s7/s8/t15,Success
s9/t16,Success
s9/t17,Success
s9/t18,Failure
");
            ClassUnderTest.Hierarchy = hierarchy;

            selections = new string[] { "s1", "s9" };

            MockFor<IWorkspaceSelectorDialog>().Expect(x => x.GetSelections()).Return(selections);


            ClassUnderTest.SelectWorkspaces();
        }

        [Test]
        public void should_apply_the_changes_to_project()
        {
            ClassUnderTest.Project.SelectedWorkspaces.Select(x => x.Name).ShouldHaveTheSameElementsAs(selections);
        }

        [Test]
        public void should_find_the_selections_from_the_dialog()
        {
            MockFor<IWorkspaceSelectorDialog>().VerifyAllExpectations();
        }

        [Test]
        public void should_update_the_selection_menu()
        {
            ClassUnderTest.AssertWasCalled(x => x.UpdateSelectionMenu());
        }

        [Test]
        public void should_raise_the_workflow_filter_changed_method()
        {
            MockFor<IEventAggregator>().AssertWasCalled(x => x.SendMessage(new WorkflowFiltersChanged(ClassUnderTest.Project)));
        }
    }
}