using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FubuCore;
using StoryTeller.Domain;
using StoryTeller.Execution;
using StoryTeller.Html;
using StoryTeller.Model;
using StructureMap;

namespace StoryTeller.Engine
{
    public class TestRunner : ITestRunner
    {
        private readonly FixtureRegistry _registry;
        private TestRun _currentRun;
        private bool _environmentIsInitialized;
        private IFixtureObserver _fixtureObserver = new NulloFixtureObserver();
        private FixtureLibrary _library;
        private ITestObserver _listener = new ConsoleListener();

        public TestRunner(FixtureRegistry registry)
        {
            _registry = registry;
        }

        public TestRunner(Action<FixtureRegistry> action)
        {
            _registry = new FixtureRegistry();
            action(_registry);
        }

        protected FixtureRegistry registry { get { return _registry; } }
        
        public IFixtureObserver FixtureObserver { get { return _fixtureObserver; } set { _fixtureObserver = value; } }

        #region ITestRunner Members

        public FixtureLibrary Library
        {
            get
            {
                if (_library == null)
                {
                    _library = buildLibrary();
                }

                return _library;
            }
        }


        public ITestObserver Listener { get { return _listener; } set { _listener = value; } }

        public virtual void RunTests(IEnumerable<Test> tests, IBatchListener listener)
        {
            var batch = new BatchRunner(tests, listener, this);
            batch.Execute();
        }

        public virtual TestResult RunTest(TestExecutionRequest request)
        {
            try
            {
                _currentRun = new TestRun(request, this);

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
            tearDownEnvironment();
        }

        #endregion

        protected void logTraceMessage(string message)
        {
            try
            {
                _fixtureObserver.RecordStatus(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RecycleEnvironment()
        {
            tearDownEnvironment();
            setUpEnvironment();
        }

        public void Abort()
        {
            if (_currentRun != null) _currentRun.Abort();
        }

        public void DebugContainer()
        {
            Debug.WriteLine(_registry.BuildContainer().WhatDoIHave());
        }


        private FixtureLibrary buildLibrary()
        {
            try
            {
                var builder = new LibraryBuilder(_fixtureObserver);
                logTraceMessage("Starting to rebuild the fixture model");

                var context = new TestContext(_registry);
                registerServices(context);

                builder.Finder = context.Finder;
                return builder.Build(context);
            }
            finally
            {
                logTraceMessage("Finished rebuilding the fixture model");
            }
        }

        public static TestRunner For<T>() where T : IFixture
        {
            return new TestRunner(x => x.AddFixture<T>());
        }


        protected virtual void setUp(ITestContext context)
        {
            // no-op
        }

        protected virtual void tearDown(ITestContext context)
        {
            // no-op
        }

        protected virtual void setUpEnvironment()
        {
            // no-op
        }

        protected virtual void tearDownEnvironment()
        {
            // no-op
        }

        protected virtual void registerServices(ITestContext context)
        {
            // no-op
        }

        public bool IsExecuting()
        {
            return _currentRun != null;
        }

        #region Nested type: TestRun

        internal class TestRun : ITestRun
        {
            private readonly TestRunner _runner;
            private readonly TestExecutionRequest _request;
            private Thread _testThread;
            private ManualResetEvent reset;
            private readonly TestContext _context;
            private TestResult _result;

            internal TestRun(TestExecutionRequest request, TestRunner runner)
            {
                _request = request;
                _runner = runner;

                _runner.logTraceMessage("Finding Fixtures");

                _result = new TestResult();

                IContainer container = _runner._registry.BuildContainer();
                _context = new TestContext(container, _request.Test, _runner.Listener);
            }

            #region ITestRun Members

            public void Abort()
            {
                captureException("Test Execution was forcibly aborted");

                _runner._listener.Exception("Test Execution was forcibly aborted");
                if (_testThread != null) _testThread.Abort();
            }

            #endregion

            private void captureException(Exception ex)
            {
                _runner.Listener.Exception(ex.ToString());
                captureException(ex.ToString());
            }

            private void captureException(string exceptionText)
            {
                _context.IncrementExceptions();
                _context.ResultsFor(_request.Test).CaptureException(exceptionText);
            }

            internal TestResult Execute()
            {
                Stopwatch timer = Stopwatch.StartNew();

                try
                {
                    reset = new ManualResetEvent(false);

                    _testThread = new Thread(() =>
                    {
                        try
                        {
                            setupContext();

                            _runner.logTraceMessage("Executing the test");
                            _context.Execute();
                        }
                        catch (ThreadAbortException)
                        {
                            // do nothing, it's logged elsewhere
                        }
                        catch (Exception e)
                        {
                            captureException(e);
                        }

                        reset.Set();
                    });

                    _testThread.SetApartmentState(ApartmentState.STA);
                    _testThread.Name = "StoryTeller-Test-Execution";

                    _testThread.Start();

                    bool timedOut = !reset.WaitOne(_request.TimeoutInSeconds*1000);
                    if (timedOut)
                    {
                        string exception = "Timed Out in {0} seconds".ToFormat(_request.TimeoutInSeconds);
                        captureException(exception);
                        _runner._listener.Exception(exception);
                        _testThread.Abort();
                    }
                }
                catch (Exception e)
                {
                    captureException(e);
                    _runner.Listener.Exception(e.ToString());
                }
                finally
                {
                    performTeardown();

                }

                timer.Stop();

                _result.ExecutionTime = timer.Elapsed.TotalSeconds;
                _result.Counts = _context.Counts;
                _result.ExceptionText = _context.ResultsFor(_request.Test).ExceptionText;
                _result.Html = writeResults();

                return _result;
            }

            private void setupContext()
            {
                _runner.registerServices(_context);

                if (!_runner._environmentIsInitialized)
                {
                    _runner.logTraceMessage("Setting up environment");
                    _runner.setUpEnvironment();
                    _runner._environmentIsInitialized = true;
                }

                _runner.logTraceMessage("Setting up the testing context");
                _runner.setUp(_context);
            }

            private string writeResults()
            {
                var writer = new ResultsWriter(_context);
                var parser = new TestParser(_request.Test, writer, _runner.Library);
                parser.Parse();

                return writer.Document.ToString();
            }

            private void performTeardown()
            {
                if (_context != null)
                {
                    try
                    {
                        _runner.tearDown(_context);
                    }
                    catch (Exception e)
                    {
                        captureException(e);
                    }
                }

                _testThread = null;
            }
        }

        #endregion
    }
}