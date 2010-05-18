using System;
using System.Collections.Generic;
using FubuCore;
using StoryTeller.Model;

namespace StoryTeller.Engine
{
    public interface IFixtureObserver
    {
        void ReadingFixture(int total, int number, string fixtureName);
        void RecordStatus(string statusMessage);
    }

    public class LibraryBuilder : IFixtureVisitor
    {
        private readonly IFixtureObserver _observer;
        private FixtureLibrary _library = new FixtureLibrary();
        private int _number = 1;
        private int _total;
        private ObjectFinder _finder = new ObjectFinder();


        public LibraryBuilder(IFixtureObserver observer)
        {
            _observer = observer;
        }

        public FixtureLibrary Library { get { return _library; } }

        public ObjectFinder Finder { set { _finder = value; } }

        #region IFixtureVisitor Members

        public int FixtureCount { set { _total = value; } }

        public void ReadFixture(string fixtureName, IFixture fixture)
        {
            sendMessage(fixtureName);

            FixtureGraph fixtureGraph = _library.FixtureFor(fixtureName);
            fixtureGraph.FixtureClassName = fixture.GetType().FullName;
            fixtureGraph.Policies = fixture.Policies;
            fixtureGraph.Description = fixture.Description;
            fixtureGraph.Title = fixture.Title.IsEmpty() ? fixtureName : fixture.Title;

            fixture.Errors.Each(x =>
            {
                x.Node = fixtureGraph;
                fixtureGraph.LogError(x);
            });

            fixture.ForEachGrammar((key, grammar) => readGrammar(grammar, fixtureGraph, key));
        }

        public void LogFixtureFailure(string fixtureName, Exception exception)
        {
            sendMessage(fixtureName);

            FixtureGraph fixtureGraph = _library.FixtureFor(fixtureName);
            
            fixtureGraph.LogError(exception);
        }

        #endregion

        public FixtureLibrary Build(ITestContext context)
        {
            _library = new FixtureLibrary()
            {
                Finder = _finder
            };

            context.VisitFixtures(this);

            return _library;
        }

        private void readGrammar(IGrammar grammar, FixtureGraph fixtureGraph, string key)
        {
            GrammarStructure structure = grammar.ToStructure(_library);
            structure.Description = grammar.Description;

            fixtureGraph.AddStructure(key, structure);
        }

        private void sendMessage(string fixtureName)
        {
            _observer.ReadingFixture(_total, _number++, fixtureName);
        }
    }
}