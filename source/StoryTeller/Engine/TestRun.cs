using System;
using System.Diagnostics;
using System.Threading;
using FubuCore;
using StoryTeller.Execution;
using StoryTeller.Html;
using StoryTeller.Model;

namespace StoryTeller.Engine
{
    public class TestRun : ITestRun
    {
        private readonly IFixtureContainerSource _fetchContainer;
        private readonly FixtureLibrary _library;
        private readonly SystemLifecycle _lifecycle;
        private readonly ITestObserver _listener;
        private readonly TestExecutionRequest _request;
        private readonly TestResult _result;
        private TestContext _context;
        private ManualResetEvent _reset;
        private Thread _testThread;

        internal TestRun(TestExecutionRequest request, IFixtureContainerSource fetchContainer, ITestObserver listener,
                         FixtureLibrary library, SystemLifecycle lifecycle)
        {
            _request = request;
            _fetchContainer = fetchContainer;
            _listener = listener;
            _library = library;
            _lifecycle = lifecycle;

            _result = new TestResult();
        }

        #region ITestRun Members

        public void Abort()
        {
            captureException("Test Execution was forcibly aborted");

            _listener.Exception("Test Execution was forcibly aborted");
            if (_testThread != null) _testThread.Abort();
        }

        #endregion

        private void captureException(Exception ex)
        {
            _listener.Exception(ex.ToString());
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

            _context = new TestContext(_fetchContainer.Build(), _request.Test, _listener)
            {
                StartupActionNames = _request.StartupActions ?? new string[0]
            };

            _reset = new ManualResetEvent(false);

            try
            {
                startThread();

                recordTimeout();
            }
            catch (Exception e)
            {
                captureException(e);
                _listener.Exception(e.ToString());
            }

            _testThread = null;

            timer.Stop();

            recordResults(timer);

            return _result;
        }

        private void startThread()
        {
            _testThread = new Thread(() =>
            {
                try
                {
                    _lifecycle.ExecuteContext(_context, executeContext);
                }
                catch (Exception e)
                {
                    captureException(e);
                }
                finally
                {
                    _reset.Set();
                }
            });

            _testThread.SetApartmentState(ApartmentState.STA);
            _testThread.Name = "StoryTeller-Test-Execution";
            _testThread.Start();
        }

        private void recordTimeout()
        {
            bool timedOut = !_reset.WaitOne(_request.TimeoutInSeconds*1000);
            if (timedOut)
            {
                string exception = "Timed Out in {0} seconds".ToFormat(_request.TimeoutInSeconds);
                captureException(exception);
                _listener.Exception(exception);
                _testThread.Abort();
            }
        }


        private void recordResults(Stopwatch timer)
        {
            _result.ExecutionTime = timer.Elapsed.TotalSeconds;
            _result.Counts = _context.Counts;
            _result.ExceptionText = _context.ResultsFor(_request.Test).ExceptionText;
            _result.Html = writeResults();
        }

        private void executeContext()
        {
            try
            {
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
        }

        private string writeResults()
        {
            var writer = new ResultsWriter(_context);
            var parser = new TestParser(_request.Test, writer, _library);
            parser.Parse();

            return writer.Document.ToString();
        }
    }
}