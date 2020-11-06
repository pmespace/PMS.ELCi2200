#include "pch.h"
#define DRIVER_EXPORTS
#include	"driver.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <winuser.h>
#include <setupapi.h>
#include <iostream>
//#include <synchapi.h>
//#include <ioapiset.h>

#pragma region compilation options

//**********
// Compilation options
// sets using read & write by blocks
//#define SENDORDEREX
//#define GETORDEREX
// sets/cancels asynchronous file operations
#define DOOVERLAPPING
//**********

#pragma endregion

#pragma region declarations

/// <summary>
/// Actions that must run inside a thread
/// </summary>
enum class Action
{
	_actionBegin,
	actionRead,
	actionPrint,
	_actionEnd,
};

// constants
#define ONEKB							1024
#define HALFKB							ONEKB / 2
#define QUARTERKB						ONEKB / 4
#define ONESECOND						1000
#define HALFSECOND					ONESECOND /2 
#define QUARTERSECOND				ONESECOND / 4
#define ONEMINUTE						ONESECOND * 60
#define HALFMINUTE					ONESECOND * 30
#define QUARTERMINUTE				ONESECOND * 15
#define ONESECONDIN100NANO			-10000000LL
#define START_TRANSACTION_TIMER	2*ONESECOND
#define NO_FILE						INVALID_HANDLE_VALUE
#define NO_DRIVER						-1

// timers to adjust
#define SECONDS_TO_WAIT_A_COMMAND	5

// ELC structures sizes
#define COMMAND_SIZE		3
#define CR_SIZE			3

// ELC structs
typedef struct
{
	char stx;
} ELCSTX, * PELCSTX;
typedef struct
{
	char cmd[COMMAND_SIZE];
} ELCCMD, * PELCCMD;
typedef struct
{
	char etx;
} ELCETX, * PELCETX;
typedef struct
{
	char lrc;
} ELCLRC, * PELCLRC;
typedef struct
{
	ELCSTX stx;
	ELCCMD cmd;
} REQUESTHEADER, * PREQUESTHEADER;
typedef struct
{
	ELCETX etx;
	ELCLRC lrc;
} REQUESTTRAILER, * PREQUESTTRAILER;
typedef struct
{
	ELCSTX stx;
	ELCCMD cmd;
} REPLYHEADER, * PREPLYHEADER;
typedef struct
{
	char cr[CR_SIZE];
} REPLYCR, * PREPLYCR;
typedef struct
{
	ELCETX etx;
	ELCLRC lrc;
} REPLYTRAILER, * PREPLYTRAILER;

// Request management macros
#define REQUEST								_request_			/* the request order to send */
#define REQUEST_EX							_request_ex_		/* additional request order data */
#define REQUEST_DATA							_request_data_		/* data to send to the ELC */
#define REPLY									_reply_				/* expected reply from the ELC */
#define REPLY_EX								_reply_ex_			/* expected additional reply order data */
#define REPLY_DATA							_reply_data_		/* data returned from the ELC */
#define REPLY_CR								_reply_cr_			/* data returned from the ELC */
#define SETREQUESTANDREPLY					 memset(&MYPELC->CR, 0x00, sizeof(REPLYCR));MYPFNC->pRequest = REQUEST;MYPFNC->pRequestEx = REQUEST_EX;MYPFNC->pRequestData = REQUEST_DATA;MYPFNC->pReply = REPLY;MYPFNC->pReplyEx = REPLY_EX;MYPFNC->pReplyCR=REPLY_CR;

// status management macros
#define CR1										MYPELC->pCurrentFnc->CR.cr[0]
#define CR2										MYPELC->pCurrentFnc->CR.cr[1]
#define CR3										MYPELC->pCurrentFnc->CR.cr[2]

typedef struct
{
	char * pFnc;
	char * pRequest;				// type of order to the ELC (state, read, write)
	char * pRequestEx;			// additional order details to the ELC (write only)
	char * pRequestData;			// data transmitted for processing to the ELC (write only)
	char * pReply;					// reply from the ELC (state, read, write)
	char * pReplyEx;				// additional order details from the ELC (read only)
	char * pReplyData;			// data received from the ELC (read only)
	char * pReplyCR;				// indicates which CR is meaningfull on return
	DWORD dwReplyDataSize;		// size of data received from the ELC (read only)
	DWORD nbSession;				// current number of session initiation attempts
	DWORD nbData;					// current number of data reception attempts
	REPLYCR CR;						// the CR received from the ELC
	EventResult eventResult;	// reqult of the async operation (read and write operations only)
	//char * pchBuffer;				// exchange buffer used to pass data to or from the order to send to the ELC (read and write operations only)
} ELCFNC, * PELCFNC;

// driver working structure
typedef struct
{
	HANDLE handle;				// handle to the ELC
	DCB dcbInitial;			// COMM initial settings (when opening the ELC)
	DCB dcbCurrent;			// COMM as set for use with the ELC
	DWORD BaudRate;			// Baudrate at which running
	int port;					// port used to connect the ELC
	HANDLE logHandle;			// log file handle
	HANDLE eventCompleted,	// used to signal the thread the process has been completed
		eventCancelled,		// used to signal the async thread the process has been cancelled
		eventError,				// used to signal the async thread the process has encountered an error
		mutexStopReading,		// used to indicate the thread must continue to read incoming data
		eventInProgress;		// allows managing async operations access
	HANDLE userEventCompleted,
		userEventCancelled,
		userEventTimeout,
		userEventError;
	DWORD timerInitiateRequest,// timer used while initiating a request exchange
		timerTerminateRequest,	// timer used while terminating a request exchange
		timerInitiateReply,		// timer used while initiating a reply exchange
		timerTerminateReply,		// timer used while terminating a reply request exchange
		timerSendOrder,			// timer used while sending an order command to the ELC (status, abort, read, write)
		receiveAttemps;			// number of attempts when trying to receive data from the ELC
	DWORD T0,
		T1,
		T2,
		TR,
		TRA,
		TD;
	ELCFNC status,
		abort,
		read,
		write;
	PELCFNC pCurrentFnc;
	REPLYCR CR;					// the last CR received from the ELC
} ELCSTRUCT, * PELCSTRUCT;
#define MYPELC									((PELCSTRUCT)pelc)
#define MYPELCSTRUCT							PELCSTRUCT pelcstruct = (PELCSTRUCT)pelc;
#define MYFNC(_FnC_)							MYPELC->pCurrentFnc=&_FnC_;
#define MYPFNC									MYPELC->pCurrentFnc
#define MYPELCEX(_PeLC_)					((PELCSTRUCT)_PeLC_)
#define LOGCRLF								LOG(pelc, NULL, true)

static char STATUS_REQUEST[COMMAND_SIZE + 1] = { 0x30, 0x30, 0x34, 0x00 };
static char STATUS_REQUEST_EX[1] = { 0x00 };
static char STATUS_REQUEST_DATA[1] = { 0x00 };
static char STATUS_REPLY[COMMAND_SIZE + 1] = { 0x30, 0x34, 0x34, 0x00 };
static char STATUS_REPLY_CR[CR_SIZE] = { 0x01, 0x00, 0x03 };
static char STATUS_REPLY_EX[1] = { 0x00 };

static char ABORT_REQUEST[COMMAND_SIZE + 1] = { 0x30, 0x30, 0x30, 0x00 };
static char ABORT_REQUEST_EX[1] = { 0x00 };
static char ABORT_REQUEST_DATA[1] = { 0x00 };
static char ABORT_REPLY[COMMAND_SIZE + 1] = { 0x30, 0x34, 0x30, 0x00 };
static char ABORT_REPLY_CR[CR_SIZE] = { 0x01, 0x02, 0x00 };
static char ABORT_REPLY_EX[1] = { 0x00 };

static char READ_REQUEST[COMMAND_SIZE + 1] = { 0x31, 0x30, 0x30 ,0x00 };
static char READ_REQUEST_EX[1] = { 0x00 };
static char READ_REQUEST_DATA[1] = { 0x00 };
static char READ_REPLY[COMMAND_SIZE + 1] = { 0x31, 0x34, 0x30 ,0x00 };
static char READ_REPLY_CR[CR_SIZE] = { 0x01, 0x02, 0x03 };
static char READ_REPLY_EX[2] = { 0x68 , 0x00 };

static char WRITE_REQUEST[COMMAND_SIZE + 1] = { 0x30, 0x31, 0x30, 0x00 };	// write order
static char WRITE_REQUEST_EX[6] = { 0x65, 0x30, 0x30, 0x30, 0x30 ,0x00 };	// additional write order
static char WRITE_REPLY[COMMAND_SIZE + 1] = { 0x31, 0x34, 0x30,0x00 };	// expected reply
static char WRITE_REPLY_CR[CR_SIZE] = { 0x01, 0x02, 0x00 };
static char WRITE_REPLY_EX[1] = { 0x00 };	// expected additional reply

// protocol timers
#define TT0										10
#define SLEEPT0								Sleep(MYPELC->T0)
#define TT1										500
#define SLEEPT1								Sleep(MYPELC->T1)
#define TT2										1000
#define SLEEPT2								Sleep(MYPELC->T2)
#define TTR										10
#define SLEEPTR								Sleep(MYPELC->TR)
#define TTRA									100
#define SLEEPTRA								Sleep(MYPELC->TRA)
#define TTD										(CBR_9600 == pelcstruct->dcbCurrent.BaudRate ? ONESECOND : CBR_4800 == pelcstruct->dcbCurrent.BaudRate ? ONESECOND * 2 : CBR_2400 == pelcstruct->dcbCurrent.BaudRate ? ONESECOND * 4 : ONESECOND * 6)
#define SLEEPTD								Sleep(MYPELC->TD)
#define SLEEPAFTERSETCOMMSTATE			Sleep(100)

// processing result verification
#define ISTIMEOUT(_1_)						(WAIT_TIMEOUT == _1_)

// command exchange I/O timers
#define WRITE_COMMAND_TIMER				1000 /* 1 second */
#define READ_COMMAND_TIMER					1000 /* 1 second */

// control characters used throughout the protocol
#define ENQ										0x05
#define ACK										0x06
#define NAK										0x15
#define STX										0x02
#define ETX										0x03
#define EOT										0x04
#define NOCR									0x00
#define SPACE									0x20

#pragma endregion

#pragma region functions

#pragma region tools

/// <summary>
/// Log to the log file
/// </summary>
/// <param name="psz">Text to log</param>
/// <param name="addCRLFBefore">If true add a CRLF before the text to add</param>
DRIVERAPI void __stdcall LOG(ELC pelc, const char * psz, bool addCRLFBefore)
{
	MYPELCSTRUCT;
	if (NO_FILE != MYPELC->logHandle)
	{
		DWORD dwWritten;
		if (addCRLFBefore)
		{
			char * crlf = (char *)"\r\n";
			SetFilePointer(MYPELC->logHandle, 0, NULL, FILE_END);
			WriteFile(MYPELC->logHandle,
						 crlf,
						 strlen(crlf),
						 &dwWritten,
						 NULL);
			std::cout << std::endl;
		}
		if (NULL != psz)
		{
			SetFilePointer(MYPELC->logHandle, 0, NULL, FILE_END);
			WriteFile(MYPELC->logHandle,
						 psz,
						 strlen(psz),
						 &dwWritten,
						 NULL);
			std::cout << psz;
		}
	}
}

/// <summary>
/// Format and log a message
/// </summary>
/// <param name="pelc"></param>
/// <param name="psz">Message to log</param>
/// <param name="value">Eventually value to log</param>
/// <param name="addCRLFBefore">If true add a CRLF before the text to add</param>
DRIVERAPI void __stdcall LOGEX(ELC pelc, const char * psz, int value, bool addCRLFBefore)
{
	MYPELCSTRUCT;
	char * format;
	char * format1 = (char *)"%s";
	char * format2 = (char *)"%s (%d)";
	char dummy[ONEKB];
	format = (0 == value ? format1 : format2);
	sprintf_s(dummy, sizeof(dummy), format, psz, value);
	LOG(pelc, dummy, addCRLFBefore);
}

/// <summary>
/// Release a memory buffer and resets the pointer itself
/// </summary>
/// <param name="pp">Pointer to pointer of the buffer to release</param>
DRIVERAPI void __stdcall FREE(char ** pp)
{
	if (NULL != *pp)
	{
		free(*pp);
		*pp = NULL;
	}
}

/// <summary>
/// Allocate memory and restes it
/// </summary>
/// <param name="size">Size of buffer to allocate</param>
/// <returns>A pointer to a buffer is successfull, NULL otherwise</returns>
DRIVERAPI char * __stdcall CALLOC(size_t size)
{
	if (0 != size)
	{
		char * pv = (char *)calloc(1, size);
		if (NULL != pv)
			memset(pv, 0, size);
		return pv;
	}
	return NULL;
}

#pragma endregion

#pragma region display

/// <summary>
/// Get the hexadecimal representation of a character
/// </summary>
/// <param name="buffer">Buffer which will receive the string</param>
/// <param name="size">Size of buffer to allocate</param>
/// <returns></returns>
static char * RawRepresentation(char b, char * buffer, int size)
{
	sprintf_s(buffer, size, "%0.2X", b);
	return buffer;
}

/// <summary>
/// Get how to display a char
/// </summary>
/// <param name="b">Char to display</param>
/// <param name="buffer">Buffer which will receive the string</param>
/// <param name="size">Size of buffer to allocate</param>
/// <returns></returns>
static char * ConvertedRepresentation(char b, char * buffer, int size)
{
	switch (b)
	{
	case ENQ:
		sprintf_s(buffer, size, "ENQ");
		break;
	case ACK:
		sprintf_s(buffer, size, "ACK");
		break;
	case NAK:
		sprintf_s(buffer, size, "NAK");
		break;
	case STX:
		sprintf_s(buffer, size, "STX");
		break;
	case ETX:
		sprintf_s(buffer, size, "ETX");
		break;
	case EOT:
		sprintf_s(buffer, size, "EOT");
		break;
	default:
		RawRepresentation(b, buffer, size);
		break;
	}
	return buffer;
}

#pragma endregion

#pragma region IO

/// <summary>
/// Allows specifying that reading the COM poprt is allowed
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
static bool AllowReading(ELC pelc)
{
	MYPELCSTRUCT;
	switch (DWORD dw = WaitForSingleObject(MYPELC->mutexStopReading, 0))
	{
	case WAIT_ABANDONED:
	case WAIT_FAILED:
	case WAIT_TIMEOUT:
		return false;
	case WAIT_OBJECT_0:
		return true;
	default:
		return false;
	}
}

/// <summary>
/// ALlows specifying reading the COM port is no longer allowed
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
static bool ForbidReading(ELC pelc)
{
	MYPELCSTRUCT;
	bool f = ReleaseMutex(MYPELC->mutexStopReading);
	return f;
}

/// <summary>
/// Indicates whether reading the COM port is still allowed or not
/// Being able to take the mutex means it's been released by the process thus no more reading is requested
/// Only if the mutex can't be taken should reading to continue
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
static bool CanContinueReading(ELC pelc)
{
	MYPELCSTRUCT;
	if (WAIT_OBJECT_0 == WaitForSingleObject(MYPELC->mutexStopReading, 0))
	{
		ReleaseMutex(MYPELC->mutexStopReading);
		return false;
	}
	return true;
}

/// <summary>
/// Write data onto the ELC communication port
/// </summary>
/// <param name="pelc"></param>
/// <param name="data">Data to write</param>
/// <param name="length">Length of data to write</param>
/// <param name="piWritten">Number of characters written</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait for</param>
/// <returns></returns>
static bool WriteData(ELC pelc, char * data, int length, int * piWritten, LPDWORD pdwError, DWORD dwTimeout)
{
	MYPELCSTRUCT;
	bool success = false;
	char dummy[ONEKB];

#ifdef DOOVERLAPPING

	OVERLAPPED o = { };
	o.hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	if (NULL != o.hEvent)
	{
		if (!(success = WriteFile(MYPELC->handle, (LPCVOID)data, (DWORD)length, NULL, &o)))
		{
			if (ERROR_IO_PENDING == (*pdwError = GetLastError()))
			{
				if (WAIT_OBJECT_0 == (*pdwError = WaitForSingleObject(o.hEvent, (DWORD)dwTimeout)))
				{
					if (GetOverlappedResult(MYPELC->handle, &o, (LPDWORD)piWritten, FALSE))
						success = true;
					else
						*pdwError = GetLastError();
				}
				else
					*pdwError = GetLastError();
			}
		}
		CloseHandle(o.hEvent);
	}

#else

	success = WriteFile(MYPELC->handle, (LPCVOID)data, length, (LPDWORD)piWritten, NULL);

#endif // DOOVERLAPPING

	if (success)
	{
		LOG(pelc, " - WRITE OK ", false);
		if (0 == *piWritten)
			LOG(pelc, "- NO DATA WRITTEN", false);
		else
		{
			bool fRaw = false;
			LOG(pelc, "- DATA: ", false);
			for (int i = 1; *piWritten >= i; i++)
			{
				LOG(pelc, (fRaw ? RawRepresentation(data[i - 1], dummy, sizeof(dummy)) : ConvertedRepresentation(data[i - 1], dummy, sizeof(dummy))), false);
				LOG(pelc, " ", false);
				fRaw = (ETX == data[i - 1]);
			}
		}
	}
	else
	{
		DWORD dw = GetLastError();
		sprintf_s(dummy, ONEKB, " - WRITE KO (%d)", dw);// *pdwError);
		LOG(pelc, dummy, true);
	}

	success = success && *piWritten == length;
	return success;
}

/// <summary>
/// Read data fromthe ELC communication port
/// </summary>	
/// <param name="pelc"></param>
/// <param name="data">Buffer receiving the read data</param>
/// <param name="length">Size of buffer receiving the read data</param>
/// <param name="piRead">Number of characters read</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait for</param>
/// <returns></returns>
static bool ReadData(ELC pelc, char * data, int length, LPDWORD pdwRead, LPDWORD pdwError, DWORD dwTimeout)
{
	MYPELCSTRUCT;
	bool success = false;
	bool keepCarryOn = true;

#ifdef DOOVERLAPPING

	OVERLAPPED o = { 0 };
	o.hEvent = CreateEvent(NULL, false, false, NULL);
	if (NULL != o.hEvent)
	{
		HANDLE handles[] = { o.hEvent, MYPELC->mutexStopReading };
		*pdwRead = 0;
		while (keepCarryOn)
		{
			// read incoming characters
			success = ReadFile(MYPELC->handle, data, (DWORD)length, NULL, &o);
			if (!success)
			{
				keepCarryOn = false;
				if (ERROR_IO_PENDING == (*pdwError = GetLastError()))
				{
					switch (DWORD dw = WaitForMultipleObjects(_countof(handles), handles, false, INFINITE))
					{
					case WAIT_FAILED:
						*pdwError = GetLastError();
						break;

					case WAIT_TIMEOUT:
						*pdwError = GetLastError();
						break;

					case WAIT_ABANDONED_0:
						*pdwError = GetLastError();
						break;

					case WAIT_OBJECT_0:
						switch (dw - WAIT_OBJECT_0)
						{
						case 0: // overlap
							if (GetOverlappedResult(MYPELC->handle, &o, pdwRead, true))
							{
								success = true;
								if (keepCarryOn = 0 == *pdwRead && CanContinueReading(pelc))
									SLEEPT0;
							}
							else
							{
								*pdwError = GetLastError();
								//if (ERROR_IO_INCOMPLETE)
							}
							break;

						case 1: // stop reading
							break;

						default:
							*pdwError = GetLastError();
							break;
						}
						break;

					default:
						*pdwError = GetLastError();
						break;
					}
				}
				else
				{
					*pdwError = GetLastError();
				}
			}
			else
			{
				if (GetOverlappedResult(MYPELC->handle, &o, pdwRead, true))
				{
					if (keepCarryOn = 0 == *pdwRead && CanContinueReading(pelc))
						SLEEPT0;
				}
				else
				{
					keepCarryOn = false;
				}
			}
		}
		CloseHandle(o.hEvent);
	}
#else
	do
	{
		if (success = ReadFile(MYPELC->handle, data, length, (LPDWORD)piRead, NULL))
		{
			if (fContinue = (0 == *pdwRead && CanContinueReading(pelc)))
				SLEEPT0;
		}
	}
	while (fContinue && success);


#endif // DOOVERLAPPING

	//	if (success)
	//	{
	//#ifdef GETORDEREX
	//		if (fStart)
	//#endif // GETORDEREX
	//			LOG(pelc, " - READ OK ", false);
	//		if (0 == *pdwRead && 0 != length)
	//			LOG(pelc, "- NO DATA RECEIVED", false);
	//		else if (0 != *pdwRead)
	//		{
	//			bool fRaw = false;
	//#ifdef GETORDEREX
	//			if (fStart)
	//#endif // GETORDEREX
	//				LOG(pelc, "- DATA: ", false);
	//			for (DWORD i = 1; *pdwRead >= i; i++)
	//			{
	//				LOG(pelc, (fRaw ? RawRepresentation(data[i - 1], (char *)dummy, ONEKB) : ConvertedRepresentation(data[i - 1], (char *)dummy, ONEKB)), false);
	//				LOG(pelc, " ", false);
	//				//fRaw = fRaw || (ETX == data[i - 1]);
	//				fRaw = (ETX == data[i - 1]);
	//			}
	//		}
	//	}
	//	else
	//	{
	//		DWORD dw = GetLastError();
	//		sprintf_s(dummy, ONEKB, " - READ KO (%d)", dw);// *pdwError);
	//		LOG(pelc, dummy, false);
	//	}

	return success;
}

#pragma endregion

#pragma region synchronisation

/// <summary>
/// Safely close a handle
/// </summary>
/// <param name="h"></param>
static void CloseMyHandle(HANDLE h)
{
	if (NULL != h) CloseHandle(h);
}

/// <summary>
/// Indicates whether inside an async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="fAcquireMutex">If true the mutex can be acquired, if false the mutex is released if acquired</param>
/// <returns>True if within an async operation, false otherwise</returns>
static bool IsInProgress(ELC pelc, bool fAcquireMutex)
{
	MYPELCSTRUCT;
	// veriy an async operation is in progress
	DWORD dw = WaitForSingleObject(MYPELC->eventInProgress, 0);
	switch (dw)
	{
	case WAIT_OBJECT_0:
		// not inside an async operation
		if (!fAcquireMutex)
			ReleaseMutex(MYPELC->eventInProgress);
		return false;

	case WAIT_TIMEOUT:
		// an async operation is already in progress
		return true;
	}
	// arrived here a cCRITICAL ERROR HAS OCCURRED
	return false;
}

/// <summary>
/// Indicates the async operation has ended
/// </summary>
/// <param name="pelc"></param>
static void RazInProgress(ELC pelc)
{
	MYPELCSTRUCT;
	ReleaseMutex(MYPELC->eventInProgress);
}

/// <summary>
/// Prepare async environment
/// </summary>
/// <param name="pelc"></param>
static void RazAsyncEnvironment(ELC pelc)
{
	MYPELCSTRUCT;
	CloseMyHandle(MYPELC->userEventCompleted);
	CloseMyHandle(MYPELC->userEventCancelled);
	CloseMyHandle(MYPELC->userEventTimeout);
	CloseMyHandle(MYPELC->userEventError);
}

typedef struct
{
	ELC pelc;
	int timer;
	HANDLE started;
} THREADPROCASYNCRESULT, * PTHREADPROCASYNCRESULT;
/// <summary>
/// Determines the result of the operation (finished, cancelled, timeout or error)
/// </summary>
/// <param name="lpParameter"></param>
/// <returns></returns>
DWORD WINAPI ThreadAsyncResult(_In_ LPVOID lpParameter)
{
	PTHREADPROCASYNCRESULT pthr = (PTHREADPROCASYNCRESULT)lpParameter;
	ELC pelc = (PELCSTRUCT)pthr->pelc;

	HANDLE handles[] = { MYPELC->eventCompleted, MYPELC->eventCancelled, MYPELC->eventError };
	// ready to start
	SetEvent(pthr->started);
	DWORD dw = WaitForMultipleObjects(sizeof(handles) / sizeof(HANDLE), handles, false, (INFINITE != pthr->timer && 0 < pthr->timer ? pthr->timer * ONESECOND : INFINITE));
	switch (dw)
	{
	case WAIT_FAILED:
		dw = GetLastError();
		SetEvent(MYPELC->userEventError);
		break;
	case WAIT_TIMEOUT:
		SetEvent(MYPELC->userEventTimeout);
		break;
	default:
		{
			int index = dw - WAIT_OBJECT_0;
			switch (index)
			{
			case 0:
				SetEvent(MYPELC->userEventCompleted);
				break;
			case 1:
				SetEvent(MYPELC->userEventCancelled);
				break;
			default:
				SetEvent(MYPELC->userEventError);
				break;
			}
		}
	}
	CloseMyHandle(MYPELC->eventCompleted);
	CloseMyHandle(MYPELC->eventCancelled);
	CloseMyHandle(MYPELC->eventError);
	CloseHandle(pthr->started);
	ForbidReading(pelc);
	free(pthr);
	return 0;
}

/// <summary>
/// Start the thread which will determine the result of the async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="iTimer"></param>
/// <returns></returns>
static bool StartAsyncResultThread(ELC pelc, int iTimer)
{
	MYPELCSTRUCT;
	PTHREADPROCASYNCRESULT pthr = (PTHREADPROCASYNCRESULT)CALLOC(sizeof(THREADPROCASYNCRESULT));
	pthr->pelc = pelc;
	pthr->timer = iTimer;
	pthr->started = CreateEvent(NULL, true, false, NULL);
	MYPELC->eventCompleted = CreateEvent(NULL, true, false, NULL);
	MYPELC->eventCancelled = CreateEvent(NULL, true, false, NULL);
	MYPELC->eventError = CreateEvent(NULL, true, false, NULL);
	HANDLE h = CreateThread(NULL, ONEKB * 50, ThreadAsyncResult, pthr, 0, NULL);
	if (NULL != pthr->started)
		WaitForSingleObject(pthr->started, INFINITE);
	if (NULL == h)
	{
		CloseMyHandle(MYPELC->eventCompleted);
		CloseMyHandle(MYPELC->eventCancelled);
		return false;
	}
	return true;
}

/// <summary>
/// Prepare async environment
/// </summary>
/// <param name="pelc"></param>
static void PrepareAsyncEnvironment(ELC pelc)
{
	MYPELCSTRUCT;
	MYPELC->userEventCompleted = CreateEvent(NULL, true, false, NULL);
	MYPELC->userEventTimeout = CreateEvent(NULL, true, false, NULL);
	MYPELC->userEventCancelled = CreateEvent(NULL, true, false, NULL);
	MYPELC->userEventError = CreateEvent(NULL, true, false, NULL);
}

#pragma endregion

#pragma region COM functions

/// <summary>
/// Retrieve the COM port used by the ELC when connected using a USB cable
/// </summary>
/// <param name="usbDriver">USD driver to try to open</param>
/// <returns>COM port if found, NO_DRIVER otherwise</returns>
static int GetUSBComPort(char * usbDriver)
{
	int result = NO_DRIVER;
	HDEVINFO DeviceInfoSet;
	DWORD dwDeviceIndex = 0;
	SP_DEVINFO_DATA DeviceInfoData;
	PCSTR DevEnum = "USB";
	TCHAR ExpectedDeviceId[ONEKB] = { 0 }; //Store hardware id
	TCHAR szBuffer[ONEKB] = { 0 };
	DEVPROPTYPE ulPropertyType;
	DWORD dwSize = 0;
	DWORD dwError = 0;

	//create device hardware id
	strcpy_s(ExpectedDeviceId, usbDriver);

	//SetupDiGetClassDevs returns a handle to a device information set
	DeviceInfoSet = SetupDiGetClassDevs(NULL, DevEnum, NULL, DIGCF_ALLCLASSES | DIGCF_PRESENT);
	if (DeviceInfoSet == INVALID_HANDLE_VALUE)
		return result;

	//Fills a block of memory with zeros
	ZeroMemory(&DeviceInfoData, sizeof(SP_DEVINFO_DATA));
	DeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);

	//Receive information about an enumerated device
	while (SetupDiEnumDeviceInfo(DeviceInfoSet, dwDeviceIndex, &DeviceInfoData) && NO_DRIVER == result)
	{
		dwDeviceIndex++;
		//Retrieves a specified Plug and Play device property
		if (SetupDiGetDeviceRegistryProperty(DeviceInfoSet, &DeviceInfoData, SPDRP_HARDWAREID, &ulPropertyType, (PBYTE)szBuffer, sizeof(szBuffer), &dwSize))
		{
			HKEY hDeviceRegistryKey;
			if (0 == _strnicmp(szBuffer, ExpectedDeviceId, __min(strlen((char *)ExpectedDeviceId), strlen(szBuffer))))
			{
				//Get the key
				hDeviceRegistryKey = SetupDiOpenDevRegKey(DeviceInfoSet, &DeviceInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
				if (hDeviceRegistryKey == INVALID_HANDLE_VALUE)
				{
					dwError = GetLastError();
					break; //Not able to open registry
				}
				else
				{
					// Read in the name of the port
					wchar_t pszPortName[ONEKB];
					dwSize = sizeof(pszPortName);
					DWORD dwType = 0;

					if ((RegQueryValueEx(hDeviceRegistryKey, "PortName", NULL, &dwType, (LPBYTE)pszPortName, &dwSize) == ERROR_SUCCESS) && (dwType == REG_SZ))
					{
						// Check if it is really a com port
						if (0 == _strnicmp((TCHAR *)pszPortName, "COM", 3))
						{
							result = atoi((TCHAR *)pszPortName + 3);
						}
					}
					// Close the key now that we are finished with it
					RegCloseKey(hDeviceRegistryKey);
				}
			}
		}
	}
	if (DeviceInfoSet)
	{
		SetupDiDestroyDeviceInfoList(DeviceInfoSet);
	}
	return result;
}

/// <summary>
/// loop on all known USB->COM drivers to find the COM port used by the ELC
/// </summary>
/// <returns></returns>
static int GetComPort()
{
	char * ELC_NATIVE_USB_DRIVER = (char *)"USB\\VID_10C4&PID_EA60";
	char * ATEN_USB_DRIVER = (char *)"USB\\VID_0557&PID_2022";
	int port;

	// check all USB drivers
	if (NO_DRIVER == (port = GetUSBComPort(ATEN_USB_DRIVER)))
		if (NO_DRIVER == (port = GetUSBComPort(ELC_NATIVE_USB_DRIVER)))
			port = port;
	// return fournd port (if any)
	return port;
}

/// <summary>
/// Set the communication characteristics
/// </summary>
/// <param name="pelc"></param>
/// <param name="speed"></param>
/// <returns></returns>
static bool SetCommunicationState(ELC pelc, bool first, char eofChar)
{
	MYPELCSTRUCT;
	bool result = false;
	char dummy[ONEKB];
	DCB dcb;
	// set serial port exchange conditions
	PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
	GetCommState(MYPELC->handle, &dcb);
	if (first)
		memcpy(&MYPELC->dcbInitial, &dcb, sizeof(DCB));
	else
		memcpy(&MYPELC->dcbCurrent, &dcb, sizeof(DCB));
	dcb.ByteSize = 7;
	dcb.Parity = EVENPARITY;// (fSlave ? EVENPARITY : ODDPARITY);
	dcb.StopBits = ONESTOPBIT;
	dcb.BaudRate = (2400 == MYPELC->BaudRate ? CBR_2400 : (4800 == MYPELC->BaudRate ? CBR_4800 : CBR_9600));
	dcb.fDsrSensitivity = false;
	dcb.fDtrControl = DTR_CONTROL_DISABLE;
	dcb.fOutX = false;
	dcb.fInX = false;
	dcb.fErrorChar = false;
	dcb.fNull = false;
	dcb.fRtsControl = RTS_CONTROL_DISABLE;
	dcb.fAbortOnError = false;
	dcb.EofChar = eofChar;
	if (!(result = SetCommState(MYPELC->handle, &dcb)))
		ELCClose(pelc);
	else
	{
		SLEEPAFTERSETCOMMSTATE;
		GetCommState(MYPELC->handle, &dcb);
		memcpy(&MYPELC->dcbCurrent, &dcb, sizeof(DCB));
		sprintf_s(dummy, sizeof(dummy), "OPENING COM%d\n\tSpeed: %d\n\t%d data bits\n\t%s stop bit(s)\n\t%s parity\n", MYPELC->port, dcb.BaudRate, dcb.ByteSize, (ONESTOPBIT == dcb.StopBits ? "1" : ONE5STOPBITS == dcb.StopBits ? "1.5" : "2"), (EVENPARITY == dcb.Parity ? "EVEN (paire)" : ODDPARITY == dcb.Parity ? "ODD (impaire)" : NOPARITY == dcb.Parity ? "NO" : "OTHER"));
		LOG(pelc, dummy, true);
	}
	return result;
}

#pragma endregion

#pragma region communications

/// <summary>
/// Display read buffer
/// </summary>
/// <param name="pelc"></param>
/// <param name="result"></param>
/// <param name="pb"></param>
/// <param name="size"></param>
/// <param name="pdwReceived"></param>
/// <param name="pdwError"></param>
/// <param name="psz"></param>
static void DisplayReceivedBuffer(ELC pelc, bool result, char * pb, int size, DWORD dwReceived, DWORD dwError, const char * psz)
{
	char dummy[ONEKB];
	if (result)
	{
		if (NULL != psz)
			LOG(pelc, " - READ OK ", false);
		if (0 == dwReceived && 0 != size)
		{
			if (NULL != psz)
				LOG(pelc, "- NO DATA RECEIVED", false);
		}
		else if (0 != dwReceived)
		{
			bool fRaw = false;
			if (NULL != psz)
				LOG(pelc, "- DATA: ", false);
			for (DWORD i = 1; dwReceived >= i; i++)
			{
				LOG(pelc, (fRaw ? RawRepresentation(pb[i - 1], (char *)dummy, sizeof(dummy)) : ConvertedRepresentation(pb[i - 1], (char *)dummy, sizeof(dummy))), false);
				LOG(pelc, " ", false);
				//fRaw = fRaw || (ETX == data[i - 1]);
				fRaw = (ETX == pb[i - 1]);
			}
		}
	}
	else
	{
		sprintf_s(dummy, ONEKB, " - READ KO (%d)", dwError);
		LOG(pelc, dummy, false);
	}
}

/// <summary>
/// Receive data from the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pb"></param>
/// <param name="size"></param>
/// <param name="pdwReceived"></param>
/// <param name="pdwError"></param>
/// <param name="dwTimeout"></param>
/// <param name="psz"></param>
/// <returns></returns>
static bool GetReply(ELC pelc, char * pb, int size, LPDWORD pdwReceived, LPDWORD pdwError, DWORD dwTimeout, const char * psz)
{
#define GETREPLYATTEMPTS 5

	MYPELCSTRUCT;

	bool fOK, fContinue;
	int nbAttempts = 1;
	char dummy[ONEKB];

	if (NULL != psz)
	{
		sprintf_s(dummy, "%s", psz);

#ifdef DOOVERLLAPED
		LOGEX(pelc, psz, dwTimeout,
#else
		LOG(pelc, psz,
#endif // DOOVERLLAPED
			 true);
	}

	do
	{
		fOK = ReadData(pelc, pb, size, pdwReceived, pdwError, dwTimeout);
		SLEEPTR;
		if (fOK && 0 == *pdwReceived)
			if (fContinue = GETREPLYATTEMPTS > nbAttempts)
				nbAttempts++;
			else
				fOK = false;
		else
			fContinue = false;
	}
	while (fContinue && fOK);

	DisplayReceivedBuffer(pelc, fOK, pb, size, *pdwReceived, *pdwError, psz);
	return fOK;
}

/// <summary>
/// Receive a command from the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="chExpectedCommand">The expected command</param>
/// <param name="pchCmd">The command received</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait the command for</param>
/// <param name="psz">Message to log waiting for the command</param>
/// <returns>True if the received command is the expected one, false otherwise</returns>
static bool GetCMD(ELC pelc, char chExpectedCommand, char * pchCmd, LPDWORD pdwError, DWORD dwTimeout, const char * psz)
{
	bool fOK;
	DWORD dwRead;
	if (!(fOK = chExpectedCommand == *pchCmd))
	{
		if (fOK = GetReply(pelc, pchCmd, 1, &dwRead, pdwError, dwTimeout, psz))
		{
			fOK = (chExpectedCommand == *pchCmd);
		}
	}
	else
	{
		char dummy[ONEKB];
		char dummy1[ONEKB];
		sprintf_s(dummy1, "\t%s HAS ALREADY BEEN RECEIVED", ConvertedRepresentation(chExpectedCommand, (char *)dummy, sizeof(dummy)));
		LOG(pelc, dummy1, true);
	}
	return fOK;
}
static bool GetACK(ELC pelc, char * pchCmd, LPDWORD pdwError, DWORD dwTimeout) { bool fOK = GetCMD(pelc, ACK, pchCmd, pdwError, dwTimeout, "\tWAITING ACK"); if (fOK) { } return fOK; }
static bool GetNAK(ELC pelc, char * pchCmd, LPDWORD pdwError, DWORD dwTimeout) { bool fOK = GetCMD(pelc, NAK, pchCmd, pdwError, dwTimeout, "\tWAITING NAK"); if (fOK) { } return fOK; }
static bool GetENQ(ELC pelc, char * pchCmd, LPDWORD pdwError, DWORD dwTimeout) { bool fOK = GetCMD(pelc, ENQ, pchCmd, pdwError, dwTimeout, "\tWAITING ENQ"); if (fOK) { } return fOK; }
static bool GetEOT(ELC pelc, char * pchCmd, LPDWORD pdwError, DWORD dwTimeout) { bool fOK = GetCMD(pelc, EOT, pchCmd, pdwError, dwTimeout, "\tWAITING EOT"); return fOK; }

/// <summary>
/// Send an order to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pbBuffer"></param>
/// <param name="size"></param>
/// <param name="piSent"></param>
/// <param name="pdwError"></param>
/// <param name="dwTimeout"></param>
/// <param name="psz"></param>
/// <returns></returns>
static bool SendRequest(ELC pelc, char * pchBuffer, int size, int * piSent, LPDWORD pdwError, DWORD dwTimeout, const char * psz)
{
	MYPELCSTRUCT;

#ifdef SENDORDEREX
	if (1 > size) return false;
	bool fOK;
	int sent = 0;
	// send STX or command
	if (fOK = WriteData(pelc, pchBuffer, 1, &sent, pdwError, dwTimeout))
	{
		*piSent = sent;
		if (1 < size)
		{
			// send data part
			if (fOK = WriteData(pelc, &pbBuffer[1], size - 1, &sent, pdwError, dwTimeout))
			{
				*piSent += sent;
			}
		}
	}
	return fOK;
#else
	bool fOK;
	char dummy[ONEKB];
	sprintf_s(dummy, "%s", psz);

#ifdef DOOVERLLAPED
	LOGEX(pelc, dummy, dwTimeout,
#else
	LOG(pelc, dummy,
#endif // DOOVERLLAPED
		 true);

	if (fOK = WriteData(pelc, pchBuffer, size, piSent, pdwError, dwTimeout))
		if (EOT != pchBuffer[0])
			SLEEPT1;
	return fOK;
#endif // SENDORDEREX
}

/// <summary>
/// Send command to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="cmd">Command to send</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="psz">Message to log waiting for the command</param>
/// <returns>True if the command has been sent, false otherwise</returns>
static bool SendCMD(ELC pelc, char cmd, LPDWORD pdwError, const char * psz)
{
	int dw;
	char buffer[2] = { cmd, 0 };
	bool fOK = SendRequest(pelc, buffer, strlen((char *)buffer), &dw, pdwError, WRITE_COMMAND_TIMER, psz);
	return fOK;
}
static bool SendENQ(ELC pelc, LPDWORD pdwError) { bool fOK = SendCMD(pelc, ENQ, pdwError, "\tSENDING ENQ"); return fOK; }
static bool SendACK(ELC pelc, LPDWORD pdwError) { bool fOK = SendCMD(pelc, ACK, pdwError, "\tSENDING ACK"); if (fOK) { SLEEPTRA; } return fOK; }
static bool SendNAK(ELC pelc, LPDWORD pdwError) { bool fOK = SendCMD(pelc, NAK, pdwError, "\tSENDING NAK"); if (fOK) { SLEEPTRA; } return fOK; }
static bool SendEOT(ELC pelc, LPDWORD pdwError) { bool fOK = SendCMD(pelc, EOT, pdwError, "\tSENDING EOT"); return fOK; }

/// <summary>
/// Compute LRC
/// </summary>
/// <param name="pelc"></param>
/// <param name="buffer"></param>
/// <param name="size"></param>
/// <returns></returns>
static char LRC(ELC pelc, char * buffer, int size)
{
	MYPELCSTRUCT;
	char dummy[ONEKB];
	char lrc = 0;
	LOG(pelc, "COMPUTING LRC", true);
	LOG(pelc, "\tLRC DATA: ", true);
	for (int i = 1; i <= size; i++)
	{
		LOG(pelc, ConvertedRepresentation(buffer[i - 1], dummy, ONEKB), false);
		LOG(pelc, " ", false);
		lrc = lrc ^ buffer[i - 1];
	}
	LOG(pelc, "\tLRC: ", true);
	LOG(pelc, RawRepresentation(lrc, (char *)dummy, ONEKB), false);
	return lrc;
}

/// <summary>
/// Test whether the ELC device is open
/// </summary>
/// <param name="pelc"></param>
/// <returns>True if open, false otherwise</returns>
static bool IsOpen(ELC pelc)
{
	MYPELCSTRUCT;
	return (NULL != pelc && NO_FILE != MYPELC->handle);
}

#pragma endregion

#pragma region dialog management

/// <summary>
/// Warn the ELC we're going to send him data
/// </summary>
/// <param name="pelc"></param>
/// <param name="lastReadCharacter">last read character if any, 0x00 otherwise</param>
/// <param name="fSendACK">True if must send an ACK</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait the command reply for</param>
/// <returns></returns>
static bool EndReply(ELC pelc, char lastReadCharacter, bool fSendACK, LPDWORD pdwError, DWORD dwTimeout)
{
	MYPELCSTRUCT;
	bool fOK;
	char buffer[2] = { ENQ, 0 };
	// send ENQ
	LOG(pelc, "END RECEIVING", true);
	if (fOK = (fSendACK ? SendACK(pelc, pdwError) : SendNAK(pelc, pdwError)))
	{
		char b = lastReadCharacter;
		// wait EOT
		fOK = GetEOT(pelc, &b, pdwError, dwTimeout);
		// purge COM line to complete
		PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
	}
	return fOK;
}

/// <summary>
/// Wait for the ELC to start sending data
/// </summary>
/// <param name="pelc"></param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait the command for</param>
/// <returns></returns>
static bool StartReply(ELC pelc, LPDWORD pdwError, DWORD dwTimeout)
{
	MYPELCSTRUCT;
	bool fOK;
	char b = 0x00;
	// wait ENQ
	LOG(pelc, "START RECEIVING", true);
	if (fOK = GetENQ(pelc, &b, pdwError, dwTimeout))
	{
		// send ACK
		fOK = SendACK(pelc, pdwError);
	}
	else
	{
		// got inappropriate response
		SendEOT(pelc, pdwError);
	}
	return fOK;
}

/// <summary>
/// Wait confirmation from the ELC the order has been received
/// </summary>
/// <param name="pelc"></param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait the command reply for</param>
/// <returns></returns>
static bool EndRequest(ELC pelc, LPDWORD pdwError, DWORD dwTimeout)
{
	MYPELCSTRUCT;
	bool fOK;
	char b = 0x00;
	// wait ENQ
	LOG(pelc, "END SENDING", true);
	if (fOK = GetACK(pelc, &b, pdwError, dwTimeout))
	{
		// send ACK
		fOK = SendEOT(pelc, pdwError);
	}
	else
	{
		// got inappropriate response
		SendEOT(pelc, pdwError);
	}
	return fOK;
}

/// <summary>
/// Warn the ELC we're going to send him data
/// </summary>
/// <param name="pelc"></param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="dwTimeout">Timer to wait the command for</param>
/// <returns></returns>
static bool StartSending(ELC pelc, LPDWORD pdwError, DWORD dwTimeout)
{
	MYPELCSTRUCT;
	bool fOK;
	char buffer[2] = { ENQ, 0 };
	// send ENQ
	LOG(pelc, "START SENDING", true);
	if (fOK = SendENQ(pelc, pdwError))
	{
		char b = 0x00;
		// wait ACK
		if (!(fOK = GetACK(pelc, &b, pdwError, dwTimeout)))
		{
			SendEOT(pelc, pdwError);
			PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
		}
	}
	return fOK;
}

/// <summary>
/// Process in case of timeout while waiting to receive a command.
/// Return value indicates whether waiting for the command must carry on (true) or not (false)
/// </summary>
/// <param name="pelc"></param>
/// <param name="pError"></param>
/// <returns></returns>
static bool InCaseOfTimeout(ELC pelc, LPDWORD pdwError)
{
	MYPELCSTRUCT;
	bool fContinue = ISTIMEOUT(*pdwError);// && CANCONTINUESESSION;
	SendEOT(pelc, pdwError);
	if (!fContinue)
		PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
	return fContinue;
}

#pragma endregion

#pragma region dialog

static bool Read1Char(ELC pelc, char * pchBuffer, LPDWORD pdwError, DWORD dwTimeout, const char * psz)
{
	DWORD dwReceived;
	bool fOK;
	char c = 0x00;
	//if (fOK = ReadData(pelc, &c, 1, &dwReceived, pdwError, dwTimeout))
	if (fOK = GetReply(pelc, &c, 1, &dwReceived, pdwError, dwTimeout, psz))
	{
		if (NULL != pchBuffer)
			*pchBuffer = c;
	}
	return (fOK && 0 != dwReceived);
}

static bool ReadUntil(ELC pelc, char toReach, char * pchBuffer, const DWORD dwSize, LPDWORD pdwReceived, LPDWORD pdwError, DWORD dwTimeout, const char * psz)
{
	bool fOK;
	char c;
	DWORD counter = 0;
	if (NULL != pdwReceived)
		*pdwReceived = 0;
	do
	{
		if (fOK = (((NULL != pchBuffer && dwSize > counter) || (NULL == pchBuffer))) && Read1Char(pelc, &c, pdwError, dwTimeout, psz))
		{
			counter++;
			if (NULL != pchBuffer && dwSize >= counter)
				pchBuffer[counter - 1] = c;
			if (NULL != pdwReceived)
				(*pdwReceived)++;
		}
	}
	while (fOK && toReach != c);
	return fOK;
}

/// <summary>
/// Receive the reply of an order
/// </summary>
/// <param name="pelc"></param>
/// <param name="dwExpectedDataSize"></param>
/// <param name="dwTimeout"></param>
/// <param name="psz"></param>
/// <param name="dwTimeout">Timer to wait for</param>
/// <returns></returns>
static bool ReceiveReply(ELC pelc, DWORD dwTimeout)
{
	PELCSTRUCT pelcstruct = (PELCSTRUCT)pelc;
	//MYPELCSTRUCT;

	bool fOK, fContinue = true;
	DWORD dwReceived, dwError;
	char * mypsz = MYPFNC->pFnc;

	// wait request to send from ELC
	if (fOK = StartReply(pelc, &dwError, dwTimeout))
	{
		// reply buffer sizes
		DWORD dwReplyExSize = strlen(MYPFNC->pReplyEx);
		DWORD dwReplyBufferSize = ONEKB;// sizeof(REPLYHEADER) + sizeof(REPLYCR) + dwReplyExSize + dwExpectedDataSize + sizeof(REPLYTRAILER) + 1; // +1 for safety
		char * pReplyBuffer = (char *)CALLOC(dwReplyBufferSize);
		char * pReplyBufferTmp = (char *)CALLOC(dwReplyBufferSize);
		char chAfterReply = 0x00;
		DWORD dwReceivedReplyBufferSize = 0;
		//DWORD offset = 0;
		bool fSTX = false, fETX = false, fLRC = false;
		char * pbETX = NULL;
		//SetCommunicationState(pelc, false, ETX);
		// read until STX
		if (fOK = ReadUntil(pelc, STX, NULL, 0, NULL, &dwError, dwTimeout, mypsz))
		{
			mypsz = NULL;
			pReplyBuffer[0] = STX;
			dwReceivedReplyBufferSize = 1;
			// read until STX
			if (fOK = ReadUntil(pelc, ETX, pReplyBuffer + dwReceivedReplyBufferSize, dwReplyBufferSize - dwReceivedReplyBufferSize, &dwReceived, &dwError, dwTimeout, mypsz))
			{
				dwReceivedReplyBufferSize += dwReceived;
				pbETX = pReplyBuffer + dwReceivedReplyBufferSize - 1;
				// read LRC
				if (fOK = (dwReplyBufferSize > dwReceivedReplyBufferSize && Read1Char(pelc, pReplyBuffer + dwReceivedReplyBufferSize, &dwError, dwTimeout, mypsz)))
				{
					mypsz = (char *)"TRASHING";
					// determine exact command size
					DWORD dwExactReplySize = pbETX - pReplyBuffer + 2; // 2 for LRC + ETX position
					DWORD dwReceivedReplyExDataSize = dwExactReplySize - sizeof(REPLYHEADER) - sizeof(REPLYCR) - sizeof(REPLYTRAILER);
					DWORD dwReceivedReplyDataSize = dwReceivedReplyExDataSize - dwReplyExSize;
					PREPLYHEADER pReplyHeader = (PREPLYHEADER)pReplyBuffer;
					PREPLYCR pReplyCR = (PREPLYCR)(pReplyBuffer + sizeof(REPLYHEADER));
					char * pReplyEx = (char *)pReplyCR + sizeof(REPLYCR);
					char * pReplyData = pReplyEx + dwReplyExSize;
					PREPLYTRAILER pReplyTrailer = (PREPLYTRAILER)(pReplyData + dwReceivedReplyDataSize);
					//PELCSTRUCT ps = MYPELCEX(pelc);
					// test received format
					if (fOK = (0 == memcmp(&pReplyHeader->cmd, MYPFNC->pReply, sizeof(ELCCMD))
						 && (0 == memcmp(pReplyEx, MYPFNC->pReplyEx, dwReplyExSize))
						 && (ETX == pReplyTrailer->etx.etx)))
					{
						// test LRC
						if (fOK = LRC(pelc, pReplyHeader->cmd.cmd, dwExactReplySize - sizeof(ELCSTX) - sizeof(REPLYTRAILER) + sizeof(ELCETX)) == pReplyTrailer->lrc.lrc)
						{
							LOG(pelc, " IS VALID", false);
							// save data
							memcpy_s(&MYPFNC->CR, sizeof(REPLYCR), pReplyCR, sizeof(REPLYCR));
							if (0 != dwReceivedReplyDataSize)
							{
								if (NULL != (MYPFNC->pReplyData = CALLOC(dwReceivedReplyDataSize + 1)))
								{
									memcpy_s(MYPFNC->pReplyData, dwReceivedReplyDataSize, pReplyData, dwReceivedReplyDataSize);
								}
							}
							MYPFNC->dwReplyDataSize = dwReceivedReplyDataSize;
						}
						else
						{
							LOG(pelc, " IS INVALID", false);
						}
						// read additonal output if necessary
						bool keepCarryOn = true;
						while (keepCarryOn && Read1Char(pelc, &chAfterReply, &dwError, dwTimeout, mypsz))
						{
							mypsz = NULL;
							char c;
							// if there's another STX let's read the full command
							if (STX == chAfterReply)
							{

								if (ReadUntil(pelc, ETX, NULL, 0, NULL, &dwError, dwTimeout, mypsz))
								{
									// read LRC
									if (Read1Char(pelc, &c, &dwError, dwTimeout, mypsz))
										// arrived here the full command has been read
										;
								}
							}
							// not a STX, test the character but it could be the next character in the protocol
							else if (0x00 != chAfterReply)
							{
								// it is we must carry on with the protocol itself
								keepCarryOn = false;
							}
							else
							{
								// THIS SHOULD NEVER HAPPEN - null character, let's carry on reading
								//keepCarryOn = false;
							}
						}
					}
					else
					{
						// invalid format
					}
				}
				else
				{
					// invalid buffer size
				}
			}
			else
			{
				// read has failed
			}
		}
		// terminate dialog
		EndReply(pelc, chAfterReply, fOK, &dwError, MYPELC->timerTerminateReply);
	}
	else
	{
		// error initialising reception
	}
	return fOK;
}

typedef struct
{
	ELC pelc;
	HANDLE started;
} THREADASYNCPROCESSING, * PTHREADASYNCPROCESSING;

/// <summary>
/// Thread processing an async reply
/// </summary>
/// <param name="lpParameter"></param>
/// <returns></returns>
DWORD WINAPI ThreadAsyncProcessing(_In_ LPVOID lpParameter)
{
	PTHREADASYNCPROCESSING pthr = (PTHREADASYNCPROCESSING)lpParameter;
	ELC pelc = pthr->pelc;
	DWORD dw = 0;

	MYPELCSTRUCT;

	// ready to start
	SetEvent(pthr->started);
	// start receiving the reply
	if (ReceiveReply(pthr->pelc, INFINITE))
		SetEvent(MYPELC->eventCompleted);
	else
		SetEvent(MYPELC->eventError);
	CloseHandle(pthr->started);
	free(pthr);
	return dw;
}

/// <summary>
/// Start the thread that will process the async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="psz"></param>
/// <param name="action"></param>
/// <returns></returns>
static bool StartAsyncProcessingThread(ELC pelc)
{
	MYPELCSTRUCT;
	PTHREADASYNCPROCESSING pthr = (PTHREADASYNCPROCESSING)CALLOC(sizeof(THREADASYNCPROCESSING));
	pthr->pelc = pelc;
	pthr->started = CreateEvent(NULL, true, false, NULL);
	// start thread
	HANDLE h = CreateThread(NULL, ONEKB * 50, ThreadAsyncProcessing, pthr, 0, NULL);
	if (NULL != pthr->started)
		WaitForSingleObject(pthr->started, INFINITE);
	return (NULL != h);
}

/// <summary>
/// Send an ELC order
/// </summary>
/// <param name="pelc"></param>
/// <param name="psz">A string describing the operation is progress</param>
/// <returns></returns>
static bool SendOrder(ELC pelc)
{
	MYPELCSTRUCT;
	bool fOK, fContinue = true;
	DWORD dwError;

	// purge COM port
	PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);

	// warn the ELC we want to send data
	if (fOK = StartSending(pelc, &dwError, MYPELC->timerInitiateRequest))
	{
		// request buffer sizes
		int dwRequestExSize = strlen(MYPFNC->pRequestEx);
		int dwRequestDataSize = strlen(MYPFNC->pRequestData);
		int dwRequestExDataSize = dwRequestExSize + dwRequestDataSize;
		int dwRequestBufferSize = sizeof(REQUESTHEADER) + dwRequestExDataSize + sizeof(REQUESTTRAILER);
		char * pRequestBuffer = (char *)CALLOC(dwRequestBufferSize);
		PREQUESTHEADER pRequestHeader = (PREQUESTHEADER)pRequestBuffer;
		PREQUESTTRAILER pRequestTrailer = (PREQUESTTRAILER)(pRequestBuffer + sizeof(REQUESTHEADER) + dwRequestExDataSize);
		char * pRequestEx = pRequestBuffer + sizeof(REQUESTHEADER);
		char * pRequestData = pRequestBuffer + sizeof(REQUESTHEADER) + dwRequestExDataSize;
		// prepare request buffer
		pRequestHeader->stx.stx = STX;
		memcpy_s(pRequestHeader->cmd.cmd, COMMAND_SIZE, MYPFNC->pRequest, COMMAND_SIZE);
		if (0 != dwRequestExSize)
			memcpy_s(pRequestEx, dwRequestExSize, MYPFNC->pRequestData, dwRequestExSize);
		if (0 != dwRequestDataSize)
			memcpy_s(pRequestData, dwRequestDataSize, MYPFNC->pRequestData, dwRequestDataSize);
		pRequestTrailer->etx.etx = ETX;
		pRequestTrailer->lrc.lrc = LRC(pelc, pRequestHeader->cmd.cmd, dwRequestBufferSize - sizeof(ELCSTX) - sizeof(REQUESTTRAILER) + sizeof(ELCETX));
		// send order to the ELC
		int dwWritten;
		if (fOK = SendRequest(pelc, pRequestBuffer, dwRequestBufferSize, &dwWritten, &dwError, MYPELC->timerSendOrder, MYPFNC->pFnc))
		{
			// wait ELC's confirmation the order has been received and processed
			if (fOK = EndRequest(pelc, &dwError, MYPELC->timerTerminateRequest))
			{
				// arrived here everything went right
			}
			else
			{
				// error finishing sending process
			}
		}
		FREE(&pRequestBuffer);
	}
	else
	{
		// error initialising communication
	}
	if (!fOK)
		PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
	return fOK;
}

/// <summary>
/// Save function CR to current CR
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfnc"></param>
static void SaveCR(ELC pelc)
{
	MYPELCSTRUCT;
	memcpy_s(&MYPELC->CR, sizeof(REPLYCR), &MYPELC->pCurrentFnc->CR, sizeof(REPLYCR));
}

#pragma endregion

#pragma region APIs

/// <summary>
/// Initialise the driver
/// Calling this function is mandatory before calling any other one
/// </summary>
/// <param name="pelc"></param>
/// <returns>A pointer to an internal structure if successfull, NULL otherwise</returns>
DRIVERAPI ELC __stdcall ELCInit()
{
	PELCSTRUCT pelcstruct;
	if (NULL != (pelcstruct = (PELCSTRUCT)CALLOC(sizeof(ELCSTRUCT))))
	{
		pelcstruct->handle = NO_FILE;
		pelcstruct->port = NO_DRIVER;
		pelcstruct->BaudRate = CBR_9600;
		pelcstruct->logHandle = NO_FILE;
		pelcstruct->mutexStopReading = NULL;
		pelcstruct->timerInitiateRequest = 2 * ONESECOND;
		pelcstruct->timerTerminateRequest = 2 * ONESECOND;
		pelcstruct->timerInitiateReply = 2 * ONESECOND;
		pelcstruct->timerTerminateReply = 2 * ONESECOND;
		pelcstruct->timerSendOrder = 2 * ONESECOND;
		pelcstruct->T0 = TT0;
		pelcstruct->T1 = TT1;
		pelcstruct->T2 = TT2;
		pelcstruct->TR = TTR;
		pelcstruct->TRA = TTRA;
		pelcstruct->TD = TTD;
	}
	return pelcstruct;
}

/// <summary>
/// Release resources
/// </summary>
/// <param name="pelc"></param>
DRIVERAPI void __stdcall ELCRelease(ELC * ppelc)
{
	ELCClose(*ppelc);
	FREE((char **)ppelc);
}

/// <summary>
/// Set the port to use to open the ELC
/// Setting the port will close the device if open
/// </summary>
/// <param name="pelc"></param>
/// <param name="port">the port to use. It should be greater or equal to 0</param>
/// <returns></returns>
DRIVERAPI bool __stdcall ELCSetPort(ELC pelc, int port)
{
	MYPELCSTRUCT;
	if (NULL != pelc && NO_DRIVER < port)
	{
		ELCClose(pelc);
		MYPELC->port = port;
		return true;
	}
	return false;
}

/// <summary>
/// Open the ELC device
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
DRIVERAPI bool __stdcall ELCOpen(ELC pelc)
{
	MYPELCSTRUCT;
	bool result = true;
	char dummy[ONEKB];

	// open the requested port
	if (NO_DRIVER == MYPELC->port)
		// get COM port used by the ELC
		MYPELC->port = GetComPort();
	if (NO_DRIVER != MYPELC->port)
		sprintf_s(dummy, "\\\\.\\COM%d", MYPELC->port);
	else
		return false;

	// open the COM port
	MYPELC->handle = CreateFile((LPCSTR)dummy,
										 GENERIC_READ | GENERIC_WRITE,
										 0,
										 0,
										 OPEN_EXISTING,

#ifdef DOOVERLAPPING

										 FILE_FLAG_OVERLAPPED,

#else

										 FILE_ATTRIBUTE_NORMAL,

#endif // DOOVERLAPPING

										 NULL);
	MYPELC->mutexStopReading = CreateMutex(NULL, false, NULL);
	if (result = (INVALID_HANDLE_VALUE != MYPELC->handle && NULL != MYPELC->mutexStopReading && SetCommunicationState(pelc, true, 0x00)))
	{
		// open log file
		char * pszLogFileName = (char *)CALLOC(ONEKB);
		sprintf_s(pszLogFileName, ONEKB, "ELC on COM%d.log", MYPELC->port);
		MYPELC->logHandle = CreateFile((LPCSTR)pszLogFileName,
												 FILE_GENERIC_WRITE,
												 FILE_SHARE_READ,
												 NULL,
												 OPEN_ALWAYS,
												 FILE_ATTRIBUTE_NORMAL,
												 NULL);
		free(pszLogFileName);
		LOGCRLF;
	}
	else
	{
		ELCClose(pelc);
	}
	return result;
}

/// <summary>
/// Get the port to which the ELC is connected
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
DRIVERAPI int __stdcall ELCPort(ELC pelc)
{
	MYPELCSTRUCT;
	if (NULL != pelc && IsOpen(pelc))
		return MYPELC->port;
	else
		return NO_DRIVER;
}

/// <summary>
/// Close the ELC device
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
DRIVERAPI void __stdcall ELCClose(ELC pelc)
{
	MYPELCSTRUCT;
	if (IsOpen(pelc))
	{
		// release all <<<>>>
		SetCommState(MYPELC->handle, &MYPELC->dcbInitial);
		SLEEPAFTERSETCOMMSTATE;
		if (NO_FILE != MYPELC->logHandle)
			CloseMyHandle(MYPELC->logHandle);
		MYPELC->logHandle = NO_FILE;
		if (NO_FILE != MYPELC->handle)
			CloseMyHandle(MYPELC->handle);
		MYPELC->handle = NO_FILE;
		if (NULL != MYPELC->mutexStopReading)
			CloseHandle(MYPELC->mutexStopReading);
		MYPELC->mutexStopReading = NULL;
	}
}

/// <summary>
/// Get teh status of the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfDocumentIsPresent">true if a check note was still inside the ELC, false otherwise</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI bool __stdcall ELCStatus(ELC pelc, bool * pfDocumentIsPresent)
{
	MYPELCSTRUCT;
	bool fOK = false;
	*pfDocumentIsPresent = false;

	char * REQUEST = STATUS_REQUEST;
	char * REQUEST_EX = STATUS_REQUEST_EX;
	char * REQUEST_DATA = STATUS_REQUEST_DATA;

	char * REPLY = STATUS_REPLY;
	char * REPLY_CR = STATUS_REPLY_CR;
	char * REPLY_EX = STATUS_REPLY_EX;

	MYPELC->status.pFnc = (char *)"STATUS";
	MYFNC(MYPELC->status);
	SETREQUESTANDREPLY;

#define STATUS_CR1_OK					0x31
#define STATUS_CR1_KO					0x34
#define STATUS_CR2_OK					0x30
#define STATUS_CR3_NO_DOCUMENT		0x30
#define STATUS_CR3_DOCUMENT			0x31

	if (fOK = AllowReading(pelc))
	{
		if (fOK = SendOrder(pelc))
		{
			if (fOK = ReceiveReply(pelc, MYPELC->timerInitiateReply))
			{
				SaveCR(pelc);
				if (fOK = (STATUS_CR1_OK == CR1))
					*pfDocumentIsPresent = STATUS_CR3_DOCUMENT == CR3;
			}
		}
		ForbidReading(pelc);
	}
	return fOK;
}

/// <summary>
/// Send an abort to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfDocumentEjected">OUT, true if the document has been ejected or no document was inside, false otherwise</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI bool __stdcall ELCAbort(ELC pelc, bool * pfDocumentEjected)
{
	MYPELCSTRUCT;
	bool fOK = true;
	*pfDocumentEjected = false;

	char * REQUEST = ABORT_REQUEST;
	char * REQUEST_EX = ABORT_REQUEST_EX;
	char * REQUEST_DATA = ABORT_REQUEST_DATA;

	char * REPLY = ABORT_REPLY;
	char * REPLY_CR = ABORT_REPLY_CR;
	char * REPLY_EX = ABORT_REPLY_EX;

	MYPELC->abort.pFnc = (char *)"ABORT";
	MYFNC(MYPELC->abort);
	SETREQUESTANDREPLY;

#define ABORT_CR1_OK					0x31
#define ABORT_CR1_KO					0x34
#define ABORT_CR2_OK					0x30
#define ABORT_CR2_KO					0x31

	if (fOK = AllowReading(pelc))
	{
		if (fOK = SendOrder(pelc))
		{
			if (fOK = ReceiveReply(pelc, MYPELC->timerInitiateReply))
			{
				SaveCR(pelc);
				if (fOK = (ABORT_CR1_OK == CR1))
					*pfDocumentEjected = ABORT_CR2_OK == CR2;
			}
		}
		ForbidReading(pelc);
	}
	return fOK;
}

/// <summary>
/// Internal start async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfnc"></param>
/// <param name="psz">The operation to start</param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <returns></returns>
static bool StartAsync(ELC pelc, int iTimer)
{
	MYPELCSTRUCT;
	bool fOK;

	// initialise sync environment
	PrepareAsyncEnvironment(pelc);

	if (fOK = AllowReading(pelc))
	{
		// start timeout thread
		if (fOK = StartAsyncResultThread(pelc, iTimer))
		{
			// send read order
			if (fOK = SendOrder(pelc))
			{
				// start thread waiting to receive answer
				if (fOK = StartAsyncProcessingThread(pelc))
				{
					// arrived here everything went right
				}
				else
				{
					// stop result thread
					SetEvent(MYPELC->eventError);
				}
			}
			else
			{
				// stop result thread
				SetEvent(MYPELC->eventError);
			}
		}

		// release resources if an error has occurred
		if (!fOK)
		{
			RazAsyncEnvironment(pelc);
			RazInProgress(pelc);
			ForbidReading(pelc);
		}
	}
	return fOK;
}

/// <summary>
/// Allows waiting for an async operation to end
/// </summary>
/// <param name="pelc"></param>
/// <param name="iTimer">Timer to wait for</param>
/// <returns><see </returns>
DRIVERAPI EventResult __stdcall ELCWaitAsync(ELC pelc, int iTimer)
{
	MYPELCSTRUCT;
	if (IsInProgress(pelc, false))
		return EventResult::eventError;

	MYFNC(MYPELC->read);

	HANDLE handles[] = { MYPELC->userEventCompleted, MYPELC->userEventCancelled, MYPELC->userEventTimeout, MYPELC->userEventError };
	DWORD dw = WaitForMultipleObjects(sizeof(handles) / sizeof(HANDLE), handles, false, (INFINITE != iTimer && 0 < iTimer ? iTimer * ONESECOND : INFINITE));
	switch (dw)
	{
	case WAIT_FAILED:
		MYPFNC->eventResult = EventResult::eventError;
		break;
	case WAIT_TIMEOUT:
		MYPFNC->eventResult = EventResult::eventNone;
		break;
	default:
		{
			int index = dw - WAIT_OBJECT_0;
			switch (index)
			{
			case 0:
				MYPFNC->eventResult = EventResult::eventCompleted;
				break;
			case 1:
				MYPFNC->eventResult = EventResult::eventCancelled;
				break;
			case 2:
				MYPFNC->eventResult = EventResult::eventTimeout;
				break;
			default:
				MYPFNC->eventResult = EventResult::eventError;
				break;
			}
			// arrived here all events can be closed
			RazAsyncEnvironment(pelc);
			RazInProgress(pelc);
		}
	}
	ForbidReading(pelc);
	if (EventResult::eventCompleted != MYPFNC->eventResult)
	{
		bool f;
		ELCAbort(pelc, &f);
	}
	return MYPFNC->eventResult;
}

/// <summary>
/// Read a CMC7
/// </summary>
/// <param name="pelc"></param>
/// <param name="ppchBuffer"></param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI bool __stdcall ELCReadAsync(ELC pelc, int iTimer)
{
	MYPELCSTRUCT;
	bool fOK = false;
	//	*pfRead = *pfDocumentInside = false;

		// are we already inside an async operation ?
	if (IsInProgress(pelc, true))
		return false;

	char * REQUEST = READ_REQUEST;
	char * REQUEST_EX = READ_REQUEST_EX;
	char * REQUEST_DATA = READ_REQUEST_DATA;

	char * REPLY = READ_REPLY;
	char * REPLY_CR = READ_REPLY_CR;
	char * REPLY_EX = READ_REPLY_EX;

	MYPELC->read.pFnc = (char *)"READ";
	MYFNC(MYPELC->read);
	SETREQUESTANDREPLY;

	return StartAsync(pelc, iTimer);
}

/// <summary>
/// Get
/// </summary>
/// <param name="pelc"></param>
/// <param name="ppchRawBuffer">Pointer to a pointer of char that will contain the content read by the ELC</param>
/// <param name="ppchChpnBuffer">Pointer to a pointer of char that will contain the content read by the ELC modified to fit CHPN needs</param>
/// <param name="pfDocumentInside">True if the document is still inside after reading</param>
/// <returns>True is successfull, false otherwise</returns>
DRIVERAPI bool __stdcall ELCReadResult(ELC pelc, char ** ppchRawBuffer, char ** ppchChpnBuffer, bool * pfDocumentInside)
{
	MYPELCSTRUCT;
	// are we already inside an async operation ?
	if (IsInProgress(pelc, true))
		return false;
	if (&MYPELC->read != MYPELC->pCurrentFnc)
		return false;
	if (EventResult::eventCompleted != MYPFNC->eventResult)
		return false;
	if (0 == MYPFNC->dwReplyDataSize || NULL == MYPFNC->pReplyData)
		return false;

#define	READ_CR1_OK						0x31
#define	READ_CR1_KO						0x34
#define	READ_CR2_OK						0x30
#define	READ_CR2_PAPER_ERROR			0x31
#define	READ_CR2_PARTIALLY_READ		0x32
#define	READ_CR3_DOCUMENT_EJECTED	0x30
#define	READ_CR3_DOCUMENT_INSIDE	0x31

	int len = MYPFNC->dwReplyDataSize + 1;
	// prepare output buffers
	if (NULL != ppchRawBuffer)
	{
		*ppchRawBuffer = CALLOC(len);
		strcpy_s(*ppchRawBuffer, len, MYPFNC->pReplyData);
	}
	if (NULL != ppchChpnBuffer)
	{
		*ppchChpnBuffer = CALLOC(len);
		strcpy_s(*ppchChpnBuffer, len, MYPFNC->pReplyData);
		for (DWORD i = 0; i < MYPFNC->dwReplyDataSize; i++)
		{
			switch (*ppchChpnBuffer[i])
			{
			case 0x3A: // S1
				*ppchChpnBuffer[i] = 'B';
				break;
			case 0x3B: // S2
				break;
			case 0x3C: // S3
				*ppchChpnBuffer[i] = 'D';
				break;
			case 0x3D: // S4
				break;
			case 0x3E: // S5
				*ppchChpnBuffer[i] = 'F';
				break;
			case 0x3F: // character failed to be read
				*ppchChpnBuffer[i] = 'A';
				break;
			}
		}
	}
	SaveCR(pelc);
	if (READ_CR1_OK == CR1)
	{
		if (READ_CR2_OK == CR2)
		{
			*pfDocumentInside = READ_CR3_DOCUMENT_INSIDE == CR3;
			return true;
		}
	}
	return false;
}

/// <summary>
/// Synchronous read
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess"></param>
/// <param name="iTimer"></param>
/// <returns></returns>
DRIVERAPI bool __stdcall ELCRead(ELC pelc, int iTimer, char ** ppchRawBuffer, char ** ppchChpnBuffer, bool * pfDocumentInside)
{
	MYPELCSTRUCT;
	if (ELCReadAsync(pelc, iTimer))
		switch (ELCWaitAsync(pelc, 0))
		{
		case EventResult::eventCompleted:
			return ELCReadResult(pelc, ppchRawBuffer, ppchChpnBuffer, pfDocumentInside);
		}
	return false;
}

/// <summary>
/// Send a print order to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess">true if printing went successfully, false otherwise</param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI bool __stdcall ELCWriteAsync(ELC pelc, const char * pszData, int iTimer)
{
	MYPELCSTRUCT;
	bool fOK = false;

	// are we already inside an async operation ?
	if (IsInProgress(pelc, true))
		return false;

	char * pb;
	// set size of data to send for printing
	if (NULL != pszData && MAX_WRITE_DATA_SIZE < (strlen(pszData)))
	{
		pb = (char *)CALLOC(MAX_WRITE_DATA_SIZE + 1);
		if (NULL != pb)
			strncpy_s(pb, sizeof(pb), pszData, sizeof(pb) - 1);
	}
	else if (NULL != pszData)
	{
		pb = (char *)CALLOC(strlen(pszData) + 1);
		if (NULL != pb)
			strcpy_s(pb, sizeof(pb), pszData);
	}
	else
	{
		return false;
	}

	char * REQUEST = WRITE_REQUEST;
	char * REQUEST_EX = WRITE_REQUEST_EX;
	char * REQUEST_DATA = pb;	// data to write

	char * REPLY = WRITE_REPLY;
	char * REPLY_CR = WRITE_REPLY_CR;
	char * REPLY_EX = WRITE_REPLY_EX;

	MYPELC->write.pFnc = (char *)"WRITE";
	MYFNC(MYPELC->write);
	SETREQUESTANDREPLY(MYPELC->write);

	return StartAsync(pelc, iTimer);
}

/// <summary>
/// Send a print order to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess">true if printing went successfully, false otherwise</param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI bool __stdcall ELCWriteResult(ELC pelc)
{
	MYPELCSTRUCT;
	// are we already inside an async operation ?
	if (IsInProgress(pelc, true))
		return false;
	if (&MYPELC->write != MYPELC->pCurrentFnc)
		return false;
	if (EventResult::eventCompleted != MYPFNC->eventResult)
		return false;

#define WRITE_CR1_OK					0x31
#define WRITE_CR1_KO					0x34
#define WRITE_CR2_OK					0x30
#define WRITE_CR2_KO					0x31

	SaveCR(pelc);
	if (WRITE_CR1_OK == CR1)
	{
		if (WRITE_CR2_OK == CR2)
		{
			return true;
		}
	}
	return false;
}

/// <summary>
/// Synchronous write
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess"></param>
/// <param name="iTimer"></param>
/// <returns></returns>
DRIVERAPI bool __stdcall ELCWrite(ELC pelc, const char * pszData, int iTimer)
{
	MYPELCSTRUCT;
	if (ELCWriteAsync(pelc, pszData, iTimer))
		switch (ELCWaitAsync(pelc, 0))
		{
		case EventResult::eventCompleted:
			return ELCWriteResult(pelc);
		}
	return false;
}

/// <summary>
/// Initiate a dialog with the ELC
/// </summary>
/// <param name="pelc"></param>
/// <returns>true if successfull, false otherwise</returns>
DRIVERAPI bool __stdcall ELCInitiateDialog(ELC pelc)
{
	MYPELCSTRUCT;
	bool fOK;
	DWORD dwError;
	if (fOK = AllowReading(pelc))
	{
		if (fOK = StartSending(pelc, &dwError, ONESECOND))
			SendEOT(pelc, &dwError);
		ForbidReading(pelc);
	}
	return fOK;
}

/// <summary>
/// Get ELC CR by index
/// </summary>
/// <param name="pelc"></param>
/// <param name="index">Index of CR to return</param>
/// <returns>The CR as pointed by the index if valid, 0 otherwise</returns>
DRIVERAPI char __stdcall ELCCR(ELC pelc, int index)
{
	MYPELCSTRUCT;
	if (0 <= index && CR_SIZE > index)
		return MYPELC->CR.cr[index - 1];
	else
		return 0x00;
}

/// <summary>
/// Set communication speed with ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="BaudRate">rate to use</param>
/// <returns>rate to use if successful, </returns>
DRIVERAPI DWORD __stdcall ELCSpeed(ELC pelc, DWORD BaudRate)
{
	MYPELCSTRUCT;
	if (IsOpen(pelc))
	{
		if (CBR_2400 == BaudRate || CBR_4800 == BaudRate || CBR_9600 == BaudRate)
		{
			if (MYPELC->BaudRate != BaudRate)
			{
				MYPELC->BaudRate = BaudRate;
				SetCommunicationState(pelc, false, 0x00);
			}
		}
		return MYPELC->BaudRate;
	}
	return MYPELC->BaudRate = 0;
}

/// <summary>
/// Indicate the operation is cancelled
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
DRIVERAPI bool __stdcall ELCCancelAsync(ELC pelc)
{
	MYPELCSTRUCT;
	if (IsInProgress(pelc, false))
	{
		return SetEvent(MYPELC->eventCancelled);
	}
	return false;
}

/// <summary>
/// Allows setting the timers guiding dialog 
/// </summary>
/// <param name="pelc"></param>
/// <param name="timer"></param>
/// <param name="value">In seconds for all timer..., in milliseconds for all Tx</param>
/// <returns></returns>
DRIVERAPI void __stdcall ELCSetTimer(ELC pelc, ELCTimer timer, DWORD value)
{
	MYPELCSTRUCT;
	if (0 == value)
		return;
	switch (timer)
	{
	case ELCTimer::timerInitiateRequest:
		MYPELC->timerInitiateRequest = value;
		break;
	case 	ELCTimer::timerTerminateRequest:
		MYPELC->timerTerminateRequest = value;
		break;
	case ELCTimer::timerInitiateReply:
		MYPELC->timerInitiateReply = value;
		break;
	case ELCTimer::timerTerminateReply:
		MYPELC->timerTerminateReply = value;
		break;
	case ELCTimer::timerSendOrder:
		MYPELC->timerSendOrder = value;
		break;
	case ELCTimer::T0:
		pelcstruct->T0 = value;
		break;
	case ELCTimer::T1:
		pelcstruct->T1 = value;
		break;
	case ELCTimer::T2:
		pelcstruct->T2 = value;
		break;
	case ELCTimer::TR:
		pelcstruct->TR = value;
		break;
	case ELCTimer::TRA:
		pelcstruct->TRA = value;
		break;
	case ELCTimer::TD:
		pelcstruct->TD = value;
		break;
	}
}

#pragma endregion

#pragma endregion
