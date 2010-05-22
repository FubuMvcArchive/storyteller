using System.Windows.Controls;
using NUnit.Framework;
using StoryTeller.Model;
using StoryTeller.UserInterface.Workspace;
using System.Linq;
using StoryTeller.Workspace;

namespace StoryTeller.Testing.UserInterface.Workspace
{
    [TestFixture]
    public class FixtureSelectorTester
    {
        private FixtureDto dto;
        private FixtureSelector selector;

        [SetUp]
        public void SetUp()
        {
            dto = new FixtureDto()
            {
                Fullname = "the full name",
                Name = "Math",
                Namespace = "some namespace"
            };

            selector = new FixtureSelector(dto);
        }

        [Test]
        public void is_selected_positive()
        {
            selector.Children[0].As<CheckBox>().IsChecked = true;
            selector.IsSelected().ShouldBeTrue();
        }

        [Test]
        public void is_selected_negative()
        {
            selector.Children[0].As<CheckBox>().IsChecked = false;
            selector.IsSelected().ShouldBeFalse();
        }

        [Test]
        public void get_filter_when_selected()
        {
            selector.Select(true);

            var filters = selector.GetFilters();
            filters.Count().ShouldEqual(1);

            FixtureFilter fixtureFilter = filters.First();
            fixtureFilter.Name.ShouldEqual(dto.Name);
            fixtureFilter.Type.ShouldEqual(FilterType.Fixture);
        }

        [Test]
        public void get_filter_when_not_selected_should_be_0_length()
        {
            selector.Select(false);
            selector.GetFilters().Any().ShouldBeFalse();
        }
    }
}