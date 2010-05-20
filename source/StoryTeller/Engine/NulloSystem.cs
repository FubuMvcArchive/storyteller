namespace StoryTeller.Engine
{
    public class NulloSystem : ISystem
    {
        public T Get<T>() where T : class
        {
            return null;            
        }

        public void RegisterServices(ITestContext context)
        {
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
}