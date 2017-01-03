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

namespace WIR.Fx.Data.Migration.Fluent.Triggers
{
  public interface ITriggerTableSyntax
  {
    /// <summary>
    /// Sets trigger's table
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns></returns>
    ITriggerActivateSyntax OnTable(string tableName);
  }

  public interface ITriggerActivateSyntax
  {
    /// <summary>
    /// Sets before action order for the trigger
    /// </summary>
    ITriggerActivateTypeSyntax Before { get; }
    /// <summary>
    /// Sets after action order for the trigger
    /// </summary>
    ITriggerActivateTypeSyntax After { get; }
  }

  public interface ITriggerActivateTypeSyntax
  {
    ITriggerSyntax Insert();
    ITriggerSyntax Update();
    ITriggerSyntax Delete();
  
  }

  public interface ITriggerSyntax : IDbObjectSyntax<ITriggerSyntax>, 
                                    ITriggerActivateTypeSyntax
  {
    /// <summary>
    /// Sets trigger's text between BEGIN END
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    ITriggerSyntax HasTriggerText(string text);
    /// <summary>
    /// Sets trigger's execution position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    ITriggerSyntax OnPosition(int position);

    /// <summary>
    /// Sets trigger active
    /// </summary>
    /// <returns></returns>
    ITriggerSyntax IsActive();
    /// <summary>
    /// Sets trigger inactive
    /// </summary>
    /// <returns></returns>
    ITriggerSyntax IsInactive();
  }
}
