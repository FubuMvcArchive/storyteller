using System;
using System.Reflection;
using FubuCore.Conversion;
using StructureMap;

namespace StoryTeller.Engine
{
    public class NulloSystem : ISystem
    {
        private readonly Action<FixtureRegistry> _configure;

        public NulloSystem(Action<FixtureRegistry> configure)
        {
            _configure = configure;
        }

        public NulloSystem(Assembly assembly) : this(r => r.AddFixturesFromAssembly(assembly))
        {
            
        }

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

        public void RegisterFixtures(FixtureRegistry registry)
        {
            _configure(registry);
        }

        public IContainer BuildFixtureContainer()
        {
            return new Container();
        }

        public IObjectConverter BuildConverter()
        {
            return new ObjectConverter();
        }
    }
}