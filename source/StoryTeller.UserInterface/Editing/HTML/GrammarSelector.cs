using System;
using System.Collections.Generic;
using System.Linq;
using HtmlTags;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class SelectorLinkTag : HtmlTag
    {
        private readonly HtmlTag _link;

        public SelectorLinkTag(string key)
            : base("div")
        {
            AddClass(GrammarConstants.ADD_LINK).Id(key).MetaData(GrammarConstants.KEY, key);
            _link = Add("a").Attr("href", "#");
        }

        public void Label(string text)
        {
            _link.Add("span").Text(text);
        }

        public void Input(string text)
        {
            _link.Add("span").Text(text).AddClass(GrammarConstants.VARIABLE);
        }
    }

    public class GrammarSelector : IGrammarVisitor, ISentenceVisitor
    {
        private readonly FixtureGraph _fixture;
        private SelectorLinkTag _link;

        public GrammarSelector(FixtureGraph fixture)
        {
            _fixture = fixture;
        }

        #region IGrammarVisitor Members

        void IGrammarVisitor.Sentence(Sentence sentence, IStep step)
        {
            sentence.Parts.Each(x => x.AcceptVisitor(this));
        }

        void IGrammarVisitor.Table(Table table, IStep step)
        {
            _link.Label(table.Label);
        }

        void IGrammarVisitor.SetVerification(SetVerification setVerification, IStep step)
        {
            _link.Label(setVerification.Label);
        }

        void IGrammarVisitor.Paragraph(Paragraph paragraph, IStep step)
        {
            _link.Label(paragraph.Label);
        }

        void IGrammarVisitor.EmbeddedSection(EmbeddedSection section, IStep step)
        {
            _link.Label(section.Label);
        }

        void IGrammarVisitor.DoGrammar(DoGrammarStructure grammar, IStep step)
        {
            throw new NotImplementedException();
        }

        #endregion


        public void Label(Label label)
        {
            _link.Label(label.Text);
        }

        public void Input(TextInput input)
        {
            _link.Input(input.Cell.Header);
        }

        public HtmlTag Build()
        {
            HtmlTag tag = buildTopNode();

            addCommentLink(tag);

            _fixture.PossibleGrammarsFor(new StepLeaf()).Where(x => !(x is DoGrammarStructure)).Each(grammar =>
            {
                _link = new SelectorLinkTag(grammar.Name);
                tag.Child(_link);

                grammar.AcceptVisitor(this, new Step());
            });

            return tag;
        }

        private void addCommentLink(HtmlTag tag)
        {
            var commentLink = new SelectorLinkTag(GrammarConstants.COMMENT);
            commentLink.Label(GrammarConstants.COMMENT);
            tag.Child(commentLink);
        }

        private HtmlTag buildTopNode()
        {
            HtmlTag tag = new HtmlTag("div").AddClass(GrammarConstants.GRAMMAR_SELECTOR).Hide();
            tag.Add("div", div =>
            {
                div.AddClass(GrammarConstants.HEADER_CONTAINER);
                div.ActionLink(GrammarConstants.CLOSE, GrammarConstants.CLOSER).Visible(!_fixture.IsSingleSelection());

                div.Add("span").Text(_fixture.Policies.AddGrammarText);
                div.Add("span").AddClass(GrammarConstants.SELECTION_REQUIRED).Text(GrammarConstants.REQUIRED).Visible(
                    _fixture.IsSingleSelection());
            });
            return tag;
        }
    }
}