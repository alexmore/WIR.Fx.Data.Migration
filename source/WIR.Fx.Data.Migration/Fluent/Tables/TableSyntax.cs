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
using System.Threading.Tasks;

using WIR.Fx.Data.Migration.DbObjects;

namespace WIR.Fx.Data.Migration.Fluent.Tables
{
  public sealed class TableSyntax : ITableNewColumnSyntax, ITableCreateSyntax, 
                             ITableColumnTypeSyntax, ITableColumnSyntax,       
                             ITableAlterSyntax, ITablePKSyntax,
    ITableFKOnTableSyntax, ITableFKWithColumnSyntax, ITableFKRulesSyntax,
    ITableColumnWithComputedIndexSyntax, ITableColumnWithIndexSyntax
  {
    Column _currentColumn;
    List<Column> _columns = new List<Column>();
    Table _table;
    List<DbObject> _dbObjects;

    public TableSyntax(Table table, List<DbObject> collection)
    {
      _table = table;
      _dbObjects = collection;
    }

    public ITableCreateSyntax HasDescription(string tableDescription)
    {
      _table.Description += tableDescription;
      return this;
    }

    public ITableColumnTypeSyntax WithColumn(string name)
    {
      var c = new Column(name, DbAction.Create);
      _currentColumn = c;
      _currentColumn.TableName = _table.Name;
      _columns.Add(c);      
      _table.NewColumns = _columns.ToArray();
      return this;
    }

    public ITableColumnSyntax AsDomain(string domainName)
    {
      _currentColumn.DomainName = domainName;
      return this;
    }

    public ITableNewColumnSyntax ComputedBy(string expression)
    {
      _currentColumn.ComputedBy = expression;
      return this;
    }

    public ITableColumnSyntax HasDefault(string defaultValue)
    {
      _currentColumn.Default = defaultValue;
      return this;
    }

    public ITableColumnSyntax IsNotNull()
    {
      _currentColumn.NotNull = true;
      return this;
    }

    public ITableColumnSyntax HasColumnDescription(string columnDescription)
    {
      _currentColumn.Description += columnDescription;
      return this;
    }

    ConstraintPrimaryKey _currentPK;
    public ITablePKSyntax AsPrimaryKey()
    {
      AsPrimaryKey("Pk" + _table.Name);      
      return this;
    }

    public ITablePKSyntax AsPrimaryKey(string pkName)
    {
      _currentColumn.NotNull = true;
      ConstraintPrimaryKey pk = new ConstraintPrimaryKey(pkName, DbAction.Create);
      pk.TableName = _table.Name;
      pk.Columns = new[] { _currentColumn.Name };
      _dbObjects.Add(pk);
      _currentPK = pk;
      return this;
    }

    public ITableColumnSyntax UsingIndex(string name, FbSorting sorting)
    {
      _currentPK.Index = new Index(name, DbAction.Create) { Sorting = sorting };
      return this;
    }

    public void AddDescription(string tableDescription)
    {
      _table.Description += tableDescription;      
    }

    ConstraintForeignKey _currentFk;

    public ITableFKOnTableSyntax AsForeignKey(string name)
    {
      var fk = new ConstraintForeignKey(name, DbAction.Create);
      fk.Action = DbAction.Create;
      fk.TableName = _table.Name;
      fk.Columns = new[] { _currentColumn.Name };
      _dbObjects.Add(fk);
      _currentFk = fk;
      return this;
    }

    public ITableFKWithColumnSyntax WithReferenceTable(string tableName)
    {
      _currentFk.ReferenceTable = tableName;
      return this;
    }

    ITableFKRulesSyntax ITableFKWithColumnSyntax.OnColumn(string columnName)
    {
      _currentFk.ReferenceColumns = new[] { columnName };
      return this;
    }

    

    public ITableColumnSyntax AsUnique(string name)
    {
      var u = new ConstraintUnique(name, DbAction.Create);
      u.TableName = _table.Name;
      u.Columns = new[] { _currentColumn.Name };
      _dbObjects.Add(u);
      return this;
    }
    
    public ITableFKRuleTypeSyntax HasDeleteRule
    {
      get
      {
        return new TableFKRuleType(this, _currentFk, true);
      }
    }

    public ITableFKRuleTypeSyntax HasUpdateRule
    {
      get
      {
        return new TableFKRuleType(this, _currentFk, false);
      }
    }

    Index _idx;
    public ITableColumnWithIndexSyntax WithIndex(string idxName)
    {
      _idx = new DbObjects.Index(idxName, DbAction.Create);
      _idx.Columns = new string[1] { _currentColumn.Name };
      _idx.TableName = _currentColumn.TableName;
      _idx.Sorting = FbSorting.Ascending;
      _idx.IsUnique = false;
      _idx.IsActive = true;
      _dbObjects.Add(_idx);
      return this;
    }

    public ITableColumnWithComputedIndexSyntax WithComputedIndex(string idxName)
    {
      _idx = new DbObjects.Index(idxName, DbAction.Create);
      _idx.TableName = _currentColumn.TableName;
      _idx.Sorting = FbSorting.Ascending;
      _idx.IsUnique = false;
      _idx.IsActive = true;
      _dbObjects.Add(_idx);
      return this;
    }

    public ITableColumnWithIndexSyntax HasExpression(string expression)
    {
      _idx.Expression = expression;
      return this;
    }

    public ITableColumnWithIndexSyntax AscendingSorting()
    {
      _idx.Sorting = FbSorting.Ascending;
      return this;
    }

    public ITableColumnWithIndexSyntax DescendingSorting()
    {
      _idx.Sorting = FbSorting.Descending;
      return this;
    }

    public ITableColumnWithIndexSyntax IsUnique()
    {
      _idx.IsUnique = true;
      return this;
    }

    public ITableColumnWithIndexSyntax Active()
    {
      _idx.IsActive = true;
      return this;
    }

    public ITableColumnWithIndexSyntax Inactive()
    {
      _idx.IsActive = false;
      return this;
    }
  }

  public class TableFKRuleType : ITableFKRuleTypeSyntax
  {
    TableSyntax _ts;
    ConstraintForeignKey _constr;
    bool _isDeleteRule = false;

    public TableFKRuleType(TableSyntax ts, ConstraintForeignKey constr, bool isDeleteRule)
    {
      _ts = ts;
      _constr = constr;
      _isDeleteRule = isDeleteRule;
    }

    public ITableFKRulesSyntax Cascade()
    {
      if (_isDeleteRule)
        _constr.DeleteRule = FbForeignKeyRules.Cascade;
      else
        _constr.UpdateRule = FbForeignKeyRules.Cascade;
      return _ts;
    }

    public ITableFKRulesSyntax SetNull()
    {
      if (_isDeleteRule)
        _constr.DeleteRule = FbForeignKeyRules.SetNull;
      else
        _constr.UpdateRule = FbForeignKeyRules.SetNull;
      return _ts;
    }

    public ITableFKRulesSyntax SetDefault()
    {
      if (_isDeleteRule)
        _constr.DeleteRule = FbForeignKeyRules.SetDefault;
      else
        _constr.UpdateRule = FbForeignKeyRules.SetDefault;
      return _ts;
    }













  }
}
