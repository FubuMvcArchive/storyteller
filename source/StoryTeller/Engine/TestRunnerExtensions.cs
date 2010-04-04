using System.Diagnostics;
using System.IO;
using System.Xml;
using HtmlTags;
using StoryTeller.Domain;
using StoryTeller.Examples;
using StoryTeller.Execution;
using StoryTeller.Html;

namespace StoryTeller.Engine
{
    public static class TestRunnerExtensions
    {
        public static TestResult RunTest(this ITestRunner runner, Test test)
        {
            return runner.RunTest(new TestExecutionRequest(test, new TestStopConditions()));
        }

        public static HtmlDocument WritePreview(this ITestRunner runner, Test test)
        {
            return runner.Writer().WritePreview(test);
        }

        internal static HtmlWriter Writer(this ITestRunner runner)
        {
            return new HtmlWriter(runner.Library);
        }

        public static void WriteResultsToFile(this Test test, string fileName)
        {
            File.WriteAllText(fileName, test.LastResult.Html);
        }

        public static void OpenResultsInBrowser(this Test test)
        {
            string fileName = Path.GetTempFileName() + ".htm";
            File.WriteAllText(fileName, test.LastResult.Html);
            Process.Start(fileName);
        }

        public static XmlDocument CreateExampleXml(this ITestRunner runner)
        {
            ExampleSource source = ExampleSource.For(runner);
            return source.ExampleTest.ToXml();
        }

        public static Test CreateExample(this ITestRunner runner)
        {
            return ExampleSource.For(runner).ExampleTest;
        }
    }
}