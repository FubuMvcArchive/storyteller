using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class BatchRunnerTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            tests = new List<Test>
            {
                new Test("1"),
                new Test("2"),
                new Test("3")
            };
            mocks = new MockRepository();
            runner = mocks.StrictMock<TestRunner>(new FixtureRegistry());
            listener = mocks.StrictMock<IBatchListener>();

            batch = new BatchRunner(tests, listener, runner);
        }

        #endregion

        private List<Test> tests;
        private MockRepository mocks;
        private TestRunner runner;
        private IBatchListener listener;
        private BatchRunner batch;

        [Test]
        public void executing_the_batch_runs_each_test_and_calls_the_listener_before_and_after()
        {
            using (mocks.Record())
            {
                using (mocks.Ordered())
                {
                    foreach (Test test in tests)
                    {
                        listener.StartingTest(test);
                        runner.RunTest(test);
                        LastCall.Return(new TestResult());

                        listener.FinishedTest(test);
                    }
                }
            }

            using (mocks.Playback())
            {
                batch.Execute();
            }
        }
    }
}