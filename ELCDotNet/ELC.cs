using System;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using COMMON;

namespace ELCDotNet
{
	/// <summary>
	/// Indicates tha available printings
	/// </summary>
	public enum PrintType
	{
		_begin,
		/// <summary>
		/// Guaranted check
		/// </summary>
		garantie,
		/// <summary>
		/// FNCI verified check
		/// </summary>
		fnci,
		/// <summary>
		/// No verification
		/// </summary>
		rien,
		_end,
	}

	/// <summary>
	/// An object to print using the ELC
	/// </summary>
	public class ObjectToPrint
	{
		/// <summary>
		/// Index of the check (merchant number assigned); it won't be used if set to 0
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		/// Merchant name - MANDATORY
		/// </summary>
		public string MerchantName { get; set; }
		/// <summary>
		/// Merchant address - MANDATORY
		/// </summary>
		public string MerchantAddress { get; set; }
		/// <summary>
		/// Amount - MANDATORY - Not valid if set to 0
		/// </summary>
		public uint Amount { get; set; }
		/// <summary>
		/// Type of check to print (<see cref="PrintType"/>)
		/// </summary>
		public PrintType Type { get; set; }
		/// <summary>
		/// Seal certifying the transaction (if any) - Refer to CHPN field 38 (guarantee) or 39 (FNCI)
		/// Not used if not guarantee or FNCI
		/// </summary>
		public string Seal { get; set; }
		/// <summary>
		/// Authorisation response code (if any)
		/// Not used if not guarantee or FNCI
		/// </summary>
		public string ResponseCode { get; set; }
		/// <summary>
		/// Indicates whether the object is valid or not
		/// </summary>
		/// <returns>True if valid, false otherwise</returns>
		public bool IsValid() { return !string.IsNullOrEmpty(MerchantName) && !string.IsNullOrEmpty(MerchantAddress) && 0 != Amount && PrintType._begin < Type && PrintType._end > Type; }
	}

	public class ELC
	{
		#region constructor
		public ELC()
		{
			AsyncStartUserTimerEvent = new ManualResetEvent(false);
			AsyncProcessHasEndedEvent = new ManualResetEvent(false);
			pelc = ELCi2200.ELCInit();
		}
		~ELC()
		{
			AsyncStartUserTimerEvent.Dispose();
			AsyncProcessHasEndedEvent.Dispose();
			ELCi2200.ELCRelease(ref pelc);
		}
		#endregion

		#region public properties
		/// <summary>
		/// Main window to overtake when displaying a processing window)
		/// It is very godd behaviour to set this with the handle of the application main window
		/// </summary>
		public IntPtr MainWindow { get; set; }
		/// <summary>
		/// Indicates whether the ELC device has been opened or not
		/// </summary>
		public bool Opened { get => _opened; private set => _opened = value; }
		private bool _opened = false;
		/// <summary>
		/// COM port used by the ELC
		/// </summary>
		public int Port { get => IntPtr.Zero != pelc ? ELCi2200.ELCPort(pelc) : ELCi2200.NO_COM_PORT; }
		/// <summary>
		/// True if the last operation timed out
		/// </summary>
		public bool Timeout { get => ELCi2200.ELCResult.timeout == LastAsyncResult; }
		/// <summary>
		/// True if the last operation was cancelled by the user
		/// </summary>
		public bool Cancelled { get => ELCi2200.ELCResult.cancelled == LastAsyncResult; }
		/// <summary>
		/// True if the last operation was cancelled by the user
		/// </summary>
		public bool CompletedWithError { get => ELCi2200.ELCResult.completedWithError == LastAsyncResult; }
		/// <summary>
		/// Event used to warn the calling application it can start its own waiting timer
		/// </summary>
		public ManualResetEvent AsyncStartUserTimerEvent { get; private set; }
		/// <summary>
		/// Message that will be sent to the caller's window <see cref="WindowToWarn"/> when waiting can start
		/// </summary>
		public uint WMStartTimer { get => _wmstarttimer; set => _wmstarttimer = value; }
		private uint _wmstarttimer = Win32.WMUser + 2666;
		/// <summary>
		/// Event used to warn the calling application processing is finished on the ELC
		/// </summary>
		public ManualResetEvent AsyncProcessHasEndedEvent { get; private set; }
		/// <summary>
		/// Message that will be sent to the caller's window <see cref="WindowToWarn"/> when ELC processing is finished
		/// </summary>
		public uint WMProcessHasEndedEvent { get => _wmprocesshasendedevent; set => _wmprocesshasendedevent = value; }
		private uint _wmprocesshasendedevent = Win32.WMUser + 2667;
		/// <summary>
		/// Handle of the window to which to send a message when an event is signaled <see cref="AsyncStartUserTimerEvent"/> and <see cref="AsyncProcessHasEndedEvent"/>
		/// </summary>
		public IntPtr WindowToWarn { get => _windowtowarn; set => _windowtowarn = value; }
		private IntPtr _windowtowarn = IntPtr.Zero;
		/// <summary>
		/// Get last async result
		/// </summary>
		public ELCi2200.ELCResult LastAsyncResult { get => IntPtr.Zero != pelc ? ELCi2200.ELCLastAsyncResult(pelc) : ELCi2200.ELCResult.none; }
		#endregion

		#region private properties
		/// <summary>
		/// Internal ELC structure
		/// </summary>
		private IntPtr pelc = IntPtr.Zero;
		/// <summary>
		/// Async thread
		/// </summary>
		private CThread thread = null;
		private ManualResetEvent stopThreadEvent = null;
		private AutoResetEvent startUserTimerEvent = null;
		private AutoResetEvent processHasEndedEvent = null;
		#endregion

		#region public methods
		/// <summary>
		/// Open the ELC using a known COM port
		/// </summary>
		/// <param name="port">COM port to use</param>
		/// <param name="useLog">Indicates whether a log file must be opened (and an output created). File is called "ELC on COMx.log" with "x" indicating the COM port.
		/// It is created first in the current directory, if not possible then in Documents, if not possible then in the TEMP folder.
		/// Beware that file is never purged and can become very big.</param>
		/// <returns>True if opened, false otherwise</returns>
		public bool Open(int port, bool useLog = true)
		{
			if (ELCi2200.NO_COM_PORT != port)
			{
				ELCi2200.ELCSetPort(pelc, port);
				Opened = ELCi2200.ELCOpen(pelc, useLog);
				return Opened;
			}
			return false;
		}
		/// <summary>
		/// Open the ELC using a USB device driver
		/// </summary>
		/// <param name="driver">COM port to use</param>
		/// <param name="useLog">Indicates whether a log file must be opened (and an output created)</param>
		/// <returns>True if opened, false otherwise</returns>
		public bool Open(string driver, bool useLog = true)
		{
			int port = ELCi2200.ELCGetUSBComPort(driver);
			return Open(port, useLog);
		}
		/// <summary>
		/// Try to open using a list a USB device drivers
		/// </summary>
		/// <param name="availableDrivers">Name of the JSON file containing the drivers (<see cref="USBDriver"/> for each item inside a <see cref="USBDrivers"/> collection)</param>
		/// <param name="useLog">Indicates whether a log file must be opened (and an output created)</param>
		/// <returns>True if opened, false otherwise</returns>
		public bool OpenWithDrivers(string availableDrivers, bool useLog = true)
		{
			CJson<USBDrivers> json = new CJson<USBDrivers>();
			// try to open the drivers file
			json.FileName = availableDrivers;
			USBDrivers drivers = json.ReadSettings();
			if (null != drivers)
			{
				foreach (USBDriver driver in drivers)
				{
					if (Open(driver.DriverName, useLog))
						return true;
				}
			}
			else
			{
				drivers = new USBDrivers();
				drivers.Add(new USBDriver() { Description = "ATEN USB Driver", DriverName = @"USB\VID_0557&PID_2022" });
				drivers.Add(new USBDriver() { Description = "ELC Native Driver (doesn't work under Windows 10)", DriverName = @"USB\VID_10C4&PID_EA60" });
				json.WriteSettings(drivers, true);
			}
			return false;
		}
		/// <summary>
		/// Close the ELC device
		/// </summary>
		public void Close()
		{
			ELCi2200.ELCClose(pelc);
			Opened = false;
		}
		/// <summary>
		/// Get ELC current status
		/// </summary>
		/// <param name="documentIsPresent">If return is true, indicates whether a check is inside the ELC or not</param>
		/// <returns>True if operation has been successfully completed, false otherwise</returns>
		public bool Status(ref bool documentIsPresent)
		{
			if (Opened)
			{
				return ELCi2200.ELCStatus(pelc, ref documentIsPresent);
			}
			return false;
		}
		/// <summary>
		/// Abort ELC current operation (ejecting a check note if necessary)
		/// </summary>
		/// <param name="documentHasBeenEjected">If return is true, indicates whether a document has been ejcted from the reader or not</param>
		/// <returns>True if operation has been successfully completed, false otherwise</returns>
		public bool Abort(ref bool documentHasBeenEjected)
		{
			if (Opened)
			{
				return ELCi2200.ELCAbort(pelc, ref documentHasBeenEjected);
			}
			return false;
		}
		/// <summary>
		/// Read a check
		/// </summary>
		/// <param name="raw">If return is true, indicates the check CMC7 as read</param>
		/// <param name="chpnCompatible">If return is true, indicates the check CMC7 in CHPN compatible format</param>
		/// <param name="documentIsInside">If return is true, indicates whether a check note is inside the reader or notthe check CMC7 in CHPN compatible format</param>
		/// <param name="timer">Timer to wait for reading to complete or times out, <see cref="ELCi2200.NO_TIMER"/> for infinite wait</param>
		/// <returns>True if operation has been successfully completed, false otherwise</returns>
		public bool Read(ref string raw, ref string chpnCompatible, ref bool documentIsInside, int timer = ELCi2200.NO_TIMER)
		{
			raw = null;
			chpnCompatible = null;
			if (Opened)
			{
				StringBuilder rawx = new StringBuilder(1024);
				StringBuilder chpnx = new StringBuilder(1024);
				ELCi2200.ELCResult res = ELCi2200.ELCRead(pelc, timer, IntPtr.Zero, rawx, rawx.Capacity, chpnx, chpnx.Capacity, ref documentIsInside);
				if (ELCi2200.ELCResult.completed == LastAsyncResult)
				{
					raw = rawx.ToString();
					chpnCompatible = chpnx.ToString();
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Asynchronously read a check
		/// Asynchronous warning is made using the <see cref="AsyncStartUserTimerEvent"/>, <see cref="AsyncProcessHasEndedEvent"/> and <see cref="WindowToWarn"/> completed with <see cref="WMStartTimer"/> and <see cref="WMProcessHasEndedEvent"/> objects
		/// </summary>
		/// <param name="timer">Timer to wait for reading to complete or times out, <see cref="ELCi2200.NO_TIMER"/> for infinite wait</param>
		/// <returns>True if operation has been successfully started, false otherwise</returns>
		public bool ReadAsync(int timer = ELCi2200.NO_TIMER)
		{
			if (Opened && !ELCi2200.ELCIsInProgress(pelc))
			{
				if (StartAsyncThread())
					return ELCi2200.ELCReadAsync(pelc, timer, startUserTimerEvent.SafeWaitHandle, processHasEndedEvent.SafeWaitHandle);
			}
			return false;
		}
		/// <summary>
		/// Get write async result
		/// </summary>
		/// <returns>True if result has been retrieved, false otherwise</returns>
		public ELCi2200.ELCResult ReadAsyncResult(ref string raw, ref string chpnCompatible, ref bool documentIsInside)
		{
			StringBuilder rawx = new StringBuilder(1024);
			StringBuilder chpnx = new StringBuilder(1024);
			ELCi2200.ELCResult res = ELCi2200.ELCReadAsyncResult(pelc, rawx, rawx.Capacity, chpnx, chpnx.Capacity, ref documentIsInside);
			if (ELCi2200.ELCResult.completed == LastAsyncResult)
			{
				raw = rawx.ToString();
				chpnCompatible = chpnx.ToString();
			}
			return res;
		}
		/// <summary>
		/// Read a check
		/// </summary>
		/// <param name="o"><see cref="ObjectToPrint"/></param>
		/// <param name="printed">If return is true, indicates the text which has been printed</param>
		/// <param name="timer">Timer to wait for printing to complete, <see cref="ELCi2200.NO_TIMER"/> for infinite wait</param>
		/// <returns>True if operation has been successfully completed, false otherwise</returns>
		public bool Write(ObjectToPrint o, ref string printed, int timer = ELCi2200.NO_TIMER)
		{
			string s = PrepareStringToPrint(o);
			if (WriteEx(ref s, timer))
			{
				printed = s;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Read a check
		/// </summary>
		/// <param name="text">Text to print</param>
		/// <param name="timer">Timer to wait for printing to complete, <see cref="ELCi2200.NO_TIMER"/> for infinite wait</param>
		/// <returns>True if operation has been successfully completed, false otherwise</returns>
		public bool WriteEx(ref string text, int timer = ELCi2200.NO_TIMER)
		{
			if (Opened && !string.IsNullOrEmpty(text))
			{
				string s = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
				StringBuilder toprintx = new StringBuilder(s);
				StringBuilder printedx = new StringBuilder(1024);
				ELCi2200.ELCResult res = ELCi2200.ELCWrite(pelc, toprintx, printedx, printedx.Capacity, timer, IntPtr.Zero);
				if (ELCi2200.ELCResult.completed == LastAsyncResult)
				{
					text = printedx.ToString();
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Asynchronously write a check
		/// Asynchronous warning is made using the <see cref="AsyncStartUserTimerEvent"/>, <see cref="AsyncProcessHasEndedEvent"/> and <see cref="WindowToWarn"/> completed with <see cref="WMStartTimer"/> and <see cref="WMProcessHasEndedEvent"/> objects
		/// </summary>
		/// <param name="o"><see cref="ObjectToPrint"/></param>
		/// <param name="printed">If return is true, indicates the text which has been printed</param>
		/// <param name="timer">Timer to wait for reading to complete or times out, <see cref="ELCi2200.NO_TIMER"/> for infinite wait</param>
		/// <returns>True if operation has been successfully started, false otherwise</returns>
		public bool WriteAsync(ObjectToPrint o, ref string printed, int timer = ELCi2200.NO_TIMER)
		{
			string s = PrepareStringToPrint(o);
			if (WriteAsyncEx(ref s, timer))
			{
				printed = s;
				return true;
			}
			return false;
		}
		/// <summary>
		/// Asynchronously write a check
		/// </summary>
		/// <param name="text">Text to print</param>
		/// <param name="timer">Timer to wait for printing to complete, <see cref="ELCi2200.NO_TIMER"/> for infinite wait</param>
		/// <returns>True if operation has been successfully completed, false otherwise</returns>
		public bool WriteAsyncEx(ref string text, int timer = ELCi2200.NO_TIMER)
		{
			if (Opened && !string.IsNullOrEmpty(text) && !ELCi2200.ELCIsInProgress(pelc))
			{
				if (StartAsyncThread())
				{
					string s = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text));
					StringBuilder toprintx = new StringBuilder(s);
					StringBuilder printedx = new StringBuilder(1024);
					if (ELCi2200.ELCWriteAsync(pelc, toprintx, printedx, printedx.Capacity, timer, startUserTimerEvent.SafeWaitHandle, processHasEndedEvent.SafeWaitHandle))
					{
						text = printedx.ToString();
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Get write async result
		/// </summary>
		/// <returns>True if result has been retrieved, false otherwise</returns>
		public ELCi2200.ELCResult WriteAsyncResult()
		{
			return ELCi2200.ELCWriteAsyncResult(pelc);
		}
		/// <summary>
		/// Wait for an asynchronous operation terminates (or not) on the ELC
		/// </summary>
		/// <param name="timer">Timer to wait for. In case the timer expires before the operation completes result is <see cref="ELCi2200.ELCResult.none"/></param>
		/// <returns><see cref="ELCi2200.ELCResult"/></returns>
		public ELCi2200.ELCResult WaitAsync(int timer = ELCi2200.NO_TIMER)
		{
			if (Opened && ELCi2200.ELCIsInProgress(pelc))
				return ELCi2200.ELCWaitAsync(pelc, timer);
			return ELCi2200.ELCResult.error;
		}
		/// <summary>
		/// Cancel the current asynchronous operation
		/// </summary>
		/// <returns></returns>
		public bool CancelAsync()
		{
			if (Opened && ELCi2200.ELCIsInProgress(pelc))
				return ELCi2200.ELCCancelAsync(pelc);
			return false;
		}
		#endregion

		#region private methods
		/// <summary>
		/// Thread processing events from the ELC driver
		/// </summary>
		/// <param name="threadData"></param>
		/// <param name="o"></param>
		/// <returns></returns>
		private int AsyncThread(CThreadData threadData, object o)
		{
			bool keepCarryOn = true;
			ThreadResult res = ThreadResult.OK;
			WaitHandle[] handles = { startUserTimerEvent, processHasEndedEvent, stopThreadEvent };
			while (keepCarryOn)
			{
				int index = WaitHandle.WaitAny(handles);
				if (stopThreadEvent == handles[index])
				{
					CLog.Add("Externally stopping thread", TLog.WARNG);
					// stop the thread
					keepCarryOn = false;
					res = ThreadResult.KO;
				}
				else if (startUserTimerEvent == handles[index])
				{
					CLog.Add("ELC processing timer is ready to start - warning application");
					// warn the application to start the timer
					AsyncStartUserTimerEvent.Set();
					if (IntPtr.Zero != WindowToWarn)
						Win32.PostMessage(WindowToWarn, WMStartTimer, 0, 0);
				}
				else if (processHasEndedEvent == handles[index])
				{
					CLog.Add("ELC process is finished - warning application");
					// warn the application that ELC processiong is finished
					AsyncProcessHasEndedEvent.Set();
					if (IntPtr.Zero != WindowToWarn)
						Win32.PostMessage(WindowToWarn, WMProcessHasEndedEvent, 0, 0);
					keepCarryOn = false;
				}
			}
			// stop ELC processing if necessary
			ELCi2200.ELCCancelAsync(pelc);
			// release async environment
			stopThreadEvent = null;
			startUserTimerEvent = null;
			processHasEndedEvent = null;
			return (int)res;
		}
		/// <summary>
		/// Start the thread which will process events from the ELC driver
		/// </summary>
		/// <returns></returns>
		private bool StartAsyncThread()
		{
			thread = new CThread();
			AsyncStartUserTimerEvent.Reset();
			AsyncProcessHasEndedEvent.Reset();
			stopThreadEvent = new ManualResetEvent(false);
			startUserTimerEvent = new AutoResetEvent(false);
			processHasEndedEvent = new AutoResetEvent(false);
			if (!thread.Start(AsyncThread))
			{
				stopThreadEvent = null;
				startUserTimerEvent = null;
				processHasEndedEvent = null;
				return false;
			}
			return true;
		}
		/// <summary>
		/// Prepare a string to print on a check note, according to current standards
		/// Mandatory
		/// - Amount must be indicated twice (one on the check written part, once on the amount box)
		/// - Merchant address must be present with at least 1 character
		/// - Current date must be indicated
		/// Optional
		/// - Index of check note: only if not 0
		/// - Authorisation code and seal from a guarantee company: only if guarantee
		/// - Authorisation code and seal from FNCI: only if FNCI
		/// </summary>
		/// <param name="o"><see cref="ObjectToPrint"/></param>
		/// <returns></returns>
		private string PrepareStringToPrint(ObjectToPrint o)
		{
			// prepare string to print
			const string SEP = "*";
			int maxLengthWithAmount = ELCi2200.MAX_WRITE_DATA_SIZE;
			int maxLengthWithoutAmount = 60;
			int minprintlocation = 1;
			DateTime dt = DateTime.Now;
			string sdt = dt.ToString("dd/MM/yy") + SEP;
			string samount = SEP + (o.Amount / 100M).ToString("0.00") + SEP;
			string toprint = null;
			string fmt = null;
			string indexFmt = (0 != o.Index ? o.Index.ToString("00") + " " : string.Empty);
			string seal = null;
			string rc = null;
			int seallenmax;
			int rclenmax;
			if (PrintType.garantie == o.Type) // check guarantee
			{
				seallenmax = 6;
				rclenmax = 3;
				try { seal = null != o.Seal && seallenmax < o.Seal.Length ? o.Seal.Substring(0, seallenmax) : o.Seal; } catch (Exception) { }
				try { rc = null != o.Seal && rclenmax < o.ResponseCode.Length ? o.ResponseCode.Substring(0, rclenmax) : o.ResponseCode; } catch (Exception) { }
				// guarantee seal + guarantee RC + amount [+ check index] + space
				fmt = @"{0," + seallenmax + "}" + SEP + @"{1," + rclenmax + "}" +
					samount +
					indexFmt;
				toprint = string.Format(fmt, new object[] { seal, rc });
			}
			else if (PrintType.fnci == o.Type) // FNCI
			{
				// fnci number + fnci rc + amount + check index + space
				seallenmax = 4;
				rclenmax = 2;
				try { seal = null != o.Seal && seallenmax < o.Seal.Length ? o.Seal.Substring(0, seallenmax) : o.Seal; } catch (Exception) { }
				try { rc = null != o.Seal && rclenmax < o.ResponseCode.Length ? o.ResponseCode.Substring(0, rclenmax) : o.ResponseCode; } catch (Exception) { }
				// guarantee seal + guarantee RC + amount [+ check index] + space
				fmt = @"{0," + seallenmax + "}" + SEP + @"{1," + rclenmax + "}" +
					samount +
					indexFmt;
				toprint = string.Format(fmt, new object[] { seal, rc });
			}
			else
			{
				// amount + check index + space
				fmt = "   " +
					SEP +
					samount +
					indexFmt;
			}

			// add merchantName name
			int maxprint = maxLengthWithoutAmount - toprint.Length - SEP.Length - sdt.Length - minprintlocation;
			toprint += o.MerchantName.Substring(0, Math.Min(maxprint, o.MerchantName.Length)) + SEP + sdt;
			// add merchantName merchantAddress
			maxprint = maxLengthWithoutAmount - toprint.Length;
			toprint += o.MerchantAddress.Substring(0, Math.Min(maxprint, o.MerchantAddress.Length));
			// align on right boundary
			toprint += string.Format("{0," + (maxLengthWithoutAmount - toprint.Length).ToString() + "}", string.Empty);
			// add amount
			fmt = @"{0," + (maxLengthWithAmount - maxLengthWithoutAmount).ToString() + @"}";
			toprint += string.Format(fmt, new object[] { samount });
			// trim the resulting string
			toprint = toprint.Trim();
			// suppress diacritic characters
			try
			{
				toprint = Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-8").GetBytes(toprint));
			}
			catch (Exception) { }
			return toprint;
		}
		#endregion
	}
}
