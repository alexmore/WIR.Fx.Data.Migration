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

namespace WIR.Fx.Data.Migration.Fluent
{
  public class DropSyntax
  {
    List<DbObject> _dbObjects;

    public DropSyntax(List<DbObject> dbObjects)
    {
      this._dbObjects = dbObjects;
    }

    /// <summary>
    /// Drops domain
    /// </summary>
    /// <param name="name">Domain name</param>
    /// <returns></returns>
    public DropSyntax Domain(string name)
    {
      _dbObjects.Add(new Domain(name, DbAction.Drop));
      return this;
    }

    /// <summary>
    /// Drops generator
    /// </summary>
    /// <param name="name">Generator name</param>
    /// <returns></returns>
    public DropSyntax Generator(string name)
    {
      _dbObjects.Add(new Generator(name, DbAction.Drop));
      return this;
    }

    /// <summary>
    /// Drops procedudre
    /// </summary>
    /// <param name="name">Procedure name</param>
    /// <returns></returns>
    public DropSyntax Procedure(string name)
    {
      _dbObjects.Add(new Procedure(name, DbAction.Drop));
      return this;
    }

    /// <summary>
    /// Drops index
    /// </summary>
    /// <param name="name">Index name</param>
    /// <returns></returns>
    public DropSyntax Index(string name)
    {
      _dbObjects.Add(new Index(name, DbAction.Drop));
      return this;
    }

    /// <summary>
    /// Drops constraint
    /// </summary>
    /// <param name="name">Constraint name</param>
    /// <param name="tableName">Constraint on tabel</param>
    /// <returns></returns>
    public DropSyntax Constraint(string name, string tableName)
    {
      _dbObjects.Add(new Constraint(name, DbAction.Drop) { TableName = tableName });
      return this;
    }

    /// <summary>
    /// Drops trigger
    /// </summary>
    /// <param name="name">Trigger name</param>
    /// <returns></returns>
    public DropSyntax Trigger(string name)
    {
      _dbObjects.Add(new Trigger(name, DbAction.Drop));
      return this;
    }

    /// <summary>
    /// Drops view
    /// </summary>
    /// <param name="name">View name</param>
    /// <returns></returns>
    public DropSyntax View(string name)
    {
      _dbObjects.Add(new View(name, DbAction.Drop));
      return this;
    }

    /// <summary>
    /// Drops column in table
    /// </summary>
    /// <param name="name"></param>
    /// <param name="onTable"></param>
    /// <returns></returns>
    public DropSyntax Column(string name, string onTable)
    {
      _dbObjects.Add(new Column(name, DbAction.Drop) { TableName = onTable });
      return this;
    }

    /// <summary>
    /// Drops table
    /// </summary>
    /// <param name="name">Table name</param>
    /// <returns></returns>
    public DropSyntax Table(string name)
    {
      _dbObjects.Add(new Table(name, DbAction.Drop));
      return this;
    }
  }
}
