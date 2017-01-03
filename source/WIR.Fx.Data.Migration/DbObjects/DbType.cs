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
  /// Database raw data type class
  /// </summary>
  public class DbType
  {    
    public DbType() 
    {
      Type = null;
      Size = null;
      Scale = null;
      SubType = null;
      NotNull = false;
      Charset = null;
      Collate = null;
      Default = null;
      Check = null;
      Array = null;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="type">Firebird data type</param>
    public DbType(FbType type) : this()
    {
      this.Type = type;
    }   

    public FbType? Type { get; set; }    
    public int? Size { get; set; }    
    public int? Scale { get; set; }    
    public FbBlobSubType? SubType { get; set; }    
    public int? SegmentSize { get; set; }    
    public bool NotNull { get; set; }
    public string NotNullAsString { get { return NotNull ? "NOT NULL" : null; } }
    public FbCharset? Charset { get; set; }    
    public FbCollate? Collate { get; set; }
    public string Default { get; set; }
    public string Check { get; set; }
    public DbTypeArrayBound Array { get; set; }

    #region Array bounds class
    /// <summary>
    /// Array bounds class
    /// </summary>
    public class DbTypeArrayBound
    {
      public DbTypeArrayBound() { }

      public DbTypeArrayBound(int lowerBound, int upperBound)
      {
        this.UpperBound = upperBound;
        this.LowerBound = lowerBound;
      }
            
      public int UpperBound { get; set; }      
      public int LowerBound { get; set; }
    }
    #endregion

    #region Sql generaing
    /// <summary>    
    /// Generates parts of the data type sql query 
    /// </summary>    
    /// <returns>Data type sql parts</returns>
    public DbTypeSql BuildSql()
    {
      var r = new DbTypeSql();      
      r.TypeSql = GetTypeSqlString();
      r.DefaultSql = GetDefaultSqlString();
      r.NotNullSql = GetNotNullSqlString();
      r.CheckSql = GetCheckSqlString();
      r.CollateSql = GetCollateSqlString();      

      // Compiling
      StringBuilder sb = new StringBuilder();
      
      sb.Append(r.TypeSql);
      if (r.DefaultSql != null) sb.Append(" " + r.DefaultSql);
      if (r.NotNullSql != null) sb.Append(" " + r.NotNullSql);
      if (r.CheckSql != null) sb.Append(" "+r.CheckSql);
      if (r.CollateSql != null) sb.Append(" " + r.CollateSql);
      
      r.CompiltedSql = sb.ToString().Trim();
      return r;
    }

    /// <summary>
    /// Returns compiled data type sql query
    /// </summary>    
    /// <returns></returns>
    public override string ToString()
    {
      return BuildSql().CompiltedSql;
    }

    #region Generating methods
    private string GetTypeSqlString()
    {
      if (!Type.HasValue) return null;
      StringBuilder sb = new StringBuilder();
      // Base data type
      sb.Append(Type.GetSqlString(Size, Scale));
      // Массив
      if (Array != null && Type != FbType.Blob)      
        sb.Append("["+Array.LowerBound.ToString()+":"+Array.UpperBound.ToString()+"]");
      
      // BLOB
      if (Type == FbType.Blob)
      {
        if (!SegmentSize.HasValue) 
          throw new ArgumentNullException("SegmentSize can not be null for the "+Type.ToString());

        if (!SubType.HasValue)
          throw new ArgumentNullException("SubType can not be null for the " + Type.ToString());

        sb.Append(" SUB_TYPE " + ((int)SubType.Value).ToString());
        sb.Append(" SEGMENT SIZE "+SegmentSize.Value.ToString());        
      }

      // CHARACTER SET
      if (Type.ContainedIn(FbType.Varchar, FbType.Char, FbType.Blob) && Charset != null)
      {        
        if ((Type == FbType.Blob && SubType.HasValue && SubType.Value == FbBlobSubType.Text) 
          || Type.ContainedIn(FbType.Varchar, FbType.Char))          
            sb.Append(" CHARACTER SET "+Charset);       
      }

      return sb.ToString().Trim();
    }

    private string GetCollateSqlString()
    {
      if (Type.ContainedIn(FbType.Varchar, FbType.Char) && Collate != null)
        return "COLLATE "+Collate;      
      
      return null;
    }

    private string GetNotNullSqlString()
    {
      if (NotNull) return "NOT NULL";

      return null;
    }  

    private string GetDefaultSqlString()
    {
      if (!string.IsNullOrEmpty(Default))      
        return "DEFAULT "+Default;
      
      return null;
    }

    private string GetCheckSqlString()
    {
      if (!string.IsNullOrEmpty(Check))      
        return "CHECK ("+Check+")";

      return null;
    }
    #endregion

    #region Класс составных частей
    /// <summary>
    /// Data type sql parts class
    /// </summary>
    public class DbTypeSql
    {
      public string CompiltedSql { get; set; }     
      public string TypeSql { get; set; }     
      public string DefaultSql { get; set; }      
      public string CheckSql { get; set; }      
      public string NotNullSql { get; set; }      
      public string CollateSql { get; set; }
    }
    #endregion
    #endregion
  }
}
