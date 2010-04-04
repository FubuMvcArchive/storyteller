using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FubuCore;
using StoryTeller.Engine;

namespace StoryTeller.Domain
{
    public class Section : ITestPart, IStepHolder
    {
        private readonly List<ITestPart> _parts = new List<ITestPart>();
        private FixtureLoader _loader;

        private Section()
        {
        }

        public Section(string fixtureName)
        {
            _loader = new FixtureKeyLoader(fixtureName, this);
        }

        public string Description { get; set; }

        public string FixtureName { get { return _loader.GetName(); } }

        public string Label { get; set; }
        public ReadOnlyCollection<ITestPart> Parts { get { return new ReadOnlyCollection<ITestPart>(_parts); } }

        #region IStepHolder Members

        public void Add(Comment comment)
        {
            _parts.Add(comment);
        }

        public void Remove(Comment comment)
        {
            _parts.Remove(comment);
        }

        public void MoveUp(Comment t)
        {
            _parts.MoveUp(t);
        }

        public void MoveDown(Comment t)
        {
            _parts.MoveDown(t);
        }

        public void Add(IStep step)
        {
            _parts.Add(step);
        }

        public void Remove(IStep subject)
        {
            _parts.Remove(subject);
        }

        public void MoveUp(IStep t)
        {
            _parts.MoveUp(t);
        }

        public void MoveDown(IStep t)
        {
            _parts.MoveDown(t);
        }

        public IList<IStep> AllSteps()
        {
            return _parts.AllSteps();
        }

        #endregion

        #region ITestPart Members

        public void AcceptVisitor(ITestVisitor visitor)
        {
            visitor.StartSection(this);

            _parts.ForEach(x => x.AcceptVisitor(visitor));

            visitor.EndSection(this);
        }

        public int StepCount()
        {
            return _parts.Sum(x => x.StepCount());
        }

        public IEnumerable<ITestPart> Children
        {
            get
            {
                foreach (ITestPart child in _parts)
                {
                    yield return child;

                    foreach (ITestPart descendent in child.Children)
                    {
                        yield return descendent;
                    }
                }
            }
        }

        #endregion

        public static Section For<T>() where T : IFixture
        {
            var section = new Section();
            section._loader = new FixtureLoader<T>(section);

            return section;
        }

        public static Section For<T>(IList<ITestPart> list) where T : IFixture
        {
            Section section = For<T>();
            section._parts.AddRange(list);

            return section;
        }

        public void StartFixture(IFixtureContext context)
        {
            _loader.LoadFixture(context);
        }

        public Section WithStep(string key, string data)
        {
            Step step = new Step(key).With(data);
            _parts.Add(step);

            return this;
        }

        public Section WithStep(string key, Action<Step> action)
        {
            var step = new Step(key);
            _parts.Add(step);

            action(step);

            return this;
        }

        public Section WithStep(string key)
        {
            var step = new Step(key);
            _parts.Add(step);

            return this;
        }

        public string GetName()
        {
            return Label ?? _loader.GetName();
        }

        public Section WithComment(string text)
        {
            var comment = new Comment
            {
                Text = text
            };
            _parts.Add(comment);

            return this;
        }

        public Section WithDescription(string text)
        {
            Description = text;
            return this;
        }

        public void RemoveParts(Predicate<ITestPart> filter)
        {
            _parts.RemoveAll(filter);
        }

        public override string ToString()
        {
            return string.Format("Section: {0}", GetName());
        }

        public Step AddStep(string grammarKey)
        {
            var step = new Step(grammarKey);
            Add(step);

            return step;
        }

        #region Nested type: FixtureKeyLoader

        internal class FixtureKeyLoader : FixtureLoader
        {
            private readonly string _fixtureName;
            private readonly Section _section;

            public FixtureKeyLoader(string fixtureName, Section section)
            {
                _fixtureName = fixtureName;
                _section = section;
            }

            #region FixtureLoader Members

            public void LoadFixture(IFixtureContext context)
            {
                context.LoadFixture(_fixtureName, _section);
            }

            public string GetName()
            {
                return _fixtureName;
            }

            #endregion
        }

        #endregion

        #region Nested type: FixtureLoader

        internal interface FixtureLoader
        {
            void LoadFixture(IFixtureContext context);
            string GetName();
        }

        internal class FixtureLoader<T> : FixtureLoader where T : IFixture
        {
            private readonly Section _section;

            public FixtureLoader(Section section)
            {
                _section = section;
            }

            #region FixtureLoader Members

            public void LoadFixture(IFixtureContext context)
            {
                context.LoadFixture<T>(_section);
            }

            public string GetName()
            {
                return typeof(T).GetFixtureAlias();
            }

            #endregion

        }

        #endregion
    }
}