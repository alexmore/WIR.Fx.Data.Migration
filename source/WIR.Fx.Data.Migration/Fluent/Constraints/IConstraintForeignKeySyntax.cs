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

namespace WIR.Fx.Data.Migration.Fluent.Constraints
{
  public interface IConstraintFKReferenceSyntax
  {
    /// <summary>
    /// Sets foreign key reference table name
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    IConstraintFKReferenceColumnsSyntax WithReferenceTable(string tableName);
  }

  public interface IConstraintFKReferenceColumnsSyntax
  {
    /// <summary>
    /// Sets foreign key reference columns
    /// </summary>
    /// <param name="columns"></param>
    /// <returns></returns>
    IConstraintFKSyntax OnColumns(params string[] columns);
  }

  public interface IConstraintFKSyntax : IConstraintUsingIndex<IConstraintFKSyntax>
  {
    /// <summary>
    /// Sets update rule for the foreign key
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    IConstraintFKRuleValueSyntax HasUpdateRule { get; }
    /// <summary>
    /// Sets delete rule for the foreign key
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    IConstraintFKRuleValueSyntax HasDeleteRule {get; }
  }

  public interface IConstraintFKRuleValueSyntax
  {    
    IConstraintFKSyntax Cascade();
    IConstraintFKSyntax SetNull();
    IConstraintFKSyntax SetDefault();
  }
}
