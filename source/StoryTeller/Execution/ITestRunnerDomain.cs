using System;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Workspace;

namespace StoryTeller.Execution
{
    public interface ITestRunnerDomain : IDisposable
    {
        void Start(IProject project);
        void Teardown();
        void RecycleEnvironment();
        bool HasStarted();

        TestResult RunTest(TestExecutionRequest request);
        void AbortCurrentTest();
        bool IsExecuting();

        FixtureLibrary Library { get; }
    }


}