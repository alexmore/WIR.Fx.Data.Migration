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
  public class ConstraintQueryBuilder : QueryBuilderCore
  {
    public ConstraintQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }

    string _createConstraint = "ALTER TABLE {0} ADD CONSTRAINT {1}";
    string _pk = "PRIMARY KEY ({0})";
    string _usingIndex = "USING {0}INDEX {1}";
    string _fk = "FOREIGN KEY ({0})";
    string _fkReferences = "REFERENCES {0}({1})";
    string _fkOnUpdate = "ON UPDATE {0}";
    string _fkOnDelete = "ON DELETE {0}";
    string _check = "CHECK ({0})";
    string _unique = "UNIQUE ({0})";
    string _drop = "ALTER TABLE {0} DROP CONSTRAINT {1}{2}";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var c = (Constraint)dbObject;

      if (string.IsNullOrEmpty(c.TableName))
        throw new InvalidOperationException("TableName property can not be null for " +
          c.Name + " in constraint create operation");

      string sql = string.Format(
        _createConstraint,
        Settings.FormatName(c.TableName),
        Settings.FormatName(c.Name)
        );

      if (c.GetType() == typeof(ConstraintPrimaryKey))
        sql += " " + string.Format(_pk, Settings.FormatNameCollection(c.Columns));

      if (c.GetType() == typeof(ConstraintForeignKey))
      {
        ConstraintForeignKey fk = (ConstraintForeignKey)c;

        if (fk.ReferenceTable == null)
          throw new InvalidOperationException("ReferenceTableName property can not be null for " +
            fk.Name + " in foreign key constraint create operation");

        sql += " " + string.Format(_fk, Settings.FormatNameCollection(c.Columns));
        sql += " " + string.Format(
          _fkReferences,
          Settings.FormatName(fk.ReferenceTable),
          Settings.FormatNameCollection(fk.ReferenceColumns)
          );

        if (fk.DeleteRule.HasValue)
          sql += " " + string.Format(
            _fkOnDelete,
            UpdateDeleteRuleToString(fk.DeleteRule.Value)
            );
        if (fk.UpdateRule.HasValue)
          sql += " " + string.Format(
            _fkOnUpdate,
            UpdateDeleteRuleToString(fk.UpdateRule.Value)
            );
      }

      if (c.GetType() == typeof(ConstraintCheck))
        sql += " " + string.Format(_check, ((ConstraintCheck)c).CheckExpression);
      if (c.GetType() == typeof(ConstraintUnique))
        sql += " " + string.Format(_unique, Settings.FormatNameCollection(c.Columns));


      if (c.Index != null && dbObject.GetType() != typeof(ConstraintCheck))
        sql += " " + string.Format(
          _usingIndex,
          c.Index.Sorting == FbSorting.Descending ? "DESCENDING " : "",
          Settings.FormatName(c.Index.Name)
          );

      return sql + Settings.ScriptTerminationSymbol;
    }

    private string UpdateDeleteRuleToString(FbForeignKeyRules rule)
    {
      switch (rule)
      {
        case FbForeignKeyRules.Cascade: return "CASCADE";
        case FbForeignKeyRules.SetDefault: return "SET DEFAULT";
        case FbForeignKeyRules.SetNull: return "SET NULL";
        default: return null;
      }
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      throw new NotSupportedException("Not supported action ALTER for constraints");
    }

    protected override string GetDropSqlQuery(DbObject dbObject)
    {
      var c = (Constraint)dbObject;
      return string.Format(_drop, 
        Settings.FormatName(c.TableName), 
        Settings.FormatName(dbObject.Name),
        Settings.ScriptTerminationSymbol);
    }  
  }
}
