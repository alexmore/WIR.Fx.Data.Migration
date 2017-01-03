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
using System.Reflection;

using WIR.Fx.Data.Migration.Engine;
using WIR.Fx.Data.Migration.Engine.Tools;

namespace WIR.Fx.Data.Migration
{
  public class Migrator
  {
    MigrationSettings _settings;
    long _databaseCurrentVersion = 0;

    public Migrator(MigrationSettings settings)
    {
      _settings = settings;

      if (settings.CreateNewDatabaseIfMissing && !_settings.SqlQueryPerformer.IsDatabaseExists())
        _settings.SqlQueryPerformer.CreateDatabase();

      if (!IsVersionLogTableExists())
        CreateVersionLogTable();

      _databaseCurrentVersion = GetDatabaseCurrentVersion();
    }

    public void Migrate(string assemblyFile, long? migrateUpToVersion = null)
    {
      Migrate(Assembly.LoadFile(assemblyFile), migrateUpToVersion);
    }

    public void Migrate(Assembly assembly, long? migrateUpToVersion = null)
    {
      var scanner = new AssebmlyScanner();
      var contextInfos = scanner.Scan(assembly);

      if (contextInfos == null || contextInfos.Count() == 0) return;

      if (!migrateUpToVersion.HasValue)
        migrateUpToVersion = long.MaxValue;


      foreach (var i in contextInfos
        .Where(x => x.Version > _databaseCurrentVersion && x.Version <= migrateUpToVersion.Value)
        .OrderBy(x => x.Version))
      {
        Migrate(i.CreateContext(_settings));
      }

    }

    public void Migrate(MigrationContext context)
    {      
      try
      {
        PerformSqlQueries(CreateSqlQueryListFromContext(context));
      }
      catch (Exception e)
      {
        string message = "Performing migration context failed. {0}See inner exception for details.";
        var a = context.GetType().GetAttribute<MigrationVersionAttribute>();
        string ver = "";
        if (a != null)
          ver = "Migration context version: "+a.Version.ToString()+". ";

        throw new Exception(string.Format(message, ver), e);
      }
    }

    public void Migrate(IEnumerable<MigrationContext> contexts)
    {
      foreach (var i in contexts)
        Migrate(i);
    }


    #region Database & Log table routines
    private bool IsVersionLogTableExists()
    {      
      string sql = "select 1 from rdb$relations where rdb$relation_name = '"
        + _settings.FormatName(_settings.MigrationLogTableName, true)
        + "'";

      _settings.SqlQueryPerformer.BeginTransaction();
      var r = _settings.SqlQueryPerformer.ExecuteScalar(new SqlQuery(sql));
      _settings.SqlQueryPerformer.CommitTransaction();

      if (r == null || r == DBNull.Value) return false;

      return true;
    }

    private void CreateVersionLogTable() 
    {
      MigrationLogTableContext c = new MigrationLogTableContext(_settings);
      Migrate(c);
    }

    private long GetDatabaseCurrentVersion()
    {
      string sql = "SELECT MAX(" +
        _settings.FormatName(_settings.MigrationLogColumnName) +
        ") FROM " +
        _settings.FormatName(_settings.MigrationLogTableName);

      _settings.SqlQueryPerformer.BeginTransaction();
      var r = _settings.SqlQueryPerformer.ExecuteScalar(new SqlQuery(sql));
      _settings.SqlQueryPerformer.CommitTransaction();
      
      if (r != null && r != DBNull.Value)
        return long.Parse(r.ToString());
      
      return 0;
    }
    #endregion

    #region SqlQuery list building
    private IEnumerable<SqlQuery> CreateSqlQueryListFromContext(MigrationContext context)
    {
      var res = new List<SqlQuery>();

      context.PullUp();

      long? contextVersion = null;
      if (context.GetType().GetAttribute<MigrationVersionAttribute>() != null)      
        contextVersion = context.GetType().GetAttribute<MigrationVersionAttribute>().Version;
      
      // Add version Script to list
      if (contextVersion > 0)
      {
        new Fluent.Scripts.ScriptSyntax(context.DbObjects)
          .Insert.Into(_settings.MigrationLogTableName)
          .ToColumns(_settings.MigrationLogColumnName)
          .Values(contextVersion);
      }

      CheckDbObjects(context.DbObjects);

      foreach (var i in context.DbObjects)
      {
        var qb = _settings.CreateQueryBuilder(i);
        var q = qb.Build(i);

        if (i.GetType() == typeof(DbObjects.Script)
          && (i as DbObjects.Script).Type == DbObjects.ScriptType.SuspendConstraints)
        {
          // Building ScriptType.SuspendConstraints Script generates suspend
          // and resume queries both and ScriptType.ResumeConstraints Script 
          // must be replaced with ScriptType.SqlQuery and apropriate query
          FillResumeConstraintsScript(i.Name, context.DbObjects, qb);
        }        
        
        res.Add(q);
      }    

      return res;
    }

    private void FillResumeConstraintsScript(string name, 
      IEnumerable<DbObjects.DbObject> dbObjects, 
      IQueryBuilder queryBuilder)
    {
      var s = dbObjects.Where(x => x.Name == name && (x as DbObjects.Script).Type == DbObjects.ScriptType.ResumeConstraints)
        .Select(x => (DbObjects.Script)x).FirstOrDefault();
      if (s == null)
        throw new InvalidOperationException("SuspendConstraints can not be "
          +"executed because it does not matched to a ResumeConstraints. "
          +"Use ResumeConstraints after running SuspendConstraints.");
            
      s.SqlQuery = queryBuilder.Build(s).Query;
      s.Type = DbObjects.ScriptType.SqlQuery;
    }
    #endregion

    #region SqlQuery lists performing
    private void PerformSqlQueries(IEnumerable<SqlQuery> queries)
    {
      _settings.SqlQueryPerformer.BeginTransaction();
      try
      {
        foreach (var i in queries)        
          _settings.SqlQueryPerformer.Execute(i);        

        _settings.SqlQueryPerformer.CommitTransaction();
      }
      catch
      {
        _settings.SqlQueryPerformer.RollbackTransaction();        
      }
    }
    #endregion

    #region Checking 
    private void CheckDbObjects(IEnumerable<DbObjects.DbObject> dbObjects)
    {
      // Checking CREATE TABLE and INSERT in that one at the same migration context      
      foreach (var i in dbObjects)
      {        
        if (i.GetType() == typeof(DbObjects.Script))
        {
          var q = (DbObjects.Script)i;
          if (q.SqlQuery == "INSERT")
          {
            if (dbObjects
              .Where(x => x.GetType() == typeof(DbObjects.Table)
              && ((DbObjects.Table)x).Name.ToUpper() == q.TableName.ToUpper())
              .Count() > 0)
              throw new InvalidOperationException("CREATE TABLE and INSERT to "
                +"this one operations can not be placed in the same migration context. "
                +"Move INSERT operations to another context with greater version. "
                +"Table name: "+q.TableName);

          }          
        }
      }
    }
    #endregion
  }
}
