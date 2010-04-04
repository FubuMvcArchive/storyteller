using System;
using System.Windows;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.UserInterface;

namespace StoryTeller.AcceptanceTesting.Fixtures
{
    public class StoryTellerUIFixture : Fixture
    {
        private readonly SystemUnderTest _system;
        private Window _window;

        public StoryTellerUIFixture(SystemUnderTest system)
        {
            _system = system;

            this["TheTestsAre"] = this["AddTest"].AsTable("The tests are").After(_system.ReloadProject);
        }

        public override void SetUp(ITestContext context)
        {
            _system.Do<Window>(x =>
            {
                _window = x;
                _window.Show();
            });
        }

        public override void TearDown()
        {
            _window.Hide();
        }


        [FormatAs("Open test {path}")]
        public void OpenTest(string path)
        {
            Test test = _system.Hierarchy.FindTest(path);
            _system.Do<IScreenConductor>(x => { x.OpenScreen(test); });
        }

        public IGrammar TheCommandButtonsAre()
        {
            throw new NotImplementedException();
        }

        public void AddTest(string path)
        {
            _system.AddTest(path);
        }
    }

    public class ButtonToken
    {
        public string Text { get; set; }
        public bool Enabled { get; set; }
    }
}