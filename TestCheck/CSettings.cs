using System.Runtime.InteropServices;
using System;
using COMMON;
using CHPN;

namespace TestCheck
{
	public class CSettings
		{
		#region privates
		private int _lasttxnid = 1;
		#endregion

		#region main settings
		/// <summary>
		/// MErchant bank
		/// </summary>
		public string F32_BankID
			{
			get => (_f32 ?? string.Empty);
			set => _f32 = value;
			}
		private string _f32 = string.Empty;
		/// <summary>
		///  Merchant center ID
		/// </summary>
		public string F37_IDC
			{
			get => (_f37 ?? string.Empty);
			set => _f37 = value;
			}
		private string _f37 = string.Empty;
		/// <summary>
		///  Merchant TID
		/// </summary>
		public string F41_TID
			{
			get => (_f41 ?? string.Empty);
			set => _f41 = value;
			}
		private string _f41 = string.Empty;
		/// <summary>
		/// Indicates whther the merchant ID is a SIRET or not
		/// </summary>
		public bool F42_IsSIRET { get; set; }
		/// <summary>
		/// Merchant ID
		/// </summary>
		public string F42_IDCF
			{
			get => (_f42 ?? string.Empty);
			set => _f42 = value;
			}
		private string _f42 = string.Empty;
		/// <summary>
		/// Last transaction ID sent for verification
		/// </summary>
		public int F11_LastTxnID
			{
			get => _lasttxnid;
			set
				{
				if (0 >= value || 10000 <= value)
					_lasttxnid = 1;
				else
					_lasttxnid = value;
				}
			}
		/// <summary>
		/// 
		/// </summary>
		public bool AllowOfflineTransactions { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int AuthorisationThreshold
			{
			get => _authorisationthreshold;
			set => _authorisationthreshold = Math.Abs(value);
			}
		private int _authorisationthreshold = 0;
		/// <summary>
		/// 
		/// </summary>
		public bool AllowModifyDecline { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int FNCICounter1
			{
			get => _fncicounter1;
			set => _fncicounter1 = Math.Abs(value);
			}
		private int _fncicounter1 = 0;
		public int FNCICounterN
			{
			get => _fncicounterN;
			set => _fncicounterN = Math.Abs(value);
			}
		private int _fncicounterN = 0;
		public int FNCICounterX
			{
			get => _fncicounterX;
			set => _fncicounterX = Math.Abs(value);
			}
		private int _fncicounterX = 0;
		#endregion

		#region other settings (user input)
		public string F2 { get; set; }
		public string F4 { get; set; }
		public string F35 { get; set; }
		public string F43_POS1TO4 { get; set; }
		public TypeDePieceIdentite F43_POS5 { get; set; }
		public string F43_POS6TO9 { get; set; }
		public TypeDeCheque F43_POS10 { get; set; }
		#endregion

		#region methods
		public override string ToString()
			{
			return "F32 - Banque: " + F32_BankID + "; F37 - IDC: " + F37_IDC + "; F41 - TID: " + F41_TID + "; F42 - IDCF: " + F42_IDCF
				+ Chars.CRLF + Chars.TAB + "; Support offline: " + AllowOfflineTransactions.ToString().ToUpper() + "; Seuil d'appel en autorisation: " + AuthorisationThreshold.ToString()
				+ Chars.CRLF + Chars.TAB + "; Possibilité d'accepter une transaction refusée: " + AllowModifyDecline.ToString().ToUpper()
				+ Chars.CRLF + Chars.TAB + ";";
			}
		#endregion
		}
	}
