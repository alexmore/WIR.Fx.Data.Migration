using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

using Moq;

using WIR.Fx.Data.Migration;
using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Engine;
using WIR.Fx.Data.Migration.Engine.QueryBuilders;

using WIR.Tests.Fx.Data.Migration;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class ScriptSuspendConstraintsQueryBuilderTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      SetUpMocks();

      _settings = new MigrationSettings(_qPerformer.Object) { DbObjectsNameFormat = FbNameFormat.Safe, ScriptTerminationSymbol = ";" };
      _settings.RegisterQueryBuilder<Script, ScriptQueryBuilder>();
      mc = new MigrationContextMoq();
    }

    MigrationSettings _settings;
    MigrationContextMoq mc;

    Mock<ISqlQueryPerformer> _qPerformer;
    Mock<IDataReader> _drTrigger;
    Mock<IDataReader> _drFk;
    Mock<IDataReader> _drFkDetails;
    Mock<IDataReader> _drCheck;

    #region Sql queries
    #region Sql queries
    string _suspendConstraintsTriggersSql =
       "SELECT TRIM(RDB$TRIGGER_NAME) " +
       "FROM RDB$TRIGGERS " +
       "WHERE " +
       "((RDB$SYSTEM_FLAG = 0) OR (RDB$SYSTEM_FLAG is null)) AND " +
       "(RDB$TRIGGER_INACTIVE = 0)";

    string _suspendConstraintsFkListSql =
      "SELECT DISTINCT TRIM(A.RDB$CONSTRAINT_NAME), TRIM(A.RDB$RELATION_NAME) " +
      "FROM RDB$RELATION_CONSTRAINTS A " +
      "WHERE A.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'";

    string _suspendConstrainsFkDetailsSql =
      "SELECT isr_ref.RDB$FIELD_NAME, relc_ref.RDB$RELATION_NAME, " +
      "isr.RDB$FIELD_NAME, rc.RDB$UPDATE_RULE, rc.RDB$DELETE_RULE " +
      "FROM " +
      "RDB$REF_CONSTRAINTS rc " +
      "INNER JOIN RDB$RELATION_CONSTRAINTS relc ON (relc.RDB$CONSTRAINT_NAME = rc.RDB$CONSTRAINT_NAME) " +
      "INNER JOIN RDB$RELATION_CONSTRAINTS relc_ref ON (rc.RDB$CONST_NAME_UQ = relc_ref.RDB$CONSTRAINT_NAME) " +
      "INNER JOIN RDB$INDEX_SEGMENTS isr ON (relc_ref.RDB$INDEX_NAME = isr.RDB$INDEX_NAME) " +
      "INNER JOIN RDB$INDEX_SEGMENTS isr_ref ON (relc.RDB$INDEX_NAME = isr_ref.RDB$INDEX_NAME) " +
      "WHERE " +
      "relc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' AND relc.RDB$CONSTRAINT_NAME = '{0}' " +
      "ORDER BY isr.RDB$FIELD_POSITION, isr_ref.RDB$FIELD_POSITION";

    string _suspendConstraintsChecksSql =
      "SELECT TRIM(A.RDB$CONSTRAINT_NAME), TRIM(A.RDB$RELATION_NAME), TRIM(C.RDB$TRIGGER_SOURCE) " +
      "FROM RDB$RELATION_CONSTRAINTS A, RDB$CHECK_CONSTRAINTS B, RDB$TRIGGERS C " +
      "WHERE (A.RDB$CONSTRAINT_TYPE = 'CHECK') AND (A.RDB$CONSTRAINT_NAME = B.RDB$CONSTRAINT_NAME) " +
      "AND (B.RDB$TRIGGER_NAME = C.RDB$TRIGGER_NAME) AND (C.RDB$TRIGGER_TYPE = 1)";
    #endregion
    #endregion

    private void SetUpMocks()
    {
      //////////////////////////////////////
      int drTriggerRead = -1;
      string[] triggerNames = { "tr1", "tr2" };
      _drTrigger = new Mock<IDataReader>();
      _drTrigger.Setup(x => x.Read()).Returns(() =>
      {
        if (drTriggerRead++ < 1)        
          return true;        
        else 
          return false;
      });
      _drTrigger.Setup(x => x.GetString(It.IsAny<int>())).Returns(() => triggerNames[drTriggerRead]);
      //////////////////////////////////////

      //////////////////////////////////////
      int drFkRead = -1;
      string[,] fkNames = { {"fk1", "t1"},
                            {"fk2", "t2"}, 
                            {"fkdouble", "t3"}};
      _drFk = new Mock<IDataReader>();
      _drFk.Setup(x => x.Read()).Returns(() =>
      {
        if (drFkRead++ < 2)
          return true;
        else
          return false;
      });
      _drFk.Setup(x => x.GetString(It.IsAny<int>())).Returns<int>(x => fkNames[drFkRead,x]);
      //////////////////////////////////////

      //////////////////////////////////////            
      int fkDetailIndex = 0;
      int fkDetailCount = 0;
      int fkDetailReads = 0;
      string[,] fkDetailsNames = { {"id", "rt1", "keyId", "RESTRICT", "SET NULL"},
                                   {"id", "rt2", "keyId", "CASCADE", "RESTRICT"}, 
                                   {"id1", "rt2", "keyId1", "CASCADE", "RESTRICT"}, 
                                   {"id1", "rt2", "keyId2", "CASCADE", "RESTRICT"},
                                   {"id2", "rt2", "keyId1", "CASCADE", "RESTRICT"},
                                   {"id2", "rt2", "keyId2", "CASCADE", "RESTRICT"}};
      _drFkDetails = new Mock<IDataReader>();
      _drFkDetails.Setup(x => x.Read()).Returns(() =>
      {
        if (fkDetailReads++ < fkDetailCount) return true;
        return false;
      });
      _drFkDetails.Setup(x => x.GetString(It.IsAny<int>()))
        .Returns<int>(x => {
          return fkDetailsNames[fkDetailIndex + fkDetailReads - 1, x];
        });
      //////////////////////////////////////

      //////////////////////////////////////
      int drCheckRead = -1;
      string[,] checkNames = { {"ch1", "t1", "(value = 10)"},
                            {"ch2", "t2", "(value = 11)"}};
      _drCheck = new Mock<IDataReader>();
      _drCheck.Setup(x => x.Read()).Returns(() =>
      {
        if (drCheckRead++ < 1)
          return true;
        else
          return false;
      });
      _drCheck.Setup(x => x.GetString(It.IsAny<int>())).Returns<int>(x => checkNames[drCheckRead,x]);
      //////////////////////////////////////


      _qPerformer = new Mock<ISqlQueryPerformer>();
      _qPerformer.Setup(x => x.ExecuteReader(It.IsAny<SqlQuery>())).Returns<SqlQuery>(x=>
      {
        if (x.Query == _suspendConstraintsTriggersSql)
          return _drTrigger.Object;

        if (x.Query == _suspendConstraintsFkListSql)
          return _drFk.Object;

        if (x.Query == string.Format(_suspendConstrainsFkDetailsSql, "fk1"))
        {
          fkDetailReads = 0; fkDetailIndex = 0; fkDetailCount = 1;
          return _drFkDetails.Object;
        }
        if (x.Query == string.Format(_suspendConstrainsFkDetailsSql, "fk2"))
        {
          fkDetailReads = 0; fkDetailIndex = 1; fkDetailCount = 1;
          return _drFkDetails.Object;
        }
        if (x.Query == string.Format(_suspendConstrainsFkDetailsSql, "fkdouble"))
        {
          fkDetailReads = 0; fkDetailIndex = 2; fkDetailCount = 4;
          return _drFkDetails.Object;
        }         

        if (x.Query == _suspendConstraintsChecksSql)
          return _drCheck.Object;

        return null;
      });
    }
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void ScriptQueryBuilderSuspendConstraintsTest()
    {
      mc.Script.SuspendConstraints();
      var qb = mc.DbObjects.Last();
      
      string expected = @"ALTER TRIGGER ""tr1"" INACTIVE;
ALTER TRIGGER ""tr2"" INACTIVE;
ALTER TABLE ""t1"" DROP CONSTRAINT ""fk1"";
ALTER TABLE ""t2"" DROP CONSTRAINT ""fk2"";
ALTER TABLE ""t3"" DROP CONSTRAINT ""fkdouble"";
ALTER TABLE ""t1"" DROP CONSTRAINT ""ch1"";
ALTER TABLE ""t2"" DROP CONSTRAINT ""ch2"";
";
      var actual = _settings.CreateQueryBuilder(qb).Build(qb);
      Assert.AreEqual(expected, actual.Query);
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptQueryBuilderResumeConstraintsTest()
    {
      mc.Script.SuspendConstraints();
      mc.Script.ResumeConstraints();
      

      var qs = mc.DbObjects.First();
      var qr = mc.DbObjects.Last();

      var builder = _settings.CreateQueryBuilder(qs);
      builder.Build(qs);
      var actual = builder.Build(qr);
      var expected = @"ALTER TRIGGER ""tr1"" ACTIVE;
ALTER TRIGGER ""tr2"" ACTIVE;
ALTER TABLE ""t1"" ADD CONSTRAINT ""fk1"" FOREIGN KEY (""id"") REFERENCES ""rt1"" (""keyId"") ON UPDATE NO ACTION ON DELETE SET NULL;
ALTER TABLE ""t2"" ADD CONSTRAINT ""fk2"" FOREIGN KEY (""id"") REFERENCES ""rt2"" (""keyId"") ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE ""t3"" ADD CONSTRAINT ""fkdouble"" FOREIGN KEY (""id1"", ""id2"") REFERENCES ""rt2"" (""keyId1"", ""keyId2"") ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE ""t1"" ADD CONSTRAINT ""ch1"" (value = 10);
ALTER TABLE ""t2"" ADD CONSTRAINT ""ch2"" (value = 11);
";
      Assert.AreEqual(expected, actual.Query);
    }

   
  }
}
