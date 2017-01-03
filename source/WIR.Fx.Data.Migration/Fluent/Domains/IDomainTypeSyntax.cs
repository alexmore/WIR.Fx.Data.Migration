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

namespace WIR.Fx.Data.Migration.Fluent.Domains
{
  public interface IDomainTypeSyntax
  {
    /// <summary>
    /// Sets domain to INTEGER type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsInteger();
    /// <summary>
    /// Sets domain to SMALLINT type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsSmallInt();
    /// <summary>
    /// Sets domain to BIGINT type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsBigInt();

    /// <summary>
    /// Sets domain to FLOAT type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsFloat();
    /// <summary>
    /// Sets domain to DOUBLE PRECISION type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsDouble();
    /// <summary>
    /// Sets domain to DECIMAL type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsDecimal(int size, int scale);
    /// <summary>
    /// Sets domain to NUMERIC type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsNumeric(int size, int scale);

    /// <summary>
    /// Sets domain to CHAR type
    /// </summary>
    /// <returns></returns>
    IDomainCharSyntax AsChar(int size);
    /// <summary>
    /// Sets domain to VARCHAR type
    /// </summary>
    /// <returns></returns>
    IDomainCharSyntax AsVarchar(int size);

    /// <summary>
    /// Sets domain to TIME type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsTime();
    /// <summary>
    /// Sets domain to DATE type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsDate();
    /// <summary>
    /// Sets domain to TIMESTAMP type
    /// </summary>
    /// <returns></returns>
    IDomainSimpleSyntax AsTimeStamp();
    
    /// <summary>
    /// Sets domain to BLOB type
    /// </summary>
    /// <returns></returns>
    IDomainBlobSyntax AsBlob(FbBlobSubType subType, int segmentSize = 16384);
  }
  
}
