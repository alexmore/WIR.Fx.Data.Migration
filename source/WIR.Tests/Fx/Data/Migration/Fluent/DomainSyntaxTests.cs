using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WIR.Fx.Data.Migration;
using WIR.Fx.Data.Migration.DbObjects;
using WIR.Fx.Data.Migration.Fluent;

namespace WIR.Tests.Fx.Data.Migration
{
  [TestClass]
  public class DomainSyntaxTests
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
    public void DomainSyntaxCreateTypesTest()
    {
      mc.Create.Domain("d").AsSmallInt();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.SmallInt);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Name, "d");
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Action, DbAction.Create);
      mc.Create.Domain("d").AsInteger();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Integer);
      mc.Create.Domain("d").AsBigInt();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.BigInt);

      mc.Create.Domain("d").AsFloat();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Float);
      mc.Create.Domain("d").AsDouble();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Double);
      mc.Create.Domain("d").AsNumeric(10,2);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Numeric);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Size, 10);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Scale, 2);
      mc.Create.Domain("d").AsDecimal(10,2);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Decimal);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Size, 10);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Scale, 2);

      mc.Create.Domain("d").AsChar(10);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Char);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Size, 10);
      mc.Create.Domain("d").AsVarchar(10);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Varchar);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Size, 10);

      mc.Create.Domain("d").AsTime();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Time);
      mc.Create.Domain("d").AsTimeStamp();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.TimeStamp);
      mc.Create.Domain("d").AsDate();
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Date);

      mc.Create.Domain("d").AsBlob(FbBlobSubType.Binary,10);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.Type, WIR.Fx.Data.Migration.FbType.Blob);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.SubType, FbBlobSubType.Binary);
      Assert.AreEqual((mc.DbObjects.Last() as Domain).Type.SegmentSize, 10);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateSimpleTypeTest()
    {
      mc.Create.Domain("d").AsInteger()
        .HasDescription("description")
        .Array(1, 5)
        .NotNull()
        .HasDefault("5")
        .HasCheck("value > 0");        

      var d = (mc.DbObjects.Last() as Domain);
      Assert.AreEqual("description", d.Description);
      Assert.AreEqual(1, d.Type.Array.LowerBound);
      Assert.AreEqual(5, d.Type.Array.UpperBound);
      Assert.IsTrue(d.Type.NotNull);
      Assert.AreEqual("5", d.Type.Default);
      Assert.AreEqual("value > 0", d.Type.Check);      
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateSimpleTypeDefaultValuesTest()
    {
      mc.Create.Domain("d").AsInteger();
        
      var d = (mc.DbObjects.Last() as Domain);
      Assert.IsNull(d.Description);
      Assert.IsNull(d.Type.Array);      
      Assert.IsFalse(d.Type.NotNull);
      Assert.IsNull(d.Type.Default);
      Assert.IsNull(d.Type.Check);      
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateSimpleTypeNullable1Test()
    {
      mc.Create.Domain("d").AsInteger();

      var d = (mc.DbObjects.Last() as Domain);
      Assert.IsNull(d.Description);
      Assert.IsNull(d.Type.Array);
      Assert.IsFalse(d.Type.NotNull);
      Assert.IsNull(d.Type.Default);
      Assert.IsNull(d.Type.Check);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateSimpleTypeNullable2Test()
    {
      mc.Create.Domain("d").AsInteger().Nullable();

      var d = (mc.DbObjects.Last() as Domain);
      Assert.IsNull(d.Description);
      Assert.IsNull(d.Type.Array);
      Assert.IsFalse(d.Type.NotNull);
      Assert.IsNull(d.Type.Default);
      Assert.IsNull(d.Type.Check);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateCharTypeTest()
    {
      mc.Create.Domain("d").AsVarchar(255)
        .HasCharset(FbCharset.ASCII)
        .HasCollate(FbCollate.BIG_5)
        .HasDescription("description")
        .Array(1, 5)
        .NotNull()
        .HasDefault("5")
        .HasCheck("value > 0");        

      var d = (mc.DbObjects.Last() as Domain);
      Assert.AreEqual(255, d.Type.Size);
      Assert.AreEqual(FbCharset.ASCII, d.Type.Charset);
      Assert.AreEqual(FbCollate.BIG_5, d.Type.Collate);
      Assert.AreEqual("description", d.Description);
      Assert.AreEqual(1, d.Type.Array.LowerBound);
      Assert.AreEqual(5, d.Type.Array.UpperBound);
      Assert.IsTrue(d.Type.NotNull);
      Assert.AreEqual("5", d.Type.Default);
      Assert.AreEqual("value > 0", d.Type.Check);      
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateCharTypeDefaultValuesTest()
    {
      mc.Create.Domain("d").AsVarchar(255)
        .HasCharset(FbCharset.ASCII)
        .HasCollate(FbCollate.BIG_5);

      var d = (mc.DbObjects.Last() as Domain);
      Assert.AreEqual(255, d.Type.Size);
      Assert.AreEqual(FbCharset.ASCII, d.Type.Charset);
      Assert.AreEqual(FbCollate.BIG_5, d.Type.Collate);
      Assert.IsNull(d.Description);
      Assert.IsNull(d.Type.Array);
      Assert.IsFalse(d.Type.NotNull);
      Assert.IsNull(d.Type.Default);
      Assert.IsNull(d.Type.Check);      
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateBlobTypeTest()
    {
      mc.Create.Domain("d").AsBlob(FbBlobSubType.Binary, 500)
        .HasCharset(FbCharset.ASCII)        
        .HasDescription("description")
        .HasSize(80);

      var d = (mc.DbObjects.Last() as Domain);
      Assert.AreEqual(FbBlobSubType.Binary, d.Type.SubType);
      Assert.AreEqual(500, d.Type.SegmentSize);
      Assert.AreEqual(80, d.Type.Size);
      Assert.AreEqual(FbCharset.ASCII, d.Type.Charset);      
      Assert.AreEqual("description", d.Description);                  
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxCreateBlobTypeDefaultValuesTest()
    {
      mc.Create.Domain("d").AsBlob(FbBlobSubType.Binary)
        .HasCharset(FbCharset.ASCII);

      var d = (mc.DbObjects.Last() as Domain);      
      Assert.AreEqual(FbCharset.ASCII, d.Type.Charset);
      Assert.AreEqual(16384, d.Type.SegmentSize);
      Assert.IsNull(d.Description);
      Assert.IsNull(d.Type.Array);
      Assert.IsFalse(d.Type.NotNull);
      Assert.IsNull(d.Type.Default);
      Assert.IsNull(d.Type.Check);      
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxAlterTest()
    {
      mc.Alter.Domain("d").SetNewName("nd")
        .SetDescription("description")
        .SetDefault("5")
        .SetCheck("value > 0");

      var d = mc.DbObjects.Last() as Domain;
      Assert.AreEqual(DbAction.Alter, d.Action);
      Assert.AreEqual("d", d.Name);
      Assert.AreEqual("nd", d.NewName);
      Assert.AreEqual("description", d.Description);
      Assert.AreEqual("5", d.Type.Default);
      Assert.AreEqual("value > 0", d.Type.Check);
    }

    [TestMethod, TestCategory("Unit")]
    public void DomainSyntaxDropTest()
    {
      mc.Drop.Domain("d");
      var d = mc.DbObjects.Last() as Domain;

      Assert.AreEqual(DbAction.Drop, d.Action);
      Assert.AreEqual("d", d.Name);
    }
 
  }
}
