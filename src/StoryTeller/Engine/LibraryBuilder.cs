using System;
using System.Collections.Generic;
using FubuCore;
using StoryTeller.Model;

namespace StoryTeller.Engine
{
    public class LibraryBuilder : IFixtureVisitor
    {
        private FixtureLibrary _library;
        private int _number = 1;
        private int _total;

        public LibraryBuilder()
        {
            _library = new FixtureLibrary();
        }

        public FixtureLibrary Library
        {
            get { return _library; }
        }

        #region IFixtureVisitor Members

        public int FixtureCount
        {
            set { _total = value; }
        }

        public void ReadFixture(string fixtureName, IFixture fixture)
        {
            FixtureStructure fixtureStructure = _library.FixtureFor(fixtureName);
            fixtureStructure.FixtureClassName = fixture.GetType().FullName;
            fixtureStructure.FixtureNamespace = fixture.GetType().Namespace;
            fixtureStructure.Policies = fixture.Policies;
            fixtureStructure.Description = fixture.Description;
            fixtureStructure.Label = fixture.Title.IsEmpty() ? fixtureName : fixture.Title;

            fixture.Errors.Each(x => {
                x.Node = fixtureStructure;
                fixtureStructure.LogError(x);
            });

            fixture.ForEachGrammar((key, grammar) => readGrammar(grammar, fixtureStructure, key));
        }

        public void LogFixtureFailure(string fixtureName, Exception exception)
        {
            FixtureStructure fixtureStructure = _library.FixtureFor(fixtureName);

            fixtureStructure.LogError(exception);
        }

        #endregion

        // TODO -- needs to change to IContainer child container
        public FixtureLibrary Build(TestContext context)
        {
            _library = new FixtureLibrary();

            readFixtures(context);

            return _library;
        }

        private void readFixtures(TestContext context)
        {
            throw new NotImplementedException();
//            var fixtureConfiguration = context.Container.Model.For<IFixture>();
//            var instanceRefs = fixtureConfiguration.Instances;
//
//            FixtureCount = instanceRefs.Count();
//
//
//            instanceRefs.Each(readInstance);
//            _library.AllFixtures = fixtureConfiguration.Instances.Select(x => new FixtureDto
//            {
//                Fullname = x.ConcreteType.FullName,
//                Name = x.ConcreteType.GetFixtureAlias(),
//                Namespace = x.ConcreteType.Namespace
//            }).ToArray();
        }

        private void readGrammar(IGrammar grammar, FixtureStructure fixtureStructure, string key)
        {
            GrammarStructure structure = grammar.ToStructure(_library);
            structure.Description = grammar.Description;

            fixtureStructure.AddStructure(key, structure);
        }
    }
}