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
  public interface IColumnTableSyntax<TNext>
  {
    /// <summary>
    /// Sets table for a column
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns></returns>
    TNext OnTable(string tableName);
  }

  public interface IColumnCreateAsDomainSyntax
  {
    /// <summary>
    /// Sets domain for a column
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IColumnCreateSyntax AsDomain(string name);
    /// <summary>
    /// Sets computed by expression for a column
    /// </summary>
    /// <param name="computeExpression"></param>
    /// <returns></returns>
    IColumnComputedBySyntax AsComputed(string computeExpression);
  }

  public interface IColumnComputedBySyntax
  {
    /// <summary>
    /// Sets column description
    /// </summary>
    /// <param name="description"></param>
    void HasDescription(string description);
  }

  public interface IColumnCreateSyntax
  {
    /// <summary>
    /// Sets NOT NULL column
    /// </summary>
    /// <returns></returns>
    IColumnCreateSyntax IsNotNull();
    /// <summary>
    /// Sets default value
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    IColumnCreateSyntax HasDefault(string defaultValue);
    /// <summary>
    /// Sets check expression 
    /// </summary>
    /// <param name="check"></param>
    /// <returns></returns>
    IColumnCreateSyntax HasCheck(string check);

    /// <summary>
    /// Sets column description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    IColumnCreateSyntax HasDescription(string description);
    /// <summary>
    /// Creates index for current column
    /// </summary>
    /// <param name="idxName"></param>
    /// <param name="sorting"></param>
    /// <returns></returns>
    IColumnCreateWithIndexSyntax WithIndex(string idxName);
    /// <summary>
    /// Creates computed by index
    /// </summary>
    /// <param name="idxName"></param>
    /// <returns></returns>
    IColumnCreateWithComputedIndexSyntax WithComputedIndex(string idxName);
  }

  public interface IColumnCreateWithComputedIndexSyntax
  {
    IColumnCreateWithIndexSyntax HasExpression(string expression);    
  }

  public interface IColumnCreateWithIndexSyntax : IColumnCreateSyntax
  {
    /// <summary>
    /// Sets Ascending sorting for index of the column
    /// </summary>
    /// <returns></returns>    
    IColumnCreateWithIndexSyntax AscendingSorting();
    /// <summary>
    /// Sets Descending sorting for index of the column
    /// </summary>
    /// <returns></returns>
    IColumnCreateWithIndexSyntax DescendingSorting();
    /// <summary>
    /// Sets unique for index of the column
    /// </summary>
    /// <returns></returns>
    IColumnCreateWithIndexSyntax IsUnique();
    /// <summary>
    /// Sets index active 
    /// </summary>
    /// <returns></returns>
    IColumnCreateWithIndexSyntax Active();
    /// <summary>
    /// Sets index inactive
    /// </summary>
    /// <returns></returns>
    IColumnCreateWithIndexSyntax Inactive();    
  }

  public interface IColumnAlterSyntax
  {
    /// <summary>
    /// Sets new column name 
    /// </summary>
    /// <param name="newName"></param>
    /// <returns></returns>
    IColumnAlterSyntax SetNewName(string newName);

    /// <summary>
    /// Sets new domain
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IColumnAlterSyntax AsDomain(string name);

    /// <summary>
    /// Sets NOT NULL column
    /// </summary>
    /// <returns></returns>
    IColumnAlterSyntax IsNotNull();
    /// <summary>
    /// Sets NOT NULL flag to 0 for column
    /// </summary>
    /// <returns></returns>
    IColumnAlterSyntax IsNullable();

    /// <summary>
    /// Sets default column value
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    IColumnAlterSyntax HasDefault(string defaultValue);    

    /// <summary>
    /// Sets column description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    IColumnAlterSyntax HasDescription(string description);    
  }
}
