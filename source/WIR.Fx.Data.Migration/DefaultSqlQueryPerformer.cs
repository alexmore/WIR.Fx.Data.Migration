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
using System.Data;

using FirebirdSql.Data.FirebirdClient;

using WIR.Fx.Data.Migration.Engine;

namespace WIR.Fx.Data.Migration
{
  public class DefaultSqlQueryPerformer : ISqlQueryPerformer
  {
    FbConnection _connection;
    bool _connectionWasOpenned = false;
    FbTransaction _transaction;    

    public DefaultSqlQueryPerformer(FbConnection connection)
    {
      this._connection = connection;      
    }

    public DefaultSqlQueryPerformer(string connectionString)
      : this (new FbConnection(connectionString))
    {
      
    }

    #region Transactions
    public void BeginTransaction()
    {
      if (_connection.State != System.Data.ConnectionState.Open &&
        _connection.State != System.Data.ConnectionState.Fetching &&
        _connection.State != System.Data.ConnectionState.Executing &&
        _connection.State != System.Data.ConnectionState.Connecting)
      {
        _connection.Open();
        _connectionWasOpenned = true;
      }

      _transaction = _connection.BeginTransaction();
    }

    public void CommitTransaction()
    {
      _transaction.Commit();
      if (_connectionWasOpenned)
        _connection.Close();
    }

    public void RollbackTransaction()
    {
      _transaction.Rollback();
      if (_connectionWasOpenned)
        _connection.Close();
    }
    #endregion

    #region Executing
    public object ExecuteScalar(SqlQuery query)
    {
      return CreateCommand(query).ExecuteScalar();
    }

    public IDataReader ExecuteReader(SqlQuery query)
    {
      return CreateCommand(query).ExecuteReader();
    }

    public void Execute(SqlQuery query)
    {      
      var sc = new FirebirdSql.Data.Isql.FbScript(query.Query);      
      sc.Parse();

      foreach (var i in sc.Results)
      {
        if (i.Text?.ToUpper().Trim() != "COMMIT WORK")          
          CreateCommand(i.Text, query.Parameters).ExecuteNonQuery();          
      }
    }   

    private FbCommand CreateCommand(SqlQuery query)
    {
      return CreateCommand(query.Query, query.Parameters);      
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    private FbCommand CreateCommand(string sqlQuery, Dictionary<string, object> parameters)
    {
      FbCommand cmd = new FbCommand(sqlQuery, _connection, _transaction);      
      if (parameters != null)
      {
        foreach (var i in parameters)
          cmd.Parameters.Add(i.Key, i.Value);
      }

      return cmd;
    }
    #endregion  

    #region Database tools
    public void CreateDatabase()
    {      
       FbConnection.CreateDatabase(_connection.ConnectionString);      
    }

    public void DropDatabase(bool force = false)
    {
      if (force)
        FbConnection.ClearAllPools();      
      FbConnection.DropDatabase(_connection.ConnectionString);
    }

    public bool IsDatabaseExists()
    {
      try
      {
        using (var c = new FbConnection(_connection.ConnectionString))
        {
          c.Open();          
        }        
      }
      catch
      {
        return false;
      }

      return true;
    }
    #endregion
  }
}
