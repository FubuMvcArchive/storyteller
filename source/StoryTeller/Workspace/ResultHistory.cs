using System;
using FubuCore.Util;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Workspace
{
    public class ResultHistory
    {
        private readonly Cache<string, TestResult> _results = new Cache<string,TestResult>(key => null);

        public TestResult this[Test test]
        {
            get
            {
                return _results[test.GetPath().Locator];
            }
            set
            {
                _results[test.GetPath().Locator] = value;
            }
        }
    }

    public interface IResultPersistor
    {
        void SaveResult(IProject project, Test test, TestResult result);
        ResultHistory LoadResults(IProject project);
        ResultHistory LoadResults(string directory);
        void ClearResults(IProject project);
        void SaveResultsToDirectory(ResultHistory theResults, string directory);
    }

    public class ResultPersistor : IResultPersistor
    {
        public void SaveResult(IProject project, Test test, TestResult result)
        {
            throw new NotImplementedException();
        }

        public ResultHistory LoadResults(IProject project)
        {
            throw new NotImplementedException();
        }

        public ResultHistory LoadResults(string directory)
        {
            throw new NotImplementedException();
        }

        public void ClearResults(IProject project)
        {
            throw new NotImplementedException();
        }

        public void SaveResultsToDirectory(ResultHistory theResults, string directory)
        {
            throw new NotImplementedException();
        }
    }
}