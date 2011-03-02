using System;
using System.Collections.Generic;

namespace StoryTeller.Domain
{
    public class Tags : ITestPart
    {
        public static readonly string TAGS = "TestTags";

        public Tags()
        {
        }

        public Tags(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        #region ITestPart Members

        public void AcceptVisitor(ITestVisitor visitor)
        {
            visitor.WriteTags(this);
        }

        public int StepCount()
        {
            return 0;
        }

        public IEnumerable<ITestPart> Children { get { return new ITestPart[0]; } }

        #endregion

        public bool Equals(Tags obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Text, Text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Tags)) return false;
            return Equals((Tags)obj);
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0);
        }
    }
}