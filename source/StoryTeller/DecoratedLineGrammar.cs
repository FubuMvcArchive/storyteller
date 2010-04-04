using System.Collections.Generic;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller
{
    public class DecoratedLineGrammar : LineGrammar
    {
        private readonly LineGrammar _inner;

        public GrammarAction After = (c, s) => { };
        public GrammarAction Before = (c, s) => { };


        public DecoratedLineGrammar(LineGrammar inner)
        {
            _inner = inner;
        }

        public string Prefix { get; set; }
        public string Suffix { get; set; }

        public override string Description { get { return "Decorated version of " + _inner.Description; } }
        public override string Template { get { return Prefix + _inner.Template + Suffix; } set { } }

        public override void Execute(IStep containerStep, ITestContext context)
        {
            Before(containerStep, context);
            _inner.Execute(containerStep, context);
            After(containerStep, context);
        }

        public override IList<Cell> GetCells()
        {
            return _inner.GetCells();
        }
    }
}