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
  public class IndexSyntaxTests
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
    public void IndexSyntaxCreateTest()
    {
      mc.Create.Index("idx").OnTable("tbl").OnColumns("col1", "col2");

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);      
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("tbl", i.TableName);
      Assert.AreEqual(2, i.Columns.Length);
      Assert.AreEqual("col1", i.Columns[0]);
      Assert.AreEqual("col2", i.Columns[1]);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxCreateComputedTest()
    {
      mc.Create.Index("idx").OnTable("tbl").ComputedBy("expression");

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("tbl", i.TableName);      
      Assert.AreEqual("expression", i.Expression);      
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxCreateActiveAscendingTest()
    {
      mc.Create.Index("idx").OnTable("tbl").OnColumns("col1", "col2")
        .AscendingSorting().Active();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("tbl", i.TableName);
      Assert.AreEqual(2, i.Columns.Length);
      Assert.AreEqual("col1", i.Columns[0]);
      Assert.AreEqual("col2", i.Columns[1]);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsTrue(i.IsActive);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxCreateInActiveAscendingTest()
    {
      mc.Create.Index("idx").OnTable("tbl").OnColumns("col1", "col2")
        .AscendingSorting().Inactive();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("tbl", i.TableName);
      Assert.AreEqual(2, i.Columns.Length);
      Assert.AreEqual("col1", i.Columns[0]);
      Assert.AreEqual("col2", i.Columns[1]);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsFalse(i.IsActive);
    }
    
    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxCreateInActiveDescendingTest()
    {
      mc.Create.Index("idx").OnTable("tbl").OnColumns("col1", "col2")
        .DescendingSorting().Inactive();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("tbl", i.TableName);
      Assert.AreEqual(2, i.Columns.Length);
      Assert.AreEqual("col1", i.Columns[0]);
      Assert.AreEqual("col2", i.Columns[1]);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsFalse(i.IsActive);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxCreateInActiveDescendingUniqueTest()
    {
      mc.Create.Index("idx").OnTable("tbl").OnColumns("col1", "col2")
        .DescendingSorting().Inactive().IsUnique();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("tbl", i.TableName);
      Assert.AreEqual(2, i.Columns.Length);
      Assert.AreEqual("col1", i.Columns[0]);
      Assert.AreEqual("col2", i.Columns[1]);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsFalse(i.IsActive);
      Assert.IsTrue(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxAlterSetActiveTest()
    {
      mc.Alter.Index("idx").SetActive();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Alter, i.Action);
      Assert.AreEqual("idx", i.Name);      
      Assert.IsTrue(i.IsActive);      
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxAlterSetInactiveTest()
    {
      mc.Alter.Index("idx").SetInactive();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Alter, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.IsFalse(i.IsActive);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexSyntaxDropTest()
    {
      mc.Drop.Index("idx");

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Drop, i.Action);
      Assert.AreEqual("idx", i.Name);     
    }
  }
}
