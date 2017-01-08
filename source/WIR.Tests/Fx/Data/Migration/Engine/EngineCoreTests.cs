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

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]  
  public class EngineCoreTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      _settings = new MigrationSettings(null) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
    }

    MigrationSettings _settings;
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void MigrationSettingsIOCTest()
    {
      MigrationSettings s = new MigrationSettings(null);
      s.RegisterQueryBuilder<Domain, DomainQueryBuilder>();
      Assert.IsInstanceOfType(s.CreateQueryBuilder(typeof(Domain)), typeof(DomainQueryBuilder));
    }

    [SqlMetadata("Test")]
    private class DbObjectTest : DbObject
    {
      public DbObjectTest()
        : base("TestObj", DbAction.NoAction)
      {
      }
    }
        
    private class DbObjectNoSqlMetadataTest : DbObject
    {
      public DbObjectNoSqlMetadataTest()
        : base("", DbAction.NoAction)
      {
      }
    }

    [TestMethod, TestCategory("Unit")]
    public void SqlMetadataExtensionGetSqlMetadataTest()
    {
      var a = new DbObjectTest().GetSqlMetadata();
      Assert.IsNotNull(a);
      Assert.AreEqual("Test", a.ObjectSqlName);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(NotImplementedException))]
    public void SqlMetadataExtensionGetSqlMetadataThrowsExceptionNoMetadataTest()
    {
      var a = new DbObjectNoSqlMetadataTest().GetSqlMetadata();      
    }

    [TestMethod, TestCategory("Unit")]
    public void SqlBuilderCoreCreateDescriptionQueryTest()
    {
      var o = new DbObjectTest() { Description = "descr" };
      QueryBuilderCore c = new QueryBuilderCore(_settings);
      var s = c.CreateDescriptionQuery(o);
      Assert.AreEqual("COMMENT ON Test \"TestObj\" IS 'descr';", s);
    }

    [TestMethod, TestCategory("Unit")]
    public void SqlBuilderCoreCreateDescriptionQueryOnColumnTest()
    {
      var o = new Column("col", DbAction.Create) { Description = "descr", TableName = "t" };
      QueryBuilderCore c = new QueryBuilderCore(_settings);
      var s = c.CreateDescriptionQuery(o);
      Assert.AreEqual("COMMENT ON COLUMN \"t\".\"col\" IS 'descr';", s);
    }

    [TestMethod, TestCategory("Unit")]
    public void SqlBuilderCoreCreateDescriptionQueryExplicitObjectNameTest()
    {
      var o = new DbObjectTest() { Description = "descr" };
      QueryBuilderCore c = new QueryBuilderCore(_settings);
      var s = c.CreateDescriptionQuery(o, "myName");
      Assert.AreEqual("COMMENT ON Test myName IS 'descr';", s);
    }
  }
}
