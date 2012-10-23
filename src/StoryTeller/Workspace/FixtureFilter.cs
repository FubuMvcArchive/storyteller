using System;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Util;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller.Workspace
{
    [Serializable]
    public class FixtureFilter
    {
        [XmlAttribute]
        public FilterType Type { get; set;}

        [XmlAttribute]
        public string Name { get; set; }

        public void Apply(CompositeFilter<Type> filter)
        {
            if (Type == FilterType.Fixture)
            {
                filter.Includes += t => t.GetFixtureAlias() == Name;
            }
            else if (Type == FilterType.All)
            {
                filter.Includes += t => true;
            }
            else
            {
                filter.Includes += t => t.IsInNamespace(Name);
            }
        }

        public void Apply(CompositeFilter<FixtureStructure> filter)
        {
            switch (Type)
            {
                case FilterType.All:
                    filter.Includes += f => true;
                    break;

                case FilterType.Fixture:
                    filter.Includes += f => f.Name == Name;
                    break;

                case FilterType.Namespace:

                    filter.Includes += f => f.FixtureNamespace.IsEmpty() ? false : f.FixtureNamespace.StartsWith(Name);
                    break;
            }
        }

        public static FixtureFilter Namespace(string ns)
        {
            return new FixtureFilter()
            {
                Name = ns,
                Type = FilterType.Namespace
            };
        }

        public static FixtureFilter All()
        {
            return new FixtureFilter()
            {
                Name = "ALL",
                Type = FilterType.All
            };
        }

        public static FixtureFilter Fixture(string name)
        {
            return new FixtureFilter()
            {
                Name = name,
                Type = FilterType.Fixture
            };
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

        public override string ToString()
        {
            return string.Format("Type: {0}, Name: {1}", Type, Name);
        }
    }
}