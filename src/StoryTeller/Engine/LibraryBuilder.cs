using System;
using System.Collections.Generic;
using System.Diagnostics;

using FubuCore;
using FubuCore.Conversion;
using FubuCore.Util;
using StoryTeller.Model;
using System.Linq;
using StructureMap;
using StructureMap.Query;

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
        private readonly CompositeFilter<Type> _filter;
        private FixtureLibrary _library;
        private int _number = 1;
        private int _total;
        
        public LibraryBuilder(IFixtureObserver observer, CompositeFilter<Type> filter)
        {
            _observer = observer;
            _filter = filter;

            _library = new FixtureLibrary();
        }

        public FixtureLibrary Library { get { return _library; } }

        #region IFixtureVisitor Members

        public int FixtureCount { set { _total = value; } }

        public void ReadFixture(string fixtureName, IFixture fixture)
        {
            sendMessage(fixtureName);

            FixtureGraph fixtureGraph = _library.FixtureFor(fixtureName);
            fixtureGraph.FixtureClassName = fixture.GetType().FullName;
            fixtureGraph.FixtureNamespace = fixture.GetType().Namespace;
            fixtureGraph.Policies = fixture.Policies;
            fixtureGraph.Description = fixture.Description;
            fixtureGraph.Label = fixture.Title.IsEmpty() ? fixtureName : fixture.Title;

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

        // TODO -- needs to change to IContainer child container
        public FixtureLibrary Build(TestContext context)
        {
            _library = new FixtureLibrary();

            readFixtures(context);
            readActions(context.Container);

            return _library;
        }

        private void readActions(IContainer container)
        {
            _library.StartupActions = container.Model.For<IStartupAction>().Instances.OrderBy(x => x.Name).Select(x => x.Name).ToArray();
        }

        private void readFixtures(TestContext context)
        {
            _observer.RecordStatus("Discovering and filtering fixtures");

            var fixtureConfiguration = context.Container.Model.For<IFixture>();
            var instanceRefs = fixtureConfiguration.Instances.Where(i => _filter.Matches(i.ConcreteType));

            FixtureCount = instanceRefs.Count();


            instanceRefs.Each(readInstance);
            _library.AllFixtures = fixtureConfiguration.Instances.Select(x => new FixtureDto
            {
                Fullname = x.ConcreteType.FullName,
                Name = x.ConcreteType.GetFixtureAlias(),
                Namespace = x.ConcreteType.Namespace
            }).ToArray();
        }

        private void readInstance(InstanceRef instance)
        {
            try
            {
                var fixture = instance.Get<IFixture>();
                ReadFixture(instance.Name, fixture);
            }
            catch (Exception e)
            {
                LogFixtureFailure(instance.Name, e);
            }
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