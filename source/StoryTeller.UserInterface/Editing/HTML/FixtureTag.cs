using System.Collections.Generic;
using HtmlTags;
using StoryTeller.Domain;
using StoryTeller.Html;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class FixtureTag : HtmlTag, IGrammarVisitor
    {
        private readonly FixtureGraph _fixture;
        private readonly Stack<GrammarTag> _grammarTags = new Stack<GrammarTag>();

        public FixtureTag(FixtureGraph fixture)
            : base("div")
        {
            Id(fixture.Name);
            _fixture = fixture;

            Child<CommentTag>();

            fixture.TopLevelGrammars().Each(writeGrammar);
        }

        public FixtureGraph Fixture { get { return _fixture; } }

        private GrammarTag grammarTag { get { return _grammarTags.Peek(); } }

        #region IGrammarVisitor Members

        void IGrammarVisitor.Sentence(Sentence sentence, IStep step)
        {
            // TODO -- extension point for external CellBuilders?
            var writer = new SentenceWriter(grammarTag, new CellBuilderLibrary());
            writer.Write();
        }

        void IGrammarVisitor.Table(Table table, IStep step)
        {
            writeTable(table);
        }

        private void writeTable(Table table)
        {
            grammarTag.MetaData(GrammarConstants.LEAF_NAME, table.LeafName);
            grammarTag.AddClass(GrammarConstants.TABLE_EDITOR);
            grammarTag.Child<HeaderTag>().Titled(table.Title);

            var editor = grammarTag.Child<TableTag>();
            editor.Attr("cellpadding", "0").Attr("cellspacing", "0");

            editor.AddFooterRow(x =>
            {
                x.Cell().AddClass("table-add-row").Configure(td =>
                {
                    td.ActionLink("add").AddClass("adder");
                    td.ActionLink("clone").AddClass("cloner");
                });
            })
            .AddFooterRow(x =>
            {
                x.Cell().Child(new ColumnSelectionTag(table));
            })
            .AddClasses("grid", "editor");

            grammarTag.Child(new TableTemplateTag(table, new CellBuilderLibrary()));
        }

        void IGrammarVisitor.SetVerification(SetVerification setVerification, IStep step)
        {
            writeTable(setVerification);
        }

        void IGrammarVisitor.Paragraph(Paragraph paragraph, IStep step)
        {
            grammarTag.AddClasses(GrammarConstants.PARAGRAPH, paragraph.Style.ToString());
            var header = new HeaderTag();
            grammarTag.Child(header);

            if (paragraph.Style == EmbedStyle.TitledAndIndented)
            {
                header.Titled(paragraph.Title);
            }

            paragraph.ForEachGrammar(g =>
            {
                var tag = new GrammarTag(g);
                grammarTag.Child(tag);

                _grammarTags.Do(tag, () => g.AcceptVisitor(this, new Step()));
            });
        }

        void IGrammarVisitor.EmbeddedSection(EmbeddedSection section, IStep step)
        {
            grammarTag.AddClasses(GrammarConstants.EMBEDDED, GrammarConstants.SECTION)
                .MetaData(GrammarConstants.LEAF_NAME, section.LeafName)
                .MetaData(GrammarConstants.FIXTURE, section.Fixture.Name)
                .MetaData(GrammarConstants.AUTO_SELECT_KEY, section.Fixture.Policies.AutoSelectGrammarKey)
                .MetaData(GrammarConstants.SELECTION_MODE, section.Fixture.Policies.SelectionMode.ToString());

            var header = new HeaderTag();
            if (section.IsTitled())
            {
                header.Titled(section.Title);
            }

            grammarTag.Child(header);
            grammarTag.Child(new HolderTag(section.Fixture));

            
            if (section.Fixture.Policies.SelectionMode == SelectionMode.MandatoryAutoSelect) return;

            HtmlTag selector = new GrammarSelector(section.Fixture).Build();
            grammarTag.Child(selector);
        }

        void IGrammarVisitor.DoGrammar(DoGrammarStructure grammar, IStep step)
        {
        }

        #endregion

        public GrammarTag Add(GrammarStructure structure)
        {
            var grammarTag = new GrammarTag(structure);
            Child(grammarTag);

            _fixture.Policies.Tags(structure.Name).Each(tag => grammarTag.AddSafeClassName(tag));

            return grammarTag;
        }

        private void writeGrammar(GrammarStructure grammar)
        {
            GrammarTag tag = Add(grammar);

            _grammarTags.Do(tag, () => grammar.AcceptVisitor(this, new Step()));

            if (Fixture.IsMandatoryAutoSelectGrammar(grammar)) return;

            if (_grammarTags.Count == 0)
            {
                tag.AddDeleteLink();
            }
        }
    }
}