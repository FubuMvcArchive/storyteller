using System;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller.Execution
{
    public class TestRunnerProxy : MarshalByRefObject
    {
        private TestRunner _runner;
        private IEventPublisher _publisher;

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
                if (_runner != null) _runner.RecycleEnvironment();
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
            _runner.Listener = new UserInterfaceTestObserver(_publisher, request);
            return _runner.RunTest(request);
        }

        public FixtureLibrary BuildFixtureLibrary()
        {
            try
            {
                return _runner.Library;
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

        public void AbortCurrentTest()
        {
            _runner.Abort();
        }

        public bool IsExecuting()
        {
            return _runner.IsExecuting();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void StartRunner(string runnerType, MarshalByRefObject remotePublisher)
        {
            // TODO -- if fails, do a Thread.Sleep and try again
            Type type = Type.GetType(runnerType);
            _runner = (TestRunner)Activator.CreateInstance(type);
            _publisher = (IEventPublisher)remotePublisher;
            _runner.FixtureObserver = new FixtureObserver(_publisher);
        }
    }
}