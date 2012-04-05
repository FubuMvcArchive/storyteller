using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore.Conversion;
using FubuCore;

namespace StoryTeller.Engine
{
    // TODO -- get tests around this thing.  Used heavy everyday already, but still
    public abstract class BasicSystem : ISystem
    {
        public virtual object Get(Type type)
        {
            if (type.IsConcreteWithDefaultCtor())
            {
                return Activator.CreateInstance(type);
            }

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

        public virtual void RegisterFixtures(FixtureRegistry registry)
        {
            Assembly assembly = GetType().Assembly;
            registry.AddFixturesFromAssembly(assembly);
        }
      
        public IObjectConverter BuildConverter()
        {
            var library = Get(typeof (ConverterLibrary)).As<ConverterLibrary>();
            _converterRegistrations.Each(x => x(library));

            return new ObjectConverter(new InMemoryServiceLocator(), library);
        }


        private readonly IList<Action<ConverterLibrary>> _converterRegistrations = new List<Action<ConverterLibrary>>();

        public void Converters(Action<ConverterLibrary> configuration)
        {
            _converterRegistrations.Add(configuration);
        }


    }


}