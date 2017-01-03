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

namespace WIR.Fx.Data.Migration.Fluent.Columns
{
  public class ColumnSyntax : IColumnTableSyntax<IColumnCreateAsDomainSyntax>, 
                              IColumnCreateAsDomainSyntax, IColumnCreateSyntax, 
                              IColumnCreateWithIndexSyntax, IColumnCreateWithComputedIndexSyntax,
                              IColumnTableSyntax<IColumnAlterSyntax>, 
                              IColumnAlterSyntax,
                              IColumnComputedBySyntax
                
  {
    Column _column;
    List<DbObject> _dbObjects;

    public ColumnSyntax(Column column, List<DbObject> dbObjects)
    {
      _column = column;
      _dbObjects = dbObjects;
    }

    #region Create
    public IColumnCreateAsDomainSyntax OnTable(string tableName)
    {
      this._column.TableName = tableName;
      return this;
    }

    public IColumnCreateSyntax AsDomain(string name)
    {
      this._column.DomainName = name;
      return this;
    }

    public IColumnCreateSyntax IsNotNull()
    {
      _column.NotNull = true;
      return this;
    }

    public IColumnCreateSyntax HasDefault(string defaultValue)
    {
      _column.Default = defaultValue;
      return this;
    }

    public IColumnCreateSyntax HasCheck(string check)
    {
      _column.Check = check;
      return this;
    }

    public IColumnCreateSyntax HasDescription(string description)
    {
      _column.Description += description;
      return this;
    }

    IColumnAlterSyntax IColumnTableSyntax<IColumnAlterSyntax>.OnTable(string tableName)
    {
      OnTable(tableName);
      return this;
    }   
    #endregion

    #region Create with index
    Index _idx;
    public IColumnCreateWithIndexSyntax WithIndex(string idxName)
    {
      _idx = new DbObjects.Index(idxName, DbAction.Create);
      _idx.Columns = new string[1] { _column.Name };
      _idx.TableName = _column.TableName;
      _idx.Sorting = FbSorting.Ascending;
      _idx.IsUnique = false;
      _idx.IsActive = true;
      _dbObjects.Add(_idx);
      return this;
    }

    public IColumnCreateWithComputedIndexSyntax WithComputedIndex(string idxName)
    {
      _idx = new DbObjects.Index(idxName, DbAction.Create);      
      _idx.TableName = _column.TableName;
      _idx.Sorting = FbSorting.Ascending;
      _idx.IsUnique = false;
      _idx.IsActive = true;
      _dbObjects.Add(_idx);
      return this;
    }  

    public IColumnCreateWithIndexSyntax AscendingSorting()
    {
      _idx.Sorting = FbSorting.Ascending;
      return this;
    }

    public IColumnCreateWithIndexSyntax DescendingSorting()
    {
      _idx.Sorting = FbSorting.Descending;
      return this;
    }

    public IColumnCreateWithIndexSyntax IsUnique()
    {
      _idx.IsUnique = true;
      return this;
    }

    public IColumnCreateWithIndexSyntax Active()
    {
      _idx.IsActive = true;
      return this;
    }

    public IColumnCreateWithIndexSyntax Inactive()
    {
      _idx.IsActive = false;
      return this;
    }

    public IColumnCreateWithIndexSyntax HasExpression(string expression)
    {
      _idx.Expression = expression;
      return this;
    }
    #endregion

    #region Alter
    public IColumnAlterSyntax SetNewName(string newName)
    {
      _column.NewName = newName;
      return this;
    }

    IColumnAlterSyntax IColumnAlterSyntax.AsDomain(string name)
    {
      this.AsDomain(name);
      return this;
    }
    
    IColumnAlterSyntax IColumnAlterSyntax.IsNotNull()
    {
      this.IsNotNull();
      return this;
    }

    public IColumnAlterSyntax IsNullable()
    {
      _column.NotNull = false;
      return this;
    }

    IColumnAlterSyntax IColumnAlterSyntax.HasDefault(string defaultValue)
    {
      this.HasDefault(defaultValue);
      return this;
    }

    IColumnAlterSyntax IColumnAlterSyntax.HasDescription(string description)
    {
      this.HasDescription(description);
      return this;
    }   
    #endregion

    void IColumnComputedBySyntax.HasDescription(string description)
    {
      this.HasDescription(description);
    }


    public IColumnComputedBySyntax AsComputed(string computeExpression)
    {
      _column.ComputedBy = computeExpression;
      return this;
    }

  }
}
