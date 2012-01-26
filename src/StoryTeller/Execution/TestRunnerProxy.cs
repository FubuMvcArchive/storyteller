using System;
using System.Runtime.Remoting;
using FubuCore.Util;
using StoryTeller.Engine;
using StoryTeller.Model;
using System.Collections.Generic;
using FubuCore;

namespace StoryTeller.Execution
{

    public class TestRunnerProxy : MarshalByRefObject
    {
        private TestRunner _runner;
        private SystemLifecycle _lifecycle;
        private IEventPublisher _publisher;
        private ISystem _system;
        private ITestObserver _listener;

        public void Dispose()
        {
            try
            {
                // TODO -- need to figure about exceptions here
                if (_runner != null) _runner.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public void RecycleEnvironment()
        {
            try
            {
                if (_runner != null) _lifecycle.RecycleEnvironment();
            }
            catch (TestEngineFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TestEngineFailureException(e.ToString());
            }
        }

        public TestResult RunTest(TestExecutionRequest request)
        {
            _runner.Listener = _listener ?? new UserInterfaceTestObserver(_publisher, request);

            return _runner.RunTest(request);
        }

        public void AbortCurrentTest()
        {
            if (_runner != null) _runner.Abort();
        }

        public bool IsExecuting()
        {
            if (_runner != null) return _runner.IsExecuting();

            return false;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public FixtureLibrary StartSystem(FixtureAssembly fixtureAssembly, MarshalByRefObject remotePublisher)
        {
            _publisher = (IEventPublisher)remotePublisher;
            var observer = new FixtureObserver(_publisher);

            // TODO -- if fails, do a Thread.Sleep and try again
            _system = fixtureAssembly.System;

            _lifecycle = new SystemLifecycle(_system);

            // TODO -- make this be async
            observer.RecordStatus("Setting up the environment");
            _lifecycle.StartApplication();

            try
            {
                var container = TestRunnerBuilder.BuildFixtureContainer(_system);
                var registry = new FixtureRegistry();
                _system.RegisterFixtures(registry);
                registry.AddFixturesToContainer(container);

                
                var library = TestRunnerBuilder.BuildLibrary(_lifecycle, observer, container, fixtureAssembly.Filter.CreateTypeFilter(), _system.BuildConverter());
                var source = new FixtureContainerSource(container);
                _runner = new TestRunner(_lifecycle, library, source);
                if (_listener != null)
                {
                    _runner.Listener = _listener;
                }

                return library;
            }
            catch (TestEngineFailureException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new TestEngineFailureException(e.ToString());
            }
        }

        public void UseTeamCityListener()
        {
            _listener = new TeamCityTestListener();
        }
    }
}