using NUnit.Framework;
using StoryTeller.Model;
using StoryTeller.UserInterface.Workspace;
using StoryTeller.Workspace;

namespace StoryTeller.Testing.UserInterface.Workspace
{
    [TestFixture]
    public class NamespaceSelectorTester
    {
        private NamespaceSelector selector;
        private NamespaceSelector child1;
        private NamespaceSelector child2;
        private FixtureSelector f1;
        private FixtureSelector f2;
        private FixtureSelector f3;
        private FixtureSelector f4;
        private FixtureSelector f5;

        [SetUp]
        public void SetUp()
        {
            selector = new NamespaceSelector("NS1");
            child1 = new NamespaceSelector("NS1-1");
            child2 = new NamespaceSelector("NS1-2");

            f1 = new FixtureSelector(new FixtureDto() {Name = "fixture1"});
            f2 = new FixtureSelector(new FixtureDto() {Name = "fixture2"});
            f3 = new FixtureSelector(new FixtureDto() {Name = "fixture3"});
            f4 = new FixtureSelector(new FixtureDto() {Name = "fixture4"});
            f5 = new FixtureSelector(new FixtureDto() {Name = "fixture5"});
        
            selector.Add(child1);
            selector.Add(child2);
            selector.Add(f1);
            child1.Add(f2);
            child1.Add(f3);
            child2.Add(f4);
            child2.Add(f5);
        }

        [Test]
        public void cherry_pick_filters_from_children_1()
        {
            child2.Selected.IsChecked = true;

            selector.GetFilters().ShouldHaveTheSameElementsAs(FixtureFilter.Namespace("NS1-2"));
        }

        [Test]
        public void cherry_pick_filters_from_children_2()
        {
            child2.Selected.IsChecked = true;
            f2.Select(true);
            f1.Select(true);

            selector.GetFilters().ShouldHaveTheSameElementsAs(
                FixtureFilter.Fixture("fixture2"), 
                FixtureFilter.Namespace("NS1-2"), 
                FixtureFilter.Fixture("fixture1"));
        }

        [Test]
        public void select_the_top_level_disables_all_children()
        {
            selector.Selected.IsChecked = true;

            child1.IsEnabled.ShouldBeFalse();
            child2.IsEnabled.ShouldBeFalse();
            f1.IsEnabled.ShouldBeFalse();
            f2.IsEnabled.ShouldBeFalse();
            f3.IsEnabled.ShouldBeFalse();
            f4.IsEnabled.ShouldBeFalse();
            f5.IsEnabled.ShouldBeFalse();
        }

        [Test]
        public void selecting_the_top_level_results_in_a_single_filter()
        {
            selector.Selected.IsChecked = true;

            // Doesn't matter, namespace rules
            f1.Select(true);
            f2.Select(true);
            f3.Select(true);

            selector.GetFilters().ShouldHaveTheSameElementsAs(FixtureFilter.Namespace("NS1"));
        }

        [Test]
        public void enabling_a_namespace_does_not_enable_children_if_it_is_already_selected()
        {
            child1.Selected.IsChecked = true;

            f2.IsEnabled.ShouldBeFalse();
            f3.IsEnabled.ShouldBeFalse();

            child1.Enable(true);

            f2.IsEnabled.ShouldBeFalse();
            f3.IsEnabled.ShouldBeFalse();
        }


    }
}