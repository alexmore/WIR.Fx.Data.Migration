using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WIR.Fx.Data.Migration;
using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Engine;
using WIR.Fx.Data.Migration.Engine.QueryBuilders;

using WIR.Tests.Fx.Data.Migration;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class TriggerQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Trigger, TriggerQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TriggerQueryBuilderCreateFailsWhenTableNameIsEmptyTest()
    {
      mc.Create.Trigger("tr");
      var qb = mc.DbObjects.Last();
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TriggerQueryBuilderCreateFailsWhenTriggerBodyeIsEmptyTest()
    {
      mc.Create.Trigger("tr").OnTable("t");
      var qb = mc.DbObjects.Last();
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderCreateBeforeInsertTest()
    {
      mc.Create.Trigger("tr").OnTable("t").Before.Insert().OnPosition(0).HasTriggerText("txt");
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER TRIGGER \"tr\" FOR \"t\" ACTIVE BEFORE INSERT POSITION 0 AS BEGIN txt END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderCreateBeforeInsertWithDescriptionTest()
    {
      mc.Create.Trigger("tr").OnTable("t").Before.Insert().OnPosition(0).HasTriggerText("txt")
        .HasDescription("desc");
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER TRIGGER \"tr\" FOR \"t\" ACTIVE BEFORE INSERT POSITION 0 AS BEGIN txt END\r\n^\r\nSET TERM ; ^\r\nCOMMENT ON TRIGGER \"tr\" IS 'desc';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderCreateBeforeInsertInactiveTest()
    {
      mc.Create.Trigger("tr").OnTable("t").Before.Insert().OnPosition(0).HasTriggerText("txt").IsInactive();
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER TRIGGER \"tr\" FOR \"t\" INACTIVE BEFORE INSERT POSITION 0 AS BEGIN txt END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderCreateAfterInsertUpdateInactiveTest()
    {
      mc.Create.Trigger("tr").OnTable("t").After.Insert().Update().OnPosition(2).HasTriggerText("txt").IsInactive();
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER TRIGGER \"tr\" FOR \"t\" INACTIVE AFTER INSERT OR UPDATE POSITION 2 AS BEGIN txt END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderCreateAfterInsertUpdateDeleteInactiveTest()
    {
      mc.Create.Trigger("tr").OnTable("t").After.Insert().Update().Delete().OnPosition(2).HasTriggerText("txt").IsInactive();
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER TRIGGER \"tr\" FOR \"t\" INACTIVE AFTER INSERT OR UPDATE OR DELETE POSITION 2 AS BEGIN txt END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderAlterTest()
    {
      mc.Alter.Trigger("tr").OnTable("t").Before.Insert().OnPosition(0).HasTriggerText("txt");
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER TRIGGER \"tr\" FOR \"t\" ACTIVE BEFORE INSERT POSITION 0 AS BEGIN txt END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TriggerQueryBuilderDropTest()
    {
      mc.Drop.Trigger("tr");
      var qb = mc.DbObjects.Last();
      string expected = "DROP TRIGGER \"tr\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

  }
}
