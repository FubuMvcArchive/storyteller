using System;
using System.Threading;
using StoryTeller.Domain;
using StoryTeller.Model;
using StoryTeller.Workspace;
using FubuCore;

namespace StoryTeller.Execution
{
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
                test.LastResult = _domain.RunTest(GetExecutionRequest(test));
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