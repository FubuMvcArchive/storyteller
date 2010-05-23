using System;

namespace StoryTeller.Engine
{
    public abstract class BasicSystem : ISystem
    {
        public virtual T Get<T>() where T : class
        {
            throw new NotSupportedException("Get<T> is not supported by this ISystem:  " + GetType().FullName);
        }

        public virtual void RegisterServices(ITestContext context)
        {
        }

        public virtual void SetupEnvironment()
        {
        }

        public virtual void TeardownEnvironment()
        {
        }

        public virtual void Setup()
        {
        }

        public virtual void Teardown()
        {
        }
    }
}