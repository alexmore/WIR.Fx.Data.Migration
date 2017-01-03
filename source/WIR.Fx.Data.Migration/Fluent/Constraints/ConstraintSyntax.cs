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

namespace WIR.Fx.Data.Migration.Fluent.Constraints
{
  public class ConstraintSyntax : IConstraintTableSyntax, IConstraintColumnsSyntax, 
                                  IConstraintTypeSyntax, IConstraintPrimaryKeySyntax, 
                                  IConstraintUniqueSyntax, IConstraintFKReferenceSyntax,
                                  IConstraintFKReferenceColumnsSyntax, 
                                  IConstraintFKSyntax,
                                  IConstraintFKRuleValueSyntax

  {
    Constraint _c;
    List<DbObject> _dbObjects;
    string _name;
    string[] _columns;
    string _tableName;

    public ConstraintSyntax(string name, List<DbObject> dbObjects)
    {
      this._name = name;
      this._dbObjects = dbObjects;
    }

    public IConstraintColumnsSyntax OnTable(string tableName)
    {
      this._tableName = tableName;
      return this;
    }

    public IConstraintTypeSyntax OnColumns(params string[] columns)
    {
      _columns = columns;
      return this;
    }

    public void AsCheck(string expression)
    {
      var c = new ConstraintCheck(_name, DbAction.Create);
      c.TableName = _tableName;
      c.CheckExpression = expression;
      _dbObjects.Add(c);
    }

    public IConstraintFKReferenceSyntax AsForeignKey()
    {
      var fk = new ConstraintForeignKey(_name, DbAction.Create);
      fk.Columns = _columns;
      fk.TableName = _tableName;
      _dbObjects.Add(fk);
      _c = fk;
      return this;
    }

    public IConstraintPrimaryKeySyntax AsPrimaryKey()
    {
      var pk = new ConstraintPrimaryKey(_name, DbAction.Create);
      pk.Columns = _columns;
      pk.TableName = _tableName;
      _dbObjects.Add(pk);
      _c = pk;
      return this;
    }

    public IConstraintUniqueSyntax AsUnique()
    {
      var u = new ConstraintUnique(_name, DbAction.Create);
      u.Columns = _columns;
      u.TableName = _tableName;
      _dbObjects.Add(u);
      _c = u;
      return this;
    }

    public IConstraintPrimaryKeySyntax UsingIndex(string indexName, FbSorting sorting)
    {
      _c.Index = new Index(indexName, DbAction.Create) { Sorting = sorting };
      return this;
    }

    IConstraintUniqueSyntax IConstraintUsingIndex<IConstraintUniqueSyntax>.UsingIndex(string indexName, FbSorting sorting)
    {
      UsingIndex(indexName, sorting);
      return this;
    }

    public IConstraintFKReferenceColumnsSyntax WithReferenceTable(string tableName)
    {
      var fk = (ConstraintForeignKey)_c;
      fk.ReferenceTable = tableName;
      return this;
    }

    IConstraintFKSyntax IConstraintFKReferenceColumnsSyntax.OnColumns(params string[] columns)
    {
      var fk = (ConstraintForeignKey)_c;
      fk.ReferenceColumns = columns;
      return this;
    }


    bool isDeleteRule = false;
    public IConstraintFKRuleValueSyntax HasUpdateRule
    {
      get { isDeleteRule = false; return this; }
    }    

    public IConstraintFKRuleValueSyntax HasDeleteRule
    {
      get { isDeleteRule = true; return this; }
    }
    

    IConstraintFKSyntax IConstraintUsingIndex<IConstraintFKSyntax>.UsingIndex(string indexName, FbSorting sorting)
    {
      UsingIndex(indexName, sorting);
      return this;
    }

    public IConstraintFKSyntax Cascade()
    {
      var fk = (ConstraintForeignKey)_c;
      if (isDeleteRule)
        fk.DeleteRule = FbForeignKeyRules.Cascade;
      else
        fk.UpdateRule = FbForeignKeyRules.Cascade;
      return this;
    }

    public IConstraintFKSyntax SetNull()
    {
      var fk = (ConstraintForeignKey)_c;
      if (isDeleteRule)
        fk.DeleteRule = FbForeignKeyRules.SetNull;
      else
        fk.UpdateRule = FbForeignKeyRules.SetNull;

      return this;
    }

    public IConstraintFKSyntax SetDefault()
    {
      var fk = (ConstraintForeignKey)_c;
      if (isDeleteRule)
        fk.DeleteRule = FbForeignKeyRules.SetDefault;
      else
        fk.UpdateRule = FbForeignKeyRules.SetDefault;
      return this;
    }
  }
}
