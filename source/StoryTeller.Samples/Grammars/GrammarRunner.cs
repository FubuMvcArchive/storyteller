using StoryTeller.Engine;

namespace StoryTeller.Samples.Grammars
{
    public class GrammarRunner : TestRunner
    {
        public GrammarRunner()
            : base(x => x.AddFixturesFromThisAssembly())
        {
        }
    }
}