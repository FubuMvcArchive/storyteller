using System;
using System.Collections.Generic;
using StoryTeller.Domain;

namespace StoryTeller.Engine
{
    public class FactGrammar : LineGrammar
    {
        private readonly Func<bool> _test;
        private readonly string _title;

        public FactGrammar(Func<bool> test, string title)
            : base(title)
        {
            _test = test;
            _title = title;
        }


        public override string Description { get { return _title; } }

        public override void Execute(IStep containerStep, ITestContext context)
        {
            cell().RecordActual(_test(), containerStep, context);
        }

        private Cell cell()
        {
            return new Cell("returnValue", typeof (bool))
            {
                IsResult = true
            };
        }

        public override IList<Cell> GetCells()
        {
            return new List<Cell>
            {
                cell()
            };
        }
    }
}