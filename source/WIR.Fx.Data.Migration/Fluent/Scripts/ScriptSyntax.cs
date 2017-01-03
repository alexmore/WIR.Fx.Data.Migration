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

namespace WIR.Fx.Data.Migration.Fluent.Scripts
{
  public class ScriptSyntax : IScriptSyntax, IScriptQueryParametersSyntax,
    IScriptInsertSyntax, IScriptInsertColumnsSyntax, IScriptInsertValuesSyntax   
  {
    List<DbObject> _dbObjects;
    Script _currentScript;
    
    public ScriptSyntax(List<DbObject> dbObjects)
    {
      this._dbObjects = dbObjects;
    }

    public IScriptQueryParametersSyntax ExecuteQuery(string sqlQuery)
    {
      _currentScript = new Script() { SqlQuery = sqlQuery, Type = ScriptType.SqlQuery };
      _dbObjects.Add(_currentScript);
      return this;
    }

    public IScriptQueryParametersSyntax HasParameter(string name, object value)
    {
      _currentScript.Parameters.Add(name, value);
      return this;
    }
     
    #region Insert
    string _insertTableName;
    string[] _insertColumns;

    public IScriptInsertSyntax Insert
    {
      get { return this; }
    }

    public IScriptInsertColumnsSyntax Into(string tableName)
    {      
      _insertTableName = tableName;
      return this;
    }

    public IScriptInsertValuesSyntax ToColumns(params string[] columns)
    {
      if (columns.Length == 0)
        throw new ArgumentException("Length of columns can not be null in Script.Insert");
      _insertColumns = columns;
      return this;
    }

    public IScriptInsertValuesSyntax Values(params object[] values)
    {      
      if (values.Length != _insertColumns.Length)
        throw new ArgumentException("Count of columns and values doesn't equal in Script.Insert");

      _currentScript = new Script();
      _dbObjects.Add(_currentScript);

      _currentScript.Type = ScriptType.InsertQuery;
      _currentScript.TableName = _insertTableName;

      for (int i = 0; i < _insertColumns.Length; i++)
        _currentScript.Parameters.Add(_insertColumns[i], values[i]);

      return this;
    }
    #endregion

    #region Script    
    public void Execute(string fileName)
    {
      _currentScript = new Script() { SqlQuery = fileName, Type = ScriptType.SqlFile };      
      _dbObjects.Add(_currentScript);      
    }    
    #endregion


    #region Suspending and resuming constraints
    string _suspendConstraintName = null;
    public void SuspendConstraints()
    {
      _currentScript = new Script() { Type = ScriptType.SuspendConstraints };
      _suspendConstraintName = _currentScript.Name;
      _dbObjects.Add(_currentScript);      
    }

    public void ResumeConstraints()
    {
      if (_suspendConstraintName == null)
        throw new InvalidOperationException("ResumeConstraint can not be called before SuspendConstraint");
      _currentScript = new Script() { Type = ScriptType.ResumeConstraints };
      _currentScript.Name = _suspendConstraintName;
      _suspendConstraintName = null;
      _dbObjects.Add(_currentScript);      
    }
    #endregion
  }
}
