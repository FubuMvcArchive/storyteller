using System;
using FubuCore;
using FubuCore.Util;
using StoryTeller.Engine;

namespace StoryTeller.Workspace
{
    [Serializable]
    public class FixtureFilter
    {
        public FilterType Type { get; set;}
        public string Name { get; set; }

        public void Apply(CompositeFilter<Type> filter)
        {
            if (Type == FilterType.Fixture)
            {
                filter.Includes += t => t.GetFixtureAlias() == Name;
            }
            else
            {
                filter.Includes += t => t.IsInNamespace(Name);
            }
        }
    }
}