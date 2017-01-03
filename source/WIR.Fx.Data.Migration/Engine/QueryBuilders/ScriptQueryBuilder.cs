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
using System.IO;

using WIR.Fx.Data.Migration.DbObjects;

namespace WIR.Fx.Data.Migration.Engine.QueryBuilders
{
  public class ScriptQueryBuilder : QueryBuilderCore
  {
    public ScriptQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }
        
    protected override SqlQuery GetNoActionSqlQuery(DbObject dbObject)
    {
      var q = (Script)dbObject;
      var res = new SqlQuery();

      switch (q.Type)
      {
        case ScriptType.SqlQuery: 
          res.Query = q.SqlQuery;
          res.Parameters = q.Parameters;
          break;
        case ScriptType.InsertQuery: 
          res.Query = GetInsertSqlQuery(q);
          res.Parameters = new Dictionary<string, object>();
          foreach (var i in q.Parameters)
            res.Parameters.Add("@" + i.Key, i.Value);
          break;
        case ScriptType.SqlFile:
          res.Query = LoadScript(q.SqlQuery);                    
          break;        
        case ScriptType.SuspendConstraints:
          res.Query = GetSuspendConstraintsScript();
          break;
        case ScriptType.ResumeConstraints:
          res.Query = GetResumeConstraintsScript();
          break;
      }
  
      return res;
    }

    #region Sql file
    private string LoadScript(string fileName)
    {
      using (StreamReader sr = new StreamReader(new FileStream(fileName, FileMode.Open)))
      {
        return sr.ReadToEnd();
      }
    }
    #endregion

    #region Insert
    private string GetInsertSqlQuery(Script q)
    {
      string sqlFormat = "INSERT INTO {0} ({1}) VALUES ({2}){3}";
      string columns = string.Empty;
      string values = string.Empty;

      foreach (var i in q.Parameters)
      {
        columns += ", " + Settings.FormatName(i.Key);
        values += ", @" + i.Key;
      }

      if (columns.Length > 2) columns = columns.Substring(2);
      if (values.Length > 2) values = values.Substring(2);

      return string.Format(sqlFormat,
        Settings.FormatName(q.TableName),
        columns,
        values,
        Settings.ScriptTerminationSymbol);
    }
    #endregion

    #region Suspending constraints
    #region Sql queries
    string _suspendConstraintsTriggersSql =
       "SELECT TRIM(RDB$TRIGGER_NAME) " +
       "FROM RDB$TRIGGERS " +
       "WHERE " +
       "((RDB$SYSTEM_FLAG = 0) OR (RDB$SYSTEM_FLAG is null)) AND " +
       "(RDB$TRIGGER_INACTIVE = 0)";

    string _suspendConstraintsFkListSql =
      "SELECT DISTINCT TRIM(A.RDB$CONSTRAINT_NAME), TRIM(A.RDB$RELATION_NAME) "+
      "FROM RDB$RELATION_CONSTRAINTS A "+
      "WHERE A.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'";

    string _suspendConstrainsFkDetailsSql =
      "SELECT isr_ref.RDB$FIELD_NAME, relc_ref.RDB$RELATION_NAME, " +
      "isr.RDB$FIELD_NAME, rc.RDB$UPDATE_RULE, rc.RDB$DELETE_RULE " +
      "FROM " +
      "RDB$REF_CONSTRAINTS rc " +
      "INNER JOIN RDB$RELATION_CONSTRAINTS relc ON (relc.RDB$CONSTRAINT_NAME = rc.RDB$CONSTRAINT_NAME) " +
      "INNER JOIN RDB$RELATION_CONSTRAINTS relc_ref ON (rc.RDB$CONST_NAME_UQ = relc_ref.RDB$CONSTRAINT_NAME) " +
      "INNER JOIN RDB$INDEX_SEGMENTS isr ON (relc_ref.RDB$INDEX_NAME = isr.RDB$INDEX_NAME) " +
      "INNER JOIN RDB$INDEX_SEGMENTS isr_ref ON (relc.RDB$INDEX_NAME = isr_ref.RDB$INDEX_NAME) " +
      "WHERE " +
      "relc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' AND relc.RDB$CONSTRAINT_NAME = '{0}' " +
      "ORDER BY isr.RDB$FIELD_POSITION, isr_ref.RDB$FIELD_POSITION";

    string _suspendConstraintsChecksSql =
      "SELECT TRIM(A.RDB$CONSTRAINT_NAME), TRIM(A.RDB$RELATION_NAME), TRIM(C.RDB$TRIGGER_SOURCE) "+
      "FROM RDB$RELATION_CONSTRAINTS A, RDB$CHECK_CONSTRAINTS B, RDB$TRIGGERS C "+
      "WHERE (A.RDB$CONSTRAINT_TYPE = 'CHECK') AND (A.RDB$CONSTRAINT_NAME = B.RDB$CONSTRAINT_NAME) "+
      "AND (B.RDB$TRIGGER_NAME = C.RDB$TRIGGER_NAME) AND (C.RDB$TRIGGER_TYPE = 1)";
    #endregion

    #region Sql query formats
    string _suspendConstraintsTriggerInactiveFormat = "ALTER TRIGGER {0} INACTIVE{1}";
    string _suspendConstraintsTriggerActiveFormat = "ALTER TRIGGER {0} ACTIVE{1}";

    string _suspendConstraintsFkDropFormat = "ALTER TABLE {0} DROP CONSTRAINT {1}{2}";
    string _suspendConstraintFkCreateFormat = 
      "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ({2}) "+
      "REFERENCES {3} ({4}) ON UPDATE {5} ON DELETE {6}{7}";

    string _suspendConstraintsCheckDropFormat = "ALTER TABLE {0} DROP CONSTRAINT {1}{2}";
    string _suspendConstraintsCheckCreateFormat =
      "ALTER TABLE {0} ADD CONSTRAINT {1} {2}{3}";
    #endregion

    string _suspendConstraintsScript = null;
    string _resumeConstraintsScript = null;

    private void CreateSuspendConstraintsScripts()
    {
      StringBuilder sbSuspend = new StringBuilder();
      StringBuilder sbResume = new StringBuilder();

      Settings.SqlQueryPerformer.BeginTransaction();

      //////////////////////// Triggers ////////////////////////
      using (var dr = Settings.SqlQueryPerformer.ExecuteReader(
        new SqlQuery(_suspendConstraintsTriggersSql)))
      {
        while (dr.Read())
        {
          sbSuspend.AppendFormat(_suspendConstraintsTriggerInactiveFormat,
            Settings.FormatName(dr.GetString(0)),
            Settings.ScriptTerminationSymbol); sbSuspend.AppendLine();

          sbResume.AppendFormat(_suspendConstraintsTriggerActiveFormat,
            Settings.FormatName(dr.GetString(0)),
            Settings.ScriptTerminationSymbol); sbResume.AppendLine();          
        }                
      }

      //////////////////////// FK ////////////////////////
      using (var dr = Settings.SqlQueryPerformer.ExecuteReader(
        new SqlQuery(_suspendConstraintsFkListSql)))
      {
        while (dr.Read())
        {
          sbSuspend.AppendFormat(_suspendConstraintsFkDropFormat,
            Settings.FormatName(dr.GetString(1)),
            Settings.FormatName(dr.GetString(0)),
            Settings.ScriptTerminationSymbol); sbSuspend.AppendLine();
          
          sbResume.AppendLine(GetCreateFkSql(dr.GetString(0).Trim(), dr.GetString(1).Trim()));          
        }        
      }
    
      //////////////////////// Checks ////////////////////////      
      using (var dr = Settings.SqlQueryPerformer.ExecuteReader(
        new SqlQuery(_suspendConstraintsChecksSql)))
      {
        while (dr.Read())
        {
          sbSuspend.AppendFormat(_suspendConstraintsCheckDropFormat,
            Settings.FormatName(dr.GetString(1)),
            Settings.FormatName(dr.GetString(0)),
            Settings.ScriptTerminationSymbol); sbSuspend.AppendLine();

          sbResume.AppendFormat(_suspendConstraintsCheckCreateFormat,
            Settings.FormatName(dr.GetString(1)),
            Settings.FormatName(dr.GetString(0)),
            dr.GetString(2).Trim(),
            Settings.ScriptTerminationSymbol); sbResume.AppendLine();      
        }        
      }

      Settings.SqlQueryPerformer.CommitTransaction();
      _suspendConstraintsScript = sbSuspend.ToString();
      _resumeConstraintsScript = sbResume.ToString();
      
    }

    private string GetCreateFkSql(string name, string tableName)
    {     
      List<string> cols = new List<string>();
      List<string> refCols =new List<string>();
      string refTable = string.Empty;
      string deleteRule = "RESTRICT";
      string updateRule = "RESTRICT";

      using (var dr = Settings.SqlQueryPerformer.ExecuteReader(
        new SqlQuery(string.Format(_suspendConstrainsFkDetailsSql, name))))
      {
        while (dr.Read())
        {
          if (!cols.Contains(dr.GetString(0).Trim())) cols.Add(dr.GetString(0).Trim());
          if (!refCols.Contains(dr.GetString(2).Trim())) refCols.Add(dr.GetString(2).Trim());

          refTable = dr.GetString(1).Trim();
          updateRule = dr.GetString(3).Trim();
          deleteRule = dr.GetString(4).Trim();
        }        
      }
      
      return string.Format(_suspendConstraintFkCreateFormat,
        Settings.FormatName(tableName),
        Settings.FormatName(name),
        Settings.FormatNameCollection(cols.ToArray()),
        Settings.FormatName(refTable),
        Settings.FormatNameCollection(refCols.ToArray()),
        updateRule.Replace("RESTRICT", "NO ACTION"),
        deleteRule.Replace("RESTRICT", "NO ACTION"),
        Settings.ScriptTerminationSymbol
        );
    }

    private string GetSuspendConstraintsScript()
    {
      if (_suspendConstraintsScript == null)
        CreateSuspendConstraintsScripts();
      return _suspendConstraintsScript;
    }

    private string GetResumeConstraintsScript()
    {
      if (_resumeConstraintsScript == null)
        throw new InvalidOperationException("ResumeConstrains script is not created. Use SuspendConstraints before call ResumeConstrains.");

      return _resumeConstraintsScript;
    }
    #endregion
  }
}
