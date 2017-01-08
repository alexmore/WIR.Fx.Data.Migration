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
  public class TabelSyntaxTests
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
    public void TableSyntaxCreateTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int").HasColumnDescription("desc")
          .HasDefault("5");
          

      var t = mc.DbObjects.Last() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);      
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("5", c.Default);
      Assert.AreEqual("desc", c.Description);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateComputedColumnTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").ComputedBy("expr");

      var t = mc.DbObjects.Last() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("expr", c.ComputedBy);
      Assert.IsNull(c.NotNull);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateTowColumnsTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int")
        .WithColumn("value").AsDomain("string");
          


      var t = mc.DbObjects.Last() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);      
      Assert.IsNull(c.NotNull);

      c = t.NewColumns[1];
      Assert.AreEqual("value", c.Name);
      Assert.AreEqual("string", c.DomainName);      
    }
   
    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateNotNullColumnTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int")
          .HasDefault("5").IsNotNull();


      var t = mc.DbObjects.Last() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("5", c.Default);
      Assert.IsTrue(c.NotNull.HasValue && c.NotNull.Value);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithPrimaryKeyTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int")
          .HasDefault("5")
          .AsPrimaryKey();


      var t = mc.DbObjects.First() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("5", c.Default);
      Assert.IsTrue(c.NotNull.HasValue && c.NotNull.Value);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintPrimaryKey));
      var co = mc.DbObjects.Last() as ConstraintPrimaryKey;
      Assert.AreEqual(DbAction.Create, co.Action);
      Assert.AreEqual("Pkt", co.Name);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithPrimaryKeyWithCustomNameTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int")
          .HasDefault("5")
          .AsPrimaryKey("Pktid");


      var t = mc.DbObjects.First() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("5", c.Default);
      Assert.IsTrue(c.NotNull.HasValue && c.NotNull.Value);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintPrimaryKey));
      var co = mc.DbObjects.Last() as ConstraintPrimaryKey;
      Assert.AreEqual(DbAction.Create, co.Action);
      Assert.AreEqual("Pktid", co.Name);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithUniqueTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int")
          .HasDefault("5")
          .AsUnique("Unqid");


      var t = mc.DbObjects.First() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("5", c.Default);
      Assert.IsNull(c.NotNull);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintUnique));
      var co = mc.DbObjects.Last() as ConstraintUnique;
      Assert.AreEqual(DbAction.Create, co.Action);
      Assert.AreEqual("Unqid", co.Name);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithForeignKeyTest()
    {
      mc.Create.Table("t").HasDescription("description")
        .WithColumn("id").AsDomain("int")
          .HasDefault("5")
          .AsForeignKey("FkId")
          .WithReferenceTable("rt")
          .OnColumn("rid")
          .HasDeleteRule.Cascade()
          .HasUpdateRule.SetDefault();
          
          


      var t = mc.DbObjects.First() as Table;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("description", t.Description);
      var c = t.NewColumns[0];
      Assert.AreEqual("id", c.Name);
      Assert.AreEqual("int", c.DomainName);
      Assert.AreEqual("5", c.Default);
      Assert.IsNull(c.NotNull);

      Assert.IsInstanceOfType(mc.DbObjects.Last(), typeof(ConstraintForeignKey));
      var co = mc.DbObjects.Last() as ConstraintForeignKey;
      Assert.AreEqual(DbAction.Create, co.Action);
      Assert.AreEqual("FkId", co.Name);
      Assert.AreEqual("rt", co.ReferenceTable);
      Assert.AreEqual("rid", co.ReferenceColumns[0]);
      Assert.AreEqual(FbForeignKeyRules.Cascade, co.DeleteRule);
      Assert.AreEqual(FbForeignKeyRules.SetDefault, co.UpdateRule);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithIndexedColumnTest()
    {
      mc.Create.Table("t").WithColumn("id").AsDomain("int")
        .WithIndex("idx");

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual("id", i.Columns[0]);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsTrue(i.IsActive);
      Assert.IsFalse(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithIndexedColumnDescendingTest()
    {
      mc.Create.Table("t").WithColumn("id").AsDomain("int")
        .WithIndex("idx").DescendingSorting();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual("id", i.Columns[0]);
      Assert.AreEqual(FbSorting.Descending, i.Sorting);
      Assert.IsTrue(i.IsActive);
      Assert.IsFalse(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithIndexedColumnUniqueTest()
    {
      mc.Create.Table("t").WithColumn("id").AsDomain("int")
        .WithIndex("idx").IsUnique();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual("id", i.Columns[0]);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsTrue(i.IsActive);
      Assert.IsTrue(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithIndexedColumnInactiveTest()
    {
      mc.Create.Table("t").WithColumn("id").AsDomain("int")
        .WithIndex("idx").Inactive();

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("t", i.TableName);
      Assert.AreEqual("id", i.Columns[0]);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsFalse(i.IsActive);
      Assert.IsFalse(i.IsUnique);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableSyntaxCreateWithIndexedColumnComputedTest()
    {
      mc.Create.Table("t").WithColumn("id").AsDomain("int")
        .WithComputedIndex("idx").HasExpression("expr");

      var i = mc.DbObjects.Last() as Index;
      Assert.AreEqual(DbAction.Create, i.Action);
      Assert.AreEqual("idx", i.Name);
      Assert.AreEqual("t", i.TableName);
      Assert.IsNull(i.Columns);
      Assert.AreEqual(FbSorting.Ascending, i.Sorting);
      Assert.IsTrue(i.IsActive);
      Assert.IsFalse(i.IsUnique);
      Assert.AreEqual("expr", i.Expression);
    }
  }
}
