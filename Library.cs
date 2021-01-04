using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;


namespace SmsAlarmExe
{
    public class OracleLibrary
    {
        #region  Sql Methods

        //string __ConnectionString = EssoAlarms.Properties.Settings.Default.DefaultConnectionString3;

        //string __ConnectionString = "User Id=LOG724DB;Password=Esso4331437;Data Source=92.42.34.111/xe";
        //string __ConnectionString = "User Id=LOG724DB;Password=Esso4331437;Data Source=136.243.45.231/xe";
        //string __ConnectionString = "User Id=LOG724DB;Password=Orcl1881;Data Source=213.239.197.134/xe";
        //string __ConnectionString = "User Id=LOG724DB;Password=Orcl1881;Data Source=195.201.199.39/xe";
        public const string __ConnectionString = "User Id=LOG724DB;Password=Orcl1881;Data Source=168.119.33.106/ESSODB";
        public DataTable GetDataTable(string _connStr, string _sql, params object[] values)
        {
            DataTable _dt = new DataTable();
            OracleCommand _cmd;

            try
            {
                OracleConnection _conn;

                if (_connStr == null) _conn = new OracleConnection(__ConnectionString);
                else _conn = new OracleConnection(_connStr);

                _conn.Open();

                _cmd = new OracleCommand(_sql, _conn);
                _cmd.CommandTimeout = 60000;

                if (values.Length > 0)
                {
                    OracleParameter[] _Parameters = new OracleParameter[values.Length];

                    for (int i = 0; i < values.Length; i++)
                    {
                        _Parameters[i] = new OracleParameter("p" + (i + 1).ToString(), values[i]);
                    }

                    _cmd.Parameters.AddRange(_Parameters);
                }

                IDataReader dr = _cmd.ExecuteReader();
                _dt.Load(dr);

                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                    _conn.Dispose();
                }

                return _dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int SqlExecute(string _connStr, string _sql, params object[] values)
        {
            OracleCommand _cmd;

            try
            {
                OracleConnection _conn;

                if (_connStr == null) _conn = new OracleConnection(__ConnectionString);
                else _conn = new OracleConnection(_connStr);

                _conn.Open();

                _cmd = new OracleCommand(_sql, _conn);
                _cmd.CommandTimeout = 50000;

                if (values.Length > 0)
                {
                    OracleParameter[] _Parameters = new OracleParameter[values.Length];

                    for (int i = 0; i < values.Length; i++)
                    {
                        _Parameters[i] = new OracleParameter("p" + (i + 1).ToString(), values[i]);
                    }

                    _cmd.Parameters.AddRange(_Parameters);
                }

                int rowAffected = _cmd.ExecuteNonQuery();

                if (_conn.State == ConnectionState.Open)
                {
                    _conn.Close();
                    _conn.Dispose();
                }

                return rowAffected;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public object GetValueFromSQL(string _connStr, string _sql, params object[] values)
        {
            DataTable _dt = new DataTable();

            _dt = GetDataTable(_connStr, _sql, values);

            if (_dt.Rows.Count > 0)
                return _dt.Rows[0][0];
            else return null;
        }

        #endregion
    }
}
