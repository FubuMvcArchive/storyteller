using System;
using StoryTeller.Domain;
using StoryTeller.Model;

namespace StoryTeller.Engine.Importing
{
    public class ImportedGrammar : IGrammar
    {
        private readonly GrammarImport _import;
        private readonly Func<IFixtureContext> _fixtureSource;

        public ImportedGrammar(GrammarImport import, Func<IFixtureContext> fixtureSource)
        {
            _import = import;
            _fixtureSource = fixtureSource;
        }

        private string _description;

        public string Description
        {
            get { return _description ?? _import.ToString(); }
            set { _description = value; }
        }

        private IGrammar _inner;
        private IGrammar inner()
        {
            if (_inner == null)
            {
                _inner = _import.FindGrammar(_fixtureSource());
            }

            return _inner;
        }

        public void Execute(IStep containerStep, ITestContext context)
        {
            inner().Execute(containerStep, context);
        }

        public GrammarStructure ToStructure(FixtureLibrary library)
        {
            return inner().ToStructure(library);
        }

        public ImportedGrammar Curry(CurryAction curryAction)
        {
            _import.CurryAction = curryAction;
            return this;
        }
    }
}