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

        public bool Equals(FixtureFilter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Type, Type) && Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FixtureFilter)) return false;
            return Equals((FixtureFilter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode()*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}