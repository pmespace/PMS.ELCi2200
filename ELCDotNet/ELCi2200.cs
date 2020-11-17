using System;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ELCDotNet
{
	public static class ELCi2200
	{
		public static int MAX_WRITE_DATA_SIZE = 80;
		public const int NO_COM_PORT = -1;
		public const int NO_TIMER = 0;
		public const int INFINITE = -1;

		/// <summary>
		/// Timers that can be set inside the ELC driver
		/// </summary>
		public enum ELCTimer
		{
			_timeBegin = -1,
			T1, // in milliseconds
			T2, // in milliseconds
			TA, // in milliseconds
			TR, // in milliseconds
			TD, // in milliseconds
			TRA, // in milliseconds
			beforeAbort, // in milliseconds
			_timeEnd,
		};

		/// <summary>
		/// Result of an asynchronous operation (read or write)
		/// </summary>
		public enum ELCResult
		{
			_eventBegin = -1,
			/// <summary>
			/// no event to signal (operation, not completed, not cancelled, not in timeout and no error), wait can carry on
			/// </summary>
			none,
			/// <summary>
			/// an error has occurred, waiting is not required any more
			/// </summary>
			error,
			/// <summary>
			/// timeout during operation
			/// </summary>
			timeout,
			/// <summary>
			/// the operation is completed
			/// </summary>
			completed,
			/// <summary>
			/// the operation is completed but an error has occurred during processing
			/// </summary>
			completedWithError,
			/// <summary>
			/// operatioon cancelled by user
			/// </summary>
			cancelled,
			_eventEnd,
		};

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr ELCInit();

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void ELCRelease(ref IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCSetPort(IntPtr pelc, int port);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCOpen(IntPtr pelc, bool useLog);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int ELCPort(IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void ELCClose(IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCStatus(IntPtr pelc, ref bool documentReadyToBeRead);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCAbort(IntPtr pelc, ref bool documentEjected);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ELCResult ELCWaitAsync(IntPtr pelc, int iTimer);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCReadAsync(IntPtr pelc, int iTimer, SafeWaitHandle timerStartedEvent, SafeWaitHandle asyncOperationEndedEvent);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ELCResult ELCReadAsyncResult(IntPtr pelc, StringBuilder rawBuffer, int sizeRawBuffer, StringBuilder chpnBuffer, int sizeChpnBuffer, ref bool documentIsStillInside);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCWriteAsync(IntPtr pelc, StringBuilder toPrint, StringBuilder printed, int sizePrinted, int iTimer, SafeWaitHandle timerStartedEvent, SafeWaitHandle asyncOperationEndedEvent);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ELCResult ELCWriteAsyncResult(IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ELCResult ELCRead(IntPtr pelc, int iTimer, IntPtr timerStartedEvent, StringBuilder rawBuffer, int sizeRawBuffer, StringBuilder chpnBuffer, int sizeChpnBuffer, ref bool documentIsStillInside);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ELCResult ELCWrite(IntPtr pelc, StringBuilder toPrint, StringBuilder printed, int sizePrinted, int iTimer, IntPtr timerStartedEvent);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCInitiateDialog(IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern char ELCCR(IntPtr pelc, int index);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int ELCSpeed(IntPtr pelc, int BaudRate);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCCancelAsync(IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern void ELCSetTimer(IntPtr pelc, ELCTimer timer, int value);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern int ELCGetUSBComPort(string usbDriver);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ELCIsInProgress(IntPtr pelc);

		[DllImport("ELCi2200.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		public static extern ELCResult ELCLastAsyncResult(IntPtr pelc);
	}
}
