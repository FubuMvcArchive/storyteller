using NUnit.Framework;
using StoryTeller.Engine;
using StoryTeller.Engine.Sets;

namespace StoryTeller.Testing.Engine.Sets
{
    [TestFixture]
    public class SetRowTester
    {
        private TestContext context;
        private SetRow row1;
        private SetRow row2;

        [SetUp]
        public void SetUp()
        {
            context = new TestContext();
            row1 = new SetRow();
            row2 = new SetRow();
        }

        [Test]
        public void matches_is_true_with_two_properties()
        {
            row1.Values["a"] = row2.Values["a"] = 1;
            row1.Values["b"] = row2.Values["b"] = 2;
        
            row1.Matches(context, row2).ShouldBeTrue();
        }

        [Test]
        public void never_matches_if_there_are_missing_values()
        {
            row1.Values["a"] = row2.Values["a"] = 1;
            row1.Values["b"] = row2.Values["b"] = 2;

            row1.MissingValues = true;

            row1.Matches(context, row2).ShouldBeFalse();
        }

        [Test]
        public void matches_is_false_for_two_properties()
        {
            row1.Values["a"] = row2.Values["a"] = 1;
            row1.Values["b"] = 2;
            row2.Values["b"] = 3;

            row1.Matches(context, row2).ShouldBeFalse();
        }

        [Test]
        public void matches_is_false_when_a_property_is_missing_in_the_expected()
        {
            row1.Values["a"] = row2.Values["a"] = 1;
            row1.Values["b"] = 2;
            //row2.Values["b"] = 2;

            row1.Matches(context, row2).ShouldBeFalse();
        }
    }
}