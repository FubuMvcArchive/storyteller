using System;

namespace StoryTeller.Engine
{
    [Serializable]
    public class TestResult
    {
        public TestResult()
        {
            Counts = new Counts();
        }

        public string Html { get; set; }
        public Counts Counts { get; set; }
        public double ExecutionTime { get; set; }
        public string ExceptionText { get; set; }
        public bool WasCancelled { get; set; }
    }
}