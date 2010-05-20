using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Model
{
    [Serializable]
    public class FixtureLibrary : IFixtureNode
    {
        private readonly Cache<string, FixtureGraph> _fixtures =
            new Cache<string, FixtureGraph>(key => new FixtureGraph(key)
            {
                Description = key
            });

        [NonSerialized] private ObjectFinder _finder;

        public FixtureLibrary()
        {
            _finder = new ObjectFinder();
        }

        public IEnumerable<FixtureGraph> AllFixtures { get { return _fixtures.OrderBy(x => x.Name); } }
        public ObjectFinder Finder { get { return _finder; } set { _finder = value; } }

        #region IFixtureNode Members

        public string Name { get { return "Fixtures"; } }

        public TPath GetPath()
        {
            return TPath.Empty;
        }

        public void ModifyExampleTest(Test example)
        {
            example.Name = Title;
        }

        public IEnumerable<GrammarError> AllErrors()
        {
            var list = new List<GrammarError>();
            _fixtures.Each(x => list.AddRange(x.AllErrors()));
            return list;
        }

        public string Title { get { return "All Fixtures"; } }

        public string Description { get { return string.Empty; } }

        #endregion

        public static FixtureLibrary For(Action<FixtureRegistry> configure)
        {
            var runner = TestRunnerBuilder.For(configure);
            return runner.Library;
        }

        public FixtureGraph FixtureFor(string name)
        {
            return _fixtures[name];
        }

        public IFixtureNode Find(TPath path)
        {
            if (path.IsRoot)
            {
                return this;
            }

            FixtureGraph fixture = FixtureFor(path.Next);

            return path.IsEnd ? fixture : (IFixtureNode) fixture.GrammarFor(path.Pop().Next);
        }

        public IEnumerable<FixtureGraph> PossibleFixturesFor(Test test)
        {
            return _fixtures.Where(x => x.CanChoose(test)).OrderBy(x => x.Name);
        }

        public bool HasErrors()
        {
            foreach (FixtureGraph graph in _fixtures)
            {
                if (graph.AllErrors().Count() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasFixture(string fixtureName)
        {
            return _fixtures.Has(fixtureName);
        }

        public FixtureGraph BuildTopLevelGraph()
        {
            var fixture = new FixtureGraph("Test");
            fixture.Policies.SelectionMode = SelectionMode.OneOrMore;
            fixture.Policies.AddGrammarText = "Add Section";

            _fixtures.Where(x => !x.Policies.IsPrivate).Each(x =>
            {
                var grammar = new EmbeddedSection(x, x.Title ?? x.Name, x.Name);
                grammar.Style = EmbedStyle.TitledAndIndented;
                fixture.AddStructure(x.Name, grammar);
            });

            return fixture;
        }
    }
}