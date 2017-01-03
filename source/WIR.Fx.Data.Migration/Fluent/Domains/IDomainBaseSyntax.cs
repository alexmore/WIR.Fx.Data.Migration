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
  public interface IDomainBaseSyntax<TNext>
  {
    /// <summary>
    /// Sets array bounds
    /// </summary>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    TNext Array(int lowerBound, int upperBound);
    /// <summary>
    /// Sets domain is not null
    /// </summary>
    /// <returns></returns>
    TNext NotNull();
    /// <summary>
    /// Sets domain as nullable
    /// </summary>
    /// <returns></returns>
    TNext Nullable();
    /// <summary>
    /// Sets domain default value
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    TNext HasDefault(string defaultValue);
    /// <summary>
    /// Sets domain check expression
    /// </summary>
    /// <param name="check"></param>
    /// <returns></returns>
    TNext HasCheck(string check);
  }
}
