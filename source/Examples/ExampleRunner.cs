using System;
using StoryTeller.Engine;

namespace Examples
{
    public class ExampleRunner : TestRunner
    {
        public ExampleRunner()
            : base(x => x.AddFixturesFromThisAssembly())
        {
        }

        protected override void registerServices(ITestContext context)
        {
            context.Store<IBrowserDriver>(new SeleniumBrowserDriver());
        }
    }

    public interface IBrowserDriver
    {
        void OpenUrl(string url);
    }

    public class SeleniumBrowserDriver : IBrowserDriver
    {
        public void OpenUrl(string url)
        {
            // open the url
        }
    }
}