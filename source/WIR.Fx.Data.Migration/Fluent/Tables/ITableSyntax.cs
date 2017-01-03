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

namespace WIR.Fx.Data.Migration.Fluent.Tables
{
  public interface ITableNewColumnSyntax
  {
    /// <summary>
    /// Creates new column in table
    /// </summary>
    /// <param name="name">Column name</param>
    /// <returns></returns>
    ITableColumnTypeSyntax WithColumn(string name);
  }

  public interface ITableCreateSyntax : ITableNewColumnSyntax
  {    
    /// <summary>
    /// Sets description to a table
    /// </summary>
    /// <param name="tableDescription"></param>
    /// <returns></returns>
    ITableCreateSyntax HasDescription(string tableDescription);    
  }

  public interface ITableColumnTypeSyntax
  {
    /// <summary>
    /// Sets domain for a column
    /// </summary>
    /// <param name="domainName">Domain name</param>
    /// <returns></returns>
    ITableColumnSyntax AsDomain(string domainName);
    /// <summary>
    /// Sets computed by expression for a column
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    ITableNewColumnSyntax ComputedBy(string expression);
  }

  public interface ITableColumnSyntax : ITableNewColumnSyntax
  {
    /// <summary>
    /// Sets default value
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    ITableColumnSyntax HasDefault(string defaultValue);
    /// <summary>
    /// Sets NOT NULL column
    /// </summary>
    /// <returns></returns>
    ITableColumnSyntax IsNotNull();
    /// <summary>
    /// Sets column description
    /// </summary>
    /// <param name="columnDescription"></param>
    /// <returns></returns>
    ITableColumnSyntax HasColumnDescription(string columnDescription);
    /// <summary>
    /// Creates primary key constraint for a column
    /// </summary>
    /// <returns></returns>
    ITablePKSyntax AsPrimaryKey();
    /// <summary>
    /// Creates primary key constraint for a column
    /// </summary>
    /// <returns></returns>
    ITablePKSyntax AsPrimaryKey(string pkName);
    /// <summary>
    /// Creates foreign key constraint for a column
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ITableFKOnTableSyntax AsForeignKey(string name);
    /// <summary>
    /// Creates unique constraint for a column
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ITableColumnSyntax AsUnique(string name);

    /// <summary>
    /// Creates index for current column
    /// </summary>
    /// <param name="idxName"></param>
    /// <param name="sorting"></param>
    /// <returns></returns>
    ITableColumnWithIndexSyntax WithIndex(string idxName);
    /// <summary>
    /// Creates computed by index
    /// </summary>
    /// <param name="idxName"></param>
    /// <returns></returns>
    ITableColumnWithComputedIndexSyntax WithComputedIndex(string idxName);
  }

  public interface ITableColumnWithComputedIndexSyntax
  {
    ITableColumnWithIndexSyntax HasExpression(string expression);
  }

  public interface ITableColumnWithIndexSyntax : ITableNewColumnSyntax
  {
    /// <summary>
    /// Sets Ascending sorting for index of the column
    /// </summary>
    /// <returns></returns>    
    ITableColumnWithIndexSyntax AscendingSorting();
    /// <summary>
    /// Sets Descending sorting for index of the column
    /// </summary>
    /// <returns></returns>
    ITableColumnWithIndexSyntax DescendingSorting();
    /// <summary>
    /// Sets unique for index of the column
    /// </summary>
    /// <returns></returns>
    ITableColumnWithIndexSyntax IsUnique();
    /// <summary>
    /// Sets index active 
    /// </summary>
    /// <returns></returns>
    ITableColumnWithIndexSyntax Active();
    /// <summary>
    /// Sets index inactive
    /// </summary>
    /// <returns></returns>
    ITableColumnWithIndexSyntax Inactive();
  }

  public interface ITableFKOnTableSyntax
  {
    /// <summary>
    /// Sets reference table for a foreign key constraint
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    ITableFKWithColumnSyntax WithReferenceTable(string tableName);
  }

  public interface ITableFKWithColumnSyntax
  {
    /// <summary>
    /// Sets referenced column for a foreign key constraint
    /// </summary>
    /// <param name="columnName"></param>
    /// <returns></returns>
    ITableFKRulesSyntax OnColumn(string columnName);
  }

  public interface ITableFKRulesSyntax : ITableColumnSyntax
  {
    /// <summary>
    /// Sets delete rule
    /// </summary>
    ITableFKRuleTypeSyntax HasDeleteRule { get; }
    /// <summary>
    /// Sets update rule
    /// </summary>
    ITableFKRuleTypeSyntax HasUpdateRule { get; }
  }

  public interface ITableFKRuleTypeSyntax
  {
    ITableFKRulesSyntax Cascade();
    ITableFKRulesSyntax SetNull();
    ITableFKRulesSyntax SetDefault();
  }

  public interface ITablePKSyntax : ITableColumnSyntax
  {
    
  }

  public interface ITableAlterSyntax
  {
    /// <summary>
    /// Sets description for a table
    /// </summary>
    /// <param name="tableDescription"></param>
    void AddDescription(string tableDescription);    
  }
}
