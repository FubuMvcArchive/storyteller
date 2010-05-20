using System;

namespace StoryTeller.Execution
{
    [Serializable, Obsolete("Time for this thing to die")]
    public class Example
    {
        public string Html { get; set; }
        public string Xml { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}