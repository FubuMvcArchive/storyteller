using System;
using NUnit.Framework;
using StoryTeller.Domain;
using System.Linq;
using StoryTeller.Workspace;

namespace StoryTeller.Testing.Domain
{
    [TestFixture]
    public class TestFilterTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            unknownAcceptanceTest = DataMother.TestWithNoResults().LifecycleIs(Lifecycle.Acceptance);
            unknownRegressionTest = DataMother.TestWithNoResults().LifecycleIs(Lifecycle.Regression);
            failedAcceptanceTest = DataMother.FailedTest().LifecycleIs(Lifecycle.Acceptance);
            failedRegressionTest = DataMother.FailedTest().LifecycleIs(Lifecycle.Regression);
            successfulAcceptanceTest = DataMother.SuccessfulTest().LifecycleIs(Lifecycle.Acceptance);
            successfulRegressionTest = DataMother.SuccessfulTest().LifecycleIs(Lifecycle.Regression);

            filter = new TestFilter();
        }

        #endregion

        private Test unknownAcceptanceTest;
        private Test unknownRegressionTest;
        private Test failedAcceptanceTest;
        private Test failedRegressionTest;
        private Test successfulAcceptanceTest;
        private Test successfulRegressionTest;
        private TestFilter filter;

        [Test]
        public void by_default_the_lifecycle_is_any()
        {
            filter.Lifecycle.ShouldEqual(Lifecycle.Any);
        }

        [Test]
        public void by_default_the_status_is_all()
        {
            filter.ResultStatus.ShouldEqual(ResultStatus.All);
        }

        [Test]
        public void do_not_show_empty_suites_when_a_result_status_is_selected()
        {
            filter.Lifecycle = Lifecycle.Any;
            filter.ResultStatus = ResultStatus.Success;

            filter.ShowEmptySuites().ShouldBeFalse();
        }

        [Test]
        public void do_not_show_empty_suites_when_lifecyle_is_set()
        {
            filter.Lifecycle = Lifecycle.Acceptance;
            filter.ResultStatus = ResultStatus.All;

            filter.ShowEmptySuites().ShouldBeFalse();
        }

        [Test]
        public void look_for_failed_regression_tests()
        {
            filter.Lifecycle = Lifecycle.Regression;
            filter.ResultStatus = ResultStatus.Failed;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void look_for_successful_acceptance_tests()
        {
            filter.Lifecycle = Lifecycle.Acceptance;
            filter.ResultStatus = ResultStatus.Success;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void only_look_for_acceptance_tests()
        {
            filter.Lifecycle = Lifecycle.Acceptance;

            filter.Matches(unknownAcceptanceTest).ShouldBeTrue();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeTrue();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void only_look_for_failed_tests()
        {
            filter.ResultStatus = ResultStatus.Failed;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeTrue();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void only_look_for_regression_tests()
        {
            filter.Lifecycle = Lifecycle.Regression;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeTrue();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeTrue();
        }

        [Test]
        public void only_look_for_successful_tests()
        {
            filter.ResultStatus = ResultStatus.Success;

            filter.Matches(unknownAcceptanceTest).ShouldBeFalse();
            filter.Matches(unknownRegressionTest).ShouldBeFalse();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeTrue();
        }

        [Test]
        public void only_look_for_unknown_tests()
        {
            filter.ResultStatus = ResultStatus.Unknown;

            filter.Matches(unknownAcceptanceTest).ShouldBeTrue();
            filter.Matches(unknownRegressionTest).ShouldBeTrue();
            filter.Matches(failedAcceptanceTest).ShouldBeFalse();
            filter.Matches(failedRegressionTest).ShouldBeFalse();
            filter.Matches(successfulAcceptanceTest).ShouldBeFalse();
            filter.Matches(successfulRegressionTest).ShouldBeFalse();
        }

        [Test]
        public void show_empty_suites_when_any_lifecycle_and_status_is_all()
        {
            filter.Lifecycle = Lifecycle.Any;
            filter.ResultStatus = ResultStatus.All;

            filter.ShowEmptySuites().ShouldBeTrue();
        }

        [Test]
        public void the_initial_filter_should_match_all_tests()
        {
            filter.Matches(unknownAcceptanceTest).ShouldBeTrue();
            filter.Matches(unknownRegressionTest).ShouldBeTrue();
            filter.Matches(failedAcceptanceTest).ShouldBeTrue();
            filter.Matches(failedRegressionTest).ShouldBeTrue();
            filter.Matches(successfulAcceptanceTest).ShouldBeTrue();
            filter.Matches(successfulRegressionTest).ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_combining_filters_with_workspace_filters
    {
        private Hierarchy hierarchy;
        private TestFilter filter;

        [SetUp]
        public void SetUp()
        {
            hierarchy =
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

            filter = new TestFilter();
        }

        private void matchingTestsShouldBe(params string[] names)
        {
            Array.Sort(names);

            hierarchy.GetAllTests().Where(t => filter.Matches(t)).OrderBy(t => t.Name).Select(t => t.Name)
                .ShouldHaveTheSameElementsAs(names);
        }

        [Test]
        public void no_workspace_filter_and_failure()
        {
            filter.Workspaces = new string[0];
            filter.ResultStatus = ResultStatus.Failed;
            matchingTestsShouldBe("t2", "t6", "t8", "t12", "t18");
        }

        [Test]
        public void one_workspace_filter()
        {
            filter.Workspaces = new string[] {"s9"};
            matchingTestsShouldBe("t16", "t17", "t18");
        }

        [Test]
        public void multiple_workspace_filter()
        {
            filter.Workspaces = new string[] {"s5", "s9"};
            matchingTestsShouldBe("t12", "t13", "t14", "t15", "t16", "t17", "t18");
        }

        [Test]
        public void multiple_workspace_filter_and_result_filter()
        {
            filter.Workspaces = new string[] { "s5", "s9" };
            filter.ResultStatus = ResultStatus.Failed;
            matchingTestsShouldBe("t12", "t18");
        }
    }

    [TestFixture]
    public class when_matching_suites
    {
        private TestFilter filter;
        private WorkspaceSuite suite1;
        private WorkspaceSuite suite2;
        private WorkspaceSuite suite3;

        [SetUp]
        public void SetUp()
        {
            filter = new TestFilter();
            suite1 = new WorkspaceSuite("a");
            suite2 = new WorkspaceSuite("b");
            suite3 = new WorkspaceSuite("c");
        }

        private void theSelectedWorkspacesAre(params string[] names)
        {
            filter.Workspaces = names;
        }

        [Test]
        public void a_plain_suite_always_matches()
        {
            var suite = new Suite("plain");

            filter.Matches(suite);

            theSelectedWorkspacesAre("a");

            filter.Matches(suite);
        }

        [Test]
        public void all_suites_match_when_there_are_no_selected_workspaces()
        {
            // no selected workspaces
            theSelectedWorkspacesAre();

            filter.Matches(suite1).ShouldBeTrue();
            filter.Matches(suite2).ShouldBeTrue();
            filter.Matches(suite3).ShouldBeTrue();
        }

        [Test]
        public void for_one_specific_workspace_selected()
        {
            theSelectedWorkspacesAre("b");

            filter.Matches(suite1).ShouldBeFalse();
            filter.Matches(suite2).ShouldBeTrue();
            filter.Matches(suite3).ShouldBeFalse();
        }

        [Test]
        public void for_two_workspaces_selected()
        {
            theSelectedWorkspacesAre("a", "b");

            filter.Matches(suite1).ShouldBeTrue();
            filter.Matches(suite2).ShouldBeTrue();
            filter.Matches(suite3).ShouldBeFalse();
        }



        [Test]
        public void METHOD()
        {

        }

    }

}