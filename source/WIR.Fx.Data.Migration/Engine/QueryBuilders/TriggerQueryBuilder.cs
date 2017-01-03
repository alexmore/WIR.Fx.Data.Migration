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
  public class TriggerQueryBuilder : QueryBuilderCore
  {
    public TriggerQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }
    string _termination = "SET TERM ^ {1}\r\n{0}\r\n^\r\nSET TERM {1} ^";
    // 0 - table, 1 - name, 2 - actiontype, 3 - actions, 4 - position, 5 - text
    string _create = "CREATE OR ALTER TRIGGER {1} FOR {0} {2} {3} {4} POSITION {5} AS BEGIN {6} END";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var t = (Trigger)dbObject;

      if (string.IsNullOrEmpty(t.TableName))
        throw new InvalidOperationException("TableName property can not be null for the trigger "
          + (t.Name ?? "") + " create operation");
      if (string.IsNullOrEmpty(t.TriggerText))
        throw new InvalidOperationException("Trigger body text can not be null for the trigger "
          + (t.Name ?? "") + " create operation");

      string sql = string.Empty;

      sql = string.Format(
        _create,
        Settings.FormatName(t.TableName),
        Settings.FormatName(t.Name),
        t.IsActive ? "ACTIVE" : "INACTIVE",
        t.TriggerOrder.ToString().ToUpper(),
        BuildActions(t.TriggerAction),
        t.Position,
        t.TriggerText
        );

      sql = string.Format(_termination, sql.Trim(), Settings.ScriptTerminationSymbol);

      if (t.Description != null) sql += "\r\n" + CreateDescriptionQuery(t);

      return sql.Trim();
    }

    private string BuildActions(TriggerAction a)
    {
      string s = string.Empty;
      if (a.HasFlag(TriggerAction.Insert)) s += " OR INSERT";
      if (a.HasFlag(TriggerAction.Update)) s += " OR UPDATE";
      if (a.HasFlag(TriggerAction.Delete)) s += " OR DELETE";

      if (s.Length > 4) s = s.Substring(4);

      return s.Trim();
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      return GetCreateSqlQuery(dbObject);
    }
  }
}
