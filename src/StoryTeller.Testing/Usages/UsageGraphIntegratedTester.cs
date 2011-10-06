using System.Collections.Generic;
using NUnit.Framework;
using StoryTeller.Execution;
using StoryTeller.Usages;
using System.Linq;

namespace StoryTeller.Testing.Usages
{
    [TestFixture]
    public class UsageGraphIntegratedTester
    {
        private ProjectTestRunner runner;
        private UsageGraph usages;

        [SetUp]
        public void SetUp()
        {
            runner = new ProjectTestRunner(DataMother.THE_GRAMMAR_FILE);
            usages = new UsageGraph(runner.GetLibary(), new ConsoleUsageGraphListener());
            usages.Rebuild(runner.Hierarchy);
        }

        [Test]
        public void find_fixture_usage_for_a_workspace()
        {
            var enumerable = usages.FixturesFor("Tables").Select(x => x.Name).ToList();
            enumerable.Sort();
            enumerable.ShouldHaveTheSameElementsAs("DataTable", "Table");
        }


        [Test]
        public void find_fixture_usage_for_a_workspace_including_paragraphs()
        {
            usages.FixturesFor("Paragraphs").Select(x => x.Name).ShouldHaveTheSameElementsAs("Composite");
        }

        [Test]
        public void find_fixture_usage_for_a_workspace_including_embedded_sections()
        {
            usages.FixturesFor("Embedded").Select(x => x.Name).ShouldHaveTheSameElementsAs("Embedded", "Math");
        }
    }
}