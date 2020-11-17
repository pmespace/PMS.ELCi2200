using System.Runtime.InteropServices;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CHPN.IPDU;
using CHPN.APDU;
using CHPN;

namespace TestCheck
{
	[ComVisible(true)]
	public enum AuthorisationType
	{
		_begin = 0,
		offline,
		guarantee = APDUs.APDU_9100,
		fnci = APDUs.APDU_9300,
		_end
	}

	[ComVisible(true)]
	public enum FinalResult
	{
		_begin = 0,
		referral,
		canceled,
		declined,
		_beginaccepted,
		acceptedOffline,
		acceptedAuthorised,
		acceptedAfterReferral,
		_endaccepted,
		_end
	}

	[ComVisible(true)]
	public enum IdType
	{
		_begin = 0,
		CarteNationaleDIdentite,
		PermisDeConduire,
		Passeport,
		CarteDeResident,
		CarteDeSejour,
		CarteEuropeenneDIdentite,
		PermisDeConduireEuropeen,
		PasseportUnionEuropéenne,
		Autre,
		_end
	}

	[ComVisible(true)]
	public enum CheckType
	{
		_begin = 0,
		personal,
		company,
		_end
	}

	public enum Status
	{
		none = 0,
		pendingCancel,
		pending,
		sent,
		closed,
	}

	public class CRecord
	{
		private const string STRING_DELIMITER = "'";

		#region properties
		/// <summary>
		/// Référence de la transaction donnée par le commerçant
		/// </summary>
		public string merchantRef { get; set; }
		/// <summary>
		/// Code banque de dépôt du chèque
		/// </summary>
		public string bankID { get; internal set; }
		/// <summary>
		/// Identification du commerçant (numéro d'abonné ou SIRET)
		/// </summary>
		public string merchantID { get; internal set; }
		/// <summary>
		/// Montant de la transaction
		/// </summary>
		public int amount { get; internal set; }
		/// <summary>
		/// Devise de la transaction
		/// </summary>
		public string currency { get; internal set; }
		/// <summary>
		/// Date et heure de la transaction sur le serveur
		/// </summary>
		public string transactionDateTime { get; internal set; }
		/// <summary>
		/// Identifiant de la transaction attribué par le portail
		/// </summary>
		public string ID { get; internal set; }
		/// <summary>
		/// Identifiant de la transaction attribué par le système lui-même
		/// </summary>
		public string internalID { get; internal set; }
		/// <summary>
		/// Terminal sur lequel la transaction a été réalisée
		/// </summary>
		public string poiID { get; internal set; }
		/// <summary>
		/// POI configuration indicator
		/// </summary>
		public string poiConfig { get; internal set; }
		/// <summary>
		/// Ligne CMC7 du chèque
		/// </summary>
		public string checknoteID { get; internal set; }
		/// <summary>
		/// Réponse du garantisseur
		/// </summary>
		public string guaranteeResponseCode { get; internal set; }
		public string guaranteeMessage { get; internal set; }
		public string guaranteeSignature { get; internal set; }
		public string guaranteeBirthDate { get; internal set; }
		public string guaranteeIdType { get; internal set; }
		public string guaranteeIdDate { get; internal set; }
		public string guaranteeCheckType { get; internal set; }
		/// <summary>
		/// Réponse FNCI
		/// </summary>
		public string fnciResponseCode { get; internal set; }
		public string fnciMessage { get; internal set; }
		public string fnciSignature { get; internal set; }
		public string fnciCounterDay { get; internal set; }
		public string fnciCounterN { get; internal set; }
		public string fnciCounterX { get; internal set; }
		public string fnciRlmc { get; internal set; }
		public string transactionSequenceNumber { get; internal set; }
		/// <summary>
		/// Le résultat final de la transaction
		/// </summary>
		public FinalResult finalResult
		{
			get => _finalresult;
			internal set
			{
				switch (value)
				{
					case FinalResult.referral:
					case FinalResult.canceled:
					case FinalResult.declined:
					case FinalResult.acceptedOffline:
					case FinalResult.acceptedAuthorised:
					case FinalResult.acceptedAfterReferral:
						_finalresult = value;
						break;
					default:
						_finalresult = FinalResult.declined;
						break;
				}
			}
		}
		private FinalResult _finalresult = FinalResult.declined;
		/// <summary>
		/// Type of transaction
		/// </summary>
		public AuthorisationType authorisationType
		{
			get => _authorisationtype;
			internal set
			{
				switch (value)
				{
					case AuthorisationType.offline:
					case AuthorisationType.guarantee:
					case AuthorisationType.fnci:
						_authorisationtype = value;
						break;
					default:
						_authorisationtype = AuthorisationType.offline;
						break;
				}
			}
		}
		private AuthorisationType _authorisationtype = AuthorisationType.offline;
		/// <summary>
		/// Transaction date/time (accepted or declined)
		/// </summary>
		public string serverTransactionDateTime { get; internal set; }
		/// <summary>
		/// Indicates whether the transaction was forced or not
		/// </summary>
		public bool forcedStatus { get; internal set; }
		/// <summary>
		/// Indicates whether the check transaction has been cancelled or not
		/// </summary>
		public FinalResult cancelResult
		{
			get => _cancelresult;
			set
			{
				switch (value)
				{
					case FinalResult.canceled:
						_cancelresult = value;
						break;
					default:
						_cancelresult = finalResult;
						break;
				}
			}
		}
		private FinalResult _cancelresult = FinalResult.declined;

		public CIPDU Request { get; internal set; }
		public CIPDU Reply { get; internal set; }

		// working data

		public Status status
		{
			get => _status;
			set
			{
				switch (value)
				{
					case Status.closed:
					case Status.sent:
					case Status.pending:
						_status = value;
						break;
					default:
						_status = Status.none;
						break;
				}
			}
		}
		private Status _status = Status.none;

		internal int attempts { get; set; }
		#endregion

		#region methods
		private static string IsString(string value) { return STRING_DELIMITER + value + STRING_DELIMITER; }
		private static string VarName(string value, bool usebrackets) { return "[" + value + "]"; }
		public static string InsertIntoRequest(string table, CRecord record, bool usebrackets = true)
		{
			record.internalID = CreateID(); // Guid.NewGuid().ToString();
			return "INSERT INTO " + table + " ("
				+ VarName(nameof(internalID), usebrackets) + ","
				+ VarName(nameof(merchantRef), usebrackets) + ","
				+ VarName(nameof(bankID), usebrackets) + ","
				+ VarName(nameof(merchantID), usebrackets) + ","
				+ VarName(nameof(amount), usebrackets) + ","
				+ VarName(nameof(currency), usebrackets) + ","
				+ VarName(nameof(transactionDateTime), usebrackets) + ","
				//+ VarName(nameof(ID), usebrackets) + ","
				+ VarName(nameof(poiID), usebrackets) + ","
				+ VarName(nameof(poiConfig), usebrackets) + ","
				+ VarName(nameof(checknoteID), usebrackets) + ","
				+ VarName(nameof(guaranteeResponseCode), usebrackets) + ","
				+ VarName(nameof(guaranteeMessage), usebrackets) + ","
				+ VarName(nameof(guaranteeSignature), usebrackets) + ","
				+ VarName(nameof(guaranteeBirthDate), usebrackets) + ","
				+ VarName(nameof(guaranteeIdType), usebrackets) + ","
				+ VarName(nameof(guaranteeIdDate), usebrackets) + ","
				+ VarName(nameof(guaranteeCheckType), usebrackets) + ","
				+ VarName(nameof(fnciResponseCode), usebrackets) + ","
				+ VarName(nameof(fnciMessage), usebrackets) + ","
				+ VarName(nameof(fnciSignature), usebrackets) + ","
				+ VarName(nameof(fnciCounterDay), usebrackets) + ","
				+ VarName(nameof(fnciCounterN), usebrackets) + ","
				+ VarName(nameof(fnciCounterX), usebrackets) + ","
				+ VarName(nameof(fnciRlmc), usebrackets) + ","
				+ VarName(nameof(finalResult), usebrackets) + ","
				+ VarName(nameof(cancelResult), usebrackets) + ","
				+ VarName(nameof(status), usebrackets) + ","
				+ VarName(nameof(authorisationType), usebrackets) + ","
				+ VarName(nameof(serverTransactionDateTime), usebrackets) + ","
				+ VarName(nameof(forcedStatus), usebrackets) + ","
				+ VarName(nameof(transactionSequenceNumber), usebrackets) + ","
				+ VarName(nameof(attempts), usebrackets)
				+ ") VALUES ("
				+ IsString(record.internalID) + ","
				+ IsString(record.merchantRef) + ","
				+ IsString(record.bankID) + ","
				+ IsString(record.merchantID) + ","
				+ record.amount + ","
				+ IsString(record.currency) + ","
				+ IsString(record.transactionDateTime) + ","
				//+ IsString(CreateID()) + ","
				+ IsString(record.poiID) + ","
				+ IsString(record.poiConfig) + ","
				+ IsString(record.checknoteID) + ","
				+ IsString(record.guaranteeResponseCode) + ","
				+ IsString(record.guaranteeMessage) + ","
				+ IsString(record.guaranteeSignature) + ","
				+ IsString(record.guaranteeBirthDate) + ","
				+ IsString(record.guaranteeIdType) + ","
				+ IsString(record.guaranteeIdDate) + ","
				+ IsString(record.guaranteeCheckType) + ","
				+ IsString(record.fnciResponseCode) + ","
				+ IsString(record.fnciMessage) + ","
				+ IsString(record.fnciSignature) + ","
				+ IsString(record.fnciCounterDay) + ","
				+ IsString(record.fnciCounterN) + ","
				+ IsString(record.fnciCounterX) + ","
				+ IsString(record.fnciRlmc) + ","
				+ IsString(record.finalResult.ToString()) + ","
				+ IsString(record.finalResult.ToString()) + ","
				+ IsString(Status.pending.ToString()) + ","
				+ IsString(record.authorisationType.ToString()) + ","
				+ IsString(record.serverTransactionDateTime) + ","
				+ IsString(record.forcedStatus.ToString().ToLower()) + ","
				+ IsString(record.transactionSequenceNumber) + ","
				+ 0 + ")";
		}
		public static string UpdateAttempts(string table, string id, int counter)
		{
			return "UPDATE " + table + " SET "
				+ nameof(attempts) + "=" + counter
				+ " WHERE " +
				PrimaryKey(id);
		}
		public static string SetCancelled(string table, string id, bool isPending)
		{
			return "UPDATE " + table + " SET "
				+ nameof(cancelResult) + "=" + IsString(FinalResult.canceled.ToString())
				+ " WHERE "
				+ PrimaryKey(id);
		}
		public static string SetSent(string table, string id)
		{
			return "UPDATE " + table + " SET "
				+ nameof(status) + "=" + IsString(Status.sent.ToString())
				+ " WHERE "
				+ PrimaryKey(id);
		}
		public static string SetPendingCancel(string table, string id)
		{
			return "UPDATE " + table + " SET "
				+ nameof(status) + "=" + IsString(Status.pendingCancel.ToString())
				+ " WHERE "
				+ PrimaryKey(id);
		}
		public static string SelectOne(string table, string id)
		{
			return "SELECT * FROM " + table +
				" WHERE " +
				PrimaryKey(id);
		}
		public static string SelectPending(string table)
		{
			return "SELECT * FROM " + table
				+ " WHERE "
				+ nameof(CRecord.status) + "=" + IsString(Status.pending.ToString())
				+ " OR " + nameof(CRecord.status) + "=" + IsString(Status.pendingCancel.ToString())
				+ " ORDER BY " + nameof(transactionDateTime) + " DESC";
		}
		public static string DeleteOne(string table, string id)
		{
			return "DELETE FROM " + table
				+ " WHERE " +
				PrimaryKey(id);
		}
		public static string PrimaryKey(string id)
		{
			return nameof(internalID) + "=" + IsString(id);
		}
		private static string CreateID()
		{
			return Guid.NewGuid().ToString();
		}
		internal static string GetIdType(TypeDePieceIdentite typ)
		{
			switch (typ)
			{
				case TypeDePieceIdentite.F43_POS5_CARTE_IDENTITE_NATIONALE:
					return IdType.CarteNationaleDIdentite.ToString();
				case TypeDePieceIdentite.F43_POS5_PERMIS_CONDUIRE_NATIONAL:
					return IdType.PermisDeConduire.ToString();
				case TypeDePieceIdentite.F43_POS5_PASSEPORT_NATIONAL:
					return IdType.Passeport.ToString();
				case TypeDePieceIdentite.F43_POS5_CARTE_RESIDENT:
					return IdType.CarteDeResident.ToString();
				case TypeDePieceIdentite.F43_POS5_CARTE_SEJOUR:
					return IdType.CarteDeSejour.ToString();
				case TypeDePieceIdentite.F43_POS5_CARTE_IDENTITE_EUROPEENE:
					return IdType.CarteEuropeenneDIdentite.ToString();
				case TypeDePieceIdentite.F43_POS5_PERMIS_CONDUIRE_EUROPEEN:
					return IdType.PermisDeConduireEuropeen.ToString();
				case TypeDePieceIdentite.F43_POS5_PASSEPORT_EUROPEEN:
					return IdType.PasseportUnionEuropéenne.ToString();
				case TypeDePieceIdentite.F43_POS5_AUTRE:
					return IdType.Autre.ToString();
				default:
					return string.Empty;
			}
		}
		internal static string GetCheckType(TypeDeCheque typ)
		{
			switch (typ)
			{
				case TypeDeCheque.F43_POS10_CHEQUE_PERSONNEL:
					return CheckType.personal.ToString();
				case TypeDeCheque.F43_POS10_CHEQUE_SOCIETE:
					return CheckType.company.ToString();
				default:
					return string.Empty;
			}
		}
		/// <summary>
		/// Indicates the record contains an accepted transactioin
		/// </summary>
		/// <returns>TRUE if accepted, FALSE otherwise</returns>
		public bool IsAccepted() { return FinalResult.acceptedAuthorised == finalResult || FinalResult.acceptedAfterReferral == finalResult || FinalResult.acceptedOffline == finalResult; }
		/// <summary>
		/// Indicates the record contains a declined (not accepted) transaction
		/// </summary>
		/// <returns></returns>
		public bool IsDeclined() { return !IsAccepted(); }
		#endregion
	}
}
