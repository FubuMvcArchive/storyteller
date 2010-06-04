using System;
using FubuCore;

namespace StoryTeller.Usages
{
    public class ConsoleUsageGraphListener : IUsageGraphListener
    {
        public void ReadingTest(string path)
        {
            Console.WriteLine("Reading " + path);
        }

        public void Start(int testCount)
        {
            Console.WriteLine("Starting to find usages for {0} tests".ToFormat(testCount));
        }

        public void Finished()
        {
            Console.WriteLine("Finished reading usages");
        }
    }


    public interface IUsageGraphListener
    {
        void ReadingTest(string path);
        void Start(int testCount);
        void Finished();
    }
}