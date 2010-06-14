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
            runner = new ProjectTestRunner(@"..\..\..\..\samples\grammars.xml");
        }

        [Test]
        public void Embeds()
        {
            var test = runner.RunTest("Embedded/Embeds");
        
            runner.WritePreview(test).OpenInBrowser();
            test.OpenResultsInBrowser();
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}