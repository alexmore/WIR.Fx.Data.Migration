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

namespace WIR.Fx.Data.Migration
{
  public static class GenericExtensions
  {
    /// <summary>
    /// Checks existing object in params values
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="val">Reference value</param>
    /// <param name="values">List of values</param>
    /// <returns></returns>
    /// 
    public static bool ContainedIn<T>(this T val, params T[] values)
    {
      return values.Contains(val);
    }
    
    
    /// <summary>    
    /// Returns an instance of the attribute T 
    /// </summary>
    /// <typeparam name="T">Attribute type</typeparam>
    /// <param name="t">Type containing attribute</param>
    /// <returns></returns>
    public static T GetAttribute<T>(this Type t) where T : Attribute
    {
      return (T)Attribute.GetCustomAttribute(t, typeof(T));
    }
  }
}
