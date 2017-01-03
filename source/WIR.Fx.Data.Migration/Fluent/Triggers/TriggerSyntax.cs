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

namespace WIR.Fx.Data.Migration.Fluent.Triggers
{
  public class TriggerSyntax : DbObjectSyntax<ITriggerSyntax>, ITriggerSyntax, 
                               ITriggerTableSyntax, ITriggerActivateTypeSyntax, 
                               ITriggerActivateSyntax
  {
    Trigger _t;

    public TriggerSyntax(Trigger trigger) : base(trigger)
    {
      _t = trigger;
    }

    public ITriggerActivateSyntax OnTable(string tableName)
    {
      _t.TableName = tableName;
      return this;
    }

    public ITriggerActivateTypeSyntax Before
    {
      get
      {
        _t.TriggerOrder = TriggerOrder.Before;
        return this;
      }
    }
    
    public ITriggerActivateTypeSyntax After
    {
      get
      {
        _t.TriggerOrder = TriggerOrder.After;
        return this;
      }
    }

    
    public ITriggerSyntax Insert()
    {
      _t.TriggerAction = this._t.TriggerAction 
        | TriggerAction.Insert;
      return this;
    }

    public ITriggerSyntax Update()
    {
      _t.TriggerAction = this._t.TriggerAction
        | TriggerAction.Update;
      return this;
    }
    public ITriggerSyntax Delete()
    {
      _t.TriggerAction = this._t.TriggerAction
        | TriggerAction.Delete;
      return this;
    }

    public ITriggerSyntax HasTriggerText(string text)
    {
      _t.TriggerText = text;
      return this;
    }
    public ITriggerSyntax OnPosition(int position)
    {
      _t.Position = position;
      return this;
    }

    public ITriggerSyntax IsActive()
    {
      _t.IsActive = true;
      return this;
    }

    public ITriggerSyntax IsInactive()
    {
      _t.IsActive = false;
      return this;
    }
  }
}
