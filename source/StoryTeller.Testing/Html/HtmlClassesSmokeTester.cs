using NUnit.Framework;
using StoryTeller.Html;

namespace StoryTeller.Testing.Html
{
    [TestFixture]
    public class HtmlClassesSmokeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void can_retrieve_style()
        {
            HtmlClasses.CSS().ShouldNotBeEmpty();
        }
    }
}