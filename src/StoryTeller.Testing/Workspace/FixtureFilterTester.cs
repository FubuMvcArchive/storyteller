using System;
using FubuCore.Util;
using NUnit.Framework;
using StoryTeller.Model;
using StoryTeller.Samples;
using StoryTeller.Workspace;
using FubuCore;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Workspace
{
    [TestFixture]
    public class FixtureFilterTester
    {
        [Test]
        public void apply_the_always_filter()
        {
            var composite = new CompositeFilter<Type>();
            FixtureFilter.All().Apply(composite);

            composite.Matches(typeof(MathFixture)).ShouldBeTrue();
            composite.Matches(typeof(AnotherFixture)).ShouldBeTrue();
            composite.Matches(this.GetType()).ShouldBeTrue();
        }


        [Test]
        public void apply_a_filter_at_namespace_level()
        {
            var filter = new FixtureFilter
            {
                Name = typeof(MathFixture).Namespace,
                Type = FilterType.Namespace
            };

            var composite = new CompositeFilter<Type>();
            filter.Apply(composite);

            composite.Matches(typeof (MathFixture)).ShouldBeTrue();

            typeof (AnotherFixture).IsInNamespace(typeof (MathFixture).Namespace);
            composite.Matches(typeof (AnotherFixture)).ShouldBeTrue();
        
            composite.Matches(GetType()).ShouldBeFalse();
        }

        [Test]
        public void apply_a_filter_at_the_type_level()
        {
            var filter = new FixtureFilter
            {
                Name = typeof(MathFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            };

            var composite = new CompositeFilter<Type>();
            filter.Apply(composite);

            composite.Matches(typeof(MathFixture)).ShouldBeTrue();

            composite.Matches(typeof(AnotherFixture)).ShouldBeFalse();

            composite.Matches(GetType()).ShouldBeFalse();
        }

        [Test]
        public void apply_a_filter_on_fixture_graph_at_the_type_level()
        {
            var filter = new FixtureFilter
            {
                Name = typeof(MathFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            };

            var composite = new CompositeFilter<FixtureStructure>();
            filter.Apply(composite);

            composite.Matches(new FixtureStructure("Math")).ShouldBeTrue();
            composite.Matches(new FixtureStructure("Math1")).ShouldBeFalse();
            composite.Matches(new FixtureStructure("SomethingElse")).ShouldBeFalse();

        }

        [Test]
        public void apply_a_filter_on_fixture_graph_at_the_namespace_level()
        {
            var filter = new FixtureFilter()
            {
                Name = "ST.Grammars",
                Type = FilterType.Namespace
            };

            var composite = new CompositeFilter<FixtureStructure>();
            filter.Apply(composite);

            composite.Matches(new FixtureStructure(){FixtureNamespace = "ST.Grammars"}).ShouldBeTrue();
            composite.Matches(new FixtureStructure(){FixtureNamespace = "ST.Grammars.More"}).ShouldBeTrue();
            composite.Matches(new FixtureStructure(){FixtureNamespace = "More.ST.Grammars.More"}).ShouldBeFalse();
            composite.Matches(new FixtureStructure(){FixtureNamespace = "SomethingDifferent"}).ShouldBeFalse();
        }


        [Test]
        public void integrate_with_workspace_filter()
        {
            var workspace = new WorkspaceFilter();
            workspace.AddFilter(new FixtureFilter()
            {
                Name = typeof(MathFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            });

            workspace.AddFilter(new FixtureFilter()
            {
                Name = typeof(AnotherFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            });

            var filter = workspace.CreateTypeFilter();

            filter.Matches(typeof(MathFixture)).ShouldBeTrue();

            filter.Matches(typeof(AnotherFixture)).ShouldBeTrue();

            filter.Matches(GetType()).ShouldBeFalse();
        }

        [Test]
        public void can_serialize_and_deserialize_a_workspace_filter()
        {
            var workspace = new WorkspaceFilter();
            workspace.AddFilter(new FixtureFilter()
            {
                Name = typeof(MathFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            });

            workspace.AddFilter(new FixtureFilter()
            {
                Name = typeof(AnotherFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            });

            workspace.ShouldBeSerializable().ShouldBeOfType<WorkspaceFilter>().FilterCount.ShouldEqual(2);
        }

        [Test]
        public void create_a_merged_workspace_filter()
        {
            var workspace1 = new WorkspaceFilter();
            workspace1.AddFilter(new FixtureFilter()
            {
                Name = typeof(MathFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            });

            var workspace2 = new WorkspaceFilter();
            workspace2.AddFilter(new FixtureFilter()
            {
                Name = typeof(AnotherFixture).GetFixtureAlias(),
                Type = FilterType.Fixture
            });

            var combined = new WorkspaceFilter(new WorkspaceFilter[]{ workspace1, workspace2});

            var filter = combined.CreateTypeFilter();

            filter.Matches(typeof(MathFixture)).ShouldBeTrue();

            filter.Matches(typeof(AnotherFixture)).ShouldBeTrue();

            filter.Matches(GetType()).ShouldBeFalse();
        }

        [Test]
        public void workspace_filter_returns_always_when_it_has_no_explicit_filters()
        {
            var workspace = new WorkspaceFilter();
            workspace.FilterCount.ShouldEqual(0);

            workspace.Filters.ShouldHaveTheSameElementsAs(FixtureFilter.All());
        }
    }
}