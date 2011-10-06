using System;
using System.Diagnostics;
using System.Reflection;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace StoryTeller.Engine
{
    public class FixtureRegistry
    {
        private readonly Registry _registry = new Registry();

        public FixtureRegistry()
        {
            _registry.RegisterInterceptor(new FixtureExtender());
        }

        public void AddFixture<T>() where T : IFixture
        {
            _registry.For<IFixture>().Add<T>().Named(typeof(T).GetFixtureAlias());
        }

        public void AliasFixture<T>(string alias) where T : IFixture
        {
            _registry.For<IFixture>().Add<T>().Named(alias);
        }

        public void AddFixturesFromThisAssembly()
        {
            Assembly assembly = findTheCallingAssembly();
            AddFixturesFromAssembly(assembly);
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();

            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }

        public void AddFixturesFromAssembly(Assembly assembly)
        {
            _registry.Scan(x =>
            {
                x.Assembly(assembly);
                x.TheCallingAssembly();
                x.Convention<FixtureScanner>();

                
            });
        }


        public void AddFixturesFromNamespaceContaining<T>()
        {
            _registry.Scan(x =>
            {
                x.AssemblyContainingType<T>();
                x.IncludeNamespaceContainingType<T>();
                x.Convention<FixtureScanner>();
            });
        }

        public void AddFixturesFromAssemblyContaining<T>()
        {
            Assembly assembly = typeof(T).Assembly;
            AddFixturesFromAssembly(assembly);
        }

        public IContainer BuildContainer()
        {
            return new Container(_registry);
        }

        internal void ConfigureContainer(IContainer container)
        {
            container.Configure(x => { x.AddRegistry(_registry); });
        }

        public void AddFixture(IFixture fixture, string name)
        {
            _registry.For<IFixture>().Add(fixture).Named(name);
        }

        public void RegisterServices(Action<Registry> configure)
        {
            configure(_registry);
        }

        public static IContainer ContainerFor(Action<FixtureRegistry> action)
        {
            var registry = new FixtureRegistry();
            action(registry);

            return registry.BuildContainer();
        }
    }
}