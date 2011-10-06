using System;
using System.Collections.Generic;
using StoryTeller.Domain;
using StoryTeller.Model;
using System.Linq;

namespace StoryTeller.Usages
{
    public class GrammarUsage
    {
        public string Key { get; set; }
        public Test Test { get; set; }

        public bool Equals(GrammarUsage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Key, Key) && Equals(other.Test, Test);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GrammarUsage)) return false;
            return Equals((GrammarUsage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0)*397) ^ (Test != null ? Test.GetHashCode() : 0);
            }
        }
    }

    public class FixtureUsage
    {
        private readonly FixtureGraph _fixture;
        private readonly IList<Test> _tests = new List<Test>();
        private readonly IList<GrammarUsage> _grammars = new List<GrammarUsage>();

        public FixtureUsage(FixtureGraph fixture)
        {
            _fixture = fixture;
        }

        public void Mark(Test test)
        {
            _tests.Fill(test);
        }

        public void MarkGrammar(Test test, string grammarKey)
        {
            _grammars.Fill(new GrammarUsage()
            {
                Key = grammarKey, 
                Test = test
            });
        }

        public FixtureGraph Fixture
        {
            get { return _fixture; }
        }

        public bool HasUsageInWorkspace(string workspaceName)
        {
            return _tests.Any(x => x.IsInWorkspace(workspaceName));
        }
    }
}