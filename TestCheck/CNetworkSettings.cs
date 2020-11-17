namespace TestCheck
	{
	public class CNetworkSettings
		{
		#region settings
		/// <summary>
		/// IP 1
		/// </summary>
		public string ServerIPAddress1
			{
			get => (_hostname1 ?? string.Empty);
			set => _hostname1 = (null == value ? string.Empty : value.Trim());
			}
		private string _hostname1;
		/// <summary>
		/// IP 2 (backup)
		/// </summary>
		public string ServerIPAddress2
			{
			get => (_hostname2 ?? string.Empty);
			set => _hostname2 = (null == value ? string.Empty : value.Trim());
			}
		private string _hostname2;
		/// <summary>
		/// Port 1
		/// </summary>
		public uint PortNumber1
			{
			get => _portnumber1;
			set => _portnumber1 = (PORT_MIN <= value && PORT_MAX >= value ? value : PORT_MIN);
			}
		private uint _portnumber1;
		/// <summary>
		/// Port 2 (backup)
		/// </summary>
		public uint PortNumber2
			{
			get => _portnumber2;
			set => _portnumber2 = (PORT_MIN <= value && PORT_MAX >= value ? value : PORT_MIN);
			}
		private uint _portnumber2;
		public const int PORT_MIN = 1;
		public const int PORT_MAX = 65535;
		/// <summary>
		/// The timeout to use when sending data
		/// </summary>
		public int SendTimeout
			{
			get => _sendtimeout;
			set => _sendtimeout = (TIMEOUT_MIN <= value && TIMEOUT_MAX >= value ? value : TIMEOUT_MIN);
			}
		private int _sendtimeout = TIMEOUT_MIN;
		/// <summary>
		/// The timeout to use when receiving data
		/// </summary>
		public int ReceiveTimeout
			{
			get => _receivetimeout;
			set => _receivetimeout = (TIMEOUT_MIN <= value && TIMEOUT_MAX >= value ? value : TIMEOUT_MIN);
			}
		private int _receivetimeout = TIMEOUT_MIN;
		public const int TIMEOUT_MIN = 1;
		public const int TIMEOUT_MAX = 300;
		/// <summary>
		/// Name of the server to authenticate against when using the primary address
		/// </summary>
		public string ServerName1 { get; set; }
		/// <summary>
		/// Name of the server to authenticate against when using the secondary address
		/// </summary>
		public string ServerName2 { get; set; }
		#endregion

		#region methods
		public override string ToString()
			{
			return "Server 1: " + ServerName1 + "; IP/URL 1: " + ServerIPAddress1 + "; Port 1: " + PortNumber1.ToString() + "; Server 2: " + ServerName2 + "; IP/URL 2: " + ServerIPAddress2 + "; Port 2: " + PortNumber2.ToString() + "; Timer émission: " + SendTimeout.ToString() + "; Timer réception: " + ReceiveTimeout.ToString();
			}
		#endregion
		}
	}
