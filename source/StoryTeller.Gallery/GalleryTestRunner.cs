using StoryTeller.Engine;

namespace StoryTeller.Gallery
{
    public class SystemUnderTest
    {
        public void CleanUp()
        {
        }
    }

    // Before I do anything else, 
    public class GalleryTestRunner : TestRunner
    {
        private SystemUnderTest _system;

        // Automatically scans the containing assembly for any public classes
        // that implement IFixture and makes them available for use
        public GalleryTestRunner()
            : base(x => x.AddFixturesFromThisAssembly())
        {
        }

        protected override void setUp(ITestContext context)
        {
            // Do any necessary bootstrapping just before a test run
            // ITestContext is effectively an IoC container, so you 
            // might be registering your application services here
            _system = new SystemUnderTest();
            context.Store(_system);
        }

        protected override void tearDown(ITestContext context)
        {
            // Do any post-Test run cleanup
            _system.CleanUp();
        }

        protected override void setUpEnvironment()
        {
            // This method runs once before the very first test.
            // We use this method to spin up the Selenium RC
            // proxy server and Selenium session
        }

        protected override void tearDownEnvironment()
        {
            // This method runs once after all tests are executed
        }
    }
}