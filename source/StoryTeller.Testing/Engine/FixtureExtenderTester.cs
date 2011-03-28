using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Engine;
using StructureMap;

namespace StoryTeller.Testing.Engine
{
    [TestFixture]
    public class FixtureExtenderTester
    {        
        [Test]
        public void should_match_a_fixture_that_is_not_an_extender()
        {
            new FixtureExtender().MatchesType(typeof(FakeFixture)).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_a_fixture_that_is_also_an_extender()
        {
            new FixtureExtender().MatchesType(typeof(FakeExtenderFixture)).ShouldBeFalse();
        }

        

    }

    [TestFixture]
    public class when_processing_a_fixture_with_extensions_happy_path
    {
        private IContext theContext;
        private FakeFixture theFixture;

        [SetUp]
        public void SetUp()
        {
            theContext = MockRepository.GenerateMock<IContext>();

            theContext.Stub(x => x.GetAllInstances<IExtender<FakeFixture>>())
                .Return(new IExtender<FakeFixture>[]{
                    new FakeExtenderFixture(), new FakeExtender2Fixture()
                });

            theFixture = new FakeFixture();
            new FixtureExtender().Process(theFixture, theContext);
        }

        [Test]
        public void the_grammars_from_the_extender_fixture_should_be_imported_to_the_fixture()
        {
            var list = new List<string>();

            theFixture.ForEachGrammar((name, g) => list.Add(name));

            list.ShouldHaveTheSameElementsAs("Go", "Go2", "Go3", "Go4", "Go5", "Go6");
        }        
    }

    [TestFixture]
    public class when_processing_a_fixture_with_extensions_sad_path
    {
        private IContext theContext;
        private FakeFixture theFixture;

        [SetUp]
        public void SetUp()
        {
            theContext = MockRepository.GenerateMock<IContext>();

            theContext.Stub(x => x.GetAllInstances<IExtender<FakeFixture>>())
                .Return(new IExtender<FakeFixture>[]{
                    new FakeExtenderFixture(), new FakeExtender2Fixture()
                });

            theFixture = new FakeFixture();
        }

        [Test]
        public void should_throw_an_exception_when_adding_a_grammar_with_a_duplicate_key()
        {
            theFixture["Go"] = Fixture.Do(() => { });
            typeof(ApplicationException).ShouldBeThrownBy(() => new FixtureExtender().Process(theFixture, theContext));
       
        }
    }

    [TestFixture, Explicit]
    public class when_building_a_container_from_fixture_registry_with_extensions
    {
        [Test]
        public void should_apply_the_extender_to_fixtures()
        {
            var fixtureRegistry = new FixtureRegistry();
            fixtureRegistry.AddFixture<FakeFixture>();
            fixtureRegistry.RegisterServices(r =>
            {
                r.For<IExtender<FakeFixture>>().AddInstances(x =>
                {
                    x.Type<FakeExtenderFixture>();
                    x.Type<FakeExtender2Fixture>();
                });
            });

            var theFixture = fixtureRegistry.BuildContainer().GetInstance<FakeFixture>();

            var list = new List<string>();

            theFixture.ForEachGrammar((name, g) => list.Add(name));

            list.ShouldHaveTheSameElementsAs("Go", "Go2", "Go3", "Go4", "Go5", "Go6");
        }
    }



    public class FakeFixture : Fixture{}

    public class FakeExtenderFixture : ExtendsFixture<FakeFixture>
    {
        public void Go(){} 
        public void Go2(){} 
        public void Go3(){} 
    }

    public class FakeExtender2Fixture : ExtendsFixture<FakeFixture>
    {
        public void Go4() { }
        public void Go5() { }
        public void Go6() { }
    }
}