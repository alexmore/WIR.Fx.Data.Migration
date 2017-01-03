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
  public class QueryBuilderCore : IQueryBuilder
  {
    public QueryBuilderCore(MigrationSettings settings)
    {
      this.Settings = settings;
    }

    public MigrationSettings Settings { get; set; }


    public virtual SqlQuery Build(DbObject dbObject)
    {
      string sql = string.Empty;

      switch (dbObject.Action)
      {
        case DbAction.Create: sql = GetCreateSqlQuery(dbObject); break;
        case DbAction.Alter: sql = GetAlterSqlQuery(dbObject); break;
        case DbAction.Drop: sql = GetDropSqlQuery(dbObject); break;
        case DbAction.NoAction: return GetNoActionSqlQuery(dbObject); break;
      }

      if (!string.IsNullOrEmpty(sql))
        return new SqlQuery(sql);
      return null;
    }

    #region Create, Alter, Drop queries
    protected virtual string GetCreateSqlQuery(DbObject dbObject)
    {
      return string.Empty;
    }

    protected virtual string GetAlterSqlQuery(DbObject dbObject)
    {
      return string.Empty;
    }

    protected virtual string GetDropSqlQuery(DbObject dbObject)
    {      
      string dropFormat = "DROP {0} {1}{2}";
      return string.Format(dropFormat, dbObject.GetSqlMetadata().ObjectSqlName, 
        Settings.FormatName(dbObject.Name), Settings.ScriptTerminationSymbol);
    }

    protected virtual SqlQuery GetNoActionSqlQuery(DbObject dbObject)
    {
      return null;
    }
    #endregion

    #region Description query
    string _commentFormat = "COMMENT ON {0} {1} IS '{2}'{3}";

    public string CreateDescriptionQuery(DbObject dbObject)
    {
      if (dbObject.GetType() == typeof(Column))
      {
        var c = (Column)dbObject;
        return CreateDescriptionQuery(dbObject, Settings.FormatName(c.TableName) + "." + Settings.FormatName(c.Name));
      }

      return CreateDescriptionQuery(dbObject, Settings.FormatName(dbObject.Name));
    }

    public string CreateDescriptionQuery(DbObject dbObject, string explicitObjectName)
    {
      if (dbObject.Description != null)
        return string.Format(_commentFormat, dbObject.GetSqlMetadata().ObjectSqlName,
          explicitObjectName, dbObject.Description, Settings.ScriptTerminationSymbol);

      return null;
    }
    #endregion    
  }
}
