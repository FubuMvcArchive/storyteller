using System;
using FubuCore.Util;
using NUnit.Framework;
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

            var filter = workspace.CreateFilter();

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

            var filter = combined.CreateFilter();

            filter.Matches(typeof(MathFixture)).ShouldBeTrue();

            filter.Matches(typeof(AnotherFixture)).ShouldBeTrue();

            filter.Matches(GetType()).ShouldBeFalse();
        }
    }
}