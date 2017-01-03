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

using WIR.Fx.Data.Migration.Engine;
using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Engine.QueryBuilders;

namespace WIR.Fx.Data.Migration
{
  public class MigrationSettings
  {
    public MigrationSettings(ISqlQueryPerformer sqlQueryPerformer)
    {
      _queryBuildersMap = new Dictionary<Type, Type>();
      SqlQueryPerformer = sqlQueryPerformer;
    }

    #region Default
    public static MigrationSettings GetDefaultSettings(ISqlQueryPerformer sqlQueryPerformer)
    {
      MigrationSettings defaultSettings = new MigrationSettings(sqlQueryPerformer);

      defaultSettings.DbObjectsNameFormat = FbNameFormat.Safe;
      defaultSettings.ScriptTerminationSymbol = ";";

      defaultSettings.MigrationLogDomainName = "MigrationVer";
      defaultSettings.MigrationLogTableName = "MigrationLog";
      defaultSettings.MigrationLogColumnName = "Version";

      defaultSettings.CreateNewDatabaseIfMissing = true;

      defaultSettings.RegisterQueryBuilder<Column, ColumnQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Constraint, ConstraintQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<ConstraintCheck, ConstraintQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<ConstraintForeignKey, ConstraintQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<ConstraintPrimaryKey, ConstraintQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<ConstraintUnique, ConstraintQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Domain, DomainQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Generator, GeneratorQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Index, IndexQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Procedure, ProcedureQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Script, ScriptQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Table, TableQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<Trigger, TriggerQueryBuilder>();
      defaultSettings.RegisterQueryBuilder<View, ViewQueryBuilder>();      

      return defaultSettings;
    }
    #endregion

    #region Database properties
    /// <summary>
    /// Sets flag to create new database if missing on migration operation
    /// </summary>
    public bool CreateNewDatabaseIfMissing { get; set; }    
    #endregion

    #region Query performers
    public ISqlQueryPerformer SqlQueryPerformer { get; set; }
    #endregion

    #region Sql buliding settings
    public FbNameFormat DbObjectsNameFormat { get; set; }
    public string ScriptTerminationSymbol { get; set; }

    public string FormatName(string dbObjectName, bool skipQuotes = false)
    {
      string nameFormat = "\"{0}\"";
      if (skipQuotes)
        nameFormat = "{0}";

      switch (DbObjectsNameFormat)
      {
        case FbNameFormat.Safe: return string.Format(nameFormat, dbObjectName.Trim());
        default: return dbObjectName.Trim().ToUpper();
      }
    }

    public string FormatNameCollection(string[] col)
    {
      string sCols = string.Empty;
      foreach (var i in col) sCols += ", " + FormatName(i);
      return sCols.Substring(2);
    }
    #endregion

    #region Migration Log table
    public string MigrationLogDomainName { get; set; }
    public string MigrationLogTableName { get; set; }
    public string MigrationLogColumnName { get; set; }    
    #endregion
    
    #region Query builders mapping
    Dictionary<Type, Type> _queryBuildersMap { get; set; }

    public void RegisterQueryBuilder<TDbObject, TQueryBuilder>()
      where TDbObject : DbObjects.DbObject
      where TQueryBuilder : IQueryBuilder
    {
      _queryBuildersMap.Add(typeof(TDbObject), typeof(TQueryBuilder));
    }

    public IQueryBuilder CreateQueryBuilder(Type dbObjectType)
    {
      if (!_queryBuildersMap.ContainsKey(dbObjectType))
        throw new NotImplementedException("Query builder for " + dbObjectType.ToString() + " not found.");

      var q = _queryBuildersMap[dbObjectType];
      return (IQueryBuilder)Activator.CreateInstance(q, this);
    }

    public IQueryBuilder CreateQueryBuilder(DbObject dbObject)
    {
      return CreateQueryBuilder(dbObject.GetType());
    }
    #endregion
  }
}
