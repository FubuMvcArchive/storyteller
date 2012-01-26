using System;
using FubuCore.Conversion;
using StoryTeller.Engine;
using StructureMap;

namespace StoryTeller.Samples
{
    public class GrammarSystem : ISystem
    {
        public T Get<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public object Get(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterServices(ITestContext context)
        {
            throw new NotImplementedException();
        }

        public void SetupEnvironment()
        {
            throw new NotImplementedException();
        }

        public void TeardownEnvironment()
        {
            throw new NotImplementedException();
        }

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public void Teardown()
        {
            throw new NotImplementedException();
        }

        public void RegisterFixtures(FixtureRegistry registry)
        {
            
        }

        public IObjectConverter BuildConverter()
        {
            throw new NotImplementedException();
        }
    }

    public class SetUserAction : IStartupAction
    {
        public void Startup(ITestContext context)
        {
            
        }

        public void Teardown(ITestContext context)
        {
        }
    }


    public class StartWebAppAction : IStartupAction
    {
        public void Startup(ITestContext context)
        {

        }

        public void Teardown(ITestContext context)
        {
        }
    }

}