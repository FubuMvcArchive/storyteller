using NUnit.Framework;
using StoryTeller.Execution;

namespace StoryTellerTestHarness
{
    [TestFixture, Explicit]
    public class Template
    {
        private ProjectTestRunner runner;

        [TestFixtureSetUp]
        public void SetupRunner()
        {
            runner = new ProjectTestRunner(@"..\..\..\..\samples\grammars.xml");
        }

        [Test]
        public void Composite_with_Errors()
        {
            runner.RunAndAssertTest("Paragraphs/Composite with Errors");
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}