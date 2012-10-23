using System;

namespace StoryTeller.Engine
{
    public class NulloSystem : ISystem
    {
        #region ISystem Members

        public IExecutionContext CreateContext()
        {
            throw new NotImplementedException();
        }

        public void Recycle()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}