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
  public class ScriptQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Script, ScriptQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void ScriptQueryBuilderTest()
    {
      mc.Script.ExecuteQuery("sql");
      var qb = mc.DbObjects.Last();
      string expected = "sql";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptQueryBuilderWithParametersTest()
    {
      mc.Script.ExecuteQuery("sql").HasParameter("p1",1).HasParameter("p2",2);
      var qb = mc.DbObjects.Last();
      string expected = "sql";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
      Assert.AreEqual(2, actual.Parameters.Count);
      Assert.AreEqual("p1", actual.Parameters.Keys.First());
      Assert.AreEqual("p2", actual.Parameters.Keys.Last());
      Assert.AreEqual(1, actual.Parameters.Values.First());
      Assert.AreEqual(2, actual.Parameters.Values.Last());
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptQueryBuilderInsertTest()
    {
      mc.Script.Insert.Into("t").ToColumns("c1", "c2").Values(1, 2);
      var qb = mc.DbObjects.Last();
      string expected = "INSERT INTO \"t\" (\"c1\", \"c2\") VALUES (@c1, @c2);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
  }
}
