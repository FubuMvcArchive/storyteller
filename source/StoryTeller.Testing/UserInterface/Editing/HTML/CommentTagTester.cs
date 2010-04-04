using NUnit.Framework;
using StoryTeller.UserInterface.Editing.HTML;

namespace StoryTeller.Testing.UserInterface.Editing.HTML
{
    [TestFixture]
    public class CommentTagTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            tag = new CommentTag();
        }

        #endregion

        private CommentTag tag;
    }
}