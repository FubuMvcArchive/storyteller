using System;
using StoryTeller.Engine;

namespace StoryTeller.Samples
{
    public class MathTestRunner : TestRunner
    {
        public MathTestRunner()
            : base(x =>
            {
                x.AddFixture<MathFixture>();
                x.AddFixture<AnotherFixture>();
                x.AddFixture<DoSomeMathFixture>();
            })
        {
        }
    }
}