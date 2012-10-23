using System;
using FubuCore.Conversion;
using FubuCore.Util;
using StoryTeller.Model;
using StructureMap;
using FubuCore;

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
            var builder = new TestRunnerBuilder(system, new NulloFixtureObserver());
            return builder.Build();
        }

        public ITestRunner Build()
        {
            throw new NotImplementedException();
//            var container = BuildFixtureContainer(_system);
//            var registry = new FixtureRegistry();
//            _system.RegisterFixtures(registry);
//            registry.AddFixturesToContainer(container);
//            var source = new FixtureContainerSource(container);
//            var nestedContainer = source.Build();
//            var observer = _observer;
//
//            var library = BuildLibrary(new SystemLifecycle(_system), observer, nestedContainer);
//            
//            return new TestRunner(_system, library, source);
        }

        // TODO -- remove the composite filter thing
        public static FixtureLibrary BuildLibrary(ISystem lifeCycle, IFixtureObserver observer)
        {

            throw new NotImplementedException();
//            try
//            {
//                var builder = new LibraryBuilder(observer);
//                observer.RecordStatus("Starting to rebuild the fixture model");
//
//                var context = new TestContext(container);
//
//                observer.RecordStatus("Starting to read fixtures");
//                return builder.Build(context);
//            }
//            finally
//            {
//                observer.RecordStatus("Finished rebuilding the fixture model");
//            }
        }

        public static ITestRunner ForFixture<T>()
        {
            throw new NotImplementedException();
        }
    }
}