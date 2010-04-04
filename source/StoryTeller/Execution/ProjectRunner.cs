using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Html;
using StoryTeller.Model;
using StoryTeller.Persistence;
using StoryTeller.Workspace;
using FileSystem=StoryTeller.Persistence.FileSystem;

namespace StoryTeller.Execution
{
    public class ProjectRunner : ConsoleListener
    {
        private readonly IDictionary<Lifecycle, TestCount> _counts = new Dictionary<Lifecycle, TestCount>();
        private readonly IList<IProject> _projects;
        private readonly string _resultsFile;
        private readonly string _resultsFolder;
        private readonly IResultsSummary _summary = new ResultsSummary();
        private readonly IFileSystem _system = new FileSystem();

        public ProjectRunner(IList<IProject> projects, string resultsFile)
        {
            _projects = projects;
            _resultsFile = resultsFile;
            string containingFolder = new FileInfo(_resultsFile).Directory.FullName;
            _resultsFolder = Path.Combine(containingFolder, "results");

            Console.WriteLine("Writing results to " + _resultsFolder);

            _counts.Add(Lifecycle.Acceptance, new TestCount(Lifecycle.Acceptance));
            _counts.Add(Lifecycle.Regression, new TestCount(Lifecycle.Regression));
        }

        public int Execute()
        {
            try
            {
                prepareResultsFolder();

                string names = _projects.Select(x => x.Name).ToArray().Join(", ");
                _summary.Start("Project(s):  " + names, DateTime.Now);


                _projects.Each(p =>
                {
                    Console.WriteLine("Running Project " + p.Name);
                    //executeProject(p);
                });


                _summary.WriteFile(_resultsFile);

                return createFinalResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        //private void executeProject(IProject project)
        //{
        //    var engine = new TestEngine();
        //    using (ITestRunnerProxy proxy = engine.BuildProxy(project))
        //    {
        //        proxy.Listener = new TeamCityTestListener();

        //        foreach (Test test in project.LoadTests().GetAllTests())
        //        {
        //            runTest(proxy, test);
        //        }
        //    }
        //}

        private void prepareResultsFolder()
        {
            _system.DeleteFolder(_resultsFolder);
            _system.CreateFolder(_resultsFolder);
        }

        //private void runTest(ITestRunnerProxy proxy, Test test)
        //{
        //    var request = new TestExecutionRequest(test);
        //    // TODO -- need to bring back the timeout in seconds
        //    // and the break on failures or exceptions

        //    test.LastResult = proxy.RunTest(request);

        //    _counts[test.Lifecycle].Tally(test);

        //    string filename = Path.GetFileNameWithoutExtension(test.FileName) +
        //                      DateTime.Now.ToString("hhmmss") + "-results.htm";
        //    string resultFile = Path.Combine(_resultsFolder,
        //                                     filename);

        //    test.WriteResultsToFile(resultFile);
        //    _summary.AddTest(test, "results/" + filename);
        //}

        private int createFinalResult()
        {
            writeDivider();
            foreach (var pair in _counts)
            {
                pair.Value.Write();
            }


            return _counts[Lifecycle.Regression].HasFailures() ? 1 : 0;
        }
    }

    public class TestCount
    {
        public TestCount(Lifecycle lifecycle)
        {
            Lifecycle = lifecycle;
        }

        private Lifecycle Lifecycle { get; set; }
        private int Passed { get; set; }
        private int Total { get; set; }

        public void Write()
        {
            int failed = Total - Passed;
            string message = "{0} Tests:  {1} of {2} passed.  {3} failed".ToFormat(Lifecycle, Passed, Total, failed);
            Console.WriteLine(message);
        }

        public bool HasFailures()
        {
            return Passed != Total;
        }

        public void Tally(Test test)
        {
            Total++;
            if (test.WasSuccessful())
            {
                Passed++;
            }
        }
    }
}