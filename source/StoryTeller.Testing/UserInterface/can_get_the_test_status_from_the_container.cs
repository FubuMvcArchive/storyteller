using NUnit.Framework;
using StoryTeller.Execution;
using StoryTeller.UserInterface;
using StructureMap;

namespace StoryTeller.Testing.UserInterface
{
    [TestFixture]
    public class can_get_the_test_status_from_the_container
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Bootstrapper.ForceRestart();
        }

        #endregion

        [Test]
        public void smoke_test_if_can_get_the_test_status()
        {
            ObjectFactory.GetInstance<TestStatusMessage>().ShouldNotBeNull();
        }
    }
}