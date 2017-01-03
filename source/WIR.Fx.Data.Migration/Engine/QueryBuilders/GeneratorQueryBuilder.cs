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

using WIR.Fx.Data.Migration.DbObjects;

namespace WIR.Fx.Data.Migration.Engine.QueryBuilders
{
  public class GeneratorQueryBuilder : QueryBuilderCore
  {
    public GeneratorQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }
       
    string _create = "CREATE {0} {1}{2}";
    string _alter = "ALTER {0} {1} RESTART WITH {2}{3}";

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(string.Format(_create, dbObject.GetSqlMetadata().ObjectSqlName,
        Settings.FormatName(dbObject.Name), Settings.ScriptTerminationSymbol));
      
      sb.AppendLine(GetAlterSqlQuery(dbObject));

      return sb.ToString().Trim();
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      Generator gen = (Generator)dbObject;
      StringBuilder sb = new StringBuilder();
      if (gen.Value != null)
        sb.AppendLine(string.Format(_alter, dbObject.GetSqlMetadata().ObjectSqlName,
          Settings.FormatName(gen.Name), gen.Value, Settings.ScriptTerminationSymbol));

      string s = CreateDescriptionQuery(dbObject); if (s != null) sb.AppendLine(s);

      return sb.ToString().Trim();
    }
  }
}
