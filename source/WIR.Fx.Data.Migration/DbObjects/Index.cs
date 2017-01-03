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

namespace WIR.Fx.Data.Migration.DbObjects
{
  /// <summary>
  /// Firebird Sql index class
  /// </summary>
  [SqlMetadata("INDEX")]
  public class Index : DbObject
  {    
    /// <summary>
    /// Constructor 
    /// </summary>
    /// <param name="name">Index name</param>
    /// <param name="action">Action</param>
    public Index(string name, DbAction action)
      : base(name, action)
    {
      Sorting = FbSorting.Ascending;
      IsActive = true;
    }

    /// <summary>
    /// Index table name
    /// </summary>
    public string TableName { get; set; }
    /// <summary>
    /// Index columns
    /// </summary>
    public string[] Columns { get; set; }

    string _expression;
    /// <summary>
    /// Index expression
    /// </summary>
    public string Expression
    {
      get { return _expression; }
      set
      {
        if (Columns != null)
          throw new InvalidOperationException("Index columns and HasError can not be specified at the same time.");
        _expression = value;
      }
    }    
    
    public bool IsUnique { get; set; }    
    public bool IsActive { get; set; }    
    public FbSorting Sorting { get; set; }   
  }
}
