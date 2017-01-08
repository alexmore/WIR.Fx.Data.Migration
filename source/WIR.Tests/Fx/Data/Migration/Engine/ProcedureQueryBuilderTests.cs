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
  public class ProcedureQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Procedure, ProcedureQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void ProcedureQueryBuilderCreateTest()
    {
      mc.Create.Procedure("pr").WithProcedureText("(\"ip1\" INTEGER, \"ip2\" INTEGER) RETURNS (\"op1\" INTEGER, \"op2\" INTEGER) AS declare variable NEW_VAR integer; BEGIN END");
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER PROCEDURE \"pr\" (\"ip1\" INTEGER, \"ip2\" INTEGER) RETURNS (\"op1\" INTEGER, \"op2\" INTEGER) AS declare variable NEW_VAR integer; BEGIN END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ProcedureQueryBuilderCreateThrowsExceptionWhenProcedureTextIsEmptyTest()
    {
      mc.Create.Procedure("pr");
      var qb = mc.DbObjects.Last();      
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);      
    }

    [TestMethod, TestCategory("Unit")]
    public void ProcedureQueryBuilderCreateWithDescriptionTest()
    {
      mc.Create.Procedure("pr").WithProcedureText("(\"ip1\" INTEGER, \"ip2\" INTEGER) RETURNS (\"op1\" INTEGER, \"op2\" INTEGER) AS declare variable NEW_VAR integer; BEGIN END")
        .HasDescription("Hello");
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER PROCEDURE \"pr\" (\"ip1\" INTEGER, \"ip2\" INTEGER) RETURNS (\"op1\" INTEGER, \"op2\" INTEGER) AS declare variable NEW_VAR integer; BEGIN END\r\n^\r\nSET TERM ; ^\r\nCOMMENT ON PROCEDURE \"pr\" IS 'Hello';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ProcedureQueryBuilderAlterTest()
    {
      mc.Alter.Procedure("pr").WithProcedureText("(\"ip1\" INTEGER, \"ip2\" INTEGER) RETURNS (\"op1\" INTEGER, \"op2\" INTEGER) AS declare variable NEW_VAR integer; BEGIN END");
      var qb = mc.DbObjects.Last();
      string expected = "SET TERM ^ ;\r\nCREATE OR ALTER PROCEDURE \"pr\" (\"ip1\" INTEGER, \"ip2\" INTEGER) RETURNS (\"op1\" INTEGER, \"op2\" INTEGER) AS declare variable NEW_VAR integer; BEGIN END\r\n^\r\nSET TERM ; ^";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ProcedureQueryBuilderDropTest()
    {
      mc.Drop.Procedure("pr");
      var qb = mc.DbObjects.Last();
      string expected = "DROP PROCEDURE \"pr\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
  }
}
