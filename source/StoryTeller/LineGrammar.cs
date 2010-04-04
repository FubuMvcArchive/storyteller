using System;
using System.Collections.Generic;
using System.Linq;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller
{
    public interface IGrammarWithCells : IGrammar
    {
        IList<Cell> GetCells();
    }

    public static class GrammarWithCellsExtensions
    {
        public static Cell FindCell(this IGrammarWithCells grammar, string key)
        {
            return grammar.GetCells().FirstOrDefault(x => x.Key == key);
        }
    }

    public abstract class LineGrammar : IGrammarWithCells
    {
        // Leave these constructors as public
        public LineGrammar()
        {
            Template = string.Empty;
        }

        public LineGrammar(string template)
        {
            Template = template;
        }

        public virtual string Template { get; set; }

        #region IGrammarWithCells Members

        public abstract string Description { get; }

        public abstract void Execute(IStep containerStep, ITestContext context);

        public abstract IList<Cell> GetCells();

        public GrammarStructure ToStructure(FixtureLibrary library)
        {
            if (library == null) throw new ArgumentNullException("library");

            Cell[] cells = GetCells()
                .Where(x => x.IsTestVariable(library.Finder))
                .Select(x => x.ToExample())
                .ToArray();

            return new Sentence(Template, cells);
        }

        #endregion
    }
}