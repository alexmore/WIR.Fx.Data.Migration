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
  public class ConstraintQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Constraint, ConstraintQueryBuilder>();
      _settings.RegisterQueryBuilder<ConstraintCheck, ConstraintQueryBuilder>();
      _settings.RegisterQueryBuilder<ConstraintForeignKey, ConstraintQueryBuilder>();
      _settings.RegisterQueryBuilder<ConstraintPrimaryKey, ConstraintQueryBuilder>();
      _settings.RegisterQueryBuilder<ConstraintUnique, ConstraintQueryBuilder>();      
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;
    #endregion

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ConstraintQueryBuilderCreateFailsWhenTableNameEmptyTest()
    {
      mc.Create.Constraint("c");
      var qb = mc.DbObjects.Last();      
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);      
    }

    [TestMethod, TestCategory("Unit")]    
    public void ConstraintQueryBuilderPKCreateTest()
    {
      mc.Create.Constraint("pk").OnTable("t").OnColumns("c1").AsPrimaryKey();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"pk\" PRIMARY KEY (\"c1\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderPKCreateManyColumnsTest()
    {
      mc.Create.Constraint("pk").OnTable("t").OnColumns("c1", "c2").AsPrimaryKey();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"pk\" PRIMARY KEY (\"c1\", \"c2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderPKCreateManyColumnsUsingIndexTest()
    {
      mc.Create.Constraint("pk").OnTable("t").OnColumns("c1", "c2").AsPrimaryKey().UsingIndex("pkidx", FbSorting.Descending);
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"pk\" PRIMARY KEY (\"c1\", \"c2\") USING DESCENDING INDEX \"pkidx\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderDropTest()
    {
      mc.Drop.Constraint("pk", "t");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" DROP CONSTRAINT \"pk\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderFKCreateTest()
    {
      mc.Create.Constraint("fk").OnTable("t").OnColumns("c1", "c2")
        .AsForeignKey().WithReferenceTable("rt").OnColumns("rc1", "rc2");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"fk\" FOREIGN KEY (\"c1\", \"c2\") REFERENCES \"rt\"(\"rc1\", \"rc2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderFKCreateUsingIndexTest()
    {
      mc.Create.Constraint("fk").OnTable("t").OnColumns("c1", "c2")
        .AsForeignKey().WithReferenceTable("rt").OnColumns("rc1", "rc2")
        .UsingIndex("idx", FbSorting.Descending);
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"fk\" FOREIGN KEY (\"c1\", \"c2\") REFERENCES \"rt\"(\"rc1\", \"rc2\") USING DESCENDING INDEX \"idx\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderFKCreateWithUpdateRuleTest()
    {
      mc.Create.Constraint("fk").OnTable("t").OnColumns("c1", "c2")
        .AsForeignKey().WithReferenceTable("rt").OnColumns("rc1", "rc2")
        .HasUpdateRule.Cascade();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"fk\" FOREIGN KEY (\"c1\", \"c2\") REFERENCES \"rt\"(\"rc1\", \"rc2\") ON UPDATE CASCADE;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderFKCreateWithDeleteRuleTest()
    {
      mc.Create.Constraint("fk").OnTable("t").OnColumns("c1", "c2")
        .AsForeignKey().WithReferenceTable("rt").OnColumns("rc1", "rc2")
        .HasDeleteRule.Cascade();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"fk\" FOREIGN KEY (\"c1\", \"c2\") REFERENCES \"rt\"(\"rc1\", \"rc2\") ON DELETE CASCADE;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderFKCreateWithDeleteUpdateRuleTest()
    {
      mc.Create.Constraint("fk").OnTable("t").OnColumns("c1", "c2")
        .AsForeignKey().WithReferenceTable("rt").OnColumns("rc1", "rc2")
        .HasDeleteRule.Cascade()
        .HasUpdateRule.SetNull();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"fk\" FOREIGN KEY (\"c1\", \"c2\") REFERENCES \"rt\"(\"rc1\", \"rc2\") ON DELETE CASCADE ON UPDATE SET NULL;";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ConstraintQueryBuilderFKFailsWhenReferenceTableIsNullTest()
    {
      mc.Create.Constraint("fk").OnTable("t").OnColumns("c1", "c2")
        .AsForeignKey();
      var qb = mc.DbObjects.Last();      
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);      
    }


    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderCheckCreateTest()
    {
      mc.Create.Constraint("ch").OnTable("t").AsCheck("check");
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"ch\" CHECK (check);";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }
        
    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderUniqueCreateTest()
    {
      mc.Create.Constraint("u").OnTable("t").OnColumns("c1", "c2").AsUnique();
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"u\" UNIQUE (\"c1\", \"c2\");";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ConstraintQueryBuilderUniqueCreateUsingIndexTest()
    {
      mc.Create.Constraint("u").OnTable("t").OnColumns("c1", "c2").AsUnique().UsingIndex("idx", FbSorting.Descending);
      var qb = mc.DbObjects.Last();
      string expected = "ALTER TABLE \"t\" ADD CONSTRAINT \"u\" UNIQUE (\"c1\", \"c2\") USING DESCENDING INDEX \"idx\";";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

  }
}
