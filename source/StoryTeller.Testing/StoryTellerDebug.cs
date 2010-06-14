using NUnit.Framework;
using StoryTeller.Execution;
using StoryTeller.Engine;

namespace StoryTellerTestHarness
{
    [TestFixture, Explicit]
    public class Template
    {
        private ProjectTestRunner runner;

        [TestFixtureSetUp]
        public void SetupRunner()
        {
            runner = new ProjectTestRunner(@"C:\svn\bluestoryteller\StoryTeller.xml");
        }

        [Test]
        public void HTML_test_Create_New_Queue()
        {
            var test = runner.RunTest("Admin/HTML test Create New Queue");
            runner.WritePreview(test).OpenInBrowser();
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}