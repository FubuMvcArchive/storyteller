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

        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
        }

        public object Get(Type type)
        {
            throw new NotSupportedException("Get<T> is not supported by this ISystem:  " + GetType().FullName);
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

        public IObjectConverter BuildConverter()
        {
            return new ObjectConverter();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}