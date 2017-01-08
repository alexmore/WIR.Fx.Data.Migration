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
  public class TableQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Table, TableQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TableQueryBuilderCreateFailsWhenNoColumnsTest()
    {
      mc.Create.Table("t");
      var qb = mc.DbObjects.Last();      
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);      
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderCreateTest()
    {
      mc.Create.Table("t").WithColumn("c1").AsDomain("int")
        .WithColumn("c2").AsDomain("string");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE TABLE \"t\" (\"c1\" \"int\", \"c2\" \"string\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderCreateWithNotNullColumnTest()
    {
      mc.Create.Table("t").WithColumn("c1").AsDomain("int").IsNotNull()
        .WithColumn("c2").AsDomain("string");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE TABLE \"t\" (\"c1\" \"int\" NOT NULL, \"c2\" \"string\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderCreateWithDesfaultColumnTest()
    {
      mc.Create.Table("t").WithColumn("c1").AsDomain("int").HasDefault("def")
        .WithColumn("c2").AsDomain("string");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE TABLE \"t\" (\"c1\" \"int\" DEFAULT def, \"c2\" \"string\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderCreateWithDesfaultAndNotNullColumnTest()
    {
      mc.Create.Table("t").WithColumn("c1").AsDomain("int").HasDefault("def").IsNotNull()
        .WithColumn("c2").AsDomain("string");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE TABLE \"t\" (\"c1\" \"int\" DEFAULT def NOT NULL, \"c2\" \"string\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderCreateWithDescriptionTest()
    {
      mc.Create.Table("t").HasDescription("desc")
        .WithColumn("c1").AsDomain("int")
        .WithColumn("c2").AsDomain("string");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE TABLE \"t\" (\"c1\" \"int\", \"c2\" \"string\");\r\nCOMMENT ON TABLE \"t\" IS 'desc';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderCreateWithDescriptionAndColumnDescriptionTest()
    {
      mc.Create.Table("t").HasDescription("desc")
        .WithColumn("c1").AsDomain("int").HasColumnDescription("desc1")
        .WithColumn("c2").AsDomain("string");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE TABLE \"t\" (\"c1\" \"int\", \"c2\" \"string\");\r\nCOMMENT ON TABLE \"t\" IS 'desc';\r\nCOMMENT ON COLUMN \"t\".\"c1\" IS 'desc1';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void TableQueryBuilderAlterTest()
    {
      mc.Alter.Table("t").AddDescription("desc");
      var qb = mc.DbObjects.Last();
      string expected = "COMMENT ON TABLE \"t\" IS 'desc';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
  }
}
