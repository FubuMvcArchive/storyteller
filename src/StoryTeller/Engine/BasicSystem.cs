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
        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
            throw new NotImplementedException();
        }

        public virtual object Get(Type type)
        {
            if (type.IsConcreteWithDefaultCtor())
            {
                return Activator.CreateInstance(type);
            }

            throw new NotSupportedException("Get<T> is not supported by this ISystem:  " + GetType().FullName);
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

            return new ObjectConverter(new DelegatingServiceLocator(Get), library);
        }


        private readonly IList<Action<ConverterLibrary>> _converterRegistrations = new List<Action<ConverterLibrary>>();

        public void Converters(Action<ConverterLibrary> configuration)
        {
            _converterRegistrations.Add(configuration);
        }


        public void Dispose()
        {
            
        }
    }

    public class DelegatingServiceLocator : IServiceLocator
    {
        private readonly Func<Type, object> _finder;

        public DelegatingServiceLocator(Func<Type, object> finder)
        {
            _finder = finder;
        }

        public T GetInstance<T>()
        {
            return (T) GetInstance(typeof (T));
        }

        public object GetInstance(Type type)
        {
            return _finder(type);
        }

        public T GetInstance<T>(string name)
        {
            return (T) GetInstance(typeof (T));
        }
    }


}