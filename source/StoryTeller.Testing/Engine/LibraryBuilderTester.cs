using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Samples;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class when_building_a_library_with_a_non_inclusive_filter
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            observer = MockRepository.GenerateMock<IFixtureObserver>();
            var filter = new CompositeFilter<Type>();
            filter.Includes += t => t.Name.StartsWith("M");


            builder = new LibraryBuilder(observer, filter);

            library = builder.Build(new TestContext(x => x.AddFixturesFromThisAssembly()));
        }

        #endregion

        private LibraryBuilder builder;
        private IFixtureObserver observer;
        private FixtureLibrary library;

        [Test]
        public void should_contain_fixtures_that_match_the_filter()
        {
            library.HasFixture("Missouri").ShouldBeTrue();
            library.HasFixture("Michigan").ShouldBeTrue();
            library.HasFixture("Montana").ShouldBeTrue();
        }

        [Test]
        public void should_exclude_fixtures_present_in_the_container_that_do_not_match_the_filter()
        {
            library.HasFixture("Arkansas").ShouldBeFalse();
        }

        [Test]
        public void still_has_dtos_for_each_possible_fixture()
        {
            var allNames = library.AllFixtures.OrderBy(x => x.Name).Select(x => x.Name).ToList();
            allNames.ShouldContain("Arkansas");
            allNames.ShouldContain("Michigan");
            allNames.ShouldContain("Missouri");
            allNames.ShouldContain("Montana");
        }

        [Test]
        public void fixture_dto_has_all_the_properties()
        {
            var dto = library.AllFixtures.FirstOrDefault(x => x.Name == "Arkansas");
            dto.Namespace.ShouldEqual(typeof (ArkansasFixture).Namespace);
            dto.Fullname.ShouldEqual(typeof (ArkansasFixture).FullName);
        }
    }

    public class MissouriFixture : Fixture{}
    public class ArkansasFixture : Fixture{}
    public class MontanaFixture : Fixture{}
    public class MichiganFixture : Fixture{}


    [TestFixture]
    public class LibraryBuilderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            observer = MockRepository.GenerateMock<IFixtureObserver>();
            builder = new LibraryBuilder(observer, new CompositeFilter<Type>());

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

            builder.Library.FixtureFor("GrammarError").Label.ShouldEqual("the bad grammars");
        }

        [Test]
        public void read_a_fixture_will_use_the_fixture_name_for_the_title_if_the_fixture_title_is_empty()
        {
            var fixture = new GrammarErrorFixture();


            builder.ReadFixture("GrammarError", fixture);

            builder.Library.FixtureFor("GrammarError").Label.ShouldEqual("GrammarError");
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
    public class creating_policies_for_fixtures
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var context = new TestContext(x => { x.AddFixture<FixtureWithHiddenGrammarsFixture>(); });

            var observer = MockRepository.GenerateMock<IFixtureObserver>();
            var builder = new LibraryBuilder(observer, new CompositeFilter<Type>());
            builder.Build(context);

            library = builder.Library;
        }

        #endregion

        private FixtureLibrary library;

        [Test]
        public void fixture_graph_should_have_the_policies_from_the_original_fixture()
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

    [TestFixture]
    public class when_reading_startup_actions_into_the_library
    {
        private FixtureLibrary library;

        [SetUp]
        public void SetUp()
        {
            var builder = new LibraryBuilder(new NulloFixtureObserver(), new CompositeFilter<Type>());
            var registry = new FixtureRegistry();
            registry.AddFixturesFromAssemblyContaining<SetUserAction>();

            var container = registry.BuildContainer();
            var context = new TestContext(container);


            library = builder.Build(context);
        }

        [Test]
        public void library_startup_action_names_should_include_the_names_of_the_registered_startup_actions()
        {
            library.StartupActions.ShouldHaveTheSameElementsAs("SetUser", "StartWebApp");
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