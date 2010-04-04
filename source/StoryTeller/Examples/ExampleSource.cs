using System.Collections.Generic;
using System.Threading;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Execution;
using StoryTeller.Model;

namespace StoryTeller.Examples
{
    public interface IExampleSource
    {
        Example BuildExample(TPath path);
        Example BuildExample(IFixtureNode node);
    }

    public class ExampleSource : IListener<BinaryRecycleFinished>, IExampleSource
    {
        private readonly ITestConverter _converter;
        private readonly object _locker = new object();
        private readonly ManualResetEvent _reset = new ManualResetEvent(false);
        private Test _exampleTest;
        private FixtureLibrary _library;

        private ExampleSource()
        {
        }

        public ExampleSource(FixtureLibrary library)
            : this(library, new TestConverter())
        {
        }

        public ExampleSource(FixtureLibrary library, ITestConverter converter)
        {
            _converter = converter;
            writeExample(library);
        }

        public Test ExampleTest
        {
            get
            {
                lock (_locker)
                {
                    return _exampleTest;
                }
            }
        }

        #region IExampleSource Members

        public Example BuildExample(TPath path)
        {
            _reset.WaitOne();

            lock (_locker)
            {
                IFixtureNode node = _library.Find(path);
                return BuildExample(node);
            }
        }

        public Example BuildExample(IFixtureNode node)
        {
            lock (_locker)
            {
                Test test = _converter.Clone(_exampleTest);
                node.ModifyExampleTest(test);

                return new Example
                {
                    Title = node.Title,
                    Description = node.Description,
                    Html = _converter.ToPreview(_library, test),
                    Xml = _converter.ToXml(test)
                };
            }
        }

        #endregion

        #region IListener<BinaryRecycleFinished> Members

        public void Handle(BinaryRecycleFinished message)
        {
            writeExample(message.Library);
        }

        #endregion

        private void writeExample(FixtureLibrary library)
        {
            _reset.Reset();
            _library = library;
            ThreadPool.QueueUserWorkItem(x =>
            {
                lock (_locker)
                {
                    buildExampleTest(library);
                    _reset.Set();
                }
            });
        }

        private void buildExampleTest(FixtureLibrary library)
        {
            _exampleTest = new Test("Fixture and Grammar Examples");
            library.PossibleFixturesFor(_exampleTest).Each(f => _exampleTest.Add(f.CreateExample()));
        }

        public static ExampleSource For(ITestRunner runner)
        {
            var source = new ExampleSource();
            source._library = runner.Library;
            source.buildExampleTest(runner.Library);

            return source;
        }
    }
}