using System.Collections.Generic;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Samples;
using System.Linq;

namespace StoryTeller.Testing.Model
{
    [TestFixture]
    public class FixtureLibraryTester
    {
        [Test]
        public void create_example_test()
        {
            var test = new Test("test1");
            test.Section("a");
            test.Section("b");
            test.Section("c");

            new FixtureLibrary().ModifyExampleTest(test);
            test.Name.ShouldEqual("All Fixtures");
            test.Parts.ShouldHaveTheSameElementsAs<ITestPart>(test.Parts);
        }

        [Test]
        public void description()
        {
            new FixtureLibrary().Description.ShouldBeEmpty();
        }

        [Test]
        public void PossibleFixtures_uses_the_constraint_model_of_each_fixture_graph()
        {
            var library = new FixtureLibrary();
            library.FixtureFor("fixture1").Policies.IsPrivate = true;
            library.FixtureFor("fixture2").Policies.IsPrivate = false;
            library.FixtureFor("fixture3").Policies.IsPrivate = true;
            library.FixtureFor("fixture4").Policies.IsPrivate = false;

            library.PossibleFixturesFor(new Test("something")).ShouldHaveTheSameElementsAs(
                library.FixtureFor("fixture2"), library.FixtureFor("fixture4"));
        }

        [Test]
        public void Title()
        {
            new FixtureLibrary().Label.ShouldEqual("All Fixtures");
        }
    }

    [TestFixture]
    public class when_filtering_a_fixture_library
    {
        private FixtureLibrary library;
        private FixtureLibrary baseLibrary;

        [SetUp]
        public void SetUp()
        {
            baseLibrary = FixtureLibrary.For(x => x.AddFixturesFromAssemblyContaining<MathFixture>());
            library = baseLibrary.Filter(f => f.Name == "Sentence" || f.Name == "Sets");
        }

        [Test]
        public void filtered_library_is_a_different_library_with_only_the_fixtures_that_matched_the_criteria()
        {
            library.ShouldNotBeTheSameAs(baseLibrary);
            library.ActiveFixtures.Select(x => x.Name).ShouldHaveTheSameElementsAs("Sentence", "Sets");
        }
    }

    [TestFixture]
    public class when_building_the_top_level_graph
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            library = new FixtureLibrary();
            library.FixtureFor("Math").Policies.IsPrivate = false;
            library.FixtureFor("Algebra").Policies.IsPrivate = false;
            library.FixtureFor("MathDetails").Policies.IsPrivate = true;
            library.FixtureFor("Calculus").Policies.IsPrivate = false;

            topFixture = library.BuildTopLevelGraph();

            mathSection = topFixture.GrammarFor("Math").ShouldBeOfType<EmbeddedSection>();
        }

        #endregion

        private FixtureLibrary library;
        private FixtureGraph topFixture;
        private EmbeddedSection mathSection;

        [Test]
        public void each_embed_sectino_should_have_the_title_from_the_fixture()
        {
            mathSection.Label.ShouldEqual("Math");
        }

        [Test]
        public void each_embed_section_should_have_the_proper_fixture()
        {
            mathSection.Fixture.Name.ShouldEqual(typeof (MathFixture).GetFixtureAlias());
        }

        [Test]
        public void embed_section_should_have_the_leaf_name_that_is_the_fixture_alias()
        {
            mathSection.LeafName.ShouldEqual(typeof (MathFixture).GetFixtureAlias());
        }

        [Test]
        public void every_child_embedded_section_should_be_titled_and_indented()
        {
            topFixture.PossibleGrammarsFor(new StepLeaf()).Each(
                x => { x.ShouldBeOfType<EmbeddedSection>().Style.ShouldEqual(EmbedStyle.TitledAndIndented); });
        }

        [Test]
        public void grammar_text()
        {
            topFixture.Policies.AddGrammarText.ShouldEqual("Add Section");
        }

        [Test]
        public void should_have_an_embed_section_for_each_non_private_fixture()
        {
            topFixture.GrammarCount.ShouldEqual(3);
        }

        [Test]
        public void the_selection_mode_has_to_be_one_or_more()
        {
            topFixture.Policies.SelectionMode.ShouldEqual(SelectionMode.OneOrMore);
        }
    }
}