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
  public class GeneratorSyntaxTests
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
    public void GeneratorSyntaxCreateTest()
    {
      mc.Create.Generator("gen")
        .HasDescription("description")
        .RestartWith(10);      

      var g = mc.DbObjects.Last() as Generator;
      Assert.AreEqual(DbAction.Create, g.Action);
      Assert.AreEqual("gen", g.Name);
      Assert.AreEqual("description", g.Description);
      Assert.AreEqual(10, g.Value);      
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorSyntaxCreateDefaultTest()
    {
      mc.Create.Generator("gen");

      var g = mc.DbObjects.Last() as Generator;
      Assert.AreEqual(DbAction.Create, g.Action);
      Assert.AreEqual("gen", g.Name);
      Assert.IsNull(g.Description);
      Assert.IsNull(g.Value);      
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorSyntaxAlterTest()
    {
      mc.Alter.Generator("gen")        
        .HasDescription("description")        
        .RestartWith(10);

      var g = mc.DbObjects.Last() as Generator;
      Assert.AreEqual(DbAction.Alter, g.Action);
      Assert.AreEqual("gen", g.Name);
      Assert.AreEqual("description", g.Description);
      Assert.AreEqual(10, g.Value);      
    }

    [TestMethod, TestCategory("Unit")]
    public void GeneratorSyntaxDropTest()
    {
      mc.Drop.Generator("gen");

      var g = mc.DbObjects.Last() as Generator;
      Assert.AreEqual(DbAction.Drop, g.Action);
      Assert.AreEqual("gen", g.Name);      
    }
  }
}
