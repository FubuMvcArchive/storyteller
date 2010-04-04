using StoryTeller;
using StoryTeller.Engine;

namespace StoryTellerUI
{
    public class TableFixture : Fixture
    {
        [ExposeAsTable("Adding Numbers", "add")]
        public void Add(int w, int x, int y, [Default("2")] int z, [Default("3")] int sum)
        {
        }
    }
}