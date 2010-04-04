using NUnit.Framework;
using Rhino.Mocks;
using StoryTeller.Execution;

namespace StoryTeller.Testing.Execution
{
    [TestFixture]
    public class FixtureObserverTester : InteractionContext<FixtureObserver>
    {
        [Test]
        public void forward_message_to_event_aggregator()
        {
            ClassUnderTest.ReadingFixture(5, 3, "Fixture1");

            MockFor<IEventPublisher>().AssertWasCalled(x => x.Publish(new BinaryRecycleProcess
            {
                FixtureName = "Fixture1",
                Index = 3,
                Total = 5
            }));
        }

        [Test]
        public void record_message_should_send_event()
        {
            ClassUnderTest.RecordStatus("Deleting data");

            MockFor<IEventPublisher>().AssertWasCalled(x => x.Publish(new BinaryRecycleMessage
            {
                Message = "Deleting data"
            }));
        }
    }
}