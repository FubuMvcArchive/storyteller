using NUnit.Framework;
using StoryTeller.Testing;
using StoryTeller.UserInterface.Tests;

namespace StoryTeller.UserInterface.Testing.UI.Tests
{
    [TestFixture]
    public class ResultsModeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            mode = new ResultsMode(null, null);
        }

        #endregion

        private ResultsMode mode;

        [Test]
        public void is_enabled_when_the_test_has_a_last_result()
        {
            mode.IsEnabled(StoryTeller.Testing.DataMother.SuccessfulTest()).ShouldBeTrue();
        }

        [Test]
        public void is_enabled_when_the_test_has_no_result()
        {
            mode.IsEnabled(StoryTeller.Testing.DataMother.TestWithNoResults()).ShouldBeFalse();
        }
    }
}