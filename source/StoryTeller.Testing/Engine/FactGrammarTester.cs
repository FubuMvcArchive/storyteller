using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class FactGrammarTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            returnsTrue = false;
            grammar = new FactGrammar(() => returnsTrue, theText);
        }

        #endregion

        private bool returnsTrue;
        private FactGrammar grammar;
        private const string theText = "the text stating this fact";

        [Test]
        public void run_when_the_fact_is_false()
        {
            returnsTrue = false;
            grammar.Execute(new Step()).ShouldEqual(0, 1, 0, 0);
        }

        [Test]
        public void run_when_the_fact_is_true()
        {
            returnsTrue = true;
            grammar.Execute(new Step()).ShouldEqual(1, 0, 0, 0);
        }
    }
}