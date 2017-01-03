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
  public class ViewQueryBuilder : QueryBuilderCore
  {
    public ViewQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }

    // 0 - name, 1 - columns, 2 - text
    string _create = "CREATE OR ALTER VIEW {0} ({1}) AS {2}{3}";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var v = (View)dbObject;

      if (v.ResultColumns == null || v.ResultColumns.Length == 0)
        throw new InvalidOperationException("ResultColumns array in View class can not be null or empty for "+ (v.Name ?? ""));
      if (string.IsNullOrEmpty(v.Query))
        throw new InvalidOperationException("Query property in View class can not be null or empty for " + (v.Name ?? ""));

      string sql = string.Empty;

      sql = string.Format(
        _create,        
        Settings.FormatName(v.Name),
        Settings.FormatNameCollection(v.ResultColumns),
        v.Query,
        Settings.ScriptTerminationSymbol
        );

      return sql.Trim(); 
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      return GetCreateSqlQuery(dbObject);
    }
  }
}
