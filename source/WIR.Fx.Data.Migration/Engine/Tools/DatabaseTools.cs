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

namespace WIR.Fx.Data.Migration.Engine.Tools
{
  public class DatabaseTools
  {
    MigrationSettings _settings;

    public DatabaseTools(MigrationSettings settings)
    {
      _settings = settings;
    }
   
    public void CreateDatabase()
    {
      _settings.SqlQueryPerformer.CreateDatabase();
    }

    public void DropDatabse(bool force = false)
    {
      _settings.SqlQueryPerformer.DropDatabase(force);
    }

    #region Objects existence
    private object ExecuteScalar(string sqlFormat, params object[] arguments)
    {
      _settings.SqlQueryPerformer.BeginTransaction();
      try
      {
        return _settings.SqlQueryPerformer
          .ExecuteScalar(new SqlQuery(
            string.Format(sqlFormat, arguments)
            ));
      }
      finally
      {
        _settings.SqlQueryPerformer.CommitTransaction();
      }
    }

    private bool IsObjectExists(string sqlFormat, params object[] arguments)
    {
      var r = ExecuteScalar(sqlFormat, arguments);
      return (int)r > 0;
    }

    public bool IsTableExists(string tableName)
    {   
      return IsObjectExists(
        "select count(*) from rdb$relations where rdb$relation_name = '{0}'",
        _settings.FormatName(tableName, true)
        );
    }

    public int GetRecordsCountInTable(string tableName)
    {
      return (int)ExecuteScalar(
        "select count(*) from {0}",
        _settings.FormatName(tableName)
        );
    }

    public bool IsDomainExists(string domainName)
    {      
      return IsObjectExists(
        "select count(*) from rdb$fields "
        +"where not (rdb$field_name like 'RDB$%') and rdb$field_name = '{0}'",
        _settings.FormatName(domainName, true)
        );
    }

    public bool IsColumnExists(string tableName, string columnName)
    {
      return IsObjectExists(
       "SELECT count(RDB$FIELD_NAME) FROM RDB$RELATION_FIELDS "
       + "WHERE RDB$RELATION_NAME='{0}' and RDB$FIELD_NAME='{1}';",
       _settings.FormatName(tableName, true),
       _settings.FormatName(columnName, true)
       );
    }

    public bool IsGeneratorExists(string genName)
    {
      return IsObjectExists(
       "SELECT COUNT(RDB$GENERATOR_NAME) FROM RDB$GENERATORS "
       + "WHERE RDB$SYSTEM_FLAG=0 AND RDB$GENERATOR_NAME = '{0}';",       
       _settings.FormatName(genName, true)
       );
    }

    public bool IsConstraintExists(string name, string tableName, string constraintType)
    {
      return IsObjectExists(
      "SELECT COUNT(A.RDB$CONSTRAINT_NAME) "+
      "FROM RDB$RELATION_CONSTRAINTS A "+
      "WHERE A.RDB$CONSTRAINT_TYPE = '{2}' AND " +
      "A.RDB$RELATION_NAME = '{1}' AND " +
      "A.RDB$CONSTRAINT_NAME = '{0}'",
      _settings.FormatName(name, true),
      _settings.FormatName(tableName, true),
      constraintType.ToUpper()
      );
    }

    public bool IsTriggerExists(string name)
    {
      return IsObjectExists(
      "SELECT COUNT(RDB$TRIGGER_NAME) " +
       "FROM RDB$TRIGGERS " +
       "WHERE " +
       "RDB$TRIGGER_NAME = '{0}'",
      _settings.FormatName(name, true)
      );
    }

    public bool IsTriggerActive(string name)
    {
      return IsObjectExists(
      "SELECT COUNT(RDB$TRIGGER_NAME) " +
       "FROM RDB$TRIGGERS " +
       "WHERE " +
       "((RDB$SYSTEM_FLAG = 0) OR (RDB$SYSTEM_FLAG is null)) AND " +
       "RDB$TRIGGER_INACTIVE = 0 AND "+
       "RDB$TRIGGER_NAME = '{0}'",
      _settings.FormatName(name, true)
      );
    }

    
    #endregion


  }
}
