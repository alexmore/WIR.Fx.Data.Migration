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
  public class ColumnQueryBuilder : QueryBuilderCore
  {
    public ColumnQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }

    string _addColumn = "ALTER TABLE {0} ADD {1} {2}{3}{4}{5}{6}"; // 3 - default, 4 - not null, 5 - check
    string _default = " DEFAULT {0}";
    string _check = " CHECK ({0})";
    string _addComputed = "ALTER TABLE {0} ADD {1} COMPUTED BY ({2}){3}";

    string _drop = "ALTER TABLE {0} DROP {1}{2}";

    string _alterRename = "ALTER TABLE {0} ALTER {1} TO {2}{3}";
    string _alterDefault = "ALTER TABLE {0} ALTER COLUMN {1} SET DEFAULT {2}{3}";
    // 0 - table, 1 - field, 2 - new domain
    string _alterDomain = "update RDB$RELATION_FIELDS set RDB$FIELD_SOURCE = '{2}' where (RDB$FIELD_NAME = '{1}') and (RDB$RELATION_NAME = '{0}'){3}";
    string _alterNotNull = "update RDB$RELATION_FIELDS set RDB$NULL_FLAG = {2} where (RDB$FIELD_NAME = '{1}') and (RDB$RELATION_NAME = '{0}'){3}";


    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var c = (Column)dbObject;

      if (string.IsNullOrEmpty(c.TableName))
        throw new InvalidOperationException("TableName can not be null for the column"+ c.Name+" at create operation");

      string sql = string.Empty;

      if (string.IsNullOrEmpty(c.ComputedBy))
      {
        string sDefault = string.Empty;
        if (c.Default != null) sDefault = string.Format(_default, c.Default);

        string sNotNull = !string.IsNullOrEmpty(c.NotNullAsString) ? " " + c.NotNullAsString : null;

        string sCheck = string.Empty;
        if (c.Check != null) sCheck = string.Format(_check, c.Check);

        sql = string.Format(_addColumn,
          Settings.FormatName(c.TableName),
          Settings.FormatName(c.Name),
          Settings.FormatName(c.DomainName),
          sDefault,
          sNotNull,
          sCheck,
          Settings.ScriptTerminationSymbol);
      }
      else
        sql = string.Format(_addComputed,
          Settings.FormatName(c.TableName),
          Settings.FormatName(c.Name),
          c.ComputedBy,
          Settings.ScriptTerminationSymbol);

      string s = CreateDescriptionQuery(c);
      if (s != null) sql += "\r\n" + s;

      return sql;
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      var c = (Column)dbObject;

      if (string.IsNullOrEmpty(c.TableName))
        throw new InvalidOperationException("TableName can not be null for the column" + c.Name+" at alter operation");
        
      if (string.IsNullOrEmpty(c.Name))
        throw new InvalidOperationException("Column name can not be null for ALTER column query");

      StringBuilder sb = new StringBuilder();
      // Domain alter
      if (!string.IsNullOrEmpty(c.DomainName))
        sb.AppendLine(
          string.Format(_alterDomain,
                        Settings.FormatName(c.TableName, true),
                        Settings.FormatName(c.Name, true),
                        Settings.FormatName(c.DomainName, true),
                        Settings.ScriptTerminationSymbol)
                        );

      // Not null alter
      if (c.NotNull.HasValue) sb.AppendLine(
        string.Format(_alterNotNull,
                      Settings.FormatName(c.TableName, true),
                      Settings.FormatName(c.Name, true),
                      c.NotNull.Value ? "1" : "NULL",
                      Settings.ScriptTerminationSymbol)
                      );

      // Default alter
      if (c.Default != null)
        sb.AppendLine(
          string.Format(_alterDefault,
                        Settings.FormatName(c.TableName),
                        Settings.FormatName(c.Name),
                        c.Default,
                        Settings.ScriptTerminationSymbol)
                        );

      // Description
      if (c.Description != null) sb.AppendLine(CreateDescriptionQuery(c));

      // Renaming at least
      if (c.NewName != null)
        sb.AppendLine(
          string.Format(_alterRename,
                        Settings.FormatName(c.TableName),
                        Settings.FormatName(c.Name),
                        Settings.FormatName(c.NewName),
                        Settings.ScriptTerminationSymbol)
                        );

      return sb.ToString().Trim();
    }

    protected override string GetDropSqlQuery(DbObject dbObject)
    {
      var c = (Column)dbObject;
      if (string.IsNullOrEmpty(c.TableName))
        throw new InvalidOperationException("TableName can not be null for the column" + c.Name+" at drop operation");

      return string.Format(_drop, 
        Settings.FormatName(c.TableName), 
        Settings.FormatName(c.Name),
        Settings.ScriptTerminationSymbol);
    }    
  }
}
