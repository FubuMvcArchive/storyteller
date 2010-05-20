namespace StoryTeller.Engine
{
    public interface IStartupAction
    {
        void Startup(ITestContext context);
        void Teardown(ITestContext context);
    }
}