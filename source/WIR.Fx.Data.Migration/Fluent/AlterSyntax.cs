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
  public class AlterSyntax
  {
    List<DbObject> _dbObjects;

    public AlterSyntax(List<DbObject> dbObjects)
    {
      this._dbObjects = dbObjects;
    }

    /// <summary>
    /// Alters domain
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Domains.IDomainAlterSyntax Domain(string name)
    {
      Domain d = new Domain(name, DbAction.Alter);
      _dbObjects.Add(d);
      return new Domains.DomainSyntax(d);
    }

    /// <summary>
    /// Alters generator
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Generators.IGeneratorSyntax Generator(string name)
    {
      Generator g = new Generator(name, DbAction.Alter);
      _dbObjects.Add(g);
      return new Generators.GeneratorSyntax(g);
    }

    /// <summary>
    /// Alters stored procedure
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Procedures.IProcedureSyntax Procedure(string name)
    {
      Procedure v = new Procedure(name, DbAction.Alter);
      _dbObjects.Add(v);
      return new Procedures.ProcedureSyntax(v);
    }

    /// <summary>
    /// Alters index
    /// </summary>
    /// <param name="name">Index name</param>
    /// <returns></returns>
    public Indexes.IIndexAlterSyntax Index(string name)
    {
      Index i = new Index(name, DbAction.Alter);
      _dbObjects.Add(i);
      return new Indexes.IndexSyntax(i);
    }

    /// <summary>
    /// Alters trigger
    /// </summary>
    /// <param name="name">Trigger name</param>
    /// <returns></returns>
    public Triggers.ITriggerTableSyntax Trigger(string name)
    {
      Trigger t = new Trigger(name, DbAction.Alter);
      _dbObjects.Add(t);
      return new Triggers.TriggerSyntax(t);
    }
    
    /// <summary>
    /// Alters view
    /// </summary>
    /// <param name="name">View name</param>
    /// <returns></returns>
    public Views.IViewSyntax View(string name)
    {
      View v = new View(name, DbAction.Alter);
      _dbObjects.Add(v);
      return new Views.ViewSyntax(v);
    }

    /// <summary>
    /// Alters column for a table
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Columns.IColumnTableSyntax<Columns.IColumnAlterSyntax> Column(string name)
    {
      Column c = new Column(name, DbAction.Alter);
      _dbObjects.Add(c);
      return new Columns.ColumnSyntax(c, _dbObjects);
    }

    /// <summary>
    /// Alters table
    /// </summary>
    /// <param name="name">Table name</param>
    /// <returns></returns>
    public Tables.ITableAlterSyntax Table(string name)
    {
      Table t = new Table(name, DbAction.Alter);
      _dbObjects.Add(t);
      return new Tables.TableSyntax(t, _dbObjects);
    }

  }
}
