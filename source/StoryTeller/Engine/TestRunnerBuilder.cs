using System;
using FubuCore.Util;
using StoryTeller.Model;
using StructureMap;

namespace StoryTeller.Engine
{
    public class TestRunnerBuilder
    {
        private readonly ISystem _system;
        private readonly IFixtureObserver _observer;
        private readonly FixtureRegistry _registry;

        public TestRunnerBuilder(ISystem system, IFixtureObserver observer, FixtureRegistry registry)
        {
            _system = system;
            _observer = observer;
            _registry = registry;
        }

        public static ITestRunner ForSystem<T>() where T : ISystem, new()
        {
            var system = new T();
            return ForSystem(system);
        }

        private static ITestRunner ForSystem(ISystem system)
        {
            var registry = new FixtureRegistry();
            registry.AddFixturesFromAssembly(system.GetType().Assembly);

            var builder = new TestRunnerBuilder(system, new NulloFixtureObserver(), registry);
            return builder.Build();
        }

        public static ITestRunner ForFixture<T>() where T : IFixture
        {
            return For(r => r.AddFixture<T>());
        }

        public static ITestRunner For(FixtureRegistry registry)
        {
            return new TestRunnerBuilder(new NulloSystem(), new NulloFixtureObserver(), registry).Build();
        }

        public static ITestRunner For(Action<FixtureRegistry> configure)
        {
            var registry = new FixtureRegistry();
            configure(registry);

            return For(registry);
        }

        public ITestRunner Build()
        {
            var containerSource = new FixtureContainerSource(_registry.BuildContainer());
            IContainer container = containerSource.Build();
            var observer = _observer;

            var library = BuildLibrary(_system, observer, container, new CompositeFilter<Type>());

            return new TestRunner(_system, library, containerSource);
        }

        public static FixtureLibrary BuildLibrary(ISystem system, IFixtureObserver observer, IContainer container, CompositeFilter<Type> filter)
        {
            try
            {
                var builder = new LibraryBuilder(observer, filter);
                observer.RecordStatus("Starting to rebuild the fixture model");

                var context = new TestContext(container);
                system.SetupEnvironment();
                system.RegisterServices(context);

                builder.Finder = context.Finder;
                
                return builder.Build(context);
            }
            finally
            {
                observer.RecordStatus("Finished rebuilding the fixture model");
            }
        }
    }
}