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
  /// <summary>
  /// Firebird Sql collate
  /// </summary>
  public enum FbCollate
  {
    ASCII,
    BIG_5,
    CP943C, CP943C_UNICODE,
    CYRL, DB_RUS, PDOX_CYRL,
    DOS437, DB_DEU437, DB_ESP437, DB_FRA437, DB_FIN437, DB_ITA437, DB_NLD437, 
            DB_SVE437, DB_UK437, DB_US437, PDOX_ASCII, PDOX_SWEDFIN, PDOX_INTL,
    DOS737,
    DOS775,
    DOS850, DB_DEU850, DB_ESP850, DB_FRA850, DB_FRC850, DB_ITA850, DB_NLD850, 
            DB_PTB850, DB_SVE850, DB_UK850, DB_US850,
    DOS852, DB_CSY, DB_PLK, DB_SLO, PDOX_PLK, PDOX_HUN, PDOX_SLO, PDOX_CSY,
    DOS857, DB_TRK,
    DOS858,
    DOS860, DB_PTG860,
    DOS861, PDOX_ISL,
    DOS862,
    DOS863, DB_FRC863,
    DOS864,
    DOS865, DB_DAN865, DB_NOR865, PDOX_NORDAN4,
    DOS866,
    DOS869,
    EUCJ_0208,
    GB18030,
    GBK,
    GB_2312,
    ISO8859_1, FR_CA, DA_DA, DE_DE, ES_ES, FI_FI, FR_FR, IS_IS, IT_IT, NO_NO, 
               DU_NL,  PT_PT, SV_SV, EN_UK, EN_US,
    ISO8859_13,
    ISO8859_2,
    ISO8859_3,
    ISO8859_4,
    ISO8859_5,
    ISO8859_6,
    ISO8859_7,
    ISO8859_8,
    ISO8859_9,
    KOI8R,
    KOI8U,
    KSC_5601, KSC_DICTIONARY, 
    NEXT, NXT_US, NXT_FRA, NXT_ITA, NXT_ESP, NXT_DEU,
    NONE,
    OCTETS,
    SJIS_0208,
    TIS620,
    UNICODE_FSS,
    UTF8,
    WIN1250, PXW_PLK, PXW_HUN, PXW_CSY, PXW_HUNDC, PXW_SLOV,
    WIN1251, WIN1251_UA, PXW_CYRL,
    WIN1252, PXW_SWEDFIN, PXW_NORDAN4, PXW_INTL, PXW_INTL850, PXW_SPAN,
    WIN1253, PXW_GREEK,
    WIN1254, PXW_TURK,
    WIN1255,
    WIN1256,
    WIN1257,
    WIN1258
  }
}
