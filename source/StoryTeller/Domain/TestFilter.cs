using System;
using FubuCore.Util;

namespace StoryTeller.Domain
{
    public enum ResultStatus
    {
        Unknown,
        Success,
        Failed,
        All
    }


    public class TestFilter
    {
        private readonly Cache<Lifecycle, Predicate<Test>> _lifecycleFilters = new Cache<Lifecycle, Predicate<Test>>();

        private readonly Cache<ResultStatus, Predicate<Test>> _resultFilters =
            new Cache<ResultStatus, Predicate<Test>>();

        private Lifecycle _lifecycle;

        private Predicate<Test> _lifecycleMatch;
        private Predicate<Test> _resultMatch;
        private ResultStatus _resultStatus;

        public TestFilter()
        {
            _resultFilters[ResultStatus.All] = t => true;
            _resultFilters[ResultStatus.Success] = t => t.HasResult() && t.WasSuccessful();
            _resultFilters[ResultStatus.Failed] = t => t.HasResult() && !t.WasSuccessful();
            _resultFilters[ResultStatus.Unknown] = t => !t.HasResult();

            _lifecycleFilters[Lifecycle.Any] = t => true;
            _lifecycleFilters[Lifecycle.Acceptance] = t => t.Lifecycle == Lifecycle.Acceptance;
            _lifecycleFilters[Lifecycle.Regression] = t => t.Lifecycle == Lifecycle.Regression;

            ResultStatus = ResultStatus.All;
            Lifecycle = Lifecycle.Any;
        }


        public ResultStatus ResultStatus
        {
            get { return _resultStatus; }
            set
            {
                _resultStatus = value;
                _resultMatch = _resultFilters[value];
            }
        }

        public Lifecycle Lifecycle
        {
            get { return _lifecycle; }
            set
            {
                _lifecycle = value;
                _lifecycleMatch = _lifecycleFilters[value];
            }
        }

        public bool Matches(Test test)
        {
            return _resultMatch(test) && _lifecycleMatch(test);
        }

        public bool ShowEmptySuites()
        {
            return _lifecycle == Lifecycle.Any && _resultStatus == ResultStatus.All;
        }
    }
}