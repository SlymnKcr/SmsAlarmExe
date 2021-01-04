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
				var deneme = "test mesaji";
				string html = string.Empty;
				string url = @"https://api.netgsm.com.tr/sms/send/get/?usercode=8503076369&password=ESSO4331437&gsmno=" + "0905548455781" + "&message=" + "67788" + "&msgheader=8503076369";

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
		public static void SendMail(string mail, string content)
		{
			try
			{
				string htmlBody = string.Empty;
				string htmlBody2 = string.Empty;

				// esso mail ayarları                           
				//SmtpClient smtp = new SmtpClient();                
				//smtp.Host = "mail.esso.com.tr";
				//smtp.Port = 587;
				//smtp.EnableSsl = false;
				//smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				//smtp.UseDefaultCredentials = false;
				//smtp.Credentials = new NetworkCredential("esoft@esso.com.tr", "Esoft1234567?");
				//using (var message = new MailMessage("esoft@esso.com.tr", mail))                


				// yandex mail ayarları
				SmtpClient smtp = new SmtpClient();
				smtp.Host = "smtp.yandex.com";
				smtp.Port = 587;
				smtp.EnableSsl = true;
				smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtp.Credentials = new NetworkCredential("report@esoft-report.com", "Hyu%ph45");
				using (var message = new MailMessage("report@esoft-report.com", mail))
				{
					message.Subject = "E-Soft Alarms";
					var body = new StringBuilder();
					body.AppendLine(content);
					body.AppendLine("</table><br/>");

					message.Body = body.ToString();
					message.IsBodyHtml = true;
					smtp.Send(message);
				}
			}
			catch (Exception ex)
			{

			}




		}

		private DataTable Get_Inv_Condition3(int stationId, int companyId, DataTable _dtSms2)
		{
			DataTable _result = new DataTable();

			_result.Columns.Add("STATIONID", typeof(int));
			_result.Columns.Add("INVERTERID", typeof(int));
			_result.Columns.Add("ERRORNUMBER", typeof(string));
			_result.Columns.Add("PHONENO", typeof(string));
			_result.Columns.Add("USERID", typeof(string));
			_result.Columns.Add("DESCRIPTION", typeof(string));
			_result.Columns.Add("TARIH", typeof(DateTime));

			for (int i = 0; i < _result.Columns.Count; i++)
			{
				_result.Columns[i].ReadOnly = false;
			}

			//string _Sql = "SELECT * FROM TBL_ALARM_STATUS WHERE STATUS = 1 AND STATION_ID = :p1";

			string _Sql = "SELECT * FROM TBL_ALARM_STATUS JOIN TBL_ALARM_DESC ON TBL_ALARM_STATUS.ERROR_NUMBER = TBL_ALARM_DESC.ERROR_NUMBER WHERE TYPE = 1 AND STATION_ID = :p1 AND TBL_ALARM_STATUS.STATUS = 1";

			DataTable _dtInvOzet = __lb.GetDataTable(null, _Sql, stationId);

			for (int i = 0; i < _dtInvOzet.Rows.Count; i++)
			{
				for (int j = 0; j < _dtSms2.Rows.Count; j++)
				{
					_result.Rows.Add(stationId, _dtInvOzet.Rows[i]["INVERTER_ID"].ToInt32(), _dtInvOzet.Rows[i]["ERROR_NUMBER"].ToString(), _dtSms2.Rows[j]["PHONENO"].ToString(), _dtSms2.Rows[j]["Id"].ToString(), _dtInvOzet.Rows[i]["ID"].ToString(), _dtInvOzet.Rows[i]["START_DATE"].ToDateTime());
				}

				//__lb.SqlExecute(null, "UPDATE TBL_ALARM_STATUS SET STATUS = 0 WHERE ID = :p1", _dtInvOzet.Rows[i]["ID"].ToInt32());
			}



			return _result;
		}
		private DataTable Get_Inv_Condition2(int stationId, int companyId, DataTable _dtSms)
		{
			DataTable _result = new DataTable();

			_result.Columns.Add("STATIONID", typeof(int));
			_result.Columns.Add("INVERTERID", typeof(int));
			_result.Columns.Add("ERRORNUMBER", typeof(string));
			_result.Columns.Add("PHONENO", typeof(string));
			_result.Columns.Add("DESCRIPTION", typeof(string));
			_result.Columns.Add("TARIH", typeof(DateTime));

			for (int i = 0; i < _result.Columns.Count; i++)
			{
				_result.Columns[i].ReadOnly = false;
			}

			//string _Sql = "SELECT * FROM TBL_ALARM_STATUS WHERE STATUS = 1 AND STATION_ID = :p1";

			string _Sql = "SELECT * FROM TBL_ALARM_STATUS JOIN TBL_ALARM_DESC ON TBL_ALARM_STATUS.ERROR_NUMBER = TBL_ALARM_DESC.ERROR_NUMBER WHERE TYPE = 1 AND STATION_ID = :p1 AND TBL_ALARM_STATUS.STATUS = 1";

			DataTable _dtInvOzet = __lb.GetDataTable(null, _Sql, stationId);

			for (int i = 0; i < _dtInvOzet.Rows.Count; i++)
			{
				for (int j = 0; j < _dtSms.Rows.Count; j++)
				{
					_result.Rows.Add(stationId, _dtInvOzet.Rows[i]["INVERTER_ID"].ToInt32(), _dtInvOzet.Rows[i]["ERROR_NUMBER"].ToString(), _dtSms.Rows[j]["PHONENO"].ToString(), _dtInvOzet.Rows[i]["ID"].ToString(), _dtInvOzet.Rows[i]["START_DATE"].ToDateTime());
				}

				//__lb.SqlExecute(null, "UPDATE TBL_ALARM_STATUS SET STATUS = 0 WHERE ID = :p1", _dtInvOzet.Rows[i]["ID"].ToInt32());
			}



			return _result;
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






			//			SELECT
			//inv.NAME ,
			//x.START_DATE ,
			//x.STATION_ID ,
			//us.PHONENO ,
			//t2.STATION_NAME ,
			//t2.LAST_ALARM_STATUS_ID ,
			//x.ID , 
			//x.INVERTER_ID ,
			//t2.USER_ID,
			//t2.STATION_ID ,
			//t2.ERROR_NAME,
			//t2.ERROR_NUMBER ,
			//us."UserName"
			//FROM LOG724DB.TBL_ALARM_STATUS x
			//JOIN TBL_ALARM_ROLES  t2 ON x.ERROR_NUMBER = t2.ERROR_NUMBER AND t2.STATION_ID = x.STATION_ID
			//JOIN "AspNetUsers"  us ON t2.USER_ID = us."Id"
			//LEFT JOIN TBL_INVERTER inv ON x.INVERTER_ID = inv.ID
			//WHERE x.END_DATE IS NULL AND us.SEND_SMS = 1 AND t2.LAST_ALARM_STATUS_ID < x.ID
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

			if (true/*DateTime.Now.Hour <= 23 && DateTime.Now.Hour >= 05*/)
			{
				SendSms("5548455781", "dasdsadas");

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



						//alarmUserStationList

					}

					SendSms(alarmUserList[i].PHONENO, smsText);

				}

















				//var alarmInverterList = alarmDtoList.Where(x => x.INVERTERID > 0 && x.USERID == userList[i].ToString()).ToList();

				//for (int j = 0; j < alarmInverterList.Count; j++)
				//{

				//	if (!inverterStationList.Contains((int)alarmInverterList[j].STATIONID)) {
				//		inverterStationList.Add((int)alarmInverterList[j].STATIONID);
				//	}


				//}

				//               for (int k = 0; k < inverterStationList.Count; k++)
				//               {
				//	var inverterStationAlarms = alarmDtoList.Where(x => x.STATIONID == inverterStationList[k] && x.USERID == userList[i].ToString()).OrderBy(x=>x.ID).ToList();
				//	var maxAlarmId = inverterStationAlarms[inverterStationAlarms.Count - 1].ID;//Update LastAlarmstatusId alanına set edilecek.
				//}

				//var alarmStation = alarmDtoList.Where(x => x.INVERTERID == 0 && x.USERID == userList[i].ToString()).ToList();




				var b = 1;
				//for (int i = 0; i < alarmDtoList.Count; i++)
				//            {
				//	var USERID = alarmDtoList[0].USERID;
				//	var STATIONID = alarmDtoList[0].STATIONID;
				//	var ERRORNUMBER = alarmDtoList[0].ERRORNUMBER;
				//	var ID = alarmDtoList[0].ID;
				//	//var alarmDtoList = TblAlarmStatusUpdate();
				//var text=	_cAlarmSms.Text += _cStation.Name + " - " + _AlarmDefinition + " - " + _TempAlarmStatus[j].TARIH + "       ";

				//}
				//for (int i = 0; i < alarmDtoList.Count; i++)
				//            {
				//                if (_ActivePhoneNumber != _ListAlarmStatus2[i].PHONENO)
				//                {
				//                    _ActivePhoneNumber = _ListAlarmStatus2[i].PHONENO;

				//                    _SmsList.Add(_ActivePhoneNumber);
				//                }
				//            }


				//GetParameters();
				//            for (int i = 0; i < _MailList.Count; i++)
				//            {
				//                List<AlarmStatus> _TempAlarmStatus = _ListAlarmStatus.Where(x => x.MAILADDRESS == _MailList[i]).OrderBy(x => x.STATIONID).ToList();

				//                AlarmMail _cAlarmMail = new AlarmMail();
				//	//AlarmSms _cAlarmSms = new AlarmSms();

				//                _cAlarmMail.MailAdress = _MailList[i];
				//                _cAlarmMail.Body = "<img src = 'http://www.e-soft.com.tr/images/EsoftLogo/EsoftLogo.png' height ='70px'><br><br>";
				//                _cAlarmMail.Body += "<table style='border-collapse:collapse;text-align:center;'><tr style=\"background-color:#167f92;font-weight:bold;font-size:13pt;color:#FFFFFF;\"><td style='padding:3px 20px;'>STATION NAME</td><td style='border-left:solid 0.5px #FFFFFF;padding:3px 20px;'>ALARM DEFINITION</td><td style='border-left:solid 0.5px #FFFFFF;padding:3px 20px;'>ALARM DATE</td></tr>";

				//                Station _cStation = new Station();


				//	for (int j = 0; j < _TempAlarmStatus.Count; j++)
				//	{
				//		_cStation = __ListStation.Where(x => x.StationId == _TempAlarmStatus[j].STATIONID).FirstOrDefault();

				//		Inverter _cInverter = __ListInverter.Where(x => x.Id == _TempAlarmStatus[j].INVERTERID).FirstOrDefault();

				//		string _AlarmDefinition = "";

				//		if (_cStation != null)
				//		{

				//			for (int k = 0; k < alarmDt.Rows.Count; k++)
				//			{
				//				if (alarmDt.Rows[k]["ERROR_NUMBER"].ToString() == _TempAlarmStatus[j].ERRORNUMBER)
				//				{

				//					if (_TempAlarmStatus[j].INVERTERID > 0)
				//					{
				//						_AlarmDefinition = _cInverter.Name + " ";
				//					}

				//					_AlarmDefinition += alarmDt.Rows[k]["ERROR_DESC"].ToString();
				//					break;
				//				}
				//			}

				//		}

				//		int _kalan = j % 2;

				//		if (_kalan > 0)
				//		{
				//			_cAlarmMail.Body += "<tr style=\"color:#333333;background-color:#ebf2f3;\" ><td style='padding:3px 20px;'>" + _cStation.Name + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _AlarmDefinition + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _TempAlarmStatus[j].TARIH + "</td></tr>";
				//			//_cAlarmSms.Text += _cStation.Name + " - " + _AlarmDefinition + " - " + _TempAlarmStatus[j].TARIH + "       ";
				//		}
				//		else { 
				//		_cAlarmMail.Body += "<tr style=\"color:#333333\" ><td style='padding:3px 20px;'>" + _cStation.Name + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _AlarmDefinition + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _TempAlarmStatus[j].TARIH + "</td></tr>";
				//			//_cAlarmSms.Text += _cStation.Name + " - " + _AlarmDefinition + " - " + _TempAlarmStatus[j].TARIH + "       ";
				//		}


				//                }

				//                _cAlarmMail.Body += "</table>";
				//                //SendMail(_cAlarmMail.MailAdress, _cAlarmMail.Body);
				//	//SendSms("5548455781", _cAlarmSms.Text);
				//                System.Threading.Thread.Sleep(5000);

				//            }
				//for (int i = 0; i < _Sms2List.Count; i++)
				//{
				//	List<AlarmStatus> _TempAlarmStatus = _ListAlarmStatus3.Where(x => x.PHONENO == _SmsList[i]).OrderBy(x => x.STATIONID).ToList();

				//	//AlarmMail _cAlarmMail = new AlarmMail();
				//	AlarmSms _cAlarmSms = new AlarmSms();

				//	_cAlarmSms.PhoneNumber = _SmsList[i];
				//	//_cAlarmMail.Body = "<img src = 'http://www.e-soft.com.tr/images/EsoftLogo/EsoftLogo.png' height ='70px'><br><br>";
				//	//_cAlarmMail.Body += "<table style='border-collapse:collapse;text-align:center;'><tr style=\"background-color:#167f92;font-weight:bold;font-size:13pt;color:#FFFFFF;\"><td style='padding:3px 20px;'>STATION NAME</td><td style='border-left:solid 0.5px #FFFFFF;padding:3px 20px;'>ALARM DEFINITION</td><td style='border-left:solid 0.5px #FFFFFF;padding:3px 20px;'>ALARM DATE</td></tr>";

				//	Station _cStation = new Station();


				//	for (int j = 0; j < _TempAlarmStatus.Count; j++)
				//	{

				//		AlarmRoles roles = new AlarmRoles();

				//		string _Sql4 = "SELECT x.*,x.ROWID FROM LOG724DB.TBL_ALARM_ROLES x" +
				//		"WHERE x.USER_ID = '" + _Sms2List[i] + "'  AND x.STATION_ID = " + _TempAlarmStatus[j].STATIONID + " AND x.ERROR_NUMBER = " + _TempAlarmStatus[j].ERRORNUMBER;


				//		_cStation = __ListStation.Where(x => x.StationId == _TempAlarmStatus[j].STATIONID).FirstOrDefault();

				//		Inverter _cInverter = __ListInverter.Where(x => x.Id == _TempAlarmStatus[j].INVERTERID).FirstOrDefault();

				//		string _AlarmDefinition = "";

				//		if (_cStation != null)
				//		{



				//                        for (int k = 0; k < alarmDt.Rows.Count; k++)
				//			{

				//				if (alarmDt.Rows[k]["ERROR_NUMBER"].ToString() == _TempAlarmStatus[j].ERRORNUMBER)
				//				{

				//					if (_TempAlarmStatus[j].INVERTERID > 0)
				//					{
				//						_AlarmDefinition = _cInverter.Name + " ";
				//					}

				//					_AlarmDefinition += alarmDt.Rows[k]["ERROR_DESC"].ToString();
				//					break;
				//				}
				//			}

				//		}

				//		int _kalan = j % 2;
				//		//var a = _ListAlarmRoles.Where(x => x.STATIONID == _SmsList[i]).OrderBy(x => x.STATIONID).FirstOrDefault();

				//		if (_kalan > 0)
				//		{

				//			//_cAlarmMail.Body += "<tr style=\"color:#333333;background-color:#ebf2f3;\" ><td style='padding:3px 20px;'>" + _cStation.Name + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _AlarmDefinition + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _TempAlarmStatus[j].TARIH + "</td></tr>";
				//			//if (_TempAlarmStatus[j].INVERTERID) { 

				//			//}
				//			_cAlarmSms.Text += _cStation.Name + " - " + _AlarmDefinition + " - " + _TempAlarmStatus[j].TARIH + "       ";
				//		}
				//		else
				//		{
				//			//_cAlarmMail.Body += "<tr style=\"color:#333333\" ><td style='padding:3px 20px;'>" + _cStation.Name + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _AlarmDefinition + "</td><td style='border-left:solid 0.5px #DDDDDD;padding:3px 20px;'>" + _TempAlarmStatus[j].TARIH + "</td></tr>";
				//			_cAlarmSms.Text += _cStation.Name + " - " + _AlarmDefinition + " - " + _TempAlarmStatus[j].TARIH + "       ";
				//		}


				//	}


				//	//_cAlarmMail.Body += "</table>";
				//	//SendMail(_cAlarmMail.MailAdress, _cAlarmMail.Body);
				//	SendSms(_cAlarmSms.PhoneNumber, _cAlarmSms.Text);
				//	System.Threading.Thread.Sleep(5000);

				//}
			}

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}
}
