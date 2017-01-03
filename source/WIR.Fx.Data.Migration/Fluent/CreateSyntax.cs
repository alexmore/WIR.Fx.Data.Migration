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
  public class CreateSyntax
  {
    List<DbObject> _dbObjects;

    public CreateSyntax(List<DbObject> dbObjects)
    {
      this._dbObjects = dbObjects;
    }

    /// <summary>
    /// Creates new domain
    /// </summary>
    /// <param name="name">Domain name</param>
    /// <returns></returns>
    public Domains.IDomainTypeSyntax Domain(string name)
    {
      Domain d = new Domain(name, DbAction.Create);
      _dbObjects.Add(d);
      return new Domains.DomainTypeSyntax(d);
    }

    /// <summary>
    /// Creates new generator
    /// </summary>
    /// <param name="name">Generator name</param>
    /// <returns></returns>
    public Generators.IGeneratorSyntax Generator(string name)
    {
      Generator g = new Generator(name, DbAction.Create);
      _dbObjects.Add(g);
      return new Generators.GeneratorSyntax(g);
    }

    /// <summary>
    /// Creates new stored procedure 
    /// </summary>
    /// <param name="name">Procedure name</param>
    /// <returns></returns>
    public Procedures.IProcedureSyntax Procedure(string name)
    {
      Procedure v = new Procedure(name, DbAction.Create);
      _dbObjects.Add(v);
      return new Procedures.ProcedureSyntax(v);
    }

    /// <summary>
    /// Creates new index
    /// </summary>
    /// <param name="name">Index name</param>
    /// <returns></returns>
    public Indexes.IIndexTableSyntax Index(string name)
    {
      Index i = new Index(name, DbAction.Create);
      _dbObjects.Add(i);
      return new Indexes.IndexSyntax(i);
    }

    /// <summary>
    /// Creates new constraint
    /// </summary>
    /// <param name="name">Constraint name</param>
    /// <returns></returns>
    public Constraints.IConstraintTableSyntax Constraint(string name)
    {
      return new Constraints.ConstraintSyntax(name, _dbObjects);
    }

    /// <summary>
    /// Creates trigger
    /// </summary>
    /// <param name="name">Trigger name</param>
    /// <returns></returns>
    public Triggers.ITriggerTableSyntax Trigger(string name)
    {
      Trigger t = new Trigger(name, DbAction.Create);
      _dbObjects.Add(t);
      return new Triggers.TriggerSyntax(t);
    }

    /// <summary>
    /// Creates new view
    /// </summary>
    /// <param name="name">View name</param>
    /// <returns></returns>
    public Views.IViewSyntax View(string name)
    {
      View v = new View(name, DbAction.Create);
      _dbObjects.Add(v);
      return new Views.ViewSyntax(v);
    }

    /// <summary>
    /// Creates new column for a table
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Columns.IColumnTableSyntax<Columns.IColumnCreateAsDomainSyntax> Column(string name)
    {
      Column c = new Column(name, DbAction.Create);
      _dbObjects.Add(c);
      return new Columns.ColumnSyntax(c, _dbObjects);
    }

    /// <summary>
    /// Creates new tabel
    /// </summary>
    /// <param name="name">Table name</param>
    /// <returns></returns>
    public Tables.ITableCreateSyntax Table(string name)
    {
      Table t = new Table(name, DbAction.Create);
      _dbObjects.Add(t);
      return new Tables.TableSyntax(t, _dbObjects);
    }
  }
}
