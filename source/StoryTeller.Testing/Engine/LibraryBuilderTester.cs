using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class LibraryBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            observer = MockRepository.GenerateMock<IFixtureObserver>();
            builder = new LibraryBuilder(observer);

            builder.FixtureCount = 23;
        }

        #endregion

        private LibraryBuilder builder;
        private IFixtureObserver observer;

        [Test]
        public void read_a_fixture_failure()
        {
            var exception = new NotImplementedException();
            builder.LogFixtureFailure("bad fixture", exception);

            FixtureGraph fixture = builder.Library.FixtureFor("bad fixture");

            fixture.AllErrors().Count().ShouldEqual(1);

            GrammarError error = fixture.AllErrors().First();
            error.Message.ShouldEqual("Fixture 'bad fixture' could not be loaded");
            error.ErrorText.ShouldEqual(exception.ToString());
        }

        [Test]
        public void read_a_fixture_will_record_the_Title_of_the_fixture_if_it_is_explicitly_set()
        {
            var fixture = new GrammarErrorFixture
            {
                Title = "the bad grammars"
            };

            builder.ReadFixture("GrammarError", fixture);

            builder.Library.FixtureFor("GrammarError").Title.ShouldEqual("the bad grammars");
        }

        [Test]
        public void read_a_fixture_will_use_the_fixture_name_for_the_title_if_the_fixture_title_is_empty()
        {
            var fixture = new GrammarErrorFixture();


            builder.ReadFixture("GrammarError", fixture);

            builder.Library.FixtureFor("GrammarError").Title.ShouldEqual("GrammarError");
        }

        [Test]
        public void read_a_fixture_with_grammar_errors()
        {
            var fixture = new GrammarErrorFixture();
            fixture.Errors.Count().ShouldEqual(2);

            builder.ReadFixture("GrammarError", fixture);

            FixtureGraph fixtureGraph = builder.Library.FixtureFor("GrammarError");
            fixtureGraph.AllErrors().Count().ShouldEqual(2);

            fixtureGraph.AllErrors().Each(x => x.Node.ShouldEqual(fixtureGraph));
        }

        [Test]
        public void reading_a_fixture_failure_will_send_a_reading_fixture_message()
        {
            var exception = new NotImplementedException();
            builder.LogFixtureFailure("bad fixture", exception);

            observer.AssertWasCalled(x => x.ReadingFixture(23, 1, "bad fixture"));
        }

        [Test]
        public void reading_a_fixture_will_send_a_message_to_the_observer()
        {
            builder.ReadFixture("Good", new StubFixture());

            observer.AssertWasCalled(x => x.ReadingFixture(23, 1, "Good"));

            builder.ReadFixture("Good2", new StubFixture());

            observer.AssertWasCalled(x => x.ReadingFixture(23, 2, "Good2"));
        }
    }


    [TestFixture]
    public class creating_constraint_models_for_fixtures
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var context = new TestContext(x => { x.AddFixture<FixtureWithHiddenGrammarsFixture>(); });

            var observer = MockRepository.GenerateMock<IFixtureObserver>();
            var builder = new LibraryBuilder(observer);
            builder.Build(context);

            library = builder.Library;
        }

        #endregion

        private FixtureLibrary library;

        [Test]
        public void fixture_graph_should_have_the_constraint_model_from_the_original_fixture()
        {
            FixtureGraph fixture = library.FixtureFor(typeof (FixtureWithHiddenGrammarsFixture).GetFixtureAlias());

            fixture.Policies.IsHidden("Hidden1").ShouldBeTrue();
            fixture.Policies.IsHidden("Hidden2").ShouldBeTrue();
            fixture.Policies.IsHidden("NotHidden1").ShouldBeFalse();
            fixture.Policies.IsHidden("NotHidden2").ShouldBeFalse();
        }

        [Test]
        public void load_a_fixture_with_fixture_level_tags()
        {
            var fixture = new TagsFixture();
            fixture.Policies.Tags().ShouldHaveTheSameElementsAs("d", "e");
        }

        [Test]
        public void load_a_fixture_with_grammar_level_tags()
        {
            var fixture = new TagsFixture();
            fixture.Policies.Tags("Go1").Count().ShouldEqual(0);
            fixture.Policies.Tags("Go2").ShouldHaveTheSameElementsAs("a", "b");
            fixture.Policies.Tags("Go3").ShouldHaveTheSameElementsAs("c");
        }
    }

    [Tag("d", "e")]
    public class TagsFixture : Fixture
    {
        public void Go1()
        {
        }

        [Tag("a", "b")]
        public void Go2()
        {
        }

        [Tag("c")]
        public void Go3()
        {
        }
    }

    public class FixtureWithHiddenGrammarsFixture : Fixture
    {
        [Hidden]
        public void Hidden1()
        {
        }

        public void NotHidden1()
        {
        }

        [Hidden]
        public IGrammar Hidden2()
        {
            return Script("something", x => { });
        }

        public IGrammar NotHidden2()
        {
            return Script("something", x => { });
        }
    }

    public class GrammarErrorFixture : Fixture
    {
        public IGrammar Bad1()
        {
            throw new NotImplementedException();
        }

        public IGrammar Bad2()
        {
            throw new NotImplementedException();
        }
    }
}