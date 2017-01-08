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
  public class ProcedureSyntaxTests
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
    public void ProcedureSyntaxCreateTest()
    {
      mc.Create.Procedure("proc")
        .HasDescription("description")        
        .WithProcedureText("proctext");        

      var p = mc.DbObjects.Last() as Procedure;
      Assert.AreEqual(DbAction.Create, p.Action);
      Assert.AreEqual("proc", p.Name);
      Assert.AreEqual("description", p.Description);
      Assert.AreEqual("proctext", p.ProcedureText);      
    }

    [TestMethod, TestCategory("Unit")]
    public void ProcedureSyntaxAlterTest()
    {
      mc.Alter.Procedure("proc")
        .HasDescription("description")        
        .WithProcedureText("proctext");

      var p = mc.DbObjects.Last() as Procedure;
      Assert.AreEqual(DbAction.Alter, p.Action);
      Assert.AreEqual("proc", p.Name);
      Assert.AreEqual("description", p.Description);
      Assert.AreEqual("proctext", p.ProcedureText);      
    }

    [TestMethod, TestCategory("Unit")]
    public void ProcedureSyntaxDropTest()
    {
      mc.Drop.Procedure("proc");

      var p = mc.DbObjects.Last() as Procedure;
      Assert.AreEqual(DbAction.Drop, p.Action);
      Assert.AreEqual("proc", p.Name);      
    }
  }
}
