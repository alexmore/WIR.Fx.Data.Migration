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
  public class ColumnSyntaxTests
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
    public void ColumnSyntaxCreateTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .HasDescription("description");
            
      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("description", c.Description);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateComputedByTest()
    {
      mc.Create.Column("col").OnTable("t").AsComputed("expr")
        .HasDescription("description");

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("expr", c.ComputedBy);
      Assert.AreEqual("description", c.Description);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithDefaultTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .HasDescription("description")
        .HasDefault("5");

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("description", c.Description);
      Assert.AreEqual("5", c.Default);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithCheckTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .HasDescription("description")
        .HasCheck("check");

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("description", c.Description);
      Assert.AreEqual("check", c.Check);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateNotNullTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .HasDescription("description")
        .IsNotNull();

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("description", c.Description);      
      Assert.IsTrue(c.NotNull.HasValue && c.NotNull.Value);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithIndexDefaultSortingTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int").WithIndex("idx");
      
      var c = mc.DbObjects.First() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("col", i.Columns[0]);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsFalse(i.IsUnique);
      Assert.IsTrue(i.IsActive);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithIndexDescendingSortingTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .WithIndex("idx").DescendingSorting();

      var c = mc.DbObjects.First() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("col", i.Columns[0]);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsTrue(i.IsActive);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithIndexUniqueTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .WithIndex("idx").DescendingSorting().IsUnique();

      var c = mc.DbObjects.First() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("col", i.Columns[0]);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsTrue(i.IsActive);
      Assert.IsTrue(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithIndexInactiveTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .WithIndex("idx").DescendingSorting().IsUnique().Inactive();

      var c = mc.DbObjects.First() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("col", i.Columns[0]);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsFalse(i.IsActive);
      Assert.IsTrue(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxCreateWithIndexComputedByTest()
    {
      mc.Create.Column("col").OnTable("t").AsDomain("int")
        .WithComputedIndex("idx").HasExpression("expr")
        .DescendingSorting().Inactive().IsUnique();

      var c = mc.DbObjects.First() as Column;
      Assert.AreEqual(DbAction.Create, c.Action);
      Assert.AreEqual("col", c.Name);

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.IsNull(i.Columns);
      Assert.AreEqual("expr", i.Expression);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsFalse(i.IsActive);
      Assert.IsTrue(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxAlterTest()
    {
      mc.Alter.Column("col").OnTable("t").SetNewName("newcol")
        .HasDescription("description").AsDomain("float")
        .HasDefault("78");

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Alter, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("float", c.DomainName);
      Assert.AreEqual("78", c.Default);
      Assert.AreEqual("description", c.Description);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxAlterNotNullTest()
    {
      mc.Alter.Column("col").OnTable("t").SetNewName("newcol")
        .HasDescription("description").AsDomain("float")
        .HasDefault("78").IsNotNull();

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Alter, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("float", c.DomainName);
      Assert.AreEqual("78", c.Default);
      Assert.AreEqual("description", c.Description);
      Assert.IsTrue(c.NotNull.HasValue && c.NotNull.Value);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxAlterNullableTest()
    {
      mc.Alter.Column("col").OnTable("t").SetNewName("newcol")
        .HasDescription("description").AsDomain("float")
        .HasDefault("78").IsNullable();

      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Alter, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);
      Assert.AreEqual("float", c.DomainName);
      Assert.AreEqual("78", c.Default);
      Assert.AreEqual("description", c.Description);
      Assert.IsTrue(c.NotNull.HasValue && !c.NotNull.Value);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnSyntaxDropTest()
    {
      mc.Drop.Column("col", "t");
      
      var c = mc.DbObjects.Last() as Column;
      Assert.AreEqual(DbAction.Drop, c.Action);
      Assert.AreEqual("col", c.Name);
      Assert.AreEqual("t", c.TableName);      
    }
  }
}
