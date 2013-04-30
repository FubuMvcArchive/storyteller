using System;
using System.Diagnostics;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Workspace;
using System.Collections.Generic;

namespace StoryTeller.Testing.Integration
{
    [TestFixture]
    public class when_a_project_loads_a_hierarchy
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            project = DataMother.MathProject();
            hierarchy = project.LoadTests();
        }

        #endregion

        private Project project;
        private Hierarchy hierarchy;

        [Test]
        public void the_top_level_suites_should_all_be_workspace_suites()
        {
            hierarchy.ChildSuites.Each(
                x => x.ShouldBeOfType<WorkspaceSuite>().Filter.ShouldBeTheSameAs(project.WorkspaceFor(x.Name)));
        }

        [Test]
        public void all_the_other_suites_below_workspace_should_just_be_suites()
        {
            hierarchy.ChildSuites.Each(x =>
            {
                x.ChildSuites.Each(s => s.ShouldBeOfType<Suite>());
            });
        }

        [Test]
        public void child_suite_has_its_child_suites()
        {
            hierarchy.FindSuite("Mixed").FindSuite("SubMixed").Tests.ShouldHaveTestNames("Sub_Mixed_1", "Sub_Mixed_2",
                                                                                         "SubMixedThatThrows");
        }

        [Test]
        public void child_suite_has_its_tests()
        {
            hierarchy.FindSuite("Adding").Tests.ShouldHaveTestNames("Bad_Add_1", "Good_Add_1", "Good_Add_2");
        }

        [Test, Explicit]
        public void has_the_empty_suite()
        {
            hierarchy.FindSuite("EmptySuite").ShouldNotBeNull();
        }

        [Test, Explicit]
        public void loads_the_suites_at_the_top_level()
        {
            hierarchy.ChildSuites.Each(x => Debug.WriteLine(x.Name));
            hierarchy.ChildSuites.ShouldHaveSuiteNames("Adding", "EmptySuite", "Mixed", "Multiplication");
        }

        [Test]
        public void loads_the_tests_in_the_root_level()
        {
            hierarchy.Tests.ShouldHaveTestNames("Bad_Add_at_Top", "Good_Add_at_Top");
        }

        [Test]
        public void the_name_of_the_hierarchy_matches_the_project()
        {
            hierarchy.Name.ShouldEqual(project.Name);
        }

        [Test]
        public void the_tests_at_the_top_level_have_the_parent_set_to_the_hierarchy()
        {
            foreach (Test test in hierarchy.Tests)
            {
                test.Parent.ShouldBeTheSameAs(hierarchy);
            }
        }
    }


    public static class HierarchyAssertionExtensions
    {
        public static void ShouldHaveTestNames(this Test[] tests, params string[] names)
        {
            string[] actual = Array.ConvertAll(tests, test => test.Name);
            actual.ShouldEqual(names);
        }

        public static void ShouldHaveSuiteNames(this Suite[] suites, params string[] names)
        {
            string[] actual = Array.ConvertAll(suites, s => s.Name);
            actual.ShouldEqual(names);
        }
    }
}