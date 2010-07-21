using System.Threading;
using StoryTeller.Engine;

namespace StoryTeller.Execution
{
    public class FixtureObserver : IFixtureObserver
    {
        private readonly IEventPublisher _events;

        public FixtureObserver(IEventPublisher events)
        {
            _events = events;
        }

        #region IFixtureObserver Members

        public void ReadingFixture(int total, int number, string fixtureName)
        {
            _events.Publish(new BinaryRecycleProcess
            {
                FixtureName = fixtureName,
                Index = number,
                Total = total
            });
        }

        public void RecordStatus(string statusMessage)
        {
            _events.Publish(new BinaryRecycleMessage
            {
                Message = statusMessage
            });
        }

        #endregion
    }
}