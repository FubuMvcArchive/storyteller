using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.AcceptanceTesting
{
    [TestFixture, Explicit]
    public class debugging
    {
        [Test]
        public void start_the_runner()
        {
            var runner = new StoryTellerRunner();
            runner.RunTest(new Test("blank"));
        }
    }
}