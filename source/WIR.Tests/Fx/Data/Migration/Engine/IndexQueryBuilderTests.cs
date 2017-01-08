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
  public class IndexQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Index, IndexQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void IndexQueryBuilderCreateFailsWhenTableNameIsEmptyTest()
    {
      mc.Create.Index("idx");
      var qb = mc.DbObjects.Last();      
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);      
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void IndexQueryBuilderCreateFailsWhenNoExpressionAndColumnTest()
    {
      mc.Create.Index("idx").OnTable("t");
      var qb = mc.DbObjects.Last();
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void IndexQueryBuilderCreateFailsWhenIndexHasColumnsAndExpressionTest()
    {
      var i = new Index("idx", DbAction.Create) { TableName = "t", Expression = "expr", Columns = new [] { "c1" } };
      var qb = mc.DbObjects.Last();
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateTest()
    {
      mc.Create.Index("idx").OnTable("t").OnColumns("c1", "c2");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE INDEX \"idx\" ON \"t\" (\"c1\", \"c2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }


    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateUniqueTest()
    {
      mc.Create.Index("idx").OnTable("t").OnColumns("c1", "c2").IsUnique();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE UNIQUE INDEX \"idx\" ON \"t\" (\"c1\", \"c2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateDescendingTest()
    {
      mc.Create.Index("idx").OnTable("t").OnColumns("c1", "c2").IsUnique();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE UNIQUE INDEX \"idx\" ON \"t\" (\"c1\", \"c2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateDescendingUniqueTest()
    {
      mc.Create.Index("idx").OnTable("t").OnColumns("c1", "c2").IsUnique().DescendingSorting();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE UNIQUE DESCENDING INDEX \"idx\" ON \"t\" (\"c1\", \"c2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateComputedByTest()
    {
      mc.Create.Index("idx").OnTable("t").ComputedBy("expr");
      var qb = mc.DbObjects.Last();
      string expected = "CREATE INDEX \"idx\" ON \"t\" COMPUTED BY (expr);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateComputedByUniqueTest()
    {
      mc.Create.Index("idx").OnTable("t").ComputedBy("expr").IsUnique();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE UNIQUE INDEX \"idx\" ON \"t\" COMPUTED BY (expr);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
       
    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateDescComputedByUniqueTest()
    {
      mc.Create.Index("idx").OnTable("t").ComputedBy("expr").IsUnique().DescendingSorting();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE UNIQUE DESCENDING INDEX \"idx\" ON \"t\" COMPUTED BY (expr);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderCreateDescComputedByTest()
    {
      mc.Create.Index("idx").OnTable("t").ComputedBy("expr").DescendingSorting();
      var qb = mc.DbObjects.Last();
      string expected = "CREATE DESCENDING INDEX \"idx\" ON \"t\" COMPUTED BY (expr);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderDropTest()
    {
      mc.Drop.Index("idx");
      var qb = mc.DbObjects.Last();
      string expected = "DROP INDEX \"idx\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderAlterInactiveTest()
    {
      mc.Alter.Index("idx").SetInactive();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER INDEX \"idx\" INACTIVE;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void IndexQueryBuilderAlterActiveTest()
    {
      mc.Alter.Index("idx").SetActive();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER INDEX \"idx\" ACTIVE;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
  }
}
