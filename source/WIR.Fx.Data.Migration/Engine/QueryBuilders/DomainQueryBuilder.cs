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
  public class DomainQueryBuilder : QueryBuilderCore
  {
    public DomainQueryBuilder(MigrationSettings settings)
      : base(settings)
    {
    }
   
    #region Sql query templates
    string _createDomainFormat = "CREATE DOMAIN {0} AS {1}{2}";
    string _alterDomainType = "ALTER DOMAIN {0} TYPE {1}{2}";
    string _alterDomainDefault = "ALTER DOMAIN {0} SET {1}{2}";
    string _alterDomainRename = "ALTER DOMAIN {0} TO {1}{2}";
    string _alterDomainAddCheck = "ALTER DOMAIN {0} DROP CONSTRAINT; ALTER DOMAIN {0} ADD {1}{2}";
    #endregion

    protected override string GetCreateSqlQuery(DbObject dbObject)
    {
      Domain domain = (Domain)dbObject;
      if (domain.Type == null)
        throw new InvalidOperationException("Domain Type property can not be null. Domain: "+domain.Name);
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat(
        _createDomainFormat,
        Settings.FormatName(dbObject.Name),
        domain.Type.ToString(),
        Settings.ScriptTerminationSymbol
        );
      sb.AppendLine();

      // Добавляем комментарий
      string s = CreateDescriptionQuery(dbObject); if (s != null) sb.AppendLine(s);

      return sb.ToString().Trim();
    }

    protected override string GetAlterSqlQuery(DbObject dbObject)
    {
      Domain domain = (Domain)dbObject;

      StringBuilder sb = new StringBuilder();
      if (domain.Type != null)
      {
        DbType.DbTypeSql type = domain.Type.BuildSql();

        // Тип
        if (domain.Type.Type != null)
          sb.AppendFormat(
            _alterDomainType,
            Settings.FormatName(dbObject.Name),
            type.TypeSql, 
            Settings.ScriptTerminationSymbol
            );
        sb.AppendLine();
        // Default
        if (type.DefaultSql != null)
          sb.AppendLine(string.Format(
            _alterDomainDefault,
            Settings.FormatName(dbObject.Name),
            type.DefaultSql, Settings.ScriptTerminationSymbol));
        // Check
        if (type.CheckSql != null)
          sb.AppendLine(string.Format(
            _alterDomainAddCheck,
            Settings.FormatName(dbObject.Name),
            type.CheckSql, Settings.ScriptTerminationSymbol)
            );
      }

      
      string s = CreateDescriptionQuery(dbObject); if (s != null) sb.AppendLine(s);

      
      if (!string.IsNullOrEmpty(domain.NewName))
        sb.AppendLine(
          string.Format(
          _alterDomainRename,
          Settings.FormatName(dbObject.Name),
          Settings.FormatName(domain.NewName),
          Settings.ScriptTerminationSymbol
          )
          );

      return sb.ToString().Trim();
    }
  }
}
