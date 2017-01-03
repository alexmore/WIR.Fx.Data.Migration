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

namespace WIR.Fx.Data.Migration.Fluent.Indexes
{
  public class IndexSyntax : IIndexSyntax, IIndexTypeSyntax, IIndexTableSyntax, 
                             IIndexAlterSyntax
  {
    Index _idx;

    public IndexSyntax(Index idx)
    {
      _idx = idx;
    }

    public IIndexSyntax DescendingSorting()    
    {
      _idx.Sorting = FbSorting.Descending;
      return this;
    }

    public IIndexSyntax AscendingSorting()
    {
      _idx.Sorting = FbSorting.Ascending;
      return this;
    }

    public IIndexSyntax IsUnique()
    {
      _idx.IsUnique = true;
      return this;
    }

    public IIndexSyntax Active()
    {
      _idx.IsActive = true;
      return this;
    }

    public IIndexSyntax Inactive()
    {
      _idx.IsActive = false;
      return this;
    }

    public void SetActive() { Active(); }
    public void SetInactive() { Inactive(); }

    public IIndexTypeSyntax OnTable(string tableName)
    {
      _idx.TableName = tableName;
      return this;
    }

    public IIndexSyntax ComputedBy(string expression)
    {
      _idx.Expression = expression;
      return this;
    }

    public IIndexSyntax OnColumns(params string[] columns)
    {
      _idx.Columns = columns;
      return this;
    }
    
  }
}
