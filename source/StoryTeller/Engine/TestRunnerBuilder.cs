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

        public TestRunnerBuilder(ISystem system, IFixtureObserver observer)
        {
            _system = system;
            _observer = observer;
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

            var builder = new TestRunnerBuilder(system, new NulloFixtureObserver());
            return builder.Build();
        }

        public static ITestRunner ForFixture<T>() where T : IFixture
        {
            return For(r => r.AddFixture<T>());
        }

        public static ITestRunner For(Action<FixtureRegistry> configure)
        {
            return new TestRunnerBuilder(new NulloSystem(configure), new NulloFixtureObserver()).Build();
        }

        public ITestRunner Build()
        {
            var registry = new FixtureRegistry();
            _system.RegisterFixtures(registry);
            var containerSource = new FixtureContainerSource(registry.BuildContainer());
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
                observer.RecordStatus("Setting up the system environment");
                system.SetupEnvironment();

                observer.RecordStatus("Registering the system services");
                system.RegisterServices(context);

                builder.Finder = context.Finder;
                
                observer.RecordStatus("Starting to read fixtures");
                return builder.Build(context);
            }
            finally
            {
                observer.RecordStatus("Finished rebuilding the fixture model");
            }
        }
    }
}