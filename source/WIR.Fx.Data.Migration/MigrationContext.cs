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
using WIR.Fx.Data.Migration.Fluent;

namespace WIR.Fx.Data.Migration
{
  /// <summary>
  /// Abstract implementation of migration context class
  /// </summary>
  public abstract class MigrationContext
  {
    public MigrationContext(MigrationSettings settings)
    {
      DbObjects = new List<DbObject>();

      _create = new CreateSyntax(DbObjects);
      _alter = new AlterSyntax(DbObjects);
      _drop = new DropSyntax(DbObjects);
      _script = new Fluent.Scripts.ScriptSyntax(DbObjects);
      _Db = new DatabaseSyntax(DbObjects);

      this.Settings = settings;
    }

    /// <summary>
    /// Migration settings
    /// </summary>
    protected MigrationSettings Settings { get; set; }

    DatabaseSyntax _Db;
    public DatabaseSyntax Db
    {
      get { return _Db; }
      set { _Db = value; }
    }

    /// <summary>
    /// List of database objects for migrating
    /// </summary>
    public List<DbObject> DbObjects { get; private set; }

    CreateSyntax _create;
    /// <summary>
    /// Create database object
    /// </summary>
    protected CreateSyntax Create
    {
      get { return _create; }
      set { _create = value; }
    }

    AlterSyntax _alter;
    /// <summary>
    /// Alter database object
    /// </summary>
    protected AlterSyntax Alter
    {
      get { return _alter; }
      set { _alter = value; }
    }

    DropSyntax _drop;
    /// <summary>
    /// Drop database object
    /// </summary>
    protected DropSyntax Drop
    {
      get { return _drop; }
      set { _drop = value; }
    }

    Fluent.Scripts.ScriptSyntax _script;
    /// <summary>
    /// Manages data
    /// </summary>
    protected Fluent.Scripts.ScriptSyntax Script
    {
      get { return _script; }
      set { _script = value; }
    }
       
    /// <summary>
    /// Creates list of database object for migrating
    /// </summary>
    public abstract void PullUp();    
  }
}
