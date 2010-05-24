using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using FubuCore;
using StoryTeller.Engine;
using StoryTeller.Persistence;

namespace StoryTeller.Domain
{
    public enum Lifecycle
    {
        Acceptance,
        Regression,
        Any
    }

    public static class TestExtensions
    {
        public static XmlDocument ToXml(this Test test)
        {
            return new TestWriter().Write(test);
        }
    }

    
    public interface ITestPartCollection : IEnumerable<ITestPart>
    {
        void Add(ITestPart testPart);
        void RemoveAll(Predicate<ITestPart> filter);
        void Clear();
        void AddRange(IEnumerable<ITestPart> parts);
        ITestPart Find(Predicate<ITestPart> func);
    }

    public class DefaultTestPartcollection : ITestPartCollection
    {
        private readonly List<ITestPart> _parts = new List<ITestPart>();
        public IEnumerator<ITestPart> GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        public void Add(ITestPart testPart)
        {
            _parts.Add(testPart);
        }

        public void RemoveAll(Predicate<ITestPart> filter)
        {
            _parts.RemoveAll(filter);
        }

        public void Clear()
        {
            _parts.Clear();
        }

        public void AddRange(IEnumerable<ITestPart> parts)
        {
            _parts.AddRange(parts);
        }

        public ITestPart Find(Predicate<ITestPart> func)
        {
            return _parts.Find(func);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Test : INamedItem, ITestPart
    {
        protected readonly ITestPartCollection _parts;
        private string _fileName;
        private Lifecycle _lifecycle = Lifecycle.Acceptance;
        private string _name;

        public Test(string name, ITestPartCollection parts)
        {
            _parts = parts;
            _name = name;
        }

        public Test(string name) : this(name, new DefaultTestPartcollection())
        {
        }


        public Test(string name, Action<Test> initialization)
            : this(name)
        {
            initialization(this);
        }

        public Lifecycle Lifecycle { get { return _lifecycle; } set { _lifecycle = value; } }

        public TestResult LastResult
        {
            get;
            set;
        }

        public string SuiteName { get; set; }
        public Suite Parent { get; set; }

         public string FileName
        {
            get
            {
                string filename = _fileName.IsNotEmpty() ? _fileName : Name + ".xml";
                TPath path = GetPath();

                return Path.Combine(path.GetContainingFolder(), filename);
            }
            set { _fileName = value; }
        }


        public ReadOnlyCollection<ITestPart> Parts { get { return new ReadOnlyCollection<ITestPart>(_parts.ToList()); } }
        public IEnumerable<ITestPart> AllParts { get { return new List<ITestPart>(allParts); } }

        private IEnumerable<ITestPart> allParts
        {
            get
            {
                foreach (ITestPart part in _parts)
                {
                    yield return part;

                    foreach (ITestPart child in part.Children)
                    {
                        yield return child;
                    }
                }
            }
        }

        #region INamedItem Members

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                Path.GetInvalidFileNameChars().Each(x => { _name = _name.Replace(x, ' '); });
            }
        }

        public TPath GetPath()
        {
            if (Parent.IsSuite())
            {
                return Parent.GetPath().Append(Name);
            }

            return new TPath(Name);
        }

        #endregion

        #region ITestVisitable Members

        public void AcceptVisitor(ITestVisitor visitor)
        {
            _parts.Each(x => x.AcceptVisitor(visitor));
        }

        #endregion



        public void Toggle()
        {
            _lifecycle = Lifecycle == Lifecycle.Acceptance
                             ? Lifecycle.Regression
                             : Lifecycle.Acceptance;
        }

        public Test LifecycleIs(Lifecycle lifecycle)
        {
            _lifecycle = lifecycle;
            return this;
        }


        public Test Section<T>(Action<Section> action) where T : IFixture
        {
            Section section = Domain.Section.For<T>();
            _parts.Add(section);

            action(section);

            return this;
        }


        public Section Section(string fixureName)
        {
            var section = new Section(fixureName);
            _parts.Add(section);

            return section;
        }

        public Test WithCounts(int rights, int wrongs, int syntaxErrors, int exceptions)
        {
            if (LastResult == null) LastResult = new TestResult() { Counts = new Counts() };
            LastResult.Counts.Rights = rights;
            LastResult.Counts.Wrongs = wrongs;
            LastResult.Counts.SyntaxErrors = syntaxErrors;
            LastResult.Counts.Exceptions = exceptions;

            return this;
        }

        public Comment AddComment(string text)
        {
            var comment = new Comment
            {
                Text = text
            };
            _parts.Add(comment);

            return comment;
        }

        public bool Equals(Test obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj.Name, Name) && Equals(obj.SuiteName, SuiteName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Test)) return false;
            return Equals((Test)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^
                       (SuiteName != null ? SuiteName.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Suite: {1}", Name, SuiteName);
        }

        public void RemoveParts(Predicate<ITestPart> filter)
        {
            _parts.RemoveAll(filter);
        }

        public string GetStatus()
        {
            if (!HasResult()) return string.Empty;

            if (LastResult.Counts.WasSuccessful())
            {
                return "Succeeded with {0} in {1} seconds".ToFormat(LastResult.Counts, LastResult.ExecutionTime);
            }

            return "Failed with {0} in {1} seconds".ToFormat(LastResult.Counts, LastResult.ExecutionTime);
        }

        public bool HasResult()
        {
            return LastResult != null;
        }

        public bool WasSuccessful()
        {
            return HasResult() ? LastResult.Counts.WasSuccessful() : false;
        }

        public void ApplyChanges(Test otherTest)
        {
            lock (_parts)
            {
                _parts.Clear();
                _parts.AddRange(otherTest._parts);
            }
        }

        public void Reset()
        {
            LastResult = null;
        }

        public bool IsEmpty()
        {
            return _parts.Find(p => p is Section) == null;
        }

        public string LocatorPath()
        {
            return GetPath().Locator;
        }

        public void Add(Section section)
        {
            _parts.Add(section);
        }

        public Test With(Section section)
        {
            Add(section);
            return this;
        }

        public Test Clone(string name)
        {
            XmlDocument document = new TestWriter().Write(this);
            Test clone = new TestReader().ReadTest(document.DocumentElement);
            clone.Name = name;

            return clone;
        }

        public IEnumerable<ITestPart> Children
        {
            get { return _parts; }
        }

        public int StepCount()
        {
            return 0;
        }

        public void Add(Comment comment)
        {
            _parts.Add(comment);
        }

        public bool IsInWorkspace(string workspace)
        {
            if (Parent == null) return false;
            return GetPath().Workspace == workspace;
        }

        public string Workspace
        {
            get
            {
                if (Parent == null) return string.Empty;

                return GetPath().Workspace;
            }
        }
    }
}