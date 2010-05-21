using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace StoryTeller.Workspace
{
    [Serializable]
    public class WorkspaceFilter
    {
        private readonly IList<FixtureFilter> _filters = new List<FixtureFilter>();

        public WorkspaceFilter(IEnumerable<WorkspaceFilter> workspaceFilters)
        {
            _filters.AddRange(workspaceFilters.SelectMany(x => x.Filters));
        }

        public WorkspaceFilter()
        {
        }

        public string Name { get; set; }
        public FixtureFilter[] Filters
        {
            get
            {
                return _filters.ToArray();
            }
            set
            {
                _filters.Clear();
                _filters.AddRange(value);
            }
        }

        public int FilterCount
        {
            get { return _filters.Count; }
        }

        public void AddFilter(FixtureFilter filter)
        {
            _filters.Add(filter);
        }

        public CompositeFilter<Type> CreateFilter()
        {
            var filter = new CompositeFilter<Type>();

            _filters.Each(x => x.Apply(filter));

            return filter;
        }
    }
}