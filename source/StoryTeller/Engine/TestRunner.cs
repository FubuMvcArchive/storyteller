using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using StoryTeller.Domain;
using StoryTeller.Execution;
using StoryTeller.Model;

namespace StoryTeller.Engine
{
    public class LibraryLifecycle
    {
        public LibraryLifecycle()
        {
        }
    }

    public class SystemLifecycle : IDisposable
    {
        private readonly ISystem _system;
        private bool _environmentIsInitialized;
        private readonly object _locker = new object();

        public SystemLifecycle(ISystem system)
        {
            _system = system;
        }

        protected void ensureEnvironmentInitialized()
        {
            if (!_environmentIsInitialized)
            {
                lock (_locker)
                {
                    if (!_environmentIsInitialized)
                    {
                        _system.SetupEnvironment();
                        _environmentIsInitialized = true;
                    }
                }
            }
        }

        public void StartApplication()
        {
            lock (_locker)
            {
                _system.SetupEnvironment();
                _environmentIsInitialized = true;
            }
        }

        protected void tearDownEnvironment()
        {
            lock (_locker)
            {
                _system.TeardownEnvironment();
                _environmentIsInitialized = false;
            }
        }

        public void RecycleEnvironment()
        {
            tearDownEnvironment();
            ensureEnvironmentInitialized();
        }

        public void ExecuteContext(ITestContext context, Action action)
        {
            ensureEnvironmentInitialized();
            _system.RegisterServices(context);

            try
            {
                _system.Setup();

                action();
            }
            finally
            {
                _system.Teardown();
            }
        }

        public void Dispose()
        {
            tearDownEnvironment();
        }
    }

    public class TestRunner : ITestRunner
    {
        private readonly FixtureLibrary _library;
        private readonly IFixtureContainerSource _source;
        private TestRun _currentRun;
        private ITestObserver _listener = new ConsoleListener();
        private readonly SystemLifecycle _lifecycle;


        public TestRunner(ISystem system, FixtureLibrary library, IFixtureContainerSource source)
            : this(new SystemLifecycle(system), library, source)
        {

        }

        public TestRunner(SystemLifecycle lifecycle, FixtureLibrary library, IFixtureContainerSource source)
        {
            _lifecycle = lifecycle;
            _library = library;
            _source = source; 
        }

        #region ITestRunner Members

        public FixtureLibrary Library
        {
            get { return _library; }
        }

        public SystemLifecycle Lifecycle
        {
            get { return _lifecycle; }
        }

        public ITestObserver Listener
        {
            get { return _listener; }
            set { _listener = value; }
        }

        public virtual void RunTests(IEnumerable<Test> tests, IBatchListener listener)
        {
            var batch = new BatchRunner(tests, listener, this);
            batch.Execute();
        }

        public virtual TestResult RunTest(TestExecutionRequest request)
        {
            try
            {
                _currentRun = new TestRun(request, _source, _listener, _library, _lifecycle);

                // Setting the LastResult on the test here is just a convenience
                // for testing
                TestResult result = _currentRun.Execute();
                request.Test.LastResult = result;

                return result;
            }
            finally
            {
                _currentRun = null;
            }
        }


        public void Dispose()
        {
            _lifecycle.Dispose();
        }

        public void Abort()
        {
            if (_currentRun != null) _currentRun.Abort();
        }

        #endregion

        public bool IsExecuting()
        {
            return _currentRun != null;
        }
    }
}