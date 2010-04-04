using System;
using System.Collections.Generic;
using System.Linq;
using StoryTeller.Domain;
using StoryTeller.DSL;
using StoryTeller.Model;
using StructureMap;

namespace StoryTeller.Engine
{
    public interface ICompositeGrammar : IGrammarWithCells
    {
        IGrammar[] Children { get; }
        CompositeGrammar ConfigureSteps(Action<CompositeGrammarBuilder> action);
    }


    public delegate void GrammarAction(IStep step, ITestContext context);

    public class CompositeGrammar : ICompositeGrammar
    {
        private readonly List<IGrammar> _grammars = new List<IGrammar>();
        private EmbedStyle _style = EmbedStyle.TitledAndIndented;
        private string _title;

        public CompositeGrammar(string title)
        {
            Title(title);
        }

        [DefaultConstructor]
        public CompositeGrammar(params IGrammar[] grammars)
        {
            _grammars.AddRange(grammars);
        }

        public EmbedStyle Style { get { return _style; } set { _style = value; } }

        public List<IGrammar> Grammars { get { return _grammars; } }

        #region ICompositeGrammar Members

        public IGrammar[] Children { get { return _grammars.ToArray(); } }

        public CompositeGrammar ConfigureSteps(Action<CompositeGrammarBuilder> action)
        {
            action(new CompositeGrammarBuilder(this));
            return this;
        }

        public void Execute(IStep step, ITestContext context)
        {
            var results = context.ResultsFor(step);

            _grammars.ForEach(g =>
            {
                context.PerformAction(step, g.Execute);
                results.MoveFrame();
            });
        }


        public IList<Cell> GetCells()
        {
            var cells = new List<Cell>();
            _grammars.CallOnEach<LineGrammar>(x =>
            {
                IEnumerable<Cell> newCells = x.GetCells().Select(c => c.ToExample());
                cells.AddRange(newCells);
            });

            return cells;
        }

        public GrammarStructure ToStructure(FixtureLibrary library)
        {
            List<GrammarStructure> list =
                _grammars.ConvertAll(x =>
                {
                    GrammarStructure grammar = x.ToStructure(library);
                    grammar.Description = x.Description;

                    return grammar;
                }).Where(x => x != null).ToList();
            return new Paragraph(Title(), list)
            {
                Style = Style,
                Description = Description
            };
        }

        public string Description { get; set; }

        #endregion

        public CompositeGrammar SetStyle(EmbedStyle style)
        {
            Style = style;
            return this;
        }

        public CompositeGrammar Title(string value)
        {
            _title = value;
            return this;
        }

        public string Title()
        {
            return _title;
        }

        public void AddGrammar(IGrammar childGrammar)
        {
            _grammars.Add(childGrammar);
        }

        // TODO: tests!
        public void InsertBefore(IGrammar grammar)
        {
            _grammars.Insert(0, grammar);
        }

        public CompositeGrammar With(params IGrammar[] grammars)
        {
            _grammars.AddRange(grammars);
            return this;
        }
    }


    public class CompositeGrammarBuilder
    {
        private readonly CompositeGrammar _grammar;

        public CompositeGrammarBuilder(CompositeGrammar grammar)
        {
            _grammar = grammar;
        }

        public string Title { set { _grammar.Title(value); } }

        public string Description { set { _grammar.Description = value; } }

        public CompositeGrammarBefore Before { get { return new CompositeGrammarBefore(_grammar); } set { } }

        public static CompositeGrammarBuilder operator +(CompositeGrammarBuilder expression, IGrammar grammar)
        {
            expression._grammar.AddGrammar(grammar);
            return expression;
        }

        public static CompositeGrammarBuilder operator +(CompositeGrammarBuilder expression, Action action)
        {
            return expression + new DoGrammar((step, context) => action());
        }

        public static CompositeGrammarBuilder operator +(CompositeGrammarBuilder expression, Action<ITestContext> action
            )
        {
            return expression + new DoGrammar((step, context) => action(context));
        }

        public static CompositeGrammarBuilder operator +(CompositeGrammarBuilder expression, GrammarAction action)
        {
            return expression + new DoGrammar(action);
        }

        public void VerifyPropertiesOf<T>(Action<ObjectVerificationExpression<T>> action)
            where T : class
        {
            var expression = new ObjectVerificationExpression<T>(_grammar);
            action(expression);
        }

        public void SetPropertiesOnCurrentObject<T>(Action<ObjectConstructionExpression<T>> action)
        {
            var expression = new ObjectConstructionExpression<T>(_grammar);
            action(expression);
        }
    }

    public class CompositeGrammarBefore
    {
        private readonly CompositeGrammar _grammar;

        public CompositeGrammarBefore(CompositeGrammar grammar)
        {
            _grammar = grammar;
        }

        // TODO:  HAS TO GET A TEST!!!!!!!!!!!
        public static CompositeGrammarBefore operator +(CompositeGrammarBefore expression, IGrammar grammar)
        {
            expression._grammar.InsertBefore(grammar);
            return expression;
        }

        public static CompositeGrammarBefore operator +(CompositeGrammarBefore expression, Action action)
        {
            return expression + new DoGrammar((step, context) => action());
        }

        public static CompositeGrammarBefore operator +(CompositeGrammarBefore expression, Action<ITestContext> action
            )
        {
            return expression + new DoGrammar((step, context) => action(context));
        }

        public static CompositeGrammarBefore operator +(CompositeGrammarBefore expression, GrammarAction action)
        {
            return expression + new DoGrammar(action);
        }
    }
}