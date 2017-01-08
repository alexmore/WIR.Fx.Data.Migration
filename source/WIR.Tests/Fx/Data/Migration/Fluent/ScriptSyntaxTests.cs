using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Fluent;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class ScriptSyntaxTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {
      mc = new MigrationContextMoq();
    }

    MigrationContextMoq mc;
    #endregion
    
    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxTest()
    {
      mc.Script.ExecuteQuery("query");

      var q = mc.DbObjects.Last() as Script;
      Assert.AreEqual(ScriptType.SqlQuery, q.Type);      
      Assert.AreEqual(DbAction.NoAction, q.Action);      
      Assert.AreEqual("query", q.SqlQuery);      
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxWithParametersTest()
    {
      mc.Script.ExecuteQuery("query").HasParameter("p1", 1);

      var q = mc.DbObjects.Last() as Script;
      Assert.AreEqual(ScriptType.SqlQuery, q.Type);      
      Assert.AreEqual(DbAction.NoAction, q.Action);
      Assert.AreEqual("query", q.SqlQuery);
      Assert.AreEqual(1, q.Parameters.Count);
      Assert.AreEqual("p1", q.Parameters.Keys.First());
      Assert.AreEqual(1, q.Parameters.Values.First());
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxManyQueriesTest()
    {
      mc.Script.ExecuteQuery("query1")
        .ExecuteQuery("query2");

      Assert.AreEqual(2, mc.DbObjects.Count);
      Assert.AreEqual("query1", (mc.DbObjects.First() as Script).SqlQuery);
      Assert.AreEqual("query2", (mc.DbObjects.Last() as Script).SqlQuery);
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxManyQueriesWithParametersTest()
    {
      mc.Script
        .ExecuteQuery("query1")
          .HasParameter("p11",1)
          .HasParameter("p12",2)
        .ExecuteQuery("query2")
          .HasParameter("p21", 3)
          .HasParameter("p22", 4);

      Assert.AreEqual(2, mc.DbObjects.Count);
      Assert.AreEqual("query1", (mc.DbObjects.First() as Script).SqlQuery);
      Assert.AreEqual(2, (mc.DbObjects.First() as Script).Parameters.Count);
      Assert.AreEqual("p11", (mc.DbObjects.First() as Script).Parameters.Keys.First());
      Assert.AreEqual("p12", (mc.DbObjects.First() as Script).Parameters.Keys.Last());
      Assert.AreEqual(1, (mc.DbObjects.First() as Script).Parameters.Values.First());
      Assert.AreEqual(2, (mc.DbObjects.First() as Script).Parameters.Values.Last());

      Assert.AreEqual("query2", (mc.DbObjects.Last() as Script).SqlQuery);
      Assert.AreEqual(2, (mc.DbObjects.Last() as Script).Parameters.Count);
      Assert.AreEqual("p21", (mc.DbObjects.Last() as Script).Parameters.Keys.First());
      Assert.AreEqual("p22", (mc.DbObjects.Last() as Script).Parameters.Keys.Last());
      Assert.AreEqual(3, (mc.DbObjects.Last() as Script).Parameters.Values.First());
      Assert.AreEqual(4, (mc.DbObjects.Last() as Script).Parameters.Values.Last());
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxInsertTest()
    {
      mc.Script.Insert.Into("t").ToColumns("c1", "c2").Values(1, 2);

      Assert.AreEqual(1, mc.DbObjects.Count);
      var actual = (Script)mc.DbObjects.First();
      Assert.AreEqual(ScriptType.InsertQuery, actual.Type);      
      Assert.AreEqual("t", actual.TableName);
      Assert.AreEqual(1, actual.Parameters["c1"]);
      Assert.AreEqual(2, actual.Parameters["c2"]);
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxInsertManyValuesTest()
    {
      mc.Script.Insert.Into("t").ToColumns("c1", "c2")
        .Values(1, 2)
        .Values(3, 4);

      Assert.AreEqual(2, mc.DbObjects.Count);
      var actual = (Script)mc.DbObjects.First();
      Assert.AreEqual(ScriptType.InsertQuery, actual.Type);      
      Assert.AreEqual("t", actual.TableName);      
      Assert.AreEqual(1, actual.Parameters["c1"]);
      Assert.AreEqual(2, actual.Parameters["c2"]);

      actual = (Script)mc.DbObjects.Last();
      Assert.AreEqual(ScriptType.InsertQuery, actual.Type);      
      Assert.AreEqual("t", actual.TableName);      
      Assert.AreEqual(3, actual.Parameters["c1"]);
      Assert.AreEqual(4, actual.Parameters["c2"]);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void ScriptSyntaxInsertFailsWhenColumnsAndValuesLengthIsDifferentTest()
    {
      mc.Script.Insert.Into("t").ToColumns("c2").Values(1,2);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentException))]
    public void ScriptSyntaxInsertFailsWhenColumnsLengthIs0Test()
    {
      mc.Script.Insert.Into("t").ToColumns();
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxScriptFromFileTest()
    {
      mc.Script.Execute("filename");
            
      var actual = (Script)mc.DbObjects.First();
      Assert.AreEqual(ScriptType.SqlFile, actual.Type);      
      Assert.AreEqual("filename", actual.SqlQuery);
      
    }

    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxSuspendConstraintsTest()
    {
      mc.Script.SuspendConstraints();

      var actual = (Script)mc.DbObjects.First();
      Assert.AreEqual(ScriptType.SuspendConstraints, actual.Type);
    }
  
    [TestMethod, TestCategory("Unit")]
    public void ScriptSyntaxSuspendConstraintsNameTest()
    {
      mc.Script.SuspendConstraints();
      mc.Script.ResumeConstraints();

      var actual1 = (Script)mc.DbObjects.First();
      var actual2 = (Script)mc.DbObjects.Last();
      Assert.AreEqual(actual1.Name, actual2.Name);
      Assert.AreEqual(ScriptType.ResumeConstraints, actual2.Type);
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ScriptSyntaxResumeConstraintsFailsWhenCalledBeforeTest()
    {      
      mc.Script.ResumeConstraints();
    }   
  }
}
