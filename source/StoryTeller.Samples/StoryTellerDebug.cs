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
        public void Composite_with_Errors()
        {
            runner.OpenOption = OpenHtmlOption.Always;
            var test = runner.RunTest("Paragraphs/Composite with Errors");
            runner.WritePreview(test).OpenInBrowser();
        }

        [TestFixtureTearDown]
        public void TeardownRunner()
        {
            runner.Dispose();
        }
    }
}