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

namespace WIR.Fx.Data.Migration
{
  public static class FbTypeExtensions
  {
    /// <summary>
    /// Returns Sql query for the database data type
    /// </summary>
    /// <param name="type">Database data type</param>
    /// <param name="size">Size of the type</param>
    /// <param name="scale">Scale of the type</param>
    /// <returns></returns>
    public static string GetSqlString(this FbType? type, int? size = null, int? scale = null)
    {
      if (type.HasValue) return type.Value.GetSqlString(size, scale);
      return null;
    }

    /// <summary>
    /// Returns Sql query for the database data type
    /// </summary>
    /// <param name="type">Database data type</param>
    /// <param name="size">Size of the type</param>
    /// <param name="scale">Scale of the type</param>
    /// <returns></returns>
    public static string GetSqlString(this FbType type, int? size = null, int? scale = null)
    {
      // Checking necessary values filled
      if (type.ContainedIn(FbType.Char, FbType.Decimal, FbType.Numeric, FbType.Varchar) && !size.HasValue)
        throw new ArgumentException("Size can not be null for the data type " + type.ToString());

      if (type.ContainedIn(FbType.Decimal, FbType.Numeric) && !scale.HasValue)
        throw new ArgumentException("Size can not be null for the data type " + type.ToString());

      switch (type)
      {
        case FbType.Blob: return "BLOB";
        case FbType.Char: return "CHAR(" + size.Value.ToString() + ")";
        case FbType.Date: return "DATE";
        case FbType.Decimal: return "DECIMAL(" + size.Value.ToString() + ","
          + scale.Value.ToString() + ")";
        case FbType.Double: return "DOUBLE PRECISION";
        case FbType.Float: return "FLOAT";
        case FbType.BigInt: return "BIGINT";
        case FbType.Integer: return "INTEGER";
        case FbType.Numeric: return "NUMERIC(" + size.Value.ToString() + ","
          + scale.Value.ToString() + ")";
        case FbType.SmallInt: return "SMALLINT";
        case FbType.Time: return "TIME";
        case FbType.TimeStamp: return "TIMESTAMP";
        case FbType.Varchar: return "VARCHAR(" + size.Value.ToString() + ")";
      }

      throw new ArgumentException("Sql query can not be generated for the data type "+type.ToString()+" with the specified parameters");
    }

  }
}
