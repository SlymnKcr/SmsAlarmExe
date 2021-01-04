using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;

namespace SmsAlarmExe
{
    public static class Extantions
    {
        public static Boolean ToBoolean(this object _Value)
        {
            return Convert.ToBoolean(_Value);
        }

        public static Int32 ToInt32(this object _Value)
        {
            return Convert.ToInt32(_Value);
        }

        public static string ToShortDateString(this DateTime? _Value)
        {
            if (_Value == null)
                return null;
            else
                return Convert.ToDateTime(_Value).ToShortDateString();
        }

        public static DateTime ToDateTime(this object _Value)
        {
            return Convert.ToDateTime(_Value);
        }

        public static Int16 ToInt16(this object _Value)
        {
            return Convert.ToInt16(_Value == null ? 0 : _Value);
        }

        public static Double ToDouble(this object _Value)
        {
            return Convert.ToDouble(_Value);
        }

        public static Decimal ToDecimal(this object _Value)
        {
            return Convert.ToDecimal(_Value == null ? 0 : _Value);
        }

        public static Guid ToGuid(this object _Value)
        {
            return _Value == null ? Guid.Empty : new Guid(_Value.ToString());
        }

        public static Guid GuidValue(this System.Data.DataRow _Row, string _ColumnName)
        {
            try { return (Guid)_Row[_ColumnName]; }
            catch { return Guid.Empty; }
        }

        public static Decimal GetPercent(this object _Value, Int32 _rate)
        {
            return _Value.ToDecimal() * _rate / 100;
        }
    }
}