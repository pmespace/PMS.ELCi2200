using System.Runtime.InteropServices;
using System;
using System.Windows.Forms;
using System.Threading;
using CHPN.APDU;
using CHPN.IPDU;
using CHPN;
using COMMON;
using System.IO;

namespace TestCheck
{
	public class CCheck
	{
		#region constants
		private const string F03_VALUE = "000000";
		private const string F22_VALUE = "042";
		private const string F25_VALUE = "00";
		private const string F45_VALUE = "999330000001001";
		private const string F46_VALUE = "0300";
		private const string F49_VALUE = "978";
		#endregion

		#region properties
		#region private
		/// <summary>
		/// The primary address to use
		/// </summary>
		private string PrimaryIP { get; set; }
		/// <summary>
		/// The secondary address to use
		/// </summary>
		private string SecondaryIP { get; set; }
		/// <summary>
		/// The primary port to use
		/// </summary>
		private uint PrimaryPort { get; set; }
		/// <summary>
		/// The secondary port to use
		/// </summary>
		private uint SecondaryPort { get; set; }
		/// <summary>
		/// Primary servername to authenticate against
		/// </summary>
		private string PrimaryServerName { get; set; }
		/// <summary>
		/// Secondary servername to authenticate against
		/// </summary>
		private string SecondaryServerName { get; set; }
		/// <summary>
		/// Retrieve the next transaction ID to use and save it for next use
		/// </summary>
		private uint NextTxnID
		{
			get
			{
				Settings.F11_LastTxnID++;
				CommitSettings();
				return (uint)Settings.F11_LastTxnID;
			}
		}

		// Private definitions
		private CPIs _pis = new CPIs();
		private CFields _fields = new CFields();
		private CJson<CSettings> _json = null;
		private CJson<CNetworkSettings> _jsonnetwork = null;
		private ManualResetEvent _threadevent = new ManualResetEvent(true);
		#endregion

		#region public
		/// <summary>
		/// A CHPN object to control CHPN requests
		/// </summary>
		public CCHPN Chpn { get; private set; } = new CCHPN();

		/// <summary>
		/// Les paramètres de fonctionement utilisés par l'objet CHPN
		/// </summary>
		public CSettings Settings
		{
			get => _settings;
			private set => _settings = value;
		}
		private CSettings _settings;
		/// <summary>
		/// Les paramètres réseau utilisés par l'objet CHPN
		/// </summary>
		public CNetworkSettings NetworkSettings
		{
			get => _networksettings;
			private set => _networksettings = value;
		}
		private CNetworkSettings _networksettings;
		/// <summary>
		/// La référence de la transaction attribuée par le commerçant
		/// </summary>
		public string MerchantReference { get; set; } = string.Empty;
		/// <summary>
		/// Le service CHPN à utiliser 
		/// </summary>
		public Service ServiceToUse
		{
			get => _servicetouse;
			set
			{
				if (Service.GarantieCheque == value || Service.InterrogationFNCI == value)
					_servicetouse = value;
			}
		}
		private Service _servicetouse = Service.GarantieCheque;
		/// <summary>
		/// Code to use if a referral was requested
		/// </summary>
		public string ReferralCode
		{
			get => _referralcode;
			set
			{
				try
				{
					_referralcode = value.Substring(0, 6);
				}
				catch (Exception)
				{
					_referralcode = value;// + new string(' ', 6 - value.Length);
				}

			}
		}
		private string _referralcode = string.Empty;
		/// <summary>
		/// Name of the settings file
		/// </summary>
		public string SettingsFileName
		{
			get => _settingsfilename;
			private set
			{
				_settingsfilename = value;
				//CLog.LogFileName = SettingsFileName;
			}
		}
		private string _settingsfilename = string.Empty;
		/// <summary>
		/// Name of the network settings file 
		/// </summary>
		public string NetworkSettingsFileName { get; private set; } = string.Empty;
		/// <summary>
		/// Name of the log file 
		/// </summary>
		public string LogFileName
		{
			get => CLog.LogFileName;
			set => CLog.LogFileName = value;
		}
		/// <summary>
		/// Name of the network settings file 
		/// </summary>
		public string DecisionsFileName { get; private set; } = string.Empty;
		#endregion
		#endregion

		#region constructors
		/// <summary>
		/// Constructor loads merchant settings
		/// </summary>
		public CCheck()
		{
		}
		#endregion

		#region public forms
		/// <summary>
		/// Displays the Settings dialog
		/// </summary>
		/// <returns>DialogResult.OK if settings have been changed, anything else otherwise</returns>
		public DialogResult DisplaySettings()
		{
			DialogResult res;
			FSettings f = new FSettings();
			f.settings = Settings;
			CLog.Add("DIALOG: DisplaySettings");
			if (DialogResult.Cancel != (res = f.ShowDialog()))
			{
				CLog.Add("DIALOG: DisplaySettings => paramètres mis à jour");
				bool fOK = CommitSettings(f.settings);
				UpdateSettings();
			}
			return res;
		}
		/// <summary>
		/// Displays the Network Settings dialog
		/// </summary>
		/// <returns>DialogResult.OK if settings have been changed, anything else otherwise</returns>
		public DialogResult DisplayNetworkSettings()
		{
			DialogResult res;
			FNetworkSettings f = new FNetworkSettings();
			f.settings = NetworkSettings;
			CLog.Add("DIALOG: DisplayNetworkSettings");
			if (DialogResult.Cancel != (res = f.ShowDialog()))
			{
				CLog.Add("DIALOG: DisplayNetworkSettings => paramètres réseau mis à jour");
				bool fOK = CommitNetworkSettings(f.settings);
				UpdateNetworkSettings();
			}
			return res;
		}
		#endregion

		#region public methods
		/// <summary>
		/// Set settings file names
		/// This must be the first method called by any program using COM to access the CHPN object
		/// </summary>
		/// <param name="fname">Settings file name</param>
		/// <param name="fnamenetwork">Network settings file name</param>
		public void OpenSettings(string fname, string fnamenetwork)
		{
			_json = new CJson<CSettings>(fname);
			SettingsFileName = _json.FileName;
			UpdateSettings();
			_jsonnetwork = new CJson<CNetworkSettings>(fnamenetwork);
			NetworkSettingsFileName = _jsonnetwork.FileName;
			UpdateNetworkSettings();

			try
			{
				FileInfo fi = new FileInfo(SettingsFileName);
				if (null != (DecisionsFileName = CMisc.VerifyDirectory(fi.DirectoryName, true, true)))
				{
					DecisionsFileName += "guarantee.decisions.json";
				}
			}
			catch (Exception)
			{
				DecisionsFileName = null;
			}
		}
		/// <summary>
		/// Send a message to the server and get the answer
		/// </summary>
		/// <returns>The <see cref="CRecord"/> class containing all information about the transaction</returns>
		public CRecord ProcessMessage()
		{
			ReferralCode = string.Empty;
			CRecord record = new CRecord();
			bool authoriseTransaction = !Settings.AllowOfflineTransactions || Settings.AuthorisationThreshold < Chpn.F04_InputAmount;

			record.merchantRef = MerchantReference;
			record.bankID = Chpn.F32_InputBankID = Settings.F32_BankID;
			Chpn.F37_InputIDC = Settings.F37_IDC;
			record.merchantID = Chpn.F42_InputIDCF = Settings.F42_IDCF;
			record.amount = (int)Chpn.F04_InputAmount;
			record.currency = Chpn.F49_InputCurrency;
			record.poiID = string.Empty;
			record.poiConfig = string.Empty;
			record.checknoteID = Chpn.F35_InputCMC7;
			record.forcedStatus = false;
			Chpn.F11_InputTransactionID = NextTxnID;

			// should there be an thorisation ?
			if (authoriseTransaction)
			{
				CLog.Add("Transaction requérant une autorisation - Montant: " + Chpn.F04_InputAmount + " - Autorisation de faire des transactions offline: " + Settings.AllowOfflineTransactions.ToString().ToUpper());
				// prepare the message to send
				record.Request = Chpn.PrepareMessage(ServiceToUse);
				CLog.Add(Chpn.DescribeMessage((ServiceToUse == Service.GarantieCheque ? "GARANTIE CHEQUE" : "FNCI") + " - Message à envoyer", record.Request, false));
				// set network preferences
				PrimaryIP = NetworkSettings.ServerIPAddress1;
				PrimaryPort = NetworkSettings.PortNumber1;
				PrimaryServerName = NetworkSettings.ServerName1;
				SecondaryIP = NetworkSettings.ServerIPAddress2;
				SecondaryPort = NetworkSettings.PortNumber2;
				SecondaryServerName = NetworkSettings.ServerName2;
				// try to open a socket to the server
				if (null != (record.Reply = TryToSendData(record.Request, PrimaryIP, PrimaryPort, PrimaryServerName)))
				{ }
				// try to use the backup socket
				else if (null != (record.Reply = TryToSendData(record.Request, SecondaryIP, SecondaryPort, SecondaryServerName)))
				{
					// this one works better, let's use it next time
					NetworkSettings.ServerIPAddress1 = SecondaryIP;
					NetworkSettings.PortNumber1 = SecondaryPort;
					NetworkSettings.ServerName1 = SecondaryServerName;
					NetworkSettings.ServerIPAddress2 = PrimaryIP;
					NetworkSettings.PortNumber2 = PrimaryPort;
					NetworkSettings.ServerName2 = PrimaryServerName;
					// write network settings for next time
					CommitNetworkSettings();
				}
				if (null != record.Reply)
				{
					CLog.Add("Envoi réalisé sur: " + NetworkSettings.ServerIPAddress1 + ":" + NetworkSettings.PortNumber1.ToString());
					CLog.Add(Chpn.DescribeMessage((ServiceToUse == Service.GarantieCheque ? "GARANTIE CHEQUE" : "FNCI") + " - Message reçu", record.Reply, false));

					// to be set when validating the transaction
					//record.transactionDateTime =;
					record.guaranteeResponseCode = Chpn.F40_OutputGuaranteeResponseCode;
					record.guaranteeMessage = Chpn.F43_OutputGuaranteeMessage;
					record.guaranteeSignature = Chpn.F38_OutputGuaranteeSignature;
					record.guaranteeBirthDate = Chpn.F43_InputGuaranteeBirthDate;
					record.guaranteeIdType = CRecord.GetIdType(Chpn.F43_InputGuaranteeIDType);
					record.guaranteeIdDate = Chpn.F43_InputGuaranteeIDTypeDate;
					record.guaranteeCheckType = CRecord.GetCheckType(Chpn.F43_InputGuaranteeCheckType);
					record.fnciResponseCode = Chpn.F39_OutputFnciResponseCode;
					record.fnciMessage = Chpn.F44_OutputFnciMessage;
					record.fnciSignature = Chpn.F44_OutputFnciSignature;
					record.fnciCounterDay = Chpn.F44_OutputFnciCounterIntraDay.ToString("00");
					record.fnciCounterN = Chpn.F44_OutputFnciCounterN.ToString("00");
					record.fnciCounterX = Chpn.F44_OutputFnciCounterX.ToString("00");
					record.fnciRlmc = Chpn.F44_OutputRLMC;
					record.transactionSequenceNumber = Chpn.F11_OutputTransactionID.ToString("000000");
					record.serverTransactionDateTime = GetServerDateTime(Chpn.F07_Output);

					record.authorisationType = (Service.GarantieCheque == ServiceToUse ? AuthorisationType.guarantee : AuthorisationType.fnci);
					record.finalResult = AutomaticResult(record.Reply);
				}
				else
				{
					// the transaction could not be authorised
					record.authorisationType = AuthorisationType.offline;
					record.finalResult = FinalResult.declined;
				}
			}
			else
			{
				CLog.Add("Transaction acceptée offline - Montant = " + Chpn.F04_InputAmount);
				record.authorisationType = AuthorisationType.offline;
				record.finalResult = FinalResult.acceptedOffline;
			}
			return record;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public bool AcceptPayment(ref CRecord record)
		{
			// impossible to accept a declined transaction if this is not allowed
			if ((FinalResult.declined == record.finalResult || FinalResult.referral == record.finalResult) && !Settings.AllowModifyDecline)
				return false;

			if (FinalResult.declined == record.finalResult)
			{
				record.finalResult = FinalResult.acceptedAuthorised;
				record.forcedStatus = true;
			}
			else if (FinalResult.referral == record.finalResult)
			{
				if (!string.IsNullOrEmpty(ReferralCode))
				{
					record.finalResult = FinalResult.acceptedAfterReferral;
					record.guaranteeSignature = ReferralCode;
				}
				else
				{
					record.finalResult = FinalResult.acceptedAuthorised;
					record.forcedStatus = true;
				}
			}
			record.transactionDateTime = GetTransactionDateTime();
			CLog.Add("Sauvegarde d'une transaction chèque acceptée");
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public bool DeclinePayment(ref CRecord record)
		{
			switch (record.finalResult)
			{
				case FinalResult.acceptedOffline:
				case FinalResult.acceptedAuthorised:
				case FinalResult.acceptedAfterReferral:
					record.finalResult = FinalResult.declined;
					record.forcedStatus = true;
					break;
				default:
					break;
			}
			record.transactionDateTime = GetTransactionDateTime();
			CLog.Add("Sauvegarde d'une transaction chèque refusée");
			return true;
		}
		/// <summary>
		/// Save settings
		/// </summary>
		/// <returns></returns>
		public bool CommitSettings(CSettings settings = null) { return (null != _json ? _json.WriteSettings(settings ?? Settings) : false); }
		/// <summary>
		/// Save network settings
		/// </summary>
		/// <returns></returns>
		public bool CommitNetworkSettings(CNetworkSettings settings = null) { return (null != _jsonnetwork ? _jsonnetwork.WriteSettings(settings ?? NetworkSettings) : false); }
		#endregion

		#region private methods
		/// <summary>
		/// Create the IP address and try to send data
		/// </summary>
		/// <param name="ipdu"></param>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <param name="servername"></param>
		/// <returns>The reply as an array of bytes or null if no reply or an error occurred</returns>
		private CIPDU TryToSendData(CIPDU ipdu, string ip, uint port, string servername)
		{
			CStreamClientSettings streamSettings = new CStreamClientSettings(CMisc.FOURBYTES, ip, port);
			streamSettings.ServerName = servername;
			streamSettings.CheckCertificate = !string.IsNullOrEmpty(servername);
			streamSettings.SendTimeout = NetworkSettings.SendTimeout;
			streamSettings.ReceiveTimeout = NetworkSettings.ReceiveTimeout;
			return Chpn.SendMessage(ipdu, streamSettings);
		}
		/// <summary>
		/// Build a transactionDateTime
		/// </summary>
		/// <returns></returns>
		private string GetTransactionDateTime()
		{
			DateTime dt = DateTime.Now;
			return dt.Year.ToString("0000") + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + " " + dt.Hour.ToString("00") + ":" + dt.Minute.ToString("00") + ":" + dt.Second.ToString("00");
		}
		private string GetServerDateTime(string s)
		{
			// warning around midnight 31/12 there can be a problem
			return DateTime.Now.ToString("yyyy") + "-" + Chpn.F07_Output.Substring(0, 2) + "-" + Chpn.F07_Output.Substring(2, 2) + " " + Chpn.F07_Output.Substring(4, 2) + ":" + Chpn.F07_Output.Substring(6, 2) + ":" + Chpn.F07_Output.Substring(8, 2); // warning around midnight 31/12 there can be a problem
		}
		/// <summary>
		/// Get the authorisation automatic result (the one returned by the server)
		/// </summary>
		/// <param name="ipdu"></param>
		/// <returns></returns>
		private FinalResult AutomaticResult(CIPDU ipdu)
		{
			FinalResult res;

			// set the automatic result
			if ((short)APDUs.APDU_9110 == ipdu.APDU.ID.Value)
			{
				//if (responses.ContainsKey(Chpn.F40_OutputGuaranteeResponseCode))
				//	// garantee
				//	switch (Chpn.F40_OutputGuaranteeResponseCodeAsCode)
				//	{
				//		case CodeReponseGarantisseur.F40_ACCEPTEE:
				//		case CodeReponseGarantisseur.F40_ACCEPTEE_SOUS_RESERVE:
				//			res = FinalResult.acceptedAuthorised;
				//			break;
				//		case CodeReponseGarantisseur.F40_NON_TRAITEE_AUTOMATIQUEMENT:
				//			res = FinalResult.referral;
				//			break;
				//		default:
				//			res = FinalResult.declined;
				//			break;
				//	}

				// read decisions file
				CJson<CResponseCodes> decisions = new CJson<CResponseCodes>(DecisionsFileName);
				CResponseCodes responses = decisions.ReadSettings();
				if (null == responses)
				{
					// set default values
					responses = new CResponseCodes();
					responses.Add(CodeReponseGarantisseur.F40_ACCEPTEE.ToString(), Decisions.OK.ToString());
					responses.Add(CodeReponseGarantisseur.F40_REFUSEE.ToString(), Decisions.KO.ToString());
					responses.Add(CodeReponseGarantisseur.F40_NON_TRAITEE_AUTOMATIQUEMENT.ToString(), Decisions.Referral.ToString());
					responses.Add(CodeReponseGarantisseur.F40_IDCF_INVALIDE.ToString(), Decisions.KO.ToString());
					responses.Add(CodeReponseGarantisseur.F40_TRANSACTION_INTERDITE.ToString(), Decisions.KO.ToString());
					responses.Add(CodeReponseGarantisseur.F40_SERVICE_INTERDIT.ToString(), Decisions.KO.ToString());
					responses.Add(CodeReponseGarantisseur.F40_ACCEPTEE_SOUS_RESERVE.ToString(), Decisions.OK.ToString());
					responses.Add(CodeReponseGarantisseur.F40_IDC_INVALIDE.ToString(), Decisions.KO.ToString());
					responses.Add(CodeReponseGarantisseur.F40_SERVICE_INDISPONIBLE.ToString(), Decisions.KO.ToString());
					decisions.WriteSettings(responses);
				}

				Decisions decision = Decisions.KO;
				try
				{
					string s = responses[Chpn.F40_OutputGuaranteeResponseCodeAsCode.ToString()];
					// arrived here the code is defined, get value to use
					decision = (Decisions)CMisc.GetEnumValue(typeof(Decisions), s, Decisions.KO);
				}
				catch (Exception)
				{
					// decline by default
					decision = Decisions.KO;
				}

				// transform decision
				if (Decisions.OK == decision)
					res = FinalResult.acceptedAuthorised;
				else if (Decisions.Referral == decision)
					res = FinalResult.referral;
				else
					res = FinalResult.declined;
			}
			else
			{
				// FNCI
				switch (Chpn.F39_OutputFnciResponseCodeAsCode)
				{
					case CodeReponseFNCI.F39_VERT:
						res = FinalResult.acceptedAuthorised;
						break;
					default:
						res = FinalResult.declined;
						break;
				}
			}
			return res;
		}

		/// <summary>
		/// Update settings of the moduke
		/// </summary>
		private void UpdateSettings()
		{
			// read settings
			CSettings mysettings = (null != _json ? _json.ReadSettings() : null);
			if (null != mysettings)
				Settings = mysettings;
			else
				Settings = new CSettings();
			CLog.Add(Settings.ToString());
		}
		/// <summary>
		/// Update network settings of the module
		/// </summary>
		private void UpdateNetworkSettings()
		{
			// read network settings
			CNetworkSettings mynetwork = (null != _jsonnetwork ? _jsonnetwork.ReadSettings() : null);
			if (null != mynetwork)
			{
				NetworkSettings = mynetwork;
				PrimaryIP = NetworkSettings.ServerIPAddress1;
				PrimaryPort = NetworkSettings.PortNumber1;
				PrimaryServerName = NetworkSettings.ServerName1;
				SecondaryIP = NetworkSettings.ServerIPAddress2;
				SecondaryPort = NetworkSettings.PortNumber2;
				SecondaryServerName = NetworkSettings.ServerName2;
			}
			else
				NetworkSettings = new CNetworkSettings();
			CLog.Add(NetworkSettings.ToString());
		}
		#endregion
	}
}
