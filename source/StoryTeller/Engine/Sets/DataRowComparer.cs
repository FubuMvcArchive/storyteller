using System;
using System.Collections.Generic;
using System.Data;
using FubuCore;
using StoryTeller.Assertions;
using StoryTeller.Domain;

namespace StoryTeller.Engine.Sets
{
    public interface IDataRowComparer
    {
        DataRowComparer MatchOn<T>(string columnName);
    }

    public class DataRowComparer : ISetComparer, IDataRowComparer
    {
        private readonly IList<ISetColumn> _columns = new List<ISetColumn>();

        public DataRowComparer MatchOn<T>(string columnName)
        {
            var match = new DataColumnMatch(columnName, typeof (T));
            _columns.Add(match);

            return this;
        }

        public IEnumerable<ISetColumn> Columns
        {
            get { return _columns; } 
        }
    }

    public class DataColumnMatch : ISetColumn
    {
        private readonly string _columnName;
        private readonly Type _columnType;

        public DataColumnMatch(string columnName, Type columnType)
        {
            _columnName = columnName;
            _columnType = columnType;
        }

        public Cell Cell
        {
            get 
            {
                return new Cell(_columnName, _columnType);
            } 
        }

        public void ReadExpected(ITestContext context, IStep step, SetRow row)
        {
            Cell.ReadArgument(context, step, x =>
            {
                row.Values[_columnName] = x;
            });
        }

        public void ReadActual(object target, SetRow row)
        {
            var dataRow = target.As<DataRow>();
            assertColumnExists(dataRow);
            
            row.Values[_columnName] = dataRow[_columnName] == DBNull.Value ? null : dataRow[_columnName];
        }

        private void assertColumnExists(DataRow row)
        {
            if (!row.Table.Columns.Contains(_columnName))
            {
                string message =
                    "Requested column '{0}' does not exist in this table\nThe available columns are:\n".ToFormat(
                        _columnName);
                foreach (DataColumn column in row.Table.Columns)
                {
                    message += column.ColumnName + ", ";
                }

                StoryTellerAssert.Fail(message);
            }
        }
    }
}