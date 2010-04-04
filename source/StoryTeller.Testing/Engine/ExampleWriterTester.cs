using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    [StoryTeller.Engine.Description("The example fixture")]
    public class ExampleFixture : Fixture
    {
        [StoryTeller.Engine.Description("this method does something")]
        public void DoSomething(string name, int age, double amount)
        {
        }

        public string CheckThisValue(string state, double amount)
        {
            return null;
        }

        public bool IsThisOkay()
        {
            return false;
        }
    }

    [TestFixture]
    public class writing_an_example_for_a_testcontext
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var registry = new FixtureRegistry();
            registry.AliasFixture<ExampleFixture>("Example");
            registry.AliasFixture<ArithmeticFixture>("Arithmetic");


            var runner = new TestRunner(registry);

            test = runner.CreateExample();
        }

        #endregion

        private Test test;

        [Test]
        public void has_a_section_for_both_fixtures()
        {
            IEnumerable<ITestPart> sections = test.Parts.Where(part => part is Section);
            sections.Count().ShouldEqual(2);
        }
    }
}