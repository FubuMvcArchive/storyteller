using System;
using System.Collections.Generic;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Model;
using StoryTeller.UserInterface;
using StoryTeller.UserInterface.Projects;
using StoryTeller.UserInterface.Workspace;
using StoryTeller.Workspace;
using Rhino.Mocks;

namespace StoryTeller.Testing.UserInterface.Workspace
{
    [TestFixture]
    public class when_building_the_screen : WorkspaceEditorContext
    {
        private List<NamespaceSelector> theSelectors;

        protected override void theContextIs()
        {
            theSelectors = new List<NamespaceSelector>()
            {
                new NamespaceSelector("a"),
                new NamespaceSelector("b"),
                new NamespaceSelector("c")
            };

            MockFor<IFixtureSelectorOrganizer>().Expect(x => x.Organize(library, suite.Filter))
                .Return(theSelectors);

            ClassUnderTest.BuildView();
        }

        [Test]
        public void should_organize_the_fixtures_into_a_namespace_tree()
        {
            MockFor<IFixtureSelectorOrganizer>().VerifyAllExpectations();
        }

        [Test]
        public void should_show_all_the_namespaces_in_the_view()
        {
            MockFor<IWorkspaceEditorView>().AssertWasCalled(x => x.ShowFixtureNamespaces(theSelectors));
        }
    }

    [TestFixture]
    public class when_saving_filters_for_a_suite : WorkspaceEditorContext
    {
        private FixtureFilter _f1;
        private FixtureFilter _f2;
        private FixtureFilter _f3;
        private FixtureFilter _f4;
        private FixtureFilter _f5;
        private IFixtureSelector[] theSelectors;

        protected override void theContextIs()
        {
            theSelectors = new IFixtureSelector[]
            {
                MockRepository.GenerateMock<IFixtureSelector>(),
                MockRepository.GenerateMock<IFixtureSelector>(),
                MockRepository.GenerateMock<IFixtureSelector>()
            };
            ClassUnderTest.Selectors = theSelectors;

            _f1 = FixtureFilter.Namespace("a");
            _f2 = FixtureFilter.Namespace("b");
            _f3 = FixtureFilter.Namespace("c");
            _f4 = FixtureFilter.Namespace("d");
            _f5 = FixtureFilter.Namespace("e");

            theSelectors[0].Expect(x => x.GetFilters()).Return(new[] {_f1});
            theSelectors[1].Expect(x => x.GetFilters()).Return(new[] {_f2, _f3});
            theSelectors[2].Expect(x => x.GetFilters()).Return(new[] {_f4, _f5});


            ClassUnderTest.Save();
        }

        [Test]
        public void should_find_all_the_selected_filters_from_the_remembered_selectors()
        {
            theSelectors.Each(s => s.VerifyAllExpectations());
        }

        [Test]
        public void applies_the_change_in_filters_to_the_project_controller()
        {
            MockFor<IProjectController>().AssertWasCalled(x => x.SaveWorkspace(suite));
        }

        [Test]
        public void should_put_the_filters_from_the_view_onto_the_workspace_filter()
        {
            suite.Filter.Filters.ShouldHaveTheSameElementsAs(_f1, _f2, _f3, _f4, _f5);
        }
    }


    public abstract class WorkspaceEditorContext : InteractionContext<WorkspaceEditor>
    {
        protected FixtureLibrary library;
        protected WorkspaceSuite suite;

        protected override sealed void beforeEach()
        {
            library = new FixtureLibrary();
            Services.Inject(new LibraryContext() {Library = library});

            suite = new WorkspaceSuite("some suite");
            suite.Filter = new WorkspaceFilter();

            Services.Inject(suite);

            theContextIs();
        }

        protected abstract void theContextIs();
    }
}