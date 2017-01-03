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

namespace WIR.Fx.Data.Migration.Engine.QueryBuilders
{
  public class ProcedureQueryBuilder  : QueryBuilderCore
  {
    public ProcedureQueryBuilder(MigrationSettings settings) : base(settings)
    {
    }
   
    string _termination = "SET TERM ^ {0}\r\n{1}\r\n^\r\nSET TERM {0} ^";
    // 0 - name, 1 - text;
    string _create = "CREATE OR ALTER PROCEDURE {0} {1}";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      var p = (Procedure)dbObject;

      if (p.ProcedureText == null)
        throw new InvalidOperationException("ProcedureText property can not be null for "+(p.Name??""));

      string sql = string.Format(_create, Settings.FormatName(p.Name), p.ProcedureText);
      sql = string.Format(_termination, Settings.ScriptTerminationSymbol, sql.Trim());

      if (p.Description != null) sql += "\r\n" + CreateDescriptionQuery(p);

      return sql.Trim();
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      return GetCreateSqlQuery(dbObject);
    }
  }
}
