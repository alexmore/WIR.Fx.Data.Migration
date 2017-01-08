using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WIR.Fx.Data.Migration;
using WIR.Fx.Data.Migration.DbObjects;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class ColumneTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {

    }
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void ColumnNotNullAsStringTest()
    {
      Assert.AreEqual(new Column("col", DbAction.NoAction) { NotNull = true }.NotNullAsString, "NOT NULL");
      Assert.IsNull(new Column("col", DbAction.NoAction) { NotNull = false }.NotNullAsString);
    }


    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ColumnDbTypeAndDomainBothExceptionTest()
    {
      Column c = new Column("col", DbAction.NoAction);
      c.DomainName = "dn";
      c.Type = new DbType();
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ColumnDomainAndDbTypeBothExceptionTest()
    {
      Column c = new Column("col", DbAction.NoAction);
      c.Type = new DbType();
      c.DomainName = "dn";      
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ColumnGetColumnTypeStringExceptionWhenDbTypeAndDomainAreNullTest()
    {
      Column c = new Column("col", DbAction.NoAction);
      c.GetColumnTypeString(x => { return x; });
    }
    
  }
}
