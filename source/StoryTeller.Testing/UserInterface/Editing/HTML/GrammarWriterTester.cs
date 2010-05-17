using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HtmlTags;
using NUnit.Framework;
using StoryTeller.Assertions;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Samples;
using StoryTeller.Samples.Grammars;
using StoryTeller.Testing.JavaScript;
using StoryTeller.UserInterface.Editing.HTML;

namespace StoryTeller.Testing.UserInterface.Editing.HTML
{
    public abstract class GrammarWriterContext
    {
        protected FixtureLibrary library;
        protected HtmlTag templates;

        [SetUp]
        public void SetUp()
        {
            var runner = new TestRunner(fixturesAre);
            library = runner.Library;
            templates = new GrammarWriter(library).Build();
        }

        protected abstract void fixturesAre(FixtureRegistry registry);
    }

    public static class TagExtensions
    {
        public static HtmlTag FixtureNode<T>(this HtmlTag tag) where T : IFixture
        {
            return tag.Children.FirstOrDefault(x => x.TagName() == "div" && x.Id() == typeof(T).GetFixtureAlias());
        }

        public static HtmlTag GrammarNode<T>(this HtmlTag tag, string grammarKey) where T : IFixture
        {
            return tag.FixtureNode<T>().Children.FirstOrDefault(x => x.HasClass(grammarKey));
        }
    }


    [TestFixture]
    public class when_writing_fixtures : GrammarWriterContext
    {
        protected override void fixturesAre(FixtureRegistry registry)
        {
            registry.AddFixture<SentenceFixture>();
            registry.AddFixture<SetsFixture>();
            registry.AddFixture<TableFixture>();
            registry.AddFixture<MathFixture>();
        }

        [Test]
        public void has_all_of_the_fixtures()
        {
            templates.FixtureNode<SentenceFixture>().ShouldNotBeNull();
            templates.FixtureNode<SetsFixture>().ShouldNotBeNull();
            templates.FixtureNode<TableFixture>().ShouldNotBeNull();
            templates.FixtureNode<MathFixture>().ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class when_writing_a_sentence_grammar : GrammarWriterContext
    {
        protected override void fixturesAre(FixtureRegistry registry)
        {
            registry.AddFixture<SentenceGrammarFixture>();
        }

        [Test]
        public void the_sentence_has_all_of_its_parts()
        {
            HtmlTag grammarTag = templates.GrammarNode<SentenceGrammarFixture>("MultiplyThenAdd");

            grammarTag.Children.Count(x => !(x is RemoveLinkTag)).ShouldEqual(4);

            grammarTag.Children[0].TagName().ShouldEqual("span");
            grammarTag.Children[1].TagName().ShouldEqual("input");
            grammarTag.Children[2].TagName().ShouldEqual("span");
            grammarTag.Children[3].TagName().ShouldEqual("input");
        }

        [Test]
        public void the_sentence_should_have_a_close_link()
        {
            HtmlTag grammarTag = templates.GrammarNode<SentenceGrammarFixture>("MultiplyThenAdd");
            grammarTag.Children.Last().ShouldBeOfType<RemoveLinkTag>();
        }
    }

    [TestFixture]
    public class when_writing_a_paragraph : GrammarWriterContext
    {
        protected override void fixturesAre(FixtureRegistry registry)
        {
            registry.AddFixture<ParagraphGrammarFixture>();
        }

        [Test]
        public void should_write_a_child_tag_for_each_grammar_underneath_a_paragraph()
        {
            HtmlTag paragraph = templates.GrammarNode<ParagraphGrammarFixture>("Sum1");

            IEnumerable<HtmlTag> children = paragraph.Children.Where(x => x.HasClass(GrammarConstants.SENTENCE));
            children.Count().ShouldEqual(3);
        }

        [Test]
        public void the_children_underneath_paragraph_should_not_have_remove_link()
        {
            HtmlTag paragraph = templates.GrammarNode<ParagraphGrammarFixture>("Sum1");

            paragraph.Children.Where(x => x.HasClass(GrammarConstants.SENTENCE)).Each(
                x => { x.Children.Count(c => c is RemoveLinkTag).ShouldEqual(0); });
        }

        [Test]
        public void the_paragraph_should_start_with_a_header_tag_with_a_remove_link()
        {
            HtmlTag paragraph = templates.GrammarNode<ParagraphGrammarFixture>("Sum1");
            paragraph.Children.First().ShouldBeOfType<HeaderTag>().Children.First()
                .Children.Count(x => x is RemoveLinkTag).ShouldEqual(1);
        }

        [Test]
        public void the_paragraph_tag_should_have_the_paragraph_class()
        {
            HtmlTag paragraph = templates.GrammarNode<ParagraphGrammarFixture>("Sum1");
            paragraph.HasClass(GrammarConstants.PARAGRAPH).ShouldBeTrue();
        }
    }


    [TestFixture]
    public class when_writing_a_single_fixture : GrammarWriterContext
    {
        protected override void fixturesAre(FixtureRegistry registry)
        {
            registry.AddFixture<SentenceGrammarFixture>();
        }

        [Test]
        public void has_named_grammars()
        {
            templates.GrammarNode<SentenceGrammarFixture>("ThisFactIsTrue").ShouldNotBeNull();
            templates.GrammarNode<SentenceGrammarFixture>("ThisFactIsFalse").ShouldNotBeNull();
            templates.GrammarNode<SentenceGrammarFixture>("ThisFactThrowsException").ShouldNotBeNull();
        }

        [Test]
        public void has_the_grammars()
        {
            var fixture = new SentenceGrammarFixture();

            // Plus 1 because of Comments
            templates.FixtureNode<SentenceGrammarFixture>().Children.Count.ShouldEqual(fixture.GrammarCount + 1);
        }
    }

    public class SentenceGrammarFixture : Fixture
    {
        private int _number;

        public SentenceGrammarFixture()
        {
            this["ThisFactIsTrue"] = Fact("This fact is always true").VerifiedBy(() => true);
            this["ThisFactIsFalse"] = Fact("This fact is always false").VerifiedBy(() => false);
            this["ThisFactThrowsException"] =
                Fact("This fact throws an exception").VerifiedBy(() => { throw new NotImplementedException(); });
        }

        [FormatAs("Start with the number {number}")]
        public void StartWithTheNumber(int number)
        {
            _number = number;
        }

        [FormatAs("The name is {name}")]
        public void TheNameIs1([SelectionValues("Jeremy", "Josh", "Chad")] string name)
        {
        }

        [FormatAs("The name is {name}")]
        public void TheNameIs2([SelectionValues("Jeremy", "Josh", "Chad")] string name)
        {
        }

        [FormatAs("The name is {name}")]
        public void TheNameIs3([SelectionValues("Jeremy", "Josh", "Chad")] string name)
        {
        }


        [FormatAs("Multiply by {multiplier} then add {delta}")]
        public void MultiplyThenAdd(int multiplier, int delta)
        {
            _number *= multiplier;
            _number += delta;
        }

        public void Subtract(int operand)
        {
            _number -= operand;
        }

        public void DivideBy(int operand)
        {
            _number /= operand;
        }

        [FormatAs("The number should now be {number}")]
        [return: AliasAs("number")]
        public int TheValueShouldBe()
        {
            return _number;
        }

        [return: AliasAs("sum")]
        [FormatAs("The sum of {number1} and {number2} should be {sum}")]
        public int TheSumOf(int number1, int number2)
        {
            return number1 + number2;
        }

        [FormatAs("This line is always true")]
        public bool ThisLineIsAlwaysTrue()
        {
            return true;
        }

        [FormatAs("This line is always false")]
        public bool ThisLineIsAlwaysFalse()
        {
            return false;
        }

        public void ThisLineAlwaysThrowsExceptions()
        {
            StoryTellerAssert.Fail("No go!");
        }


        [FormatAs("{x} + {y} should be {sum}")]
        public bool XplusYShouldBe(int x, int y, int sum)
        {
            return (x + y) == sum;
        }
    }

    [TestFixture]
    public class when_writing_an_embedded_section : GrammarWriterContext
    {
        protected override void fixturesAre(FixtureRegistry registry)
        {
            registry.AddFixture<EmbeddedFixture>();
        }

        private HtmlTag theSectionNode { get { return templates.GrammarNode<EmbeddedFixture>("EmbeddedMath"); } }

        [Test]
        public void the_embedded_section_should_have_a_holder_div()
        {
            theSectionNode.ToString().ShouldContain(GrammarConstants.STEP_HOLDER);
        }

        [Test]
        public void the_embedded_section_should_have_metadata_for_the_leaf_name_and_fixture_name()
        {
            var fixture = new EmbeddedFixture();
            var grammar = fixture["EmbeddedMath"].As<EmbeddedSectionGrammar<MathFixture>>();

            theSectionNode.MetaData(GrammarConstants.FIXTURE).ShouldEqual(typeof(MathFixture).GetFixtureAlias());
            theSectionNode.MetaData(GrammarConstants.LEAF_NAME).ShouldEqual(grammar.LeafName());
        }

        [Test]
        public void the_embedded_section_should_have_the_embedded_section_class()
        {
            theSectionNode.ShouldHaveClass(GrammarConstants.EMBEDDED);
        }

        [Test]
        public void the_embedded_section_should_have_the_section_class()
        {
            theSectionNode.ShouldHaveClass(GrammarConstants.SECTION);
        }

        [Test]
        public void the_embedded_section_should_have_the_selection_mode_set_in_the_metadata()
        {
            theSectionNode.MetaData(GrammarConstants.SELECTION_MODE).ShouldEqual(
                new EmbeddedFixture().Policies.SelectionMode.ToString());
        }

        [Test]
        public void the_embedded_section_should_start_with_a_header_tag_and_remove_link()
        {
            theSectionNode.Children.First().ShouldBeOfType<HeaderTag>().Children.Count(x => x is RemoveLinkTag);
        }
    }

    [TestFixture]
    public class write_it_out
    {
        [Test]
        public void TESTNAME()
        {
            var writer =
                new GrammarWriter(FixtureLibrary.For(x => x.AddFixture<SentenceFixture>()));
            Debug.WriteLine(writer.Build().ToString());
        }
    }
}