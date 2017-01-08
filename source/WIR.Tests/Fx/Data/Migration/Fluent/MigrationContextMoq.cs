using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Fluent;

namespace WIR.Tests.Fx.Data.Migration
{
  public class MigrationContextMoq
  {
    public MigrationContextMoq()
    {
      DbObjects = new List<DbObject>();

      _create = new CreateSyntax(DbObjects);
      _alter = new AlterSyntax(DbObjects);
      _drop = new DropSyntax(DbObjects);
      _script = new WIR.Fx.Data.Migration.Fluent.Scripts.ScriptSyntax(DbObjects);     
    }

    /// <summary>
    /// List of database objects for migrating
    /// </summary>
    public List<DbObject> DbObjects { get; private set; }

    CreateSyntax _create;
    /// <summary>
    /// Create database object
    /// </summary>
    public CreateSyntax Create
    {
      get { return _create; }
      set { _create = value; }
    }

    AlterSyntax _alter;
    /// <summary>
    /// Alter database object
    /// </summary>
    public AlterSyntax Alter
    {
      get { return _alter; }
      set { _alter = value; }
    }

    DropSyntax _drop;
    /// <summary>
    /// Drop database object
    /// </summary>
    public DropSyntax Drop
    {
      get { return _drop; }
      set { _drop = value; }
    }

    WIR.Fx.Data.Migration.Fluent.Scripts.ScriptSyntax _script;
    /// <summary>
    /// Manages data
    /// </summary>
    public WIR.Fx.Data.Migration.Fluent.Scripts.ScriptSyntax Script
    {
      get { return _script; }
      set { _script = value; }
    }   
  }
}
