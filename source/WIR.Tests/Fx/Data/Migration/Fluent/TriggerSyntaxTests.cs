using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Fluent;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class TriggerSyntaxTestsSyntaxTests
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
    public void TriggerSyntaxCreateAfterInsertActiveInPos1Test()
    {
      mc.Create.Trigger("tr").OnTable("t").After.Insert()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsActive()
        .OnPosition(1);
        
            
      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.After, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert, t.TriggerAction);
      Assert.IsTrue(t.IsActive);
      Assert.AreEqual(1, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxCreateAfterInsertInActiveInPos0Test()
    {
      mc.Create.Trigger("tr").OnTable("t").After.Insert()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsInactive()
        .OnPosition(0);


      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.After, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert, t.TriggerAction);
      Assert.IsFalse(t.IsActive);
      Assert.AreEqual(0, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxCreateBeforeInsertInActiveInPos0Test()
    {
      mc.Create.Trigger("tr").OnTable("t").Before.Insert()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsInactive()
        .OnPosition(0);


      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.Before, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert, t.TriggerAction);
      Assert.IsFalse(t.IsActive);
      Assert.AreEqual(0, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxCreateAfterMultipleAction1Test()
    {
      mc.Create.Trigger("tr").OnTable("t").After.Insert().Update().Delete()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsInactive()
        .OnPosition(0);


      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.After, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert | TriggerAction.Update | TriggerAction.Delete, t.TriggerAction);
      Assert.IsFalse(t.IsActive);
      Assert.AreEqual(0, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxCreateAfterMultipleAction2Test()
    {
      mc.Create.Trigger("tr").OnTable("t").After.Insert().Delete()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsInactive()
        .OnPosition(0);


      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.After, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert | TriggerAction.Delete, t.TriggerAction);
      Assert.IsFalse(t.IsActive);
      Assert.AreEqual(0, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxCreateBeforerMultipleAction1Test()
    {
      mc.Create.Trigger("tr").OnTable("t").Before.Insert().Update().Delete()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsInactive()
        .OnPosition(0);


      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Create, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.Before, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert | TriggerAction.Update | TriggerAction.Delete, t.TriggerAction);
      Assert.IsFalse(t.IsActive);
      Assert.AreEqual(0, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxAlterTest()
    {
      mc.Alter.Trigger("tr").OnTable("t").After.Insert().Delete()
        .HasDescription("description")
        .HasTriggerText("trigger text")
        .IsInactive()
        .OnPosition(0);


      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Alter, t.Action);
      Assert.AreEqual("tr", t.Name);
      Assert.AreEqual("t", t.TableName);
      Assert.AreEqual("description", t.Description);
      Assert.AreEqual(TriggerOrder.After, t.TriggerOrder);
      Assert.AreEqual(TriggerAction.Insert | TriggerAction.Delete, t.TriggerAction);
      Assert.IsFalse(t.IsActive);
      Assert.AreEqual(0, t.Position);
      Assert.AreEqual("trigger text", t.TriggerText);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerSyntaxDropTest()
    {
      mc.Drop.Trigger("tr");

      var t = mc.DbObjects.Last() as Trigger;
      Assert.AreEqual(DbAction.Drop, t.Action);
      Assert.AreEqual("tr", t.Name);      
    }
  }
}
