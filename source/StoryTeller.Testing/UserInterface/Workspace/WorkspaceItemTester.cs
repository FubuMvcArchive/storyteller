using System.Windows.Controls;
using NUnit.Framework;
using StoryTeller.UserInterface.Workspace;

namespace StoryTeller.Testing.UserInterface.Workspace
{
    [TestFixture]
    public class WorkspaceItemTester
    {
        [Test]
        public void create_a_selected_workspace_item()
        {
            var item = new WorkspaceItem("something", true);
            item.Children[0].ShouldBeOfType<CheckBox>().IsChecked.ShouldEqual(true);
            
            item.Selected.ShouldBeTrue();
        }

        [Test]
        public void should_put_the_workspace_name_in_the_label()
        {
            var item = new WorkspaceItem("something", true);
            item.Children[1].ShouldBeOfType<Label>().Content.ShouldEqual("something");
        }

        [Test]
        public void should_remember_the_workspace_name()
        {
            var item = new WorkspaceItem("something", true);
            item.WorkspaceName.ShouldEqual("something");
        }

        [Test]
        public void create_a_non_selected_workspace_item()
        {
            var item = new WorkspaceItem("something", false);
            item.Children[0].ShouldBeOfType<CheckBox>().IsChecked.ShouldEqual(false);

            item.Selected.ShouldBeFalse();
        }

        [Test]
        public void should_expose_the_selected_value()
        {
            var item = new WorkspaceItem("something", false);
            item.Selected.ShouldBeFalse();

            item.Children[0].ShouldBeOfType<CheckBox>().IsChecked = true;
            item.Selected.ShouldBeTrue();

            item.Children[0].ShouldBeOfType<CheckBox>().IsChecked = false;
            item.Selected.ShouldBeFalse();
        }
    }
}