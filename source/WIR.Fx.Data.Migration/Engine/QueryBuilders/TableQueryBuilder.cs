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
  public class TableQueryBuilder : QueryBuilderCore
  {
    public TableQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }

    string _createTable = "CREATE TABLE {0} ({1}){2}";
    string _createTableColumn = ", {0} {1}";
    string _createTableColumnNotNullPostfix = "NOT NULL";
    string _createTableColumnDefaultPostfix = "DEFAULT {0}";
    string _createTableComputedColumn = ", {0} COMPUTED BY {1}";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var t = (Table)dbObject;
      StringBuilder sb = new StringBuilder();
            
      if (t.NewColumns == null || t.NewColumns.Length == 0)
        throw new InvalidOperationException("Table with 0 columns count can not be created. Table "
          + t.Name + " create operation");

      string sCols = string.Empty;
      foreach (var i in t.NewColumns)
      {
        if (i.ComputedBy == null)
          sCols += string.Format(
            _createTableColumn,
            Settings.FormatName(i.Name),
            Settings.FormatName(i.DomainName)
            );
        else
          sCols += string.Format(
            _createTableComputedColumn,
            Settings.FormatName(i.Name),
            i.ComputedBy
            );

        if (i.Default != null)
          sCols += " " + string.Format(_createTableColumnDefaultPostfix, i.Default);
        if (i.NotNull.HasValue && i.NotNull.Value)
          sCols += " " + _createTableColumnNotNullPostfix;
      }

      sb.AppendLine(
        string.Format(
        _createTable,
        Settings.FormatName(t.Name),
        sCols.Substring(2),
        Settings.ScriptTerminationSymbol)
        );

      var s = CreateDescriptionQuery(dbObject); if (s != null) sb.AppendLine(s);

      // Добавляем комментарии для колонок
      foreach (Column i in t.NewColumns)
      {
        if (i.Description != null)
          sb.AppendLine(CreateDescriptionQuery(i));
      }

      return sb.ToString().Trim();
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      if (dbObject.Description != null) return CreateDescriptionQuery(dbObject);

      return null;
    }
  }
}
