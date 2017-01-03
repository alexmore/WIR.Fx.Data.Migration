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
  /// Data table column class
  /// </summary>
  [SqlMetadata("COLUMN")]
  public class Column : DbObject
  {    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Column name</param>
    /// <param name="action">Action</param>
    public Column(string name, DbAction action) 
      : base(name, action) { }

    /// <summary>
    /// Data table name
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// Column new name
    /// </summary>
    public string NewName { get; set; }

    string _domainName;
    /// <summary>
    /// Column domain
    /// </summary>
    public string DomainName
    {
      get { return _domainName; }
      set { 
        if (Type != null) 
          throw new InvalidOperationException("Column raw type and domain can not be specified for the "+
            (TableName??"")+"."+(Name??"")+" at the same time."); 
        
        _domainName = value; 
      }
    }

    DbType _type;
    /// <summary>
    /// Column raw type
    /// </summary>
    public DbType Type
    {
      get { return _type; }
      set
      {
        if (DomainName != null)
          throw new InvalidOperationException("Column raw type and domain can not be specified for the " +
            (TableName ?? "") + "." + (Name ?? "") + " at the same time."); 
        _type = value;
      }
    }

    /// <summary>
    /// Returns column raw type sql or domain name as string
    /// </summary>
    /// <param name="nameFormatter">Database objects name formatter delegate</param>
    /// <returns></returns>
    public string GetColumnTypeString(Func<string, string> nameFormatter)
    {
      if (DomainName == null && Type == null)
        throw new InvalidOperationException("Column raw type and domain can not be null for the " + (TableName ?? "") + "." + (Name ?? "")+" at the same time.");

      if (DomainName != null) return nameFormatter(DomainName);

      return Type.ToString();
    }
    
    /// <summary>
    /// Computed by expression for the column
    /// </summary>
    public string ComputedBy { get; set; }
        
    /// <summary>
    /// Column is not null
    /// </summary>
    public bool? NotNull { get; set; }
    public string NotNullAsString
    {
      get
      {
        if (NotNull.HasValue)
          return NotNull.Value ? "NOT NULL" : null; return null;
      }
    }
    
    /// <summary>
    /// Default value for the column
    /// </summary>
    public string Default { get; set; }
    /// <summary>
    /// Column check expression
    /// </summary>
    public string Check { get; set; }    
  }
}
