using System;
using System.Threading;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Workspace;
using FubuCore;

namespace StoryTeller.Execution
{
    public enum OpenHtmlOption
    {
        Always,
        Never,
        FailureOnly
    }

    public class ProjectTestRunner : IDisposable
    {
        private IProject _project;
        private TestEngine _engine;
        private Hierarchy _hierarchy;
        private OpenHtmlOption _openOption = OpenHtmlOption.Always;

        public ProjectTestRunner(string projectFile)
        {
            LoadProject(projectFile);
        }

        public void LoadProject(string projectFile)
        {
            if (_project != null)
            {
                Dispose();
            }

            _project = Project.LoadFromFile(projectFile);
            _engine = new TestEngine();  
            _engine.Handle(new ProjectLoaded(_project));
            

            _hierarchy = _project.LoadTests();
        }

        public TestStopConditions StopConditions
        {
            get
            {
                return _engine.StopConditions;
            }
        }

        public Test RunTest(string testPath)
        {
            Test test = FindTest(testPath);

            _engine.RunTest(test);


            return test;
        }

        public Test FindTest(string testPath)
        {
            var test = _hierarchy.FindTest(testPath);

            if (test == null) throw new ArgumentOutOfRangeException("Test {0} cannot be found".ToFormat(testPath));
            return test;
        }

        public void RunAndAssertTest(string testPath)
        {
            var test = RunTest(testPath);

            bool shouldOpen = shouldOpenTest(test.LastResult);
            if (shouldOpen)
            {
                test.OpenResultsInBrowser();
            }

            if (!test.WasSuccessful())
            {
                throw new StorytellerAssertionException(test.GetStatus());
            }
        }

        private bool shouldOpenTest(TestResult testResult)
        {
            switch (_openOption)
            {
                case (OpenHtmlOption.Always):
                    return true;

                case (OpenHtmlOption.FailureOnly):
                    return !testResult.Counts.WasSuccessful();

                case (OpenHtmlOption.Never):
                    return false;
            }

            return false;
        }

        public void Dispose()
        {
            _engine.Dispose();
            _engine = null;
        }
    }

    


    public class TestEngine : ITestEngine
                              , IListener<ProjectLoaded>
                              , IListener<ForceBinaryRecycle>
                              , IListener<ForceEnvironmentRecycle>
    {
        private readonly ITestRunnerDomain _domain;
        private readonly TestStopConditions _stopConditions;
        private readonly ManualResetEvent _latch = new ManualResetEvent(true);
        private readonly object _locker = new object();
        private readonly FixtureLibraryWatcher _watcher;

        private IProject _project;


        public TestEngine()
            : this(new TestRunnerDomain(new NulloEventPublisher()), new TestStopConditions())
        {
        }

        public TestEngine(ITestRunnerDomain domain, TestStopConditions stopConditions)
        {
            _domain = domain;
            _stopConditions = stopConditions;
            _watcher = new FixtureLibraryWatcher(() => reload());
        }

        public FixtureLibrary Library
        {
            get
            {
                return _domain.Library;
            }
        }

        public void Handle(ForceBinaryRecycle message)
        {
            reload();
        }

        public void Handle(ForceEnvironmentRecycle message)
        {
            _domain.RecycleEnvironment();
        }

        public void Handle(ProjectLoaded message)
        {
            _project = message.Project;
            _watcher.WatchBinariesAt(_project.GetBinaryFolder());

            reload();
        }

        public IProject Project { get { return _project; } set { _project = value; } }



        public void RunTest(Test test)
        {
            _latch.WaitOne(30000);

            // This shouldn't happen, but of course it does, so we'll
            // help the system recover from blowups
            if (!_domain.HasStarted())
            {
                performReload();
            }

            // TODO -- exception handling.  Not sure why it needs to be here,
            // but stuff is leaking through
            lock (_locker)
            {
                TestExecutionRequest request = GetExecutionRequest(test);
                var result = _domain.RunTest(request);
                if (!result.WasCancelled)
                {
                    test.LastResult = result;
                }
            }
        }

        public TestExecutionRequest GetExecutionRequest(Test test)
        {
            var request = new TestExecutionRequest(test, _stopConditions);
            string workspaceName = test.WorkspaceName;
            if (workspaceName.IsNotEmpty())
            {
                request.StartupActions = Project.WorkspaceFor(workspaceName).StartupActions ?? new string[0];
            }

            return request;
        }

        public TestStopConditions StopConditions
        {
            get { return _stopConditions; }
        }

        public void AbortCurrentTest()
        {
            _domain.AbortCurrentTest();

            
        }

        public bool IsExecuting()
        {
            return _domain.IsExecuting();
        }

        public void Dispose()
        {
            _domain.Dispose();
        }

        private void reload()
        {
            _latch.Reset();

            var thread = new Thread(performReload);
            thread.Name = "StoryTeller-Reload-Library";
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void performReload()
        {
            try
            {
                _domain.LoadProject(_project);
            }
            finally
            {
                _latch.Set();
            }
        }

        public void WaitForProcessing()
        {
            _latch.WaitOne();
        }
    }
}