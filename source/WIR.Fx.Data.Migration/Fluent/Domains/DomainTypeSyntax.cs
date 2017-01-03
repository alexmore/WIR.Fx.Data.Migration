#region Copyright
/******************************************************************************
Copyright (c) 2013 Alexandr Mordvinov, WIR LLC, alexandr.a.mordvinov@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
******************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WIR.Fx.Data.Migration.DbObjects;

namespace WIR.Fx.Data.Migration.Fluent.Domains
{
  public class DomainTypeSyntax : IDomainTypeSyntax
  {
    Domain _domain;

    public DomainTypeSyntax(Domain domain)
    {
      _domain = domain;
    }

    public IDomainSimpleSyntax AsInteger()
    {
      _domain.Type = new DbType(FbType.Integer);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsSmallInt()
    {
      _domain.Type = new DbType(FbType.SmallInt);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsBigInt()
    {
      _domain.Type = new DbType(FbType.BigInt);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsFloat()
    {
      _domain.Type = new DbType(FbType.Float);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsDouble()
    {
      _domain.Type = new DbType(FbType.Double);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsDecimal(int size, int scale)
    {
      _domain.Type = new DbType(FbType.Decimal)
      {
        Size = size,
        Scale = scale
      };
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsNumeric(int size, int scale)
    {
      _domain.Type = new DbType(FbType.Numeric)
      {
        Size = size,
        Scale = scale
      };
      return new DomainSyntax(_domain);
    }

    public IDomainCharSyntax AsChar(int size)
    {
      _domain.Type = new DbType(FbType.Char) { Size = size};
      return new DomainSyntax(_domain);
    }

    public IDomainCharSyntax AsVarchar(int size)
    {
      _domain.Type = new DbType(FbType.Varchar) { Size = size };
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsTime()
    {
      _domain.Type = new DbType(FbType.Time);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsDate()
    {
      _domain.Type = new DbType(FbType.Date);
      return new DomainSyntax(_domain);
    }

    public IDomainSimpleSyntax AsTimeStamp()
    {
      _domain.Type = new DbType(FbType.TimeStamp);
      return new DomainSyntax(_domain);
    }

    public IDomainBlobSyntax AsBlob(FbBlobSubType subType, int segmentSize = 16384)
    {
      _domain.Type = new DbType(FbType.Blob)
      {
        SubType = subType,
        SegmentSize = segmentSize        
      };
      return new DomainSyntax(_domain);
    }
  }
}
