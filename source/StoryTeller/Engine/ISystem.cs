namespace StoryTeller.Engine
{
    public interface ISystem
    {
        T Get<T>() where T : class;
        void RegisterServices(ITestContext context);
        void SetupEnvironment();
        void TeardownEnvironment();
        void Setup();
        void Teardown();
    }
}