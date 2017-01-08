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
  public class DomainQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Domain, DomainQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderCreateTest()
    {
      mc.Create.Domain("Domain").AsInteger();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE DOMAIN \"Domain\" AS INTEGER;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);      
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderCreateWithDescriptionTest()
    {
      mc.Create.Domain("Domain").AsInteger().HasDescription("descr");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE DOMAIN \"Domain\" AS INTEGER;\r\nCOMMENT ON DOMAIN \"Domain\" IS 'descr';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderCreateBlobTest()
    {
      mc.Create.Domain("Domain").AsBlob(FbBlobSubType.Binary).HasCharset(FbCharset.WIN1251);
      var qb = mc.DbObjects.Last();
      string expected = "CREATE DOMAIN \"Domain\" AS BLOB SUB_TYPE 0 SEGMENT SIZE 16384;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderAlterTypeTest()
    {
      mc.Alter.Domain("Domain").SetDefault("10");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER DOMAIN \"Domain\" SET DEFAULT 10;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderAlterCheckTest()
    {
      mc.Alter.Domain("Domain").SetCheck("check");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER DOMAIN \"Domain\" DROP CONSTRAINT; ALTER DOMAIN \"Domain\" ADD CHECK (check);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderAlterNameTest()
    {
      mc.Alter.Domain("Domain").SetNewName("NewDomain");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER DOMAIN \"Domain\" TO \"NewDomain\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderAlterDescriptionTest()
    {
      mc.Alter.Domain("Domain").SetDescription("descr");
      var qb = mc.DbObjects.Last();
      string expected = "COMMENT ON DOMAIN \"Domain\" IS 'descr';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }


    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderAlterFullTest()
    {
      mc.Alter.Domain("Domain").SetDefault("10").SetCheck("check")
        .SetNewName("NewDomain").SetDescription("descr");
      var qb = mc.DbObjects.Last();
      string expected = @"ALTER DOMAIN ""Domain"" SET DEFAULT 10;
ALTER DOMAIN ""Domain"" DROP CONSTRAINT; ALTER DOMAIN ""Domain"" ADD CHECK (check);
COMMENT ON DOMAIN ""Domain"" IS 'descr';
ALTER DOMAIN ""Domain"" TO ""NewDomain"";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainQueryBuilderDropTest()
    {
      mc.Drop.Domain("Domain");
      var qb = mc.DbObjects.Last();
      string expected = "DROP DOMAIN \"Domain\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
  }
}
