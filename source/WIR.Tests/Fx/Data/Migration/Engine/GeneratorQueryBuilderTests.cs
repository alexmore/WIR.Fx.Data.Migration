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
  public class GeneratorQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Generator, GeneratorQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void GeneratorQueryBuilderCreateTest()
    {
      mc.Create.Generator("GenName");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE SEQUENCE \"GenName\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorQueryBuilderCreateWithDescriptionTest()
    {
      mc.Create.Generator("GenName").HasDescription("d");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE SEQUENCE \"GenName\";\r\nCOMMENT ON SEQUENCE \"GenName\" IS 'd';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorQueryBuilderCreateWithRestartValueTest()
    {
      mc.Create.Generator("GenName").RestartWith(10);
      var qb = mc.DbObjects.Last();
      string expected = "CREATE SEQUENCE \"GenName\";\r\nALTER SEQUENCE \"GenName\" RESTART WITH 10;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorQueryBuilderAlterTest()
    {
      mc.Alter.Generator("GenName").RestartWith(10);
      var qb = mc.DbObjects.Last();
      string expected = "ALTER SEQUENCE \"GenName\" RESTART WITH 10;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorQueryBuilderAlterDescriptionTest()
    {
      mc.Alter.Generator("GenName").HasDescription("d");
      var qb = mc.DbObjects.Last();
      string expected = "COMMENT ON SEQUENCE \"GenName\" IS 'd';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorQueryBuilderDropTest()
    {
      mc.Drop.Generator("GenName");
      var qb = mc.DbObjects.Last();
      string expected = "DROP SEQUENCE \"GenName\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }   
  }
}
