using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using StoryTeller.Execution;

namespace StoryTeller.Testing.Execution
{
    [TestFixture]
    public class FixtureAssemblyTester
    {
        [Test]
        public void can_serialize_the_fixture_assembly_class()
        {
            var fa = new FixtureAssembly(null, "StoryTeller.Testing");
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, fa);

            stream.Position = 0;

            var fa2 = (FixtureAssembly)formatter.Deserialize(stream);
        }
    }
}