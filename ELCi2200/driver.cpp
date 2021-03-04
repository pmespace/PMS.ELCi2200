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
#include <ShlObj.h>

#pragma region compilation options

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
#define NB_ATTEMPTS					3

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
	const char* pFnc;
	char* pRequest;				// type of order to the ELC (state, read, write)
	char* pRequestEx;			// additional order details to the ELC (write only)
	char* pRequestData;			// data transmitted for processing to the ELC (write only)
	char* pReply;					// reply from the ELC (state, read, write)
	char* pReplyEx;				// additional order details from the ELC (read only)
	char* pReplyData;			// data received from the ELC (read only)
	char* pReplyCR;				// indicates which CR is meaningfull on return
	DWORD dwReplyDataSize;		// size of data received from the ELC (read only)
	DWORD nbSession;				// current number of session initiation attempts
	DWORD nbData;					// current number of data reception attempts
	REPLYCR CR;						// the CR received from the ELC
	ELCResult elcResult;	// result of the async operation (read and write operations only)
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
	HANDLE completed,			// used to signal the thread the process has been completed
		cancelled,				// used to signal the async thread the process has been cancelled
		error,					// used to signal the async thread the process has encountered an error
		stopReading,			// used to indicate the thread must stop reading incoming data
		mutexInProgress;		// allows managing async operations access
	BOOL fCarryOnReading;
	BOOL fUseLog,
		fInProgress;
	HANDLE startTimer,
		processingIsOver,
		userStartTimerEvent,				// user event to signal when timer can be started
		userAsyncOperationEndedEvent;	// user event to signal when processing is over
	DWORD T1,
		T2,
		TA,
		TR,
		TRA,
		TD,
		timerBeforeAbort;
	ELCFNC status,
		abort,
		read,
		write;
	PELCFNC pCurrentFnc;
	REPLYCR CR;					// the last CR received from the ELC
	int N1, N2;
} ELCSTRUCT, * PELCSTRUCT;
#define MYPELC									((PELCSTRUCT)pelc)
#define MYPELCSTRUCT							PELCSTRUCT pelcstruct = (PELCSTRUCT)pelc;
#define MYFNC(_FnC_)							MYPELC->pCurrentFnc=&_FnC_;
#define MYPFNC									MYPELC->pCurrentFnc
#define MYPELCEX(_PeLC_)					((PELCSTRUCT)_PeLC_)
#define LOG(_tExT_,_nEwLiNe_)				AddLog(pelc, _tExT_, _nEwLiNe_, false)
#define LOGCRLF								AddLog(pelc, NULL, true, false)
#define LOGDATETIME(_PsZ_)					AddDateTime(pelc, (char*)_PsZ_)
#define LOGONLY(_tExT_)						AddLog(pelc, _tExT_, true, true)
#define LOGEX(_tExT_,_V_,_nEwLiNe_)		AddLogEx(pelc, _tExT_, _V_, _nEwLiNe_, false)
#define LOGONLYEX(_tExT_,_V_)				AddLogEx(pelc, _tExT_, _V_, true, true)
#define LOGLASTERROR(__PsZ__,_ErR_)		AddLogLastError(pelc, __PsZ__,_ErR_)

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

static char WRITE_REQUEST[COMMAND_SIZE + 1] = { 0x30, 0x31, 0x30, 0x00 };
static char WRITE_REQUEST_EX[6] = { 0x65, 0x30, 0x30, 0x30, 0x30 ,0x00 };
static char WRITE_REPLY[COMMAND_SIZE + 1] = { 0x30, 0x35, 0x30,0x00 };
static char WRITE_REPLY_CR[CR_SIZE] = { 0x01, 0x02, 0x00 };
static char WRITE_REPLY_EX[1] = { 0x00 };

static const char* STATUS_FNC = "STATUS ORDER";
static const char* ABORT_FNC = "ABORT ORDER";
static const char* READ_FNC = "READ ORDER";
static const char* WRITE_FNC = "WRITE ORDER";
static const char* COMPLETED_SUCCESSFULLY = "COMPLETED SUCCESSFULLY";
static const char* COMPLETED_WITH_ERROR = "COMPLETED WITH ERROR";
static const char* PROCESSING_IS_OVER = "PROCESSING IS OVER";
static const char* UNKNOWN_ERROR = "UNKNOWN ERROR";
static const char* ERROR_DURING_PROCESS = "ERROR DURING PROCESS";
static const char* CANCELLED_BY_USER = "CANCELLED BY USER";
static const char* TIMEOUT = "TIMEOUT";
static const char* TIMER_STARTED = "TIMER STARTED";

// protocol timers
#define TTA										10
#define SLEEPTA								Sleep(MYPELC->TA)
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
/// Release a memory buffer and resets the pointer itself
/// </summary>
/// <param name="pp">Pointer to pointer of the buffer to release</param>
static void FREE(char** pp)
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
static char* CALLOC(size_t size)
{
	if (0 != size)
	{
		char* pv = (char*)calloc(1, size);
		if (NULL != pv)
			memset(pv, 0, size);
		return pv;
	}
	return NULL;
}

/// <summary>
/// AddLog to the log file
/// </summary>
/// <param name="psz">Text to log</param>
/// <param name="addCRLF">If true add a CRLF before the text</param>
static void AddLog(ELC pelc, const char* psz, BOOL addCRLF, BOOL fileOnly)
{
	MYPELCSTRUCT;
	if (!MYPELC->fUseLog)
		return;
	if (addCRLF)
	{
		if (!fileOnly)
			std::cout << std::endl;
		if (NO_FILE != MYPELC->logHandle)
		{
			DWORD dwWritten;
			char* crlf = (char*)"\r\n";
			SetFilePointer(MYPELC->logHandle, 0, NULL, FILE_END);
			WriteFile(MYPELC->logHandle,
				crlf,
				strlen(crlf),
				&dwWritten,
				NULL);
		}
	}
	if (NULL != psz)
	{
		DWORD dwWritten;
		if (!fileOnly)
			std::cout << psz;
		SetFilePointer(MYPELC->logHandle, 0, NULL, FILE_END);
		WriteFile(MYPELC->logHandle,
			psz,
			strlen(psz),
			&dwWritten,
			NULL);
	}
}

/// <summary>
/// 
/// </summary>
/// <param name="pelc"></param>
/// <param name="psz"></param>
static void AddDateTime(ELC pelc, char* psz)
{
	MYPELCSTRUCT;
	if (!MYPELC->fUseLog)
		return;
	SYSTEMTIME st;
	GetLocalTime(&st);
	char date[ONEKB];
	if (0 != GetDateFormat(LOCALE_USER_DEFAULT, NULL, &st, "yyyy'/'MM'/'dd", date, _countof(date)))
	{
		char time[ONEKB];
		if (0 != GetTimeFormat(LOCALE_USER_DEFAULT, NULL, &st, "HH':'mm':'ss", time, _countof(time)))
		{
			char dummy[ONEKB * 5];
			if (NULL == psz)
				sprintf_s(dummy, "%s %s", date, time);
			else
				sprintf_s(dummy, "+++ %s - %s %s +++", psz, date, time);
			LOG(dummy, NULL != psz);
		}
	}
}

/// <summary>
/// Format and log a message
/// </summary>
/// <param name="pelc"></param>
/// <param name="psz">Message to log</param>
/// <param name="value">Eventually value to log</param>
/// <param name="addCRLF">If true add a CRLF before the text to add</param>
void AddLogEx(ELC pelc, const char* psz, int value, BOOL addCRLF, BOOL fileOnly)
{
	MYPELCSTRUCT;
	if (!MYPELC->fUseLog)
		return;
	char dummy[ONEKB];
	//sprintf_s(dummy, sizeof(dummy), "%s [%u]", psz, value);
	sprintf_s(dummy, "%s [%u]", psz, value);
	AddLog(pelc, dummy, addCRLF, fileOnly);
}

/// <summary>
/// Prepare an explanation for system last error
/// </summary>
/// <param name="pelc"></param>
/// <param name="dwError"></param>
/// <returns>String describing </returns>
static void AddLogLastError(ELC pelc, const char* psz, DWORD dwError)
{
	MYPELCSTRUCT;
	if (!MYPELC->fUseLog)
		return;
	LPCTSTR strErrorMessage = NULL;
	FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_ARGUMENT_ARRAY | FORMAT_MESSAGE_ALLOCATE_BUFFER,
		NULL,
		dwError,
		0,
		(LPSTR)&strErrorMessage,
		0,
		NULL);
	DWORD size = strlen(psz) + strlen(strErrorMessage) + ONEKB;
	char* pdummy = CALLOC(size);
	sprintf_s(pdummy, size, "%s [Error: %u - %s]", psz, dwError, strErrorMessage);
	LOG(pdummy, false);
	FREE(&pdummy);
	LocalFree((void*)strErrorMessage);
}

#pragma endregion

#pragma region display

/// <summary>
/// Get the hexadecimal representation of a character
/// </summary>
/// <param name="buffer">Buffer which will receive the string</param>
/// <param name="size">Size of buffer to allocate</param>
/// <returns></returns>
static char* RawRepresentation(char b, char* buffer, int size)
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
static char* ConvertedRepresentation(char b, char* buffer, int size)
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
/// Write data onto the ELC communication port
/// </summary>
/// <param name="pelc"></param>
/// <param name="data">Data to write</param>
/// <param name="length">Length of data to write</param>
/// <param name="piWritten">Number of characters written</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <returns></returns>
static BOOL WriteData(ELC pelc, char* data, int length, int* piWritten, LPDWORD pdwError)
{
	MYPELCSTRUCT;
	BOOL success = false;
	char dummy[ONEKB];

	OVERLAPPED o = { };
	o.hEvent = CreateEvent(NULL, false, false, NULL);
	if (NULL != o.hEvent)
	{
		if (!(success = WriteFile(MYPELC->handle, (LPCVOID)data, (DWORD)length, NULL, &o)))
		{
			if (ERROR_IO_PENDING == (*pdwError = GetLastError()))
			{
				if (WAIT_OBJECT_0 == (*pdwError = WaitForSingleObject(o.hEvent, (DWORD)WRITE_COMMAND_TIMER)))
				{
					if (GetOverlappedResult(MYPELC->handle, &o, (LPDWORD)piWritten, false))
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

	if (success)
	{
		LOG(" - WRITE OK ", false);
		if (0 == *piWritten)
			LOG("- NO DATA WRITTEN", false);
		else
		{
			BOOL fRaw = false;
			LOG("- DATA: ", false);
			for (int i = 1; *piWritten >= i; i++)
			{
				LOG((fRaw ? RawRepresentation(data[i - 1], dummy, sizeof(dummy)) : ConvertedRepresentation(data[i - 1], dummy, sizeof(dummy))), false);
				LOG(" ", false);
				fRaw = (ETX == data[i - 1]);
			}
		}
	}
	else
	{
		DWORD dw = GetLastError();
		//sprintf_s(dummy, sizeof(dummy), " - WRITE KO (%u)", dw);// *pdwError);
		sprintf_s(dummy, " - WRITE KO (%u)", dw);// *pdwError);
		LOG(dummy, false);
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
/// <param name="pfTimeout">True is ended with timeout, false otherwise</param>
/// <returns></returns>
static BOOL ReadData(ELC pelc, char* data, int length, LPDWORD pdwRead, LPDWORD pdwError, DWORD dwTimeout, BOOL* pfTimeout)
{
	MYPELCSTRUCT;
	BOOL success = false;
	BOOL keepCarryOn = true;
	*pfTimeout = false;

	OVERLAPPED o = { 0 };
	o.hEvent = CreateEvent(NULL, false, false, NULL);
	if (NULL != o.hEvent)
	{
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
					HANDLE handles[] = { o.hEvent, MYPELC->stopReading };
					/* If using the native driver (thus the USB interface on the ELC) WaitForMultipleObjects NEVER returns if INFINITE wait
					or always returns with a timeout if not INFINITE wait. Has the device received the command before ??? */
					switch (DWORD dw = WaitForMultipleObjects(_countof(handles), handles, false, dwTimeout))
					{
						case WAIT_FAILED:
							LOGLASTERROR("WAIT_FAILED", *pdwError = GetLastError());
							break;

						case WAIT_TIMEOUT:
							*pfTimeout = true;
							LOGLASTERROR("WAIT_TIMEOUT", *pdwError = GetLastError());
							break;

						case WAIT_ABANDONED_0:
							LOGLASTERROR("WAIT_ABANDONED", *pdwError = GetLastError());
							break;

						case WAIT_OBJECT_0:
						{
							int index = dw - WAIT_OBJECT_0;
							if (handles[index] == o.hEvent)
							{
								if (GetOverlappedResult(MYPELC->handle, &o, pdwRead, true))
								{
									success = true;
									keepCarryOn = 0 == *pdwRead;
									LOGONLY("WAIT_OBJECT: OVERLAPPED");
								}
								else
								{
									LOGLASTERROR("WAIT_OBJECT: OVERLAPPED", *pdwError = GetLastError());
								}
							}
							else if (handles[index] == MYPELC->stopReading)
							{
								LOGONLY("WAIT_OBJECT: STOP READING ");
								keepCarryOn = false;
							}
							else
							{
								LOGLASTERROR("WAIT_OBJECT", *pdwError = GetLastError());
							}
							break;
						}

						default:
							LOGLASTERROR("READFILE", *pdwError = GetLastError());
							break;
					}
				}
				else
				{
					LOGLASTERROR("WAIT_UNKNOWN", *pdwError = GetLastError());
				}
			}
			else
			{
				if (GetOverlappedResult(MYPELC->handle, &o, pdwRead, true))
				{
					keepCarryOn = 0 == *pdwRead && WAIT_OBJECT_0 != WaitForSingleObject(MYPELC->stopReading, 0);
				}
				else
				{
					LOGLASTERROR("OVERLAPPEDRESULT", *pdwError = GetLastError());
					keepCarryOn = false;
				}
			}
		}
		CloseHandle(o.hEvent);
	}
	return success;
}

#pragma endregion

#pragma region synchronisation

/// <summary>
/// Safely close a handle
/// </summary>
/// <param name="h"></param>
static void CloseMyHandle(HANDLE* h)
{
	if (NULL != *h) CloseHandle(*h);
	*h = NULL;
}

/// <summary>
/// Indicates whether inside an async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="fAcquireMutex">If true the mutex can be acquired, if false the mutex is released if acquired</param>
/// <returns>True if within an async operation, false otherwise</returns>
static BOOL ResetInProgress(ELC pelc)
{
	MYPELCSTRUCT;
	// take the mutex
	switch (WaitForSingleObject(MYPELC->mutexInProgress, INFINITE))
	{
		case WAIT_OBJECT_0:
			MYPELC->fInProgress = false;
			SetEvent(MYPELC->stopReading);
			SetEvent(MYPELC->processingIsOver);
			if (NULL != MYPELC->userAsyncOperationEndedEvent)
				SetEvent(MYPELC->userAsyncOperationEndedEvent);
			ReleaseMutex(MYPELC->mutexInProgress);
			return true;
	}
	// arrived here a CRITICAL ERROR HAS OCCURRED
	return false;
}

/// <summary>
/// Indicates whether inside an async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="fAcquireMutex">If true the mutex can be acquired, if false the mutex is released if acquired</param>
/// <returns>True if within an async operation, false otherwise</returns>
static BOOL SetInProgress(ELC pelc)
{
	MYPELCSTRUCT;
	// take the mutex
	switch (WaitForSingleObject(MYPELC->mutexInProgress, INFINITE))
	{
		case WAIT_OBJECT_0:
			MYPELC->fInProgress = true;
			ResetEvent(MYPELC->stopReading);
			ResetEvent(MYPELC->startTimer);
			ResetEvent(MYPELC->processingIsOver);
			ReleaseMutex(MYPELC->mutexInProgress);
			return true;
	}
	// arrived here a CRITICAL ERROR HAS OCCURRED
	return false;
}

/// <summary>
/// Indicates whether inside an async operation
/// </summary>
/// <param name="pelc"></param>
/// <returns>True if within an async operation, false otherwise</returns>
static BOOL IsInProgress(ELC pelc)
{
	MYPELCSTRUCT;
	// veriy an async operation is in progress
	switch (WaitForSingleObject(MYPELC->mutexInProgress, 0))
	{
		case WAIT_OBJECT_0:
			BOOL result = MYPELC->fInProgress;
			ReleaseMutex(MYPELC->mutexInProgress);
			return result;
	}
	return false;
}

/// <summary>
/// Prepare async environment
/// </summary>
/// <param name="pelc"></param>
/// <param name="startTimerEvent">Event signaled when the caller's timer has been started</param>
/// <param name="asyncOperationEndedEvent">Event signaled when the async operation has ended</param>
static void PrepareAsyncEnvironment(ELC pelc, HANDLE startTimerEvent, HANDLE asyncOperationEndedEvent)
{
	MYPELCSTRUCT;
	MYPELC->userAsyncOperationEndedEvent = asyncOperationEndedEvent;
	MYPELC->userStartTimerEvent = startTimerEvent;
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
	MYPELCSTRUCT;

	HANDLE handles[] = { MYPELC->completed, MYPELC->cancelled, MYPELC->error };
	// ready to start
	SetEvent(pthr->started);
	DWORD timerToStart = (INFINITE == pthr->timer ? INFINITE : pthr->timer * ONESECOND);
	// wait for processing to start to start timer and warn the caller
	WaitForSingleObject(MYPELC->startTimer, INFINITE);
	// indicate the calling application it can start its own timer
	if (NULL != MYPELC->userStartTimerEvent)
		SetEvent(MYPELC->userStartTimerEvent);
	DWORD dw = WaitForMultipleObjects(sizeof(handles) / sizeof(HANDLE), handles, false, timerToStart);
	switch (dw)
	{
		case WAIT_FAILED:
			dw = GetLastError();
			MYPFNC->elcResult = ELCResult::error;
			break;
		case WAIT_TIMEOUT:
			MYPFNC->elcResult = ELCResult::timeout;
			LOG(TIMEOUT, true);
			break;
		default:
		{
			int index = dw - WAIT_OBJECT_0;
			if (handles[index] == MYPELC->completed)
			{
				MYPFNC->elcResult = ELCResult::completed;
			}
			else if (handles[index] == MYPELC->cancelled)
			{
				MYPFNC->elcResult = ELCResult::cancelled;
				LOG(CANCELLED_BY_USER, true);
			}
			else if (handles[index] == MYPELC->error)
			{
				MYPFNC->elcResult = ELCResult::error;
				LOG(ERROR_DURING_PROCESS, true);
			}
			else
			{
				MYPFNC->elcResult = ELCResult::error;
				LOG(UNKNOWN_ERROR, true);
			}
			break;
		}
	}
	LOG(PROCESSING_IS_OVER, true);
	CloseHandle(MYPELC->completed);
	CloseHandle(MYPELC->cancelled);
	CloseHandle(MYPELC->error);
	// release waiting thread
	ResetInProgress(pelc);
	free(pthr);
	return 0;
}

/// <summary>
/// Start the thread which will determine the result of the async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="iTimer"></param>
/// <returns></returns>
static BOOL StartAsyncResultThread(ELC pelc, int iTimer)
{
	MYPELCSTRUCT;
	PTHREADPROCASYNCRESULT pthr = (PTHREADPROCASYNCRESULT)CALLOC(sizeof(THREADPROCASYNCRESULT));
	pthr->pelc = pelc;
	pthr->timer = iTimer;
	pthr->started = CreateEvent(NULL, false, false, NULL);
	MYPELC->completed = CreateEvent(NULL, false, false, NULL);
	MYPELC->cancelled = CreateEvent(NULL, false, false, NULL);
	MYPELC->error = CreateEvent(NULL, false, false, NULL);
	if (NULL != pthr->started &&
		NULL != MYPELC->completed &&
		NULL != MYPELC->cancelled &&
		NULL != MYPELC->error)
	{
		HANDLE h = CreateThread(NULL, ONEKB * 50, ThreadAsyncResult, pthr, 0, NULL);
		if (NULL != h)
		{
			WaitForSingleObject(pthr->started, INFINITE);
			CloseHandle(pthr->started);
			return true;
		}
		else
		{
			CloseHandle(MYPELC->completed);
			CloseHandle(MYPELC->cancelled);
			CloseHandle(MYPELC->error);
			CloseHandle(pthr->started);
			return false;
		}
	}
	return false;
}

#pragma endregion

#pragma region COM functions

/// <summary>
/// Retrieve the COM port used by the ELC when connected using a USB cable
/// </summary>
/// <param name="usbDriver">USD driver to try to open</param>
/// <returns>COM port if found, NO_COM_PORT otherwise</returns>
DRIVERAPI int DRIVERCALL ELCGetUSBComPort(char* usbDriver)
{
	int result = NO_COM_PORT;
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
	while (SetupDiEnumDeviceInfo(DeviceInfoSet, dwDeviceIndex, &DeviceInfoData) && NO_COM_PORT == result)
	{
		dwDeviceIndex++;
		//Retrieves a specified Plug and Play device property
		if (SetupDiGetDeviceRegistryProperty(DeviceInfoSet, &DeviceInfoData, SPDRP_HARDWAREID, &ulPropertyType, (PBYTE)szBuffer, sizeof(szBuffer), &dwSize))
		{
			HKEY hDeviceRegistryKey;
			if (0 == _strnicmp(szBuffer, ExpectedDeviceId, __min(strlen((char*)ExpectedDeviceId), strlen(szBuffer))))
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
						if (0 == _strnicmp((TCHAR*)pszPortName, "COM", 3))
						{
							result = atoi((TCHAR*)pszPortName + 3);
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
	char* ELC_NATIVE_USB_DRIVER = (char*)"USB\\VID_10C4&PID_EA60";
	char* ATEN_USB_DRIVER = (char*)"USB\\VID_0557&PID_2022";
	int port;

	// check all USB drivers
	if (NO_COM_PORT == (port = ELCGetUSBComPort(ATEN_USB_DRIVER)))
		if (NO_COM_PORT == (port = ELCGetUSBComPort(ELC_NATIVE_USB_DRIVER)))
			port = port;
	// return fournd port (if any)
	return port;
}

/// <summary>
/// Purge COM queue
/// </summary>
/// <param name="pelc"></param>
static void PurgeCommunication(ELC pelc)
{
	PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
}

/// <summary>
/// Set the communication characteristics
/// </summary>
/// <param name="pelc"></param>
/// <param name="speed"></param>
/// <returns></returns>
static BOOL SetCommunicationState(ELC pelc, BOOL first, char eofChar)
{
	MYPELCSTRUCT;
	BOOL result = false;
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
	if (result = SetCommState(MYPELC->handle, &dcb))
	{
		LOGCRLF;
		SLEEPAFTERSETCOMMSTATE;
		GetCommState(MYPELC->handle, &dcb);
		memcpy(&MYPELC->dcbCurrent, &dcb, sizeof(DCB));
		//sprintf_s(dummy, sizeof(dummy), "OPENING COM%u\n\tSpeed: %u\n\t%u data bits\n\t%s stop bit(s)\n\t%s parity\n", MYPELC->port, dcb.BaudRate, dcb.ByteSize, (ONESTOPBIT == dcb.StopBits ? "1" : ONE5STOPBITS == dcb.StopBits ? "1.5" : "2"), (EVENPARITY == dcb.Parity ? "EVEN (paire)" : ODDPARITY == dcb.Parity ? "ODD (impaire)" : NOPARITY == dcb.Parity ? "NO" : "OTHER"));
		sprintf_s(dummy, "OPENING COM%u\n\tSpeed: %u\n\t%u data bits\n\t%s stop bit(s)\n\t%s parity\n", MYPELC->port, dcb.BaudRate, dcb.ByteSize, (ONESTOPBIT == dcb.StopBits ? "1" : ONE5STOPBITS == dcb.StopBits ? "1.5" : "2"), (EVENPARITY == dcb.Parity ? "EVEN (paire)" : ODDPARITY == dcb.Parity ? "ODD (impaire)" : NOPARITY == dcb.Parity ? "NO" : "OTHER"));
		LOG(dummy, true);
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
static void DisplayReceivedBuffer(ELC pelc, BOOL result, char* pb, int size, DWORD dwReceived, DWORD dwError, DWORD dwTimeout, const char* psz)
{
	char dummy[ONEKB];
	if (result)
	{
		if (0 == dwReceived)// && 0 != size)
		{
			//if (NULL != psz)
			//{
			//	sprintf_s(dummy, "- TIMEOUT [%u]", dwTimeout);
			//	LOG(dummy, false);
			//}
		}
		else //if (0 != dwReceived)
		{
			if (NULL != psz)
				LOG(" - READ OK ", false);

			BOOL fRaw = false;
			if (NULL != psz)
				LOG("- DATA: ", false);
			for (DWORD i = 1; dwReceived >= i; i++)
			{
				//sprintf_s(dummy, sizeof(dummy), "%s ", (fRaw ? RawRepresentation(pb[i - 1], dummy, sizeof(dummy)) : ConvertedRepresentation(pb[i - 1], dummy, sizeof(dummy))));
				sprintf_s(dummy, "%s ", (fRaw ? RawRepresentation(pb[i - 1], dummy, sizeof(dummy)) : ConvertedRepresentation(pb[i - 1], dummy, sizeof(dummy))));
				LOG(dummy, false);
				//fRaw = fRaw || (ETX == pb[i - 1]);
				fRaw = (ETX == pb[i - 1]);
			}
		}
	}
	else
		LOGLASTERROR(psz, dwError);
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
/// <param name="pfTimeout"></param>
/// <param name="psz"></param>
/// <returns></returns>
static BOOL GetReply(ELC pelc, char* pb, int size, LPDWORD pdwReceived, LPDWORD pdwError, DWORD dwTimeout, BOOL* pfTimeout, const char* psz)
{
#define GETREPLYATTEMPTS 5

	MYPELCSTRUCT;

	BOOL fOK;
	char dummy[ONEKB];

	if (NULL != psz)
	{
		sprintf_s(dummy, "%s", psz);
		LOGEX(psz, dwTimeout, true);
	}

	fOK = ReadData(pelc, pb, size, pdwReceived, pdwError, dwTimeout, pfTimeout);
	SLEEPTR;
	DisplayReceivedBuffer(pelc, fOK, pb, size, *pdwReceived, *pdwError, dwTimeout, psz);
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
static BOOL GetCMD(ELC pelc, char chExpectedCommand, char* pchCmd, LPDWORD pdwError, DWORD dwTimeout, BOOL* pfTimeout, const char* psz)
{
	BOOL fOK;
	DWORD dwRead;
	char dummy[ONEKB];
	//sprintf_s(dummy, sizeof(dummy), "\tWAITING %s", psz);
	sprintf_s(dummy, "\tWAITING %s", psz);
	if (!(fOK = chExpectedCommand == *pchCmd))
	{
		if (fOK = GetReply(pelc, pchCmd, 1, &dwRead, pdwError, dwTimeout, pfTimeout, dummy))
		{
			fOK = (chExpectedCommand == *pchCmd);
		}
	}
	else
	{
		char dummy[ONEKB];
		char dummy1[ONEKB];
		//sprintf_s(dummy1, sizeof(dummy1), "\t%s HAS ALREADY BEEN RECEIVED", ConvertedRepresentation(chExpectedCommand, (char*)dummy, sizeof(dummy)));
		sprintf_s(dummy1, "\t%s HAS ALREADY BEEN RECEIVED", ConvertedRepresentation(chExpectedCommand, (char*)dummy, sizeof(dummy)));
		LOG(dummy1, true);
	}
	return fOK;
}
static BOOL WaitACK(ELC pelc, char* pchCmd, LPDWORD pdwError, BOOL* pfTimeout) { BOOL fOK = GetCMD(pelc, ACK, pchCmd, pdwError, MYPELC->T1, pfTimeout, "ACK"); if (fOK) {} return fOK; }
static BOOL WaitENQ(ELC pelc, char* pchCmd, LPDWORD pdwError, BOOL* pfTimeout)
{
	int t = IsInProgress(pelc) ? INFINITE : MYPELC->TA;
	BOOL fOK = GetCMD(pelc, ENQ, pchCmd, pdwError, IsInProgress(pelc) ? INFINITE : MYPELC->TA, pfTimeout, "ENQ");
	if (fOK) {}
	return fOK;
}
static BOOL WaitEOT(ELC pelc, char* pchCmd, LPDWORD pdwError, BOOL* pfTimeout) { BOOL fOK = GetCMD(pelc, EOT, pchCmd, pdwError, MYPELC->T1, pfTimeout, "EOT"); return fOK; }
static BOOL WaitSTX(ELC pelc, char* pchCmd, LPDWORD pdwError, BOOL* pfTimeout) { BOOL fOK = GetCMD(pelc, STX, pchCmd, pdwError, MYPELC->T1, pfTimeout, "STX"); return fOK; }

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
static BOOL SendRequest(ELC pelc, char* pchBuffer, int size, int* piSent, LPDWORD pdwError, const char* psz)
{
	MYPELCSTRUCT;
	char dummy[ONEKB];
	//sprintf_s(dummy, sizeof(dummy), "\tSENDING %s", psz);
	sprintf_s(dummy, "\tSENDING %s", psz);
	LOG(dummy, true);
	BOOL fOK = WriteData(pelc, pchBuffer, size, piSent, pdwError);
	if (fOK)
		SLEEPT1;
	return fOK;
}

/// <summary>
/// Send command to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="cmd">Command to send</param>
/// <param name="pdwError">Error if any (from GetLastError)</param>
/// <param name="psz">Message to log waiting for the command</param>
/// <returns>True if the command has been sent, false otherwise</returns>
static BOOL SendCMD(ELC pelc, char cmd, LPDWORD pdwError, const char* psz)
{
	int dw;
	char buffer[2] = { cmd, 0 };
	BOOL fOK = SendRequest(pelc, buffer, strlen((char*)buffer), &dw, pdwError, psz);
	return fOK;
}
static BOOL SendENQ(ELC pelc, LPDWORD pdwError) { BOOL fOK = SendCMD(pelc, ENQ, pdwError, "ENQ"); return fOK; }
static BOOL SendACK(ELC pelc, LPDWORD pdwError) { BOOL fOK = SendCMD(pelc, ACK, pdwError, "ACK"); if (fOK) { SLEEPTRA; } return fOK; }
static BOOL SendNAK(ELC pelc, LPDWORD pdwError) { BOOL fOK = SendCMD(pelc, NAK, pdwError, "NAK"); if (fOK) { SLEEPTRA; } return fOK; }
static BOOL SendEOT(ELC pelc, LPDWORD pdwError) { BOOL fOK = SendCMD(pelc, EOT, pdwError, "EOT"); return fOK; }

/// <summary>
/// Compute LRC
/// </summary>
/// <param name="pelc"></param>
/// <param name="buffer"></param>
/// <param name="size"></param>
/// <returns></returns>
static char LRC(ELC pelc, char* buffer, int size)
{
	MYPELCSTRUCT;
	char dummy[ONEKB];
	char lrc = 0;
	LOG("COMPUTING LRC", true);
	LOG("\tLRC DATA: ", true);
	for (int i = 1; i <= size; i++)
	{
		LOG(ConvertedRepresentation(buffer[i - 1], dummy, ONEKB), false);
		LOG(" ", false);
		lrc = lrc ^ buffer[i - 1];
	}
	LOG("\tLRC: ", true);
	LOG(RawRepresentation(lrc, (char*)dummy, ONEKB), false);
	return lrc;
}

/// <summary>
/// Test whether the ELC device is open
/// </summary>
/// <param name="pelc"></param>
/// <returns>True if open, false otherwise</returns>
static BOOL IsOpen(ELC pelc)
{
	MYPELCSTRUCT;
	return (NULL != pelc && NO_FILE != MYPELC->handle);
}

#pragma endregion

#pragma region dialog management

/// <summary>
/// Process in case of timeout while waiting to receive a command.
/// Return value indicates whether waiting for the command must carry on (true) or not (false)
/// </summary>
/// <param name="pelc"></param>
/// <param name="pError"></param>
/// <returns></returns>
static BOOL InCaseOfTimeout(ELC pelc, LPDWORD pdwError)
{
	MYPELCSTRUCT;
	BOOL fContinue = ISTIMEOUT(*pdwError);// && CANCONTINUESESSION;
	SendEOT(pelc, pdwError);
	if (!fContinue)
		PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);
	return fContinue;
}

#pragma endregion

#pragma region dialog

static BOOL Read1Char(ELC pelc, char* pchBuffer, LPDWORD pdwError, DWORD dwTimeout, const char* psz)
{
	DWORD dwReceived;
	BOOL fOK;
	char c = 0x00;
	BOOL fTimeout;
	//if (fOK = ReadData(pelc, &c, 1, &dwReceived, pdwError, dwTimeout))
	if (fOK = GetReply(pelc, &c, 1, &dwReceived, pdwError, dwTimeout, &fTimeout, psz))
	{
		if (NULL != pchBuffer)
			*pchBuffer = c;
	}
	return (fOK && 0 != dwReceived);
}

static BOOL ReadUntil(ELC pelc, char toReach, char* pchBuffer, const DWORD dwSize, LPDWORD pdwReceived, LPDWORD pdwError, DWORD dwTimeout, const char* psz)
{
	BOOL fOK;
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
	} while (fOK && toReach != c);
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
static BOOL ReceiveReply(ELC pelc, DWORD dwTimeout)
{
	MYPELCSTRUCT;

	BOOL fOK;
	DWORD dwReceived, dwError;
	const char* pdummy;
	char dummy[ONEKB];

	// prepare buffers
	DWORD dwReplyExSize = strlen(MYPFNC->pReplyEx);
	DWORD dwReplyBufferSize = ONEKB; // sizeof(REPLYHEADER) + sizeof(REPLYCR) + dwReplyExSize + dwExpectedDataSize + sizeof(REPLYTRAILER) + 1; // +1 for safety
	char* pReplyBuffer = (char*)CALLOC(dwReplyBufferSize);
	char* pReplyBufferTmp = (char*)CALLOC(dwReplyBufferSize);
	char chAfterReply = 0x00;
	DWORD dwReceivedReplyBufferSize = 0;
	BOOL fSTX = false, fETX = false, fLRC = false;
	char* pbETX = NULL;

	// reset sessions
	MYPELC->N1 = 0;
	MYPELC->N2 = 0;

	// wait reply from ELC
	BOOL fContinue = true;
	while (fContinue)
	{
		BOOL fTimeout;

		// initiate communication
		if (0 == MYPELC->N2)
			pdummy = "START RECEIVING %s";
		else
			pdummy = "RESTART RECEIVING %s";
		//sprintf_s(dummy, sizeof(dummy), pdummy, MYPFNC->pFnc);
		sprintf_s(dummy, pdummy, MYPFNC->pFnc);
		LOG(dummy, true);

		char b = 0x00;
		if (fOK = WaitENQ(pelc, &b, &dwError, &fTimeout))
		{
			MYPELC->N2++;
			MYPELC->N1++;
			// send ACK
			if (fOK = SendACK(pelc, &dwError))
			{
				BOOL fReceive = true;
				while (fReceive && fContinue)
				{
					b = 0x00;
					if (fOK = WaitSTX(pelc, &b, &dwError, &fTimeout))
					{
						pReplyBuffer[0] = STX;
						dwReceivedReplyBufferSize = 1;
						// read until STX
						if (fOK = ReadUntil(pelc, ETX, pReplyBuffer + dwReceivedReplyBufferSize, dwReplyBufferSize - dwReceivedReplyBufferSize, &dwReceived, &dwError, MYPELC->TD, NULL))
						{
							dwReceivedReplyBufferSize += dwReceived;
							pbETX = pReplyBuffer + dwReceivedReplyBufferSize - 1;
							// read LRC
							if (fOK = (dwReplyBufferSize > dwReceivedReplyBufferSize && Read1Char(pelc, pReplyBuffer + dwReceivedReplyBufferSize, &dwError, dwTimeout, NULL)))
							{
								// determine exact command size
								DWORD dwExactReplySize = pbETX - pReplyBuffer + 2; // 2 for LRC + ETX position
								DWORD dwReceivedReplyExDataSize = dwExactReplySize - sizeof(REPLYHEADER) - sizeof(REPLYCR) - sizeof(REPLYTRAILER);
								DWORD dwReceivedReplyDataSize = dwReceivedReplyExDataSize - dwReplyExSize;
								PREPLYHEADER pReplyHeader = (PREPLYHEADER)pReplyBuffer;
								PREPLYCR pReplyCR = (PREPLYCR)(pReplyBuffer + sizeof(REPLYHEADER));
								char* pReplyEx = (char*)pReplyCR + sizeof(REPLYCR);
								char* pReplyData = pReplyEx + dwReplyExSize;
								PREPLYTRAILER pReplyTrailer = (PREPLYTRAILER)(pReplyData + dwReceivedReplyDataSize);
								// test LRC
								if (fOK = LRC(pelc, pReplyHeader->cmd.cmd, dwExactReplySize - sizeof(ELCSTX) - sizeof(REPLYTRAILER) + sizeof(ELCETX)) == pReplyTrailer->lrc.lrc)
								{
									LOG(" IS VALID", false);
									if (SendACK(pelc, &dwError))
									{
										b = 0x00;
										WaitEOT(pelc, &b, &dwError, &fTimeout);
									}
									// test received format
									if (fOK = (sizeof(ELCCMD) == strlen(MYPFNC->pReply) && 0 == memcmp(&pReplyHeader->cmd, MYPFNC->pReply, sizeof(ELCCMD))
										&& (dwReplyExSize == strlen(MYPFNC->pReplyEx) && 0 == memcmp(pReplyEx, MYPFNC->pReplyEx, dwReplyExSize))
										&& (ETX == pReplyTrailer->etx.etx)))
									{
										// eventually save data
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
									fContinue = false;
								}
								else
								{
									LOG(" IS INVALID", false);
								}
							}
						}
					}
					if (fOK)
					{
					}
					else
					{
						MYPELC->N1++;
						fOK = SendNAK(pelc, &dwError);
						if (NB_ATTEMPTS > MYPELC->N1)
						{
							// wait STX again
						}
						else
						{
							if (NB_ATTEMPTS > MYPELC->N2)
							{
								// return waiting for the answer
								MYPELC->N1 = 0;
								fReceive = false;
							}
							else
							{
								// failed to receive the answer
								fContinue = false;
							}

						}
					}
				}
			}
			else
			{
				// error sending ACK, what to do ?
				fContinue = false;
			}
		}
		else
		{
			// failed to receive ENQ, stop receiving
			fContinue = false;
		}
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
	// start caller's timer
	SetEvent(MYPELC->startTimer);
	LOG(TIMER_STARTED, true);
	// start receiving the reply
	if (ReceiveReply(pthr->pelc, INFINITE))
		SetEvent(MYPELC->completed);
	else
		SetEvent(MYPELC->error);
	CloseHandle(pthr->started);
	free(pthr);
	return dw;
}

/// <summary>
/// Start the thread that will process the async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="asyncOperationEndedEvent">An asyncOperationEndedEvent which will be signaled when the operation if finished</param>
/// <returns></returns>
static BOOL StartAsyncProcessingThread(ELC pelc)
{
	MYPELCSTRUCT;
	PTHREADASYNCPROCESSING pthr = (PTHREADASYNCPROCESSING)CALLOC(sizeof(THREADASYNCPROCESSING));
	pthr->pelc = pelc;
	pthr->started = CreateEvent(NULL, false, false, NULL);
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
static BOOL SendOrder(ELC pelc)
{
	MYPELCSTRUCT;
	BOOL fOK, fTimeout;
	const char* pdummy;
	char dummy[ONEKB];
	DWORD dwError;

	// purge COM port
	PurgeComm(MYPELC->handle, PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);

	// prepare the buffer to send
	DWORD dwRequestExSize = strlen(MYPFNC->pRequestEx);
	DWORD dwRequestDataSize = strlen(MYPFNC->pRequestData);
	DWORD dwRequestExDataSize = dwRequestExSize + dwRequestDataSize;
	DWORD dwRequestBufferSize = sizeof(REQUESTHEADER) + dwRequestExDataSize + sizeof(REQUESTTRAILER);
	char* pRequestBuffer = (char*)CALLOC(dwRequestBufferSize);
	PREQUESTHEADER pRequestHeader = (PREQUESTHEADER)pRequestBuffer;
	PREQUESTTRAILER pRequestTrailer = (PREQUESTTRAILER)(pRequestBuffer + sizeof(REQUESTHEADER) + dwRequestExDataSize);
	char* pRequestEx = pRequestBuffer + sizeof(REQUESTHEADER);
	char* pRequestData = pRequestBuffer + sizeof(REQUESTHEADER) + dwRequestExSize;
	// prepare request buffer
	pRequestHeader->stx.stx = STX;
	memcpy_s(pRequestHeader->cmd.cmd, COMMAND_SIZE, MYPFNC->pRequest, COMMAND_SIZE);
	if (0 != dwRequestExSize)
		memcpy_s(pRequestEx, dwRequestExSize, MYPFNC->pRequestEx, dwRequestExSize);
	if (0 != dwRequestDataSize)
		memcpy_s(pRequestData, dwRequestDataSize, MYPFNC->pRequestData, dwRequestDataSize);
	pRequestTrailer->etx.etx = ETX;
	pRequestTrailer->lrc.lrc = LRC(pelc, pRequestHeader->cmd.cmd, dwRequestBufferSize - sizeof(ELCSTX) - sizeof(REQUESTTRAILER) + sizeof(ELCETX));

	// reset sessions
	MYPELC->N1 = 0;
	MYPELC->N2 = 0;

	// trying to exchange
	BOOL fContinue = true;
	while (fContinue)
	{
		// initiate communication
		if (0 == MYPELC->N2)
			pdummy = "START SENDING %s";
		else
			pdummy = "RESTART SENDING %s";
		//sprintf_s(dummy, sizeof(dummy), pdummy, MYPFNC->pFnc);
		sprintf_s(dummy, pdummy, MYPFNC->pFnc);
		LOG(dummy, true);

		BOOL fStart = true;
		while (fStart && fContinue)
		{
			// initiating exchange
			MYPELC->N2++;
			if (fOK = SendENQ(pelc, &dwError))
			{
				char b = 0x00;
				if (fOK = WaitACK(pelc, &b, &dwError, &fTimeout))
				{
					// send order

				// send order to the ELC
					int dwWritten;

					BOOL fSendOrder = true;
					while (fSendOrder && fContinue)
					{
						MYPELC->N1++;
						if (fOK = SendRequest(pelc, pRequestBuffer, dwRequestBufferSize, &dwWritten, &dwError, MYPFNC->pFnc))
						{
							BOOL fTimeout;
							char b = 0x00;
							if (fOK = WaitACK(pelc, &b, &dwError, &fTimeout))
							{
								// everything went right
								SendEOT(pelc, &dwError);
								fContinue = false;
							}
							else
							{
								if (NB_ATTEMPTS > MYPELC->N1)
								{
									// retry sending the order
								}
								else
								{
									fOK = SendEOT(pelc, &dwError);
									if (NB_ATTEMPTS > MYPELC->N2)
									{
										SLEEPT2;
										// return to initiating conversation
										fSendOrder = false;
									}
									else
									{
										// terminate communication
										fContinue = false;
									}
								}
							}
						}
					}
				}
				else
				{
					SendEOT(pelc, &dwError);
					if (fStart = (NB_ATTEMPTS > MYPELC->N2))
					{
						SLEEPT2;
					}
					else
					{
						// stop processing the order
						fContinue = false;
					}
				}
			}
			else
			{
				//error sending ENQ
				fContinue = false;
			}
		}
	}
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
DRIVERAPI ELC DRIVERCALL ELCInit()
{
	PELCSTRUCT pelcstruct;
	if (NULL != (pelcstruct = (PELCSTRUCT)CALLOC(sizeof(ELCSTRUCT))))
	{
		ELC pelc = pelcstruct;
		MYPELC->handle = NO_FILE;
		MYPELC->port = NO_COM_PORT;
		MYPELC->BaudRate = CBR_9600;
		MYPELC->logHandle = NO_FILE;
		MYPELC->T1 = TT1;
		MYPELC->T2 = TT2;
		MYPELC->TA = TTA;
		MYPELC->TR = TTR;
		MYPELC->TRA = TTRA;
		MYPELC->TD = TTD;
		MYPELC->timerBeforeAbort = 50;
	}
	return pelcstruct;
}

/// <summary>
/// Release resources
/// </summary>
/// <param name="pelc"></param>
DRIVERAPI void DRIVERCALL ELCRelease(ELC* ppelc)
{
	ELCClose(*ppelc);
	FREE((char**)ppelc);
}

/// <summary>
/// Set the port to use to open the ELC
/// Setting the port will close the device if open
/// </summary>
/// <param name="pelc"></param>
/// <param name="port">the port to use. It should be greater or equal to 0</param>
/// <returns></returns>
DRIVERAPI BOOL DRIVERCALL ELCSetPort(ELC pelc, int port)
{
	MYPELCSTRUCT;
	if (NULL != pelc && NO_COM_PORT < port)
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
DRIVERAPI BOOL DRIVERCALL ELCOpen(ELC pelc, BOOL UseLogFile)
{
	MYPELCSTRUCT;
	BOOL result = true;
	char dummy[ONEKB];

	// open the requested port
	//if (NO_COM_PORT == MYPELC->port)
	//	// get COM port used by the ELC
	//	MYPELC->port = GetComPort();
	if (NO_COM_PORT != MYPELC->port)
	{
		//sprintf_s(dummy, sizeof(dummy), "\\\\.\\COM%u", MYPELC->port);
		sprintf_s(dummy, "\\\\.\\COM%u", MYPELC->port);
	}
	else
	{
		return false;
	}

	// open the COM port
	MYPELC->handle = CreateFile((LPCSTR)dummy,
		GENERIC_READ | GENERIC_WRITE,
		0,
		0,
		OPEN_EXISTING,
		FILE_FLAG_OVERLAPPED,
		NULL);
	MYPELC->mutexInProgress = CreateMutex(NULL, false, NULL);
	MYPELC->stopReading = CreateEvent(NULL, false, false, NULL);
	MYPELC->startTimer = CreateEvent(NULL, false, false, NULL);
	MYPELC->processingIsOver = CreateEvent(NULL, false, false, NULL);
	if (result = (NO_FILE != MYPELC->handle &&
		NULL != MYPELC->mutexInProgress &&
		NULL != MYPELC->stopReading &&
		NULL != MYPELC->startTimer &&
		NULL != MYPELC->processingIsOver &&
		SetCommunicationState(pelc, true, 0x00)))
	{
		if (MYPELC->fUseLog = UseLogFile)
		{
			ELCSetLogFile(pelc, true);
		}
	}
	else
	{
		ELCClose(pelc);
	}
	return result;
}

/// <summary>
/// Set or resets log traces
/// </summary>
/// <param name="pelc"></param>
/// <param name="f"></param>
/// <returns></returns>
DRIVERAPI void DRIVERCALL ELCSetLogFile(ELC pelc, BOOL f)
{
	if (f)
	{
		if (NO_FILE == MYPELC->logHandle)
		{
			const char* pszFileName = "ELC on COM%u.log";
			// open log file
			char* pszLogFileName = (char*)CALLOC(ONEKB);
			sprintf_s(pszLogFileName, ONEKB, "ELC on COM%u.log", MYPELC->port);
			MYPELC->logHandle = CreateFile((LPCSTR)pszLogFileName,
				FILE_GENERIC_WRITE,
				FILE_SHARE_READ,
				NULL,
				OPEN_ALWAYS,
				FILE_ATTRIBUTE_NORMAL,
				NULL);
			if (NO_FILE == MYPELC->logHandle)
			{
				TCHAR szPath[MAX_PATH + 1];
				char* pszLongLogFileName = (char*)CALLOC(ONEKB);
				// search for Documents path
				HRESULT hres = SHGetKnownFolderPath(FOLDERID_Documents, 0, NULL, (PWSTR*)&szPath);
				if (S_OK == hres)
				{
					sprintf_s(pszLongLogFileName, ONEKB, "%s\\%s", szPath, pszLogFileName);
					MYPELC->logHandle = CreateFile((LPCSTR)pszLongLogFileName,
						FILE_GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);
				}
				if (NO_FILE == MYPELC->logHandle)
				{
					// search for TEMP path
					DWORD dw = GetTempPath(_countof(szPath), szPath);
					if (0 != dw)
					{
						sprintf_s(pszLongLogFileName, ONEKB, "%s%s", szPath, pszLogFileName);
						MYPELC->logHandle = CreateFile((LPCSTR)pszLongLogFileName,
							FILE_GENERIC_WRITE,
							FILE_SHARE_READ,
							NULL,
							OPEN_ALWAYS,
							FILE_ATTRIBUTE_NORMAL,
							NULL);
					}
					if (NO_FILE == MYPELC->logHandle)
					{
						// impossible to retrieve the TEMP folder
					}
				}
				free(pszLongLogFileName);
			}
			free(pszLogFileName);
			if (NO_FILE != MYPELC->logHandle)
				LOGCRLF;
		}
	}
	else
	{
		if (NO_FILE != MYPELC->logHandle)
		{
			CloseHandle(MYPELC->logHandle);
			MYPELC->logHandle = NO_FILE;
		}
	}
	MYPELC->fUseLog = f;
}

/// <summary>
/// Get the port to which the ELC is connected
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
DRIVERAPI int DRIVERCALL ELCPort(ELC pelc)
{
	MYPELCSTRUCT;
	if (NULL != pelc && IsOpen(pelc))
		return MYPELC->port;
	else
		return NO_COM_PORT;
}

/// <summary>
/// Close the ELC device
/// </summary>
/// <param name="pelc"></param>
/// <returns></returns>
DRIVERAPI void DRIVERCALL ELCClose(ELC pelc)
{
	MYPELCSTRUCT;
	if (IsOpen(pelc))
	{
		SetCommState(MYPELC->handle, &MYPELC->dcbInitial);
		SLEEPAFTERSETCOMMSTATE;
		if (NO_FILE != MYPELC->logHandle)
			CloseHandle(MYPELC->logHandle);
		MYPELC->logHandle = NO_FILE;
		if (NO_FILE != MYPELC->handle)
			CloseHandle(MYPELC->handle);
		MYPELC->handle = NO_FILE;
		CloseMyHandle(&MYPELC->mutexInProgress);
		CloseMyHandle(&MYPELC->stopReading);
		CloseMyHandle(&MYPELC->startTimer);
		CloseMyHandle(&MYPELC->processingIsOver);
	}
}

/// <summary>
/// Get teh status of the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfDocumentReadyToBeRead">true if a check note is ready to enter the reader, false otherwise (it doesn't say whether a check is inside the reader)</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI BOOL DRIVERCALL ELCStatus(ELC pelc, BOOL* pfDocumentReadyToBeRead)
{
	MYPELCSTRUCT;
	BOOL fOK = false;
	*pfDocumentReadyToBeRead = false;

	char* REQUEST = STATUS_REQUEST;
	char* REQUEST_EX = STATUS_REQUEST_EX;
	char* REQUEST_DATA = STATUS_REQUEST_DATA;

	char* REPLY = STATUS_REPLY;
	char* REPLY_CR = STATUS_REPLY_CR;
	char* REPLY_EX = STATUS_REPLY_EX;

	MYPELC->status.pFnc = STATUS_FNC;
	MYFNC(MYPELC->status);
	SETREQUESTANDREPLY;
	LOGDATETIME(MYPELC->pCurrentFnc->pFnc);

#define STATUS_CR1_OK					0x31
#define STATUS_CR1_KO					0x34
#define STATUS_CR2_OK					0x30
#define STATUS_CR3_NO_DOCUMENT		0x30
#define STATUS_CR3_DOCUMENT			0x31

	if (fOK = SetInProgress(pelc))
	{
		if (fOK = SendOrder(pelc))
		{
			if (fOK = ReceiveReply(pelc, INFINITE))
			{
				SaveCR(pelc);
				if (fOK = (STATUS_CR1_OK == CR1))
					*pfDocumentReadyToBeRead = STATUS_CR3_DOCUMENT == CR3;
			}
		}
		ResetInProgress(pelc);
	}
	if (fOK)
		LOG(COMPLETED_SUCCESSFULLY, true);
	else
		LOG(COMPLETED_WITH_ERROR, true);
	return fOK;
}

/// <summary>
/// Send an abort to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfDocumentEjected">OUT, true if the document has been ejected or no document was inside, false otherwise</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI BOOL DRIVERCALL ELCAbort(ELC pelc, BOOL* pfDocumentEjected)
{
	MYPELCSTRUCT;
	BOOL fOK = true;
	*pfDocumentEjected = false;

	char* REQUEST = ABORT_REQUEST;
	char* REQUEST_EX = ABORT_REQUEST_EX;
	char* REQUEST_DATA = ABORT_REQUEST_DATA;

	char* REPLY = ABORT_REPLY;
	char* REPLY_CR = ABORT_REPLY_CR;
	char* REPLY_EX = ABORT_REPLY_EX;

	MYPELC->abort.pFnc = ABORT_FNC;
	MYFNC(MYPELC->abort);
	SETREQUESTANDREPLY;
	LOGDATETIME(MYPELC->pCurrentFnc->pFnc);

#define ABORT_CR1_OK					0x31
#define ABORT_CR1_KO					0x34
#define ABORT_CR2_OK					0x30
#define ABORT_CR2_KO					0x31

	MYPFNC->elcResult = ELCResult::error;
	if (fOK = SetInProgress(pelc))
	{
		if (fOK = SendOrder(pelc))
		{
			if (fOK = ReceiveReply(pelc, WRITE_COMMAND_TIMER))
			{
				SaveCR(pelc);
				if (fOK = (ABORT_CR1_OK == CR1))
				{
					if (*pfDocumentEjected = ABORT_CR2_OK == CR2)
					{
						MYPFNC->elcResult = ELCResult::completed;
						LOG(COMPLETED_SUCCESSFULLY, true);
					}
					else
					{
						MYPFNC->elcResult = ELCResult::completedWithError;
						LOG(COMPLETED_WITH_ERROR, true);
					}
				}
			}
		}
		ResetInProgress(pelc);
	}
	if (ELCResult::completed != MYPFNC->elcResult && ELCResult::completedWithError != MYPFNC->elcResult)
		LOG("ERROR DURING PROCESSING", true);
	return fOK;
}

/// <summary>
/// Internal start async operation
/// </summary>
/// <param name="pelc"></param>
/// <param name="pfnc"></param>
/// <param name="psz">The operation to start</param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <param name="startTimerEvent">Event signaled when the caller's timer has been started</param>
/// <param name="asyncOperationEndedEvent">Event signaled when the async operation has ended</param>
/// <returnsTrue if successfull, false otherwise</returns>
static BOOL StartAsync(ELC pelc, int iTimer, HANDLE startTimerEvent, HANDLE asyncOperationEndedEvent)
{
	MYPELCSTRUCT;
	BOOL fOK;

	MYPFNC->elcResult = ELCResult::none;

	// initialise sync environment
	PrepareAsyncEnvironment(pelc, startTimerEvent, asyncOperationEndedEvent);

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
				SetEvent(MYPELC->error);
			}
		}
		else
		{
			// stop result thread
			SetEvent(MYPELC->error);
		}
	}

	// release resources if an error has occurred
	if (!fOK)
	{
		ResetInProgress(pelc);
	}
	return fOK;
}

/// <summary>
/// Allows waiting for an async operation to end
/// </summary>
/// <param name="pelc"></param>
/// <param name="iTimer">Timer to wait for</param>
/// <param name="pfTimeout"></param>
/// <param name="pfCancelled"></param>
/// <returns><see </returns>
DRIVERAPI ELCResult DRIVERCALL ELCWaitAsync(ELC pelc, int iTimer)
{
	MYPELCSTRUCT;
	//if (IsInProgress(pelc))
	//{
	DWORD dw = WaitForSingleObject(MYPELC->processingIsOver, iTimer);
	if (WAIT_OBJECT_0 == dw)
		return MYPFNC->elcResult;
	else
		return ELCResult::none;
	//}
	//return MYPFNC->elcResult;
}

/// <summary>
/// Read a CMC7
/// </summary>
/// <param name="pelc"></param>
/// <param name="ppchBuffer"></param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <param name="startTimerEvent">Event signaled when the caller's timer has been started</param>
/// <param name="asyncOperationEndedEvent">Event signaled when the async operation has ended</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI BOOL DRIVERCALL ELCReadAsync(ELC pelc, int iTimer, HANDLE startTimerEvent, HANDLE asyncOperationEndedEvent)
{
	MYPELCSTRUCT;
	BOOL fOK = false;
	//	*pfRead = *pfDocumentInside = false;

		// are we already inside an async operation ?
	if (!SetInProgress(pelc))
		return false;

	char* REQUEST = READ_REQUEST;
	char* REQUEST_EX = READ_REQUEST_EX;
	char* REQUEST_DATA = READ_REQUEST_DATA;

	char* REPLY = READ_REPLY;
	char* REPLY_CR = READ_REPLY_CR;
	char* REPLY_EX = READ_REPLY_EX;

	MYPELC->read.pFnc = READ_FNC;
	MYFNC(MYPELC->read);
	SETREQUESTANDREPLY;
	LOGDATETIME(MYPELC->pCurrentFnc->pFnc);

	return StartAsync(pelc, iTimer, startTimerEvent, asyncOperationEndedEvent);
}

/// <summary>
/// Get
/// </summary>
/// <param name="pelc"></param>
/// <param name="ppchRawBuffer">Buffer to receive raw CMC7</param>
/// <param name="sizeRawBuffer">Size of raw buffer</param>
/// <param name="pchChpnBuffer">Buffer to receive CHPN compatible CMC7</param>
/// <param name="sizeChpnBuffer">Size of chpn buffer</param>
/// <param name="fDocumentInside">True if the document is still inside after reading</param>
/// <returns>True is successfull, false otherwise</returns>
DRIVERAPI ELCResult DRIVERCALL ELCReadAsyncResult(ELC pelc, char* pchRawBuffer, int sizeRawBuffer, char* pchChpnBuffer, int sizeChpnBuffer, BOOL* pfDocumentIsStillInside)
{
	MYPELCSTRUCT;
	// are we already inside an async operation ?
	if (IsInProgress(pelc))
		return MYPFNC->elcResult = ELCResult::none;
	if (&MYPELC->read != MYPELC->pCurrentFnc)
		return MYPFNC->elcResult = ELCResult::error;
	if (ELCResult::completed != MYPFNC->elcResult)
		return MYPFNC->elcResult;
	if (0 == MYPFNC->dwReplyDataSize || NULL == MYPFNC->pReplyData)
		return MYPFNC->elcResult = ELCResult::error;

#define	READ_CR1_OK						0x31
#define	READ_CR1_KO						0x34
#define	READ_CR2_OK						0x30
#define	READ_CR2_PAPER_ERROR			0x31
#define	READ_CR2_PARTIALLY_READ		0x32
#define	READ_CR3_DOCUMENT_EJECTED	0x30
#define	READ_CR3_DOCUMENT_INSIDE	0x31

	DWORD len = MYPFNC->dwReplyDataSize + 1;
	// prepare output buffers
	if (NULL != pchRawBuffer && 0 != sizeRawBuffer)
	{
		memset(pchRawBuffer, 0, sizeRawBuffer);
		strcpy_s(pchRawBuffer, sizeRawBuffer - 1, MYPFNC->pReplyData);
	}
	if (NULL != pchChpnBuffer && 0 != sizeChpnBuffer)
	{
		memset(pchChpnBuffer, 0, sizeChpnBuffer);
		strcpy_s(pchChpnBuffer, sizeChpnBuffer - 1, MYPFNC->pReplyData);
		for (DWORD i = 0; i < len; i++)
		{
			switch ((pchChpnBuffer)[i])
			{
				case 0x3A: // S1
					(pchChpnBuffer)[i] = 'B';
					break;
				case 0x3B: // S2
					break;
				case 0x3C: // S3
					(pchChpnBuffer)[i] = 'D';
					break;
				case 0x3D: // S4
					break;
				case 0x3E: // S5
					(pchChpnBuffer)[i] = 'F';
					break;
				case 0x3F: // character failed to be read
					(pchChpnBuffer)[i] = 'A';
					break;
			}
		}
	}
	SaveCR(pelc);
	if (READ_CR1_OK == CR1)
	{
		if (READ_CR2_OK == CR2)
		{
			*pfDocumentIsStillInside = READ_CR3_DOCUMENT_INSIDE == CR3;
			LOG(COMPLETED_SUCCESSFULLY, true);
			return MYPFNC->elcResult = ELCResult::completed;
		}
	}
	LOG(COMPLETED_WITH_ERROR, true);
	return MYPFNC->elcResult = ELCResult::completedWithError;
}

/// <summary>
/// Send a print order to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess">true if printing went successfully, false otherwise</param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <param name="startTimerEvent">Event signaled when the caller's timer has been started</param>
/// <param name="asyncOperationEndedEvent">Event signaled when the async operation has ended</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI BOOL DRIVERCALL ELCWriteAsync(ELC pelc, const char* pszData, char* pchBuffer, const int sizeBuffer, int iTimer, HANDLE startTimerEvent, HANDLE asyncOperationEndedEvent)
{
	MYPELCSTRUCT;
	BOOL fOK = false;

	// are we already inside an async operation ?
	if (!SetInProgress(pelc))
		return false;

	char* pb;
	int size;
	// set size of data to send for printing
	if (NULL != pszData && MAX_WRITE_DATA_SIZE < (strlen(pszData)))
	{
		size = MAX_WRITE_DATA_SIZE + 1;
		pb = (char*)CALLOC(size);
		if (NULL != pb)
			strncpy_s(pb, size, pszData, size - 1);
		else return false;
	}
	else if (NULL != pszData)
	{
		size = strlen(pszData) + 1;
		pb = (char*)CALLOC(size);
		if (NULL != pb)
			strcpy_s(pb, size, pszData);
		else return false;
	}
	else
	{
		return false;
	}
	if (NULL != pchBuffer && 0 != sizeBuffer && NULL != pb)
	{
		size = sizeBuffer - 1;
		memset(pchBuffer, 0, sizeBuffer);
		memcpy_s(pchBuffer, size, pb, min(size, (int)strlen(pb)));
	}

	char* REQUEST = WRITE_REQUEST;
	char* REQUEST_EX = WRITE_REQUEST_EX;
	char* REQUEST_DATA = pb;

	char* REPLY = WRITE_REPLY;
	char* REPLY_CR = WRITE_REPLY_CR;
	char* REPLY_EX = WRITE_REPLY_EX;

	MYPELC->write.pFnc = WRITE_FNC;
	MYFNC(MYPELC->write);
	SETREQUESTANDREPLY;
	LOGDATETIME(MYPELC->pCurrentFnc->pFnc);

	return StartAsync(pelc, iTimer, startTimerEvent, asyncOperationEndedEvent);
}

/// <summary>
/// Send a print order to the ELC
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess">true if printing went successfully, false otherwise</param>
/// <param name="iTimer">Timer to wait for completion</param>
/// <returns>true if the order has been processed successfully, false otherwise</returns>
DRIVERAPI ELCResult DRIVERCALL ELCWriteAsyncResult(ELC pelc)
{
	MYPELCSTRUCT;
	// are we already inside an async operation ?
	if (IsInProgress(pelc))
		return MYPFNC->elcResult = ELCResult::none;
	if (&MYPELC->write != MYPELC->pCurrentFnc)
		return MYPFNC->elcResult = ELCResult::error;
	if (ELCResult::completed != MYPFNC->elcResult)
		return MYPFNC->elcResult;

#define WRITE_CR1_OK					0x31
#define WRITE_CR1_KO					0x34
#define WRITE_CR2_OK					0x30
#define WRITE_CR2_KO					0x31

	SaveCR(pelc);
	if (WRITE_CR1_OK == CR1)
	{
		if (WRITE_CR2_OK == CR2)
		{
			LOG(COMPLETED_SUCCESSFULLY, true);
			return MYPFNC->elcResult = ELCResult::completed;
		}
	}
	LOG(COMPLETED_WITH_ERROR, true);
	return MYPFNC->elcResult = ELCResult::completedWithError;
}

/// <summary>
/// Synchronous read
/// </summary>
/// <param name="pelc"></param>
/// <param name="iTimer"></param>
/// <param name="ppchRawBuffer">Buffer to receive raw CMC7</param>
/// <param name="sizeRawBuffer">Size of raw buffer</param>
/// <param name="pchChpnBuffer">Buffer to receive CHPN compatible CMC7</param>
/// <param name="sizeChpnBuffer">Size of chpn buffer</param>
/// <param name="pfDocumentInside"></param>
/// <param name="pfTimeout"></param>
/// <param name="pfCancelled"></param>
/// <returns></returns>
DRIVERAPI ELCResult DRIVERCALL ELCRead(ELC pelc, int iTimer, HANDLE startTimerEvent, char* pchRawBuffer, int sizeRawBuffer, char* pchChpnBuffer, int sizeChpnBuffer, BOOL* pfDocumentInside)
{
	MYPELCSTRUCT;
	if (ELCReadAsync(pelc, iTimer, startTimerEvent, NULL))
	{
		ELCResult res;
		switch (res = ELCWaitAsync(pelc, INFINITE))
		{
			case ELCResult::completed:
				return ELCReadAsyncResult(pelc, pchRawBuffer, sizeRawBuffer, pchChpnBuffer, sizeChpnBuffer, pfDocumentInside);
			default:
				return res;
		}
	}
	return ELCResult::error;
}

/// <summary>
/// Synchronous write
/// </summary>
/// <param name="pelc"></param>
/// <param name="pszData"></param>
/// <param name="pfSuccess"></param>
/// <param name="iTimer"></param>
/// <param name="startTimerEvent">Event signaled when the caller's timer has been started</param>
/// <param name="asyncOperationEndedEvent">Event signaled when the async operation has ended</param>
/// <returns></returns>
DRIVERAPI ELCResult DRIVERCALL ELCWrite(ELC pelc, const char* pszData, char* pchBuffer, const int sizeBuffer, int iTimer, HANDLE startTimerEvent)
{
	MYPELCSTRUCT;
	if (ELCWriteAsync(pelc, pszData, pchBuffer, sizeBuffer, iTimer, startTimerEvent, NULL))
	{
		ELCResult res;
		switch (res = ELCWaitAsync(pelc, INFINITE))
		{
			case ELCResult::completed:
				return ELCWriteAsyncResult(pelc);
			default:
				return res;
		}
	}
	return ELCResult::error;
}

/// <summary>
/// Initiate a dialog with the ELC
/// </summary>
/// <param name="pelc"></param>
/// <returns>true if successfull, false otherwise</returns>
DRIVERAPI BOOL DRIVERCALL ELCInitiateDialog(ELC pelc)
{
	MYPELCSTRUCT;
	BOOL fOK;
	DWORD dwError;
	if (fOK = SetInProgress(pelc))
	{
		if (fOK = SendENQ(pelc, &dwError))
		{
			BOOL fTimeout;
			char b = 0x00;
			if (!(fOK = WaitACK(pelc, &b, &dwError, &fTimeout)))
			{
				if (fTimeout)
					LOG(" - TIMEOUT", false);
				else
					LOGEX("- ERROR:", dwError, false);
			}
			SendEOT(pelc, &dwError);
		}
		ResetInProgress(pelc);
	}
	return fOK;
}

/// <summary>
/// Get ELC CR by index
/// </summary>
/// <param name="pelc"></param>
/// <param name="index">Index of CR to return</param>
/// <returns>The CR as pointed by the index if valid, 0 otherwise</returns>
DRIVERAPI char DRIVERCALL ELCCR(ELC pelc, int index)
{
	MYPELCSTRUCT;
	if (0 <= index && CR_SIZE >= index)
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
DRIVERAPI DWORD DRIVERCALL ELCSpeed(ELC pelc, DWORD BaudRate)
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
DRIVERAPI BOOL DRIVERCALL ELCCancelAsync(ELC pelc)
{
	MYPELCSTRUCT;
	if (IsInProgress(pelc))
	{
		return SetEvent(MYPELC->cancelled);
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
DRIVERAPI void DRIVERCALL ELCSetTimer(ELC pelc, ELCTimer timer, DWORD value)
{
	MYPELCSTRUCT;
	if (0 == value)
		return;
	switch (timer)
	{
		case ELCTimer::T1:
			pelcstruct->T1 = value;
			break;
		case ELCTimer::T2:
			pelcstruct->T2 = value;
			break;
		case ELCTimer::TA:
			pelcstruct->TA = value;
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
		case ELCTimer::beforeAbort:
			pelcstruct->timerBeforeAbort = value;
			break;
	}
}

/// <summary>
/// Indicates whether inside an async operation
/// </summary>
/// <param name="pelc"></param>
/// <returns>True if within an async operation, false otherwise</returns>
DRIVERAPI  BOOL DRIVERCALL  ELCIsInProgress(ELC pelc)
{
	return IsInProgress(pelc);
}

/// <summary>
/// Indicates whether inside an async operation
/// </summary>
/// <param name="pelc"></param>
/// <returns>True if within an async operation, false otherwise</returns>
DRIVERAPI  ELCResult DRIVERCALL  ELCLastAsyncResult(ELC pelc)
{
	MYPELCSTRUCT;
	if (NULL != MYPFNC)
		return MYPFNC->elcResult;
	return ELCResult::none;
}

#pragma endregion

#pragma endregion
