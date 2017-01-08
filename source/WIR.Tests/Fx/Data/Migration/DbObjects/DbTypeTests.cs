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
  public class DbTypeTests
  {
    #region Setup
    [TestInitialize]
    public void SetupTestMethod()
    {

    }
    #endregion

    [TestMethod, TestCategory("Unit")]
    public void DbTypeNotNullAsStringTest()
    {
      Assert.AreEqual(new DbType() { NotNull = true }.NotNullAsString, "NOT NULL");
      Assert.IsNull(new DbType() { NotNull = false }.NotNullAsString);
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeGetSqlStringExtensionTest()
    {
      WIR.Fx.Data.Migration.FbType t = WIR.Fx.Data.Migration.FbType.BigInt; Assert.AreEqual("BIGINT", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Blob; Assert.AreEqual("BLOB", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Char; Assert.AreEqual("CHAR(10)", t.GetSqlString(10));
      t = WIR.Fx.Data.Migration.FbType.Date; Assert.AreEqual("DATE", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Decimal; Assert.AreEqual("DECIMAL(15,2)", t.GetSqlString(15, 2));
      t = WIR.Fx.Data.Migration.FbType.Double; Assert.AreEqual("DOUBLE PRECISION", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Float; Assert.AreEqual("FLOAT", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Integer; Assert.AreEqual("INTEGER", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Numeric; Assert.AreEqual("NUMERIC(18,4)", t.GetSqlString(18, 4));
      t = WIR.Fx.Data.Migration.FbType.SmallInt; Assert.AreEqual("SMALLINT", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Time; Assert.AreEqual("TIME", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.TimeStamp; Assert.AreEqual("TIMESTAMP", t.GetSqlString());
      t = WIR.Fx.Data.Migration.FbType.Varchar; Assert.AreEqual("VARCHAR(255)", t.GetSqlString(255));
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeGetSqlStringExtensionCheckParamsTest()
    {
      WIR.Fx.Data.Migration.FbType t = WIR.Fx.Data.Migration.FbType.Char;
      try { t.GetSqlString(); Assert.Fail("Size property check failed for WIR.Fx.Data.Migration.FbType.Char"); }
      catch { };

      t = WIR.Fx.Data.Migration.FbType.Varchar;
      try { t.GetSqlString(); Assert.Fail("Size property check failed for WIR.Fx.Data.Migration.FbType.VarChar"); }
      catch { };

      t = WIR.Fx.Data.Migration.FbType.Decimal;
      try { t.GetSqlString(); Assert.Fail("Size property check failed for WIR.Fx.Data.Migration.FbType.Decimal"); }
      catch { };

      t = WIR.Fx.Data.Migration.FbType.Decimal;
      try { t.GetSqlString(15); Assert.Fail("Scale property check failed for WIR.Fx.Data.Migration.FbType.Decimal"); }
      catch { };

      t = WIR.Fx.Data.Migration.FbType.Numeric;
      try { t.GetSqlString(); Assert.Fail("Size property check failed for WIR.Fx.Data.Migration.FbType.Numeric"); }
      catch { };

      t = WIR.Fx.Data.Migration.FbType.Numeric;
      try { t.GetSqlString(18); Assert.Fail("Scale property check failed for WIR.Fx.Data.Migration.FbType.Numeric"); }
      catch { };      
    }
    
    [TestMethod, TestCategory("Unit")]
    public void FbTypeBigIntToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.BigInt,
        Array = new DbType.DbTypeArrayBound(1,10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("BIGINT[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeSmallIntToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.SmallInt,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("SMALLINT[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeIntegerToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Integer,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("INTEGER[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeFloatToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Float,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("FLOAT[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeDoubleToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Double,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("DOUBLE PRECISION[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeNumericToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Numeric,
        Scale = 2,
        Size = 10,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("NUMERIC(10,2)[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeDecimalToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Decimal,
        Scale = 2,
        Size = 10,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("DECIMAL(10,2)[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeVarcharToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Varchar,        
        Size = 255,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true,
        Charset = FbCharset.ASCII,
        Collate = FbCollate.ASCII
      };

      Assert.AreEqual("VARCHAR(255)[1:10] CHARACTER SET ASCII DEFAULT 10 NOT NULL CHECK (value > 0) COLLATE ASCII", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeCharToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Char,
        Size = 30,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true,
        Charset = FbCharset.ASCII,
        Collate = FbCollate.ASCII
      };

      Assert.AreEqual("CHAR(30)[1:10] CHARACTER SET ASCII DEFAULT 10 NOT NULL CHECK (value > 0) COLLATE ASCII", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeTimeToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Time,  
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("TIME[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeDateToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Date,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("DATE[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeTimeStampToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.TimeStamp,
        Array = new DbType.DbTypeArrayBound(1, 10),
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("TIMESTAMP[1:10] DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeBlobSubTypeTextToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Blob,
        SegmentSize = 16384,
        SubType = FbBlobSubType.Text,        
        Default = "10",
        Check = "value > 0",
        NotNull = true,
        Charset = FbCharset.ASCII        
      };

      Assert.AreEqual("BLOB SUB_TYPE 1 SEGMENT SIZE 16384 CHARACTER SET ASCII DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    public void FbTypeBlobSubTypeBinaryToStringFullTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Blob,
        SegmentSize = 16384,
        SubType = FbBlobSubType.Binary,
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };

      Assert.AreEqual("BLOB SUB_TYPE 0 SEGMENT SIZE 16384 DEFAULT 10 NOT NULL CHECK (value > 0)", t.ToString());
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FbTypeBlobMissingSegmentSizeExceptionTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Blob,        
        SubType = FbBlobSubType.Binary,
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };
      t.ToString();
    }

    [TestMethod, TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FbTypeBlobMissingSubTypeExceptionTest()
    {
      var t = new DbType()
      {
        Type = WIR.Fx.Data.Migration.FbType.Blob,
        SegmentSize = 16384,        
        Default = "10",
        Check = "value > 0",
        NotNull = true
      };
      t.ToString();
    }
  }
}
