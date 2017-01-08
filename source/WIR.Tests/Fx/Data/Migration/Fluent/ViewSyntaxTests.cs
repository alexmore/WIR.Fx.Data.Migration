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
  public class ViewSyntaxTestsSyntaxTests
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
    public void ViewSyntaxCreateTest()
    {
      mc.Create.View("view").HasDescription("description")
        .HasResultColumns("col1", "col2")
        .HasQuery("view text");
        
            
      var v = mc.DbObjects.Last() as View;
      Assert.AreEqual(DbAction.Create, v.Action);
      Assert.AreEqual("view", v.Name);      
      Assert.AreEqual("description", v.Description);
      Assert.AreEqual("view text", v.Query);
      Assert.AreEqual(2, v.ResultColumns.Length);
      Assert.AreEqual("col1", v.ResultColumns[0]);
      Assert.AreEqual("col2", v.ResultColumns[1]);
    }

    [TestMethod, TestCategory("Unit")]
    public void ViewSyntaxAlterTest()
    {
      mc.Alter.View("view").HasDescription("description")
        .HasResultColumns("col1", "col2")
        .HasQuery("view text");


      var v = mc.DbObjects.Last() as View;
      Assert.AreEqual(DbAction.Alter, v.Action);
      Assert.AreEqual("view", v.Name);
      Assert.AreEqual("description", v.Description);
      Assert.AreEqual("view text", v.Query);
      Assert.AreEqual(2, v.ResultColumns.Length);
      Assert.AreEqual("col1", v.ResultColumns[0]);
      Assert.AreEqual("col2", v.ResultColumns[1]);
    }

    [TestMethod, TestCategory("Unit")]
    public void ViewSyntaxDropTest()
    {
      mc.Drop.View("view");


      var v = mc.DbObjects.Last() as View;
      Assert.AreEqual(DbAction.Drop, v.Action);
      Assert.AreEqual("view", v.Name);      
    }  
  }
}
