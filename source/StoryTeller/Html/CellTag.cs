using System;
using FubuCore;
using HtmlTags;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Html
{
    public class CellTag : HtmlTag
    {
        private readonly Cell _cell;
        private readonly IStep _step;

        public CellTag(Cell cell, IStep step)
            : base("span")
        {
            if (step == null) throw new ArgumentNullException("step");

            _cell = cell;
            _step = step;

            AddClass(HtmlClasses.INPUT);
        }

        public void WritePreview(ITestContext context)
        {
            string display = _cell.GetDisplay(_step);
            Text(context.GetDisplay(display));
        }

        public void WriteResults(StepResults results, ITestContext context)
        {
            if (!_cell.IsResult)
            {
                WritePreview(context);
                return;
            }

            var actual = results.HasActual(_cell.Key) ? results.GetActual(_cell.Key) : "MISSING";

            if (results.IsInException(_cell.Key))
            {
                Text("Error!");
                AddClass(HtmlClasses.EXCEPTION);

                return;
            }

            if (results.IsFailure(_cell.Key))
            {
                var expected = _step.Get(_cell.Key);
                string text = "{0}, but was '{1}'".ToFormat(expected, actual);
                Text(text);
                AddClass(HtmlClasses.FAIL);
            }
            else
            {
                Text(context.GetDisplay(actual));
                AddClass(HtmlClasses.PASS);
            }
        }
    }
}