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
  public class ViewQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<View, ViewQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void ViewQueryBuilderCreateTest()
    {
      mc.Create.View("v").HasResultColumns("r1", "r2").HasQuery("view text");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE OR ALTER VIEW \"v\" (\"r1\", \"r2\") AS view text;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ViewQueryBuilderCreateThrowsExceptionResultColumnsIsNullTest()
    {
      mc.Create.View("v").HasQuery("q");
      var qb = mc.DbObjects.Last();      
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);      
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ViewQueryBuilderCreateThrowsExceptionWhenViewQueryIsNullTest()
    {
      mc.Create.View("v").HasResultColumns("r1");
      var qb = mc.DbObjects.Last();
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
    }

    [TestMethod, TestCategory("Unit")]
    public void ProcedureQueryBuilderDropTest()
    {
      mc.Drop.View("v");
      var qb = mc.DbObjects.Last();
      string expected = "DROP VIEW \"v\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ViewQueryBuilderAlterTest()
    {
      mc.Alter.View("v").HasResultColumns("r1", "r2").HasQuery("view text");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE OR ALTER VIEW \"v\" (\"r1\", \"r2\") AS view text;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
  }
}
