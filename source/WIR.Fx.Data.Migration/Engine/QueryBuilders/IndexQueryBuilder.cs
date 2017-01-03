#region Copyright
/******************************************************************************
Copyright (c) 2013 Alexandr Mordvinov, WIR LLC, alexandr.a.mordvinov@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
******************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WIR.Fx.Data.Migration.DbObjects;

namespace WIR.Fx.Data.Migration.Engine.QueryBuilders
{
  public class IndexQueryBuilder : QueryBuilderCore
  {
    public IndexQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }

    string _fieldIndex = "CREATE {0}{1}INDEX {2} ON {3} ({4}){5}";
    string _computedIndex = "CREATE {0}{1}INDEX {2} ON {3} COMPUTED BY ({4}){5}";

    string _inactivate = "ALTER INDEX {0} INACTIVE{1}";
    string _activate = "ALTER INDEX {0} ACTIVE{1}";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var i = (Index)dbObject;

      string sql = string.Empty;

      if (string.IsNullOrEmpty(i.TableName))
        throw new InvalidOperationException("TableName property can not be null for index " + i.Name+" create operation");
      if (i.Columns != null && i.Columns.Length != 0 && i.Expression != null)
        throw new InvalidOperationException("The values for the columns and expression can not be set simultaneuosly");

      if (i.Columns != null && i.Columns.Length != 0)
        return string.Format(
          _fieldIndex,
          i.IsUnique ? "UNIQUE " : "",
          i.Sorting == FbSorting.Descending ? "DESCENDING " : "",
          Settings.FormatName(i.Name),
          Settings.FormatName(i.TableName),
          Settings.FormatNameCollection(i.Columns),
          Settings.ScriptTerminationSymbol
          );

      if (i.Expression != null)
        return string.Format(_computedIndex,
          i.IsUnique ? "UNIQUE " : "",
          i.Sorting == FbSorting.Descending ? "DESCENDING " : "",
          Settings.FormatName(i.Name),
          Settings.FormatName(i.TableName),
          i.Expression,
          Settings.ScriptTerminationSymbol
          );



      throw new InvalidOperationException("Columns or computed by expression can not be null for the index " + i.Name+" create operation");
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      var i = (Index)dbObject;
      string sql = string.Empty;
      if (!i.IsActive) sql = string.Format(_inactivate, 
        Settings.FormatName(i.Name),
        Settings.ScriptTerminationSymbol);
      
      if (i.IsActive) sql = string.Format(_activate, 
        Settings.FormatName(i.Name),
        Settings.ScriptTerminationSymbol);
      return sql;

    }
  }
}
