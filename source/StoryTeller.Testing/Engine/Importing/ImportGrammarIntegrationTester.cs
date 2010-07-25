using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Engine.Importing;

namespace StoryTeller.Testing.Engine.Importing
{
    [TestFixture]
    public class ImportGrammarIntegrationTester
    {
        [SetUp]
        public void SetUp()
        {
            StateFixture.RunningTotal = 0;
        }

        private void runTest(string key, string stepValues)
        {
            StateFixture.RunningTotal = 0;
            var test = new Test("something");
            test.Add(new Section("Imports").WithStep(key, stepValues));

            TestUtility.RunTest(test);
        }

        [Test]
        public void run_a_simple_imported_grammar()
        {
            runTest("SetTo", "value:11");
            StateFixture.RunningTotal.ShouldEqual(11);
        }

        [Test]
        public void run_a_curried_grammar()
        {
            runTest("SetTo12", "");
            StateFixture.RunningTotal.ShouldEqual(12);
        }
    }

    public class ImportsFixture : Fixture
    {
        public ImportsFixture()
        {
            this["SetTo"] = Import<StateFixture>("SetTo");
            this["SetTo12"] = Import<StateFixture>("SetTo").Curry(new CurryAction(){
                Template = "Set to 12",
                DefaultValues = "value:12"
            });
        }
    }

    public class StateFixture : Fixture
    {
        public static int RunningTotal;

    
        public void SetTo(int value)
        {
            RunningTotal = value;
        }

        public void AddNumbers(int x, int y, int z)
        {
            RunningTotal += x + y + z;
        }

        public void AddAndMultiply(int x, int y)
        {
            RunningTotal += x;
            RunningTotal += y;
        }
    }
}