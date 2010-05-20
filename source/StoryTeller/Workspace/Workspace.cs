using System.Collections.Generic;

namespace StoryTeller.Workspace
{
    public class Workspace
    {
        private readonly List<FixtureFilter> _filters = new List<FixtureFilter>();

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

        public void AddFilter(FixtureFilter filter)
        {
            _filters.Add(filter);
        }
    }

    public enum FilterType
    {
        Namespace,
        Fixture
    }
    

    public class FixtureFilter
    {
        public FilterType Type { get; set;}
        public string Name { get; set; }


    }


}