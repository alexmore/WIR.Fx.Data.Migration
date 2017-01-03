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
  public sealed class DomainSyntax : IDomainSimpleSyntax, IDomainCharSyntax, 
                              IDomainBlobSyntax, IDomainAlterSyntax
  {
    Domain _domain;

    public DomainSyntax(Domain domain)
    {
      _domain = domain;
    }

    #region Simple
    public IDomainSimpleSyntax HasDescription(string description)
    {
      _domain.Description += description;
      return this;
    }
   
    public IDomainSimpleSyntax Array(int lowerBound, int upperBound)
    {
      _domain.Type.Array = new DbType.DbTypeArrayBound(lowerBound, upperBound);
      return this;
    }

    public IDomainSimpleSyntax NotNull()
    {
      _domain.Type.NotNull = true;
      return this;
    }

    public IDomainSimpleSyntax Nullable()
    {
      _domain.Type.NotNull = false;
      return this;
    }

    public IDomainSimpleSyntax HasDefault(string defaultValue)
    {
      _domain.Type.Default = defaultValue;
      return this;
    }

    public IDomainSimpleSyntax HasCheck(string check)
    {
      _domain.Type.Check = check;
      return this;
    }
    #endregion

    #region Char
    public IDomainCharSyntax HasCharset(FbCharset charset)
    {
      _domain.Type.Charset = charset;
      return this;
    }

    public IDomainCharSyntax HasCollate(FbCollate collate)
    {
      _domain.Type.Collate = collate;
      return this;
    }

    IDomainCharSyntax IDbObjectSyntax<IDomainCharSyntax>.HasDescription(string description)
    {
      this.HasDescription(description);
      return this;
    }
    
    IDomainCharSyntax IDomainBaseSyntax<IDomainCharSyntax>.Array(int lowerBound, int upperBound)
    {
      this.Array(lowerBound, upperBound);
      return this;
    }

    IDomainCharSyntax IDomainBaseSyntax<IDomainCharSyntax>.NotNull()
    {
      this.NotNull();
      return this;
    }

    IDomainCharSyntax IDomainBaseSyntax<IDomainCharSyntax>.Nullable()
    {
      this.Nullable();
      return this;
    }

    IDomainCharSyntax IDomainBaseSyntax<IDomainCharSyntax>.HasDefault(string defaultValue)
    {
      this.HasDefault(defaultValue);
      return this;
    }

    IDomainCharSyntax IDomainBaseSyntax<IDomainCharSyntax>.HasCheck(string check)
    {
      this.HasCheck(check);
      return this;
    }
    #endregion

    #region Blob
    IDomainBlobSyntax IDomainBlobSyntax.HasCharset(FbCharset charset)
    {
      this.HasCharset(charset);
      return this;
    }

    public IDomainBlobSyntax HasSize(int size)
    {
      this._domain.Type.Size = size;
      return this;
    }

    IDomainBlobSyntax IDbObjectSyntax<IDomainBlobSyntax>.HasDescription(string description)
    {
      HasDescription(description);
      return this;
    }    
    #endregion
    
    #region Alter
    IDomainAlterSyntax IDomainAlterSyntax.SetNewName(string newName)
    {
      _domain.NewName = newName;
      return this;
    }

    IDomainAlterSyntax IDomainAlterSyntax.SetDescription(string description)
    {
      this.HasDescription(description);
      return this;
    }

    IDomainAlterSyntax IDomainAlterSyntax.SetDefault(string defaultValue)
    {
      if (_domain.Type == null) _domain.Type = new DbType();
      this.HasDefault(defaultValue);
      return this;
    }

    IDomainAlterSyntax IDomainAlterSyntax.SetCheck(string check)
    {
      if (_domain.Type == null) _domain.Type = new DbType();
      this.HasCheck(check);
      return this;
    }    
    #endregion


    
  }
}
