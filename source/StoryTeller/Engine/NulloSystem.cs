using System;

namespace StoryTeller.Engine
{
    public class NulloSystem : ISystem
    {
        public object Get(Type type)
        {
            throw new NotSupportedException("Get<T> is not supported by this ISystem:  " + GetType().FullName);
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