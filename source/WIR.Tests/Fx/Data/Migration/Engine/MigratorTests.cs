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

using Moq;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class MigratorTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _queries = new List<SqlQuery>();

      _performer = new Mock<ISqlQueryPerformer>();
      _performer.Setup(x => x.Execute(It.IsAny<SqlQuery>()))
        .Callback<SqlQuery>((x) =>
        {
          _queries.Add(x);
        });

      mc = new MigrationContextMoq();
      _settings = MigrationSettings.GetDefaultSettings(_performer.Object);      
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;

    List<SqlQuery> _queries;
    Mock<ISqlQueryPerformer> _performer;
    #endregion

    #region Migration contexts    
    private class EmptyContext : MigrationContext
    {
      public EmptyContext(MigrationSettings s)
        : base(s)
      {
      }

      public override void PullUp()
      {
        
      }
    }

    [MigrationVersion(10)]
    private class Ordered1Context : MigrationContext
    {
      public Ordered1Context(MigrationSettings s)
        : base(s)
      {
      }

      public override void PullUp()
      {
        Create.Domain("D1").AsBigInt();

      }
    }

    [MigrationVersion(5)]
    private class Ordered2Context : MigrationContext
    {
      public Ordered2Context(MigrationSettings s)
        : base(s)
      {
      }

      public override void PullUp()
      {
        Create.Domain("D2").AsBigInt();
      }
    }

    [MigrationVersion(12)]
    private class CreateTableInsertContext : MigrationContext
    {
      public CreateTableInsertContext(MigrationSettings s)
        : base(s)
      {
      }

      public override void PullUp()
      {
        Create.Table("t")
          .WithColumn("c").AsDomain("int");
        Script.Insert.Into("t1").ToColumns("c").Values(1);
      }
    }   
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void MigratorVersionLogTableNotExistsTest()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(null);

      Migrator m = new Migrator(_settings);
      m.Migrate(new EmptyContext(_settings));
      Assert.AreEqual(3, _queries.Count);
      Assert.IsTrue(_queries[0].Query.StartsWith("CREATE DOMAIN"));
      Assert.IsTrue(_queries[1].Query.StartsWith("CREATE TABLE"));
      Assert.IsTrue(_queries[2].Query.StartsWith("ALTER TABLE "));
    }

    [TestMethod, TestCategory("Unit")]
    public void MigratorContextOrdering1Test()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(2);

      Migrator m = new Migrator(_settings);
      m.Migrate(this.GetType().Assembly, 11);
      Assert.AreEqual(4, _queries.Count); // Migrator adds 2 Script.Insert
      Assert.IsTrue(_queries[0].Query.StartsWith("CREATE DOMAIN \"D2\""));
      Assert.IsTrue(_queries[2].Query.StartsWith("CREATE DOMAIN \"D1\""));      
    }

    [TestMethod, TestCategory("Unit")]
    public void MigratorContextOrdering2Test()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(7);

      Migrator m = new Migrator(_settings);
      m.Migrate(this.GetType().Assembly, 11);
      Assert.AreEqual(2, _queries.Count); // Migrator adds one more Script.Insert
      Assert.IsTrue(_queries[0].Query.StartsWith("CREATE DOMAIN \"D1\""));
      
    }

    [TestMethod, TestCategory("Unit")]
    public void MigratorContextMigrateUptoVersion1Test()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(2);

      Migrator m = new Migrator(_settings);
      m.Migrate(this.GetType().Assembly, 7);
      Assert.AreEqual(2, _queries.Count); // Migrator adds one more Script.Insert
      Assert.IsTrue(_queries[0].Query.StartsWith("CREATE DOMAIN \"D2\""));
    }

    [TestMethod, TestCategory("Unit")]
    public void MigratorContextMigrateUptoVersion2Test()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(2);

      Migrator m = new Migrator(_settings);
      m.Migrate(this.GetType().Assembly, 11);
      Assert.AreEqual(4, _queries.Count); // Migrator adds 2 Script.Inserts
      Assert.IsTrue(_queries[0].Query.StartsWith("CREATE DOMAIN \"D2\""));
      Assert.IsTrue(_queries[2].Query.StartsWith("CREATE DOMAIN \"D1\""));
    }

    [TestMethod, TestCategory("Unit")]
    public void MigratorBeginCommitOrderingTest()
    {
      int begins = 0;
      int commits = 0;

      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(2);
      _performer.Setup(x => x.BeginTransaction()).Callback(() => begins++);
      _performer.Setup(x => x.CommitTransaction()).Callback(() => 
        commits++);

      Migrator m = new Migrator(_settings);
      m.Migrate(this.GetType().Assembly, 15);
      Assert.AreEqual(5, commits);
      Assert.AreEqual(5, begins);

    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(Exception))]
    public void MigratorFailsWhenSqlQueryExecuteExceptionTest()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(7);
      _performer.Setup(x => x.Execute(It.IsAny<SqlQuery>())).Throws(new InvalidOperationException("inner"));

      Migrator m = new Migrator(_settings);
      m.Migrate(this.GetType().Assembly);
    }

    [TestMethod, TestCategory("Unit")]    
    public void MigratorFailsWhenCreateTableAndInsertInTheSameContextTest()
    {
      _performer.Setup(x => x.ExecuteScalar(It.IsAny<SqlQuery>()))
        .Returns(11);      

      Migrator m = new Migrator(_settings);
      try
      {
        m.Migrate(this.GetType().Assembly, 12);
      }
      catch (Exception e)
      {
        if (e.InnerException == null 
          || e.InnerException.GetType() != typeof(InvalidOperationException)
          || !e.InnerException.Message.StartsWith("CREATE TABLE and INSERT")
          )
          Assert.Fail("CREATE TABLE and INSERT checking failed.");

      }
    }

    [TestMethod, TestCategory("Unit")]
    public void MigratorAddsContextVersionToVersionLogTableTest()
    {      
      Migrator m = new Migrator(_settings);

      var c = new Ordered2Context(_settings);
      m.Migrate(c);
      
      var actual = c.DbObjects.Where(x => x.GetType() == typeof(Script) &&
        (x as Script).Type == ScriptType.InsertQuery &&
        (x as Script).TableName == _settings.MigrationLogTableName).Select(x => (Script)x).FirstOrDefault();

      Assert.AreEqual((Int64)5, actual.Parameters[_settings.MigrationLogColumnName]);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void MigrationVerionAttributeFailsWhenVersionIsEqualsTo0Test()
    {
      var mv = new MigrationVersionAttribute(0);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void MigrationVerionAttributeFailsWhenVersionIsLessTo0Test()
    {
      var mv = new MigrationVersionAttribute(-1);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void MigrationVerionAttributeFailsWhenVersionIsEqualsTo0WithStringArgTest()
    {
      var mv = new MigrationVersionAttribute("0");
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void MigrationVerionAttributeFailsWhenVersionIsLessTo0WithStringArgTest()
    {
      var mv = new MigrationVersionAttribute("-1");
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void MigrationVerionAttributeFailsWhenStringArgumentIsNotValidTest()
    {
      var mv = new MigrationVersionAttribute("sdf");
    }

  }
}
