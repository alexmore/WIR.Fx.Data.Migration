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
  public class ColumnQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Column, ColumnQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ColumnQueryBuilderCreateFailsWhenTableNameEmptyTest()
    {
      mc.Create.Column("Col");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE SEQUENCE \"GenName\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      //Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]    
    public void ColumnQueryBuilderCreateTest()
    {
      mc.Create.Column("c").OnTable("t").AsDomain("d");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" \"d\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderCreateWithDescriptionTest()
    {
      mc.Create.Column("c").OnTable("t").AsDomain("d").HasDescription("desc");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" \"d\";\r\nCOMMENT ON COLUMN \"t\".\"c\" IS 'desc';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderCreateWithDefaultTest()
    {
      mc.Create.Column("c").OnTable("t").AsDomain("d").HasDefault("0");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" \"d\" DEFAULT 0;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderCreateWithDefaultNotNullTest()
    {
      mc.Create.Column("c").OnTable("t").AsDomain("d").HasDefault("0").IsNotNull();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" \"d\" DEFAULT 0 NOT NULL;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderCreateWithDefaultNotNullCheckTest()
    {
      mc.Create.Column("c").OnTable("t").AsDomain("d").HasDefault("0").IsNotNull().HasCheck("check");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" \"d\" DEFAULT 0 NOT NULL CHECK (check);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderCreateComputedTest()
    {
      mc.Create.Column("c").OnTable("t").AsComputed("comp");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" COMPUTED BY (comp);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderCreateComputedWithDescriptionTest()
    {
      mc.Create.Column("c").OnTable("t").AsComputed("comp").HasDescription("desc");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD \"c\" COMPUTED BY (comp);\r\nCOMMENT ON COLUMN \"t\".\"c\" IS 'desc';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderDropTest()
    {
      mc.Drop.Column("c", "t");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" DROP \"c\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ColumnQueryBuilderDropFailsWhenNoTableTest()
    {
      mc.Drop.Column("c", null);
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" DROP \"c\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderAlterNewNameTest()
    {
      mc.Alter.Column("c").OnTable("t").SetNewName("c1");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ALTER \"c\" TO \"c1\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderAlterDefaultTest()
    {
      mc.Alter.Column("c").OnTable("t").HasDefault("0");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ALTER COLUMN \"c\" SET DEFAULT 0;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderAlterDomainTest()
    {
      mc.Alter.Column("c").OnTable("t").AsDomain("nd");
      var qb = mc.DbObjects.Last();
      string expected = "update RDB$RELATION_FIELDS set RDB$FIELD_SOURCE = 'nd' where (RDB$FIELD_NAME = 'c') and (RDB$RELATION_NAME = 't');";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderAlterNotNullTest()
    {
      mc.Alter.Column("c").OnTable("t").IsNotNull();
      var qb = mc.DbObjects.Last();
      string expected = "update RDB$RELATION_FIELDS set RDB$NULL_FLAG = 1 where (RDB$FIELD_NAME = 'c') and (RDB$RELATION_NAME = 't');";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderAlterNullablelTest()
    {
      mc.Alter.Column("c").OnTable("t").IsNullable();
      var qb = mc.DbObjects.Last();
      string expected = "update RDB$RELATION_FIELDS set RDB$NULL_FLAG = NULL where (RDB$FIELD_NAME = 'c') and (RDB$RELATION_NAME = 't');";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ColumnQueryBuilderAlterDescriptionTest()
    {
      mc.Alter.Column("c").OnTable("t").HasDescription("Hello");
      var qb = mc.DbObjects.Last();
      string expected = "COMMENT ON COLUMN \"t\".\"c\" IS 'Hello';";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

  }
}
