using System;
using StoryTeller.Engine;

namespace Examples
{
    public class ExampleSystem : ISystem
    {
        public T Get<T>() where T : class
        {
            return null;
        }

        public void RegisterServices(ITestContext context)
        {
            context.Store<IBrowserDriver>(new SeleniumBrowserDriver());
        }

        public void SetupEnvironment()
        {

        }

        public void TeardownEnvironment()
        {
        }

        public void Setup()
        {
        }

        public void Teardown()
        {
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