using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmsAlarmExe
{
	public partial class Form1 : Form
	{

		OracleLibrary __lb = new OracleLibrary();
		List<Inverter> __ListInverter = new List<Inverter>();
		List<Station> __ListStation = new List<Station>();
		Dictionary<string, string> __DictionaryText = new Dictionary<string, string>();
		public void CreateDictionary()
		{
			__DictionaryText.Add("0001", "Communication Error");
			__DictionaryText.Add("0002", "Station Voltage L2 Fault");
			__DictionaryText.Add("0003", "Station Voltage L1 Fault");
			__DictionaryText.Add("0004", "Station Voltage L3 Fault");
			__DictionaryText.Add("0005", "EKK No Connection");
			__DictionaryText.Add("0006", "No Main Voltage");
			__DictionaryText.Add("0007", " : No Production");
			__DictionaryText.Add("0008", " : AC Voltage Fault");
			__DictionaryText.Add("0009", "AC Power at Critical Level");


		}

		private static void SetCultureInfo()
		{
			System.Globalization.CultureInfo _CultureInfo = new System.Globalization.CultureInfo("tr-TR");
			_CultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
			_CultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
			_CultureInfo.NumberFormat.NumberDecimalSeparator = ",";
			_CultureInfo.NumberFormat.NumberGroupSeparator = ".";
			_CultureInfo.NumberFormat.PercentDecimalSeparator = ",";
			_CultureInfo.NumberFormat.PercentGroupSeparator = ".";
			System.Threading.Thread.CurrentThread.CurrentCulture = _CultureInfo;
		}
		public Form1()
		{
			InitializeComponent();

			CreateDictionary();

			SetCultureInfo();

		}
		private class Inverter
		{
			public int? Id { get; set; }
			public string Name { get; set; }
		}

		private class Station
		{
			public int? StationId { get; set; }
			public int? CompanyId { get; set; }
			public int? IsEkk { get; set; }
			public string Name { get; set; }
		}

		public class AlarmSms
		{

			public string Text { get; set; }
			public string PhoneNumber { get; set; }


		}
		public class AlarmStatus
		{
			public int ID { get; set; }
			public string USERID { get; set; }
			public int? STATIONID { get; set; }
			public int? INVERTERID { get; set; }
			public string ERRORNUMBER { get; set; }
			public string MAILADDRESS { get; set; }
			public string PHONENO { get; set; }
			public string DESCRIPTION { get; set; }
			public DateTime? TARIH { get; set; }
		}
		public class AlarmRoles_DTO
		{

			public string LAST_ALARM_STATUS_ID { get; set; }


		}
		public class AlarmStatus_DTO
		{

			public int ID { get; set; }
			public string USERID { get; set; }
			public int? STATIONID { get; set; }
			public string STATION_NAME { get; set; }
			public int? INVERTERID { get; set; }
			public string ERRORNUMBER { get; set; }
			public string ERRORNAME { get; set; }
			public string MAILADDRESS { get; set; }
			public string PHONENO { get; set; }
			public string INVERTERNAME { get; set; }
			public string START_DATE { get; set; }
			public int? LAST_ALARM_STATUS_ID { get; set; }

		}



		public static void SendSms(string phonenumber, string text)
		{
			try
			{
				

				string html = string.Empty;
				string url = @"https://api.netgsm.com.tr/sms/send/get/?usercode=8503076369&password=ESSO4331437&gsmno=" + phonenumber + "&message=" + text + "&msgheader=8503076369";

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.AutomaticDecompression = DecompressionMethods.GZip;

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					html = reader.ReadToEnd();
				}

				Console.WriteLine(html);

			}
			catch (Exception ex)
			{

			}




		}


		private void btnStart_Click(object sender, EventArgs e)
		{
			DevExpress.XtraEditors.SimpleButton _btn = (DevExpress.XtraEditors.SimpleButton)sender;

			if (_btn.Text == "Start Alarm")
			{
				timer1_Tick(null, null);

				_btn.Text = "Stop Alarm";

				timer1.Start();
			}
			else
			{
				_btn.Text = "Start Alarm";

				timer1.Stop();
			}
		}
		private string SqlSelectTblAlarmStatusDto()
		{


			string sqlSelectTblStatus = "SELECT " +
				"inv.NAME," +
				"x.START_DATE," +
				"us.PHONENO ," +
				"t2.STATION_NAME ," +
				"t2.LAST_ALARM_STATUS_ID ," +
				"x.ID , " +
				"x.INVERTER_ID ," +
				"t2.USER_ID," +
				"t2.STATION_ID ," +
				"t2.ERROR_NAME," +
				"t2.ERROR_NUMBER ," +
				"us.\"UserName\" " +
				"FROM LOG724DB.TBL_ALARM_STATUS x  " +
				"JOIN TBL_ALARM_ROLES  t2  ON x.ERROR_NUMBER  = t2.ERROR_NUMBER AND t2.STATION_ID  = x.STATION_ID " +
				"JOIN \"AspNetUsers\"  us  ON t2.USER_ID  = us.\"Id\"" +
				"LEFT JOIN TBL_INVERTER inv ON x.INVERTER_ID = inv.ID " +
				"WHERE x.END_DATE IS NULL AND us.SEND_SMS = 1 AND t2.LAST_ALARM_STATUS_ID < x.ID ";

			return sqlSelectTblStatus;


		}
		private string SqlUpdateTblAlarmRoles(string userId, string stationId, string errorNumber, string maxId)
		{


			string sqlUpdateTblAlarmRoles = "UPDATE " +
				"LOG724DB.TBL_ALARM_ROLES x " +
				"SET LAST_ALARM_STATUS_ID = +'" + maxId + "'" +
				"WHERE x.USER_ID = '" + userId + "' AND x.STATION_ID = '" + stationId + "' AND x.ERROR_NUMBER = '" + errorNumber + "'";

			return sqlUpdateTblAlarmRoles;

		}

		private List<AlarmRoles_DTO> TblAlarmRolesUpdate(string userId, string stationId, string errorNumber, string maxId)
		{
			string __ConnectionString = "User Id=LOG724DB;Password=Orcl1881;Data Source=168.119.33.106/ESSODB";
			OracleConnection orclCon = new OracleConnection(__ConnectionString);

			if (orclCon.State == ConnectionState.Closed)
			{
				orclCon.Open();
			}

			List<AlarmRoles_DTO> tblDst2 = new List<AlarmRoles_DTO>();

			try
			{
				if (orclCon.State == ConnectionState.Closed)
				{

					orclCon.Open();

				}

				using (OracleCommand cmd = orclCon.CreateCommand())
				{
					cmd.CommandText = SqlUpdateTblAlarmRoles(userId, stationId, errorNumber, maxId);

					using (var _reader = cmd.ExecuteReader())
					{
						tblDst2 = TblAlarmRolesDtoUpdate(_reader);
					}

				}
				if (orclCon.State == ConnectionState.Open)
				{
					orclCon.Close();
				}

				return tblDst2;
			}
			catch (Exception ex)
			{
				return null;
			}

		}

		private List<AlarmRoles_DTO> TblAlarmRolesDtoUpdate(OracleDataReader _reader)
		{

			List<AlarmRoles_DTO> tblAlarm = new List<AlarmRoles_DTO>();
			try
			{

				while (_reader.Read())
				{
					tblAlarm.Add(new AlarmRoles_DTO()
					{


						LAST_ALARM_STATUS_ID = Convert.ToString(_reader["LAST_ALARM_STATUS_ID"])

					});


				}

			}
			catch (Exception ex)
			{
			}

			return (tblAlarm);
		}

		private List<AlarmStatus_DTO> TblAlarmStatusRead()
		{
			string __ConnectionString = "User Id=LOG724DB;Password=Orcl1881;Data Source=168.119.33.106/ESSODB";
			OracleConnection orclCon = new OracleConnection(__ConnectionString);

			if (orclCon.State == ConnectionState.Closed)
			{
				orclCon.Open();
			}

			List<AlarmStatus_DTO> tblDst2 = new List<AlarmStatus_DTO>();

			try
			{
				if (orclCon.State == ConnectionState.Closed)
				{

					orclCon.Open();

				}

				using (OracleCommand cmd = orclCon.CreateCommand())
				{
					cmd.CommandText = SqlSelectTblAlarmStatusDto();

					using (var _reader = cmd.ExecuteReader())
					{
						tblDst2 = TblAlarmStatusDtoRead(_reader);
					}

				}
				if (orclCon.State == ConnectionState.Open)
				{
					orclCon.Close();
				}

				return tblDst2;
			}
			catch (Exception ex)
			{
				return null;
			}

		}


		private List<AlarmStatus_DTO> TblAlarmStatusDtoRead(OracleDataReader _reader)
		{

			List<AlarmStatus_DTO> tblAlarm = new List<AlarmStatus_DTO>();
			try
			{

				while (_reader.Read())
				{
					tblAlarm.Add(new AlarmStatus_DTO()
					{

						ID = Convert.ToInt32(_reader["ID"]),
						USERID = Convert.ToString(_reader["USER_ID"]),
						STATIONID = Convert.ToInt32(_reader["STATION_ID"]),
						STATION_NAME = Convert.ToString(_reader["STATION_NAME"]),
						INVERTERID = Convert.ToInt32(_reader["INVERTER_ID"]),
						ERRORNUMBER = Convert.ToString(_reader["ERROR_NUMBER"]),
						ERRORNAME = Convert.ToString(_reader["ERROR_NAME"]),
						INVERTERNAME = Convert.ToString(_reader["NAME"]),
						START_DATE = Convert.ToString(_reader["START_DATE"]),
						PHONENO = Convert.ToString(_reader["PHONENO"]),
						LAST_ALARM_STATUS_ID = Convert.ToInt32(_reader["LAST_ALARM_STATUS_ID"])

					});


				}

			}
			catch (Exception ex)
			{
			}

			return (tblAlarm);
		}
		private void timer1_Tick(object sender, EventArgs e)
		{

			if (DateTime.Now.Hour <= 23 && DateTime.Now.Hour >= 05)
			{
			

				var alarmDtoList = TblAlarmStatusRead();


				List<int> inverterStationList = new List<int>();


				List<string> userList = new List<string>();

				for (int i = 0; i < alarmDtoList.Count; i++)
				{
					if (!userList.Contains(alarmDtoList[i].USERID))
					{

						userList.Add(alarmDtoList[i].USERID);
					}
				}


				for (int i = 0; i < userList.Count; i++)//--
				{
					string smsText = "";
					List<string> userstationList = new List<string>();

					var alarmUserList = alarmDtoList.Where(x => x.USERID == userList[i].ToString()).ToList();


					for (int j = 0; j < alarmUserList.Count; j++)
					{
						if (!userstationList.Contains(alarmUserList[j].STATIONID.ToString()))
						{

							userstationList.Add(alarmUserList[j].STATIONID.ToString());
						}
					}



					for (int k = 0; k < userstationList.Count; k++)//--
					{
						List<string> userStationErrorList = new List<string>();


						var stationId = userstationList[k].ToInt32();
						var alarmUserStationList = alarmDtoList.Where(x => x.USERID == userList[i].ToString() && x.STATIONID == stationId).ToList();

						for (int m = 0; m < alarmUserStationList.Count; m++)
						{
							if (!userStationErrorList.Contains(alarmUserStationList[m].ERRORNUMBER.ToString()))
							{

								userStationErrorList.Add(alarmUserStationList[m].ERRORNUMBER.ToString());
							}
						}




						for (int l = 0; l < userStationErrorList.Count; l++)
						{

							var alarmUserStationErrorList = alarmDtoList.Where(x => x.USERID == userList[i].ToString() && x.STATIONID == stationId && x.ERRORNUMBER == userStationErrorList[l]).ToList();

							for (int n = 0; n < alarmUserStationErrorList.Count; n++)
							{

								if (alarmUserStationErrorList[n].INVERTERID > 0)
								{
									smsText += alarmUserStationErrorList[n].STATION_NAME + " - " + alarmUserStationErrorList[n].INVERTERNAME + ".  Hata Mesajı: " + alarmUserStationErrorList[n].ERRORNAME + " - " + alarmUserStationErrorList[n].START_DATE + "          ";
								}
								else
								{
									smsText += alarmUserStationErrorList[n].STATION_NAME + " - Hata Mesajı: " + alarmUserStationErrorList[n].ERRORNAME + " - " + alarmUserStationErrorList[n].START_DATE + "         ";

								}
							}
							var max_id = alarmUserStationErrorList.Max(x => x.ID);
							TblAlarmRolesUpdate(userList[i].ToString(), stationId.ToString(), userStationErrorList[l], max_id.ToString());



						}

					}

					SendSms(alarmUserList[i].PHONENO, smsText);

				}



			}

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}
}
