using System;
using System.Collections.Generic;
using NUnit.Framework;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Engine.Constraints;
using StoryTeller.Model;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class TestRunnerTester : AAAMockingContext<TestContext>
    {
        [Test]
        public void call_both_setup_and_teardown()
        {
            var test = new Test("something");
            var runner = new MockTestRunner();

            runner.RunTest(test);

            runner.AssertSetUpAndTearDownWereExecutedInCorrectOrder();
        }

        [Test]
        public void dispose_should_call_the_environment_teardown()
        {
            bool wasTorndown = false;
            var runner = new MockTestRunner();

            runner.EnvironmentTeardown = () => wasTorndown = true;

            runner.Dispose();

            wasTorndown.ShouldBeTrue();
        }

        [Test]
        public void environment_setup_should_only_be_called_for_the_very_first_test_run()
        {
            var test = new Test("something");
            var runner = new MockTestRunner();

            runner.EnvironmentSetUpRunCount.ShouldEqual(0);


            runner.RunTest(test);
            runner.EnvironmentSetUpRunCount.ShouldEqual(1);

            runner.RunTest(test);
            runner.EnvironmentSetUpRunCount.ShouldEqual(1);

            runner.RunTest(test);
            runner.EnvironmentSetUpRunCount.ShouldEqual(1);
        }

        [Test]
        public void run_a_test_when_setup_blows_up_do_not_rethrow_exception_and_log_the_exception_to_the_test()
        {
            var test = new Test("Some test");
            var runner = new TestRunnerThatBlowsUpInSetup();

            runner.RunTest(test);

            test.LastResult.ExceptionText.ShouldContain("NotImplementedException");
        }
    }


    public class MockTestRunner : TestRunner, IGrammar, IFixture
    {
        private int _environmentSetUpRunCount;
        private bool _setupRan;
        private bool _tearDownRan;
        public Action EnvironmentTeardown = () => { };

        public MockTestRunner()
            : base(new FixtureRegistry())
        {
            registry.AddFixture(this, "Mock");
        }

        public int EnvironmentSetUpRunCount { get { return _environmentSetUpRunCount; } }

        public IEnumerable<IGrammar> Grammars { get { throw new NotImplementedException(); } }

        #region IFixture Members

        public string Title { get; set; }

        public string Name { get { throw new NotImplementedException(); } }

        public void ForEachGrammar(Action<string, IGrammar> action)
        {
        }

        public void SetUp(ITestContext context)
        {
        }

        public void TearDown()
        {
        }

        public IPolicies Policies { get { return new Policies(); } }

        public IEnumerable<GrammarError> Errors { get { throw new NotImplementedException(); } }

        public IGrammar this[string key] { get { return this; } set { throw new NotImplementedException(); } }

        #endregion

        #region IGrammar Members

        public void Execute(IStep containerStep, ITestContext context)
        {
            _setupRan.ShouldBeTrue();
            _tearDownRan.ShouldBeFalse();
        }

        public GrammarStructure ToStructure(FixtureLibrary library)
        {
            throw new NotImplementedException();
        }

        public string Description { get { return string.Empty; } }

        #endregion

        protected override void setUpEnvironment()
        {
            _environmentSetUpRunCount++;
        }

        protected override void tearDownEnvironment()
        {
            EnvironmentTeardown();
        }

        public IList<Cell> GetCells()
        {
            throw new NotImplementedException();
        }

        public IGrammar FindGrammar(string key)
        {
            return this;
        }

        public void AssertSetUpAndTearDownWereExecutedInCorrectOrder()
        {
            _tearDownRan.ShouldBeTrue();
        }

        protected override void setUp(ITestContext context)
        {
            _setupRan = true;
        }

        protected override void tearDown(ITestContext context)
        {
            _setupRan.ShouldBeTrue();
            _tearDownRan = true;
        }
    }

    public class GrammarThatAssertsFailure : IGrammar
    {
        #region IGrammar Members

        public void Execute(IStep containerStep, ITestContext context)
        {
            StoryTellerAssert.Fail("I don't want to run");
        }

        public GrammarStructure ToStructure(FixtureLibrary library)
        {
            throw new NotImplementedException();
        }

        public string Description { get { throw new NotImplementedException(); } }

        #endregion
    }

    public class TestRunnerThatBlowsUpInSetup : TestRunner
    {
        public TestRunnerThatBlowsUpInSetup()
            : base(new FixtureRegistry())
        {
        }

        protected override void setUp(ITestContext context)
        {
            throw new NotImplementedException();
        }
    }
}