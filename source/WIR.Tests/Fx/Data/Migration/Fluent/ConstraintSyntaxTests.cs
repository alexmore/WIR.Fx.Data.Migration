using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WIR.Fx.Data.Migration;
using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Fluent;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class ConstraintSyntaxTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      mc = new MigrationContextMoq();
    }

    MigrationContextMoq mc;
    #endregion
    
    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreatePrimaryKeyTest()
    {
      mc.Create.Constraint("Pk").OnTable("t").OnColumns("id1", "id2")
        .AsPrimaryKey();

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintPrimaryKey));
      var c = mc.DbObjects.Last() as ConstraintPrimaryKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Pk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);      
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreatePrimaryKeyWithIndexAscendingTest()
    {
      mc.Create.Constraint("Pk").OnTable("t").OnColumns("id1", "id2")
        .AsPrimaryKey().UsingIndex("Idx", FbSorting.Ascending);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintPrimaryKey));
      var c = mc.DbObjects.Last() as ConstraintPrimaryKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Pk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.IsNotNull(c.Index);
      Assert.AreEqual(DbAction.Create, c.Index.Action);
      Assert.AreEqual("Idx", c.Index.Name);
      Assert.AreEqual(FbSorting.Ascending, c.Index.Sorting);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreatePrimaryKeyWithIndexDescendingTest()
    {
      mc.Create.Constraint("Pk").OnTable("t").OnColumns("id1", "id2")
        .AsPrimaryKey().UsingIndex("PkIdx", FbSorting.Descending);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintPrimaryKey));
      var c = mc.DbObjects.Last() as ConstraintPrimaryKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Pk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.IsNotNull(c.Index);
      Assert.AreEqual(DbAction.Create, c.Index.Action);
      Assert.AreEqual("PkIdx", c.Index.Name);
      Assert.AreEqual(FbSorting.Descending, c.Index.Sorting);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateUniqueTest()
    {
      mc.Create.Constraint("Unq").OnTable("t").OnColumns("id1", "id2")
        .AsUnique();

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintUnique));
      var c = mc.DbObjects.Last() as ConstraintUnique;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Unq", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);      
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateUniqueWithAscIndexTest()
    {
      mc.Create.Constraint("Unq").OnTable("t").OnColumns("id1", "id2")
        .AsUnique().UsingIndex("Idx", FbSorting.Ascending);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintUnique));
      var c = mc.DbObjects.Last() as ConstraintUnique;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Unq", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.IsNotNull(c.Index);
      Assert.AreEqual(DbAction.Create, c.Index.Action);
      Assert.AreEqual("Idx", c.Index.Name);
      Assert.AreEqual(FbSorting.Ascending, c.Index.Sorting);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateUniqueDescIndexTest()
    {
      mc.Create.Constraint("Unq").OnTable("t").OnColumns("id1", "id2")
        .AsUnique().UsingIndex("Idx", FbSorting.Descending);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintUnique));
      var c = mc.DbObjects.Last() as ConstraintUnique;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Unq", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.IsNotNull(c.Index);
      Assert.AreEqual(DbAction.Create, c.Index.Action);
      Assert.AreEqual("Idx", c.Index.Name);
      Assert.AreEqual(FbSorting.Descending, c.Index.Sorting);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateCheckTest()
    {
      mc.Create.Constraint("Chk").OnTable("t").AsCheck("expr");

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintCheck));
      var c = mc.DbObjects.Last() as ConstraintCheck;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Chk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("expr", c.CheckExpression);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateForeignKeyTest()
    {
      mc.Create.Constraint("Fk").OnTable("t").OnColumns("id1", "id2")
        .AsForeignKey()
        .WithReferenceTable("rt")
        .OnColumns("rid1", "rid2");

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintForeignKey));
      var c = mc.DbObjects.Last() as ConstraintForeignKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Fk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.AreEqual("rt", c.ReferenceTable);
      Assert.AreEqual(2, c.ReferenceColumns.Length);
      Assert.AreEqual("rid1", c.ReferenceColumns[0]);
      Assert.AreEqual("rid2", c.ReferenceColumns[1]);
      Assert.IsNull(c.UpdateRule);
      Assert.IsNull(c.DeleteRule);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateForeignKeyWithDeleteRuleTest()
    {
      mc.Create.Constraint("Fk").OnTable("t").OnColumns("id1", "id2")
        .AsForeignKey()
        .WithReferenceTable("rt")
        .OnColumns("rid1", "rid2")
        .HasDeleteRule.Cascade();

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintForeignKey));
      var c = mc.DbObjects.Last() as ConstraintForeignKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Fk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.AreEqual("rt", c.ReferenceTable);
      Assert.AreEqual(2, c.ReferenceColumns.Length);
      Assert.AreEqual("rid1", c.ReferenceColumns[0]);
      Assert.AreEqual("rid2", c.ReferenceColumns[1]);
      Assert.IsNull(c.UpdateRule);
      Assert.AreEqual(FbForeignKeyRules.Cascade, c.DeleteRule);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateForeignKeyWithUpdateRuleTest()
    {
      mc.Create.Constraint("Fk").OnTable("t").OnColumns("id1", "id2")
        .AsForeignKey()
        .WithReferenceTable("rt")
        .OnColumns("rid1", "rid2")
        .HasUpdateRule.Cascade();

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintForeignKey));
      var c = mc.DbObjects.Last() as ConstraintForeignKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Fk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.AreEqual("rt", c.ReferenceTable);
      Assert.AreEqual(2, c.ReferenceColumns.Length);
      Assert.AreEqual("rid1", c.ReferenceColumns[0]);
      Assert.AreEqual("rid2", c.ReferenceColumns[1]);
      Assert.IsNull(c.DeleteRule);
      Assert.AreEqual(FbForeignKeyRules.Cascade, c.UpdateRule);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateForeignKeyAscIndexTest()
    {
      mc.Create.Constraint("Fk").OnTable("t").OnColumns("id1", "id2")
        .AsForeignKey()
        .WithReferenceTable("rt")
        .OnColumns("rid1", "rid2")
        .UsingIndex("Idx", FbSorting.Ascending);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintForeignKey));
      var c = mc.DbObjects.Last() as ConstraintForeignKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Fk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.AreEqual("rt", c.ReferenceTable);
      Assert.AreEqual(2, c.ReferenceColumns.Length);
      Assert.AreEqual("rid1", c.ReferenceColumns[0]);
      Assert.AreEqual("rid2", c.ReferenceColumns[1]);
      Assert.IsNull(c.UpdateRule);
      Assert.IsNull(c.DeleteRule);
      Assert.IsNotNull(c.Index);
      Assert.AreEqual(DbAction.Create, c.Index.Action);
      Assert.AreEqual("Idx", c.Index.Name);
      Assert.AreEqual(FbSorting.Ascending, c.Index.Sorting);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxCreateForeignKeyDescIndexTest()
    {
      mc.Create.Constraint("Fk").OnTable("t").OnColumns("id1", "id2")
        .AsForeignKey()
        .WithReferenceTable("rt")
        .OnColumns("rid1", "rid2")
        .UsingIndex("Idx", FbSorting.Descending);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintForeignKey));
      var c = mc.DbObjects.Last() as ConstraintForeignKey;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("Fk", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual(2, c.Columns.Length);
      Assert.AreEqual("id1", c.Columns[0]);
      Assert.AreEqual("id2", c.Columns[1]);
      Assert.AreEqual("rt", c.ReferenceTable);
      Assert.AreEqual(2, c.ReferenceColumns.Length);
      Assert.AreEqual("rid1", c.ReferenceColumns[0]);
      Assert.AreEqual("rid2", c.ReferenceColumns[1]);
      Assert.IsNull(c.UpdateRule);
      Assert.IsNull(c.DeleteRule);
      Assert.IsNotNull(c.Index);
      Assert.AreEqual(DbAction.Create, c.Index.Action);
      Assert.AreEqual("Idx", c.Index.Name);
      Assert.AreEqual(FbSorting.Descending, c.Index.Sorting);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintSyntaxDropTest()
    {
      mc.Drop.Constraint("Fk", "t");

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(Constraint));
      var c = mc.DbObjects.Last() as Constraint;
      Assert.AreEqual(DbAction.Drop, c.Action);
      Assert.AreEqual("Fk", c.Name);
      Assert.AreEqual("t", c.TableName);
    }
  }
}
