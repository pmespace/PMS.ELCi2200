#pragma once

#ifndef DRIVER_H
#define DRIVER_H

#ifdef WIN32

#ifdef DRIVER_EXPORTS
#define DRIVERAPI __declspec(dllexport)
#else
#define DRIVERAPI __declspec(dllimport)
#endif

#else

#define DRIVERAPI
#define __stdcall

#endif

#ifdef WIN32
#include "windows.h"
#endif

//**********
// use MONEYLINE commands
#define MONEYLINE
//**********

typedef void * ELC;

#define MAX_WRITE_DATA_SIZE 80

enum class ELCErrorType
{
	OK = 0,
	KO,
	errorAfterInsertPaper,
	errorAfterReadCheck,
	errorAfterEject,
	errorAfterAbort,
	errorOrder,
	receivedNAKBeforeTransmitting,
	receivedNAKAfterTransmitting,
	noReplyData,
	checkProcessingFailure,
	lastOrderNotProcessed,
	paperError,
	readError,
};

enum class ELCTimer
{
	_timeBegin,
	timerInitiateRequest,	// in seconds
	timerTerminateRequest,	// in seconds
	timerInitiateReply,		// in seconds
	timerTerminateReply,		// in seconds
	timerSendOrder,			// in seconds
	T0,							// in milliseconds
	T1,							// in milliseconds
	T2,							// in milliseconds
	TR,							// in milliseconds
	TD,							// in milliseconds
	TRA,							// in milliseconds
	_timeEnd,
};

/// <summary>
/// Result of an asynchronous operation (read or write)
/// </summary>
enum class EventResult
{
	_eventBegin,
	eventNone, // no event to signal (operation, not completed, not cancelled, not in timeout and no error), wait can carry on
	eventError, // an error has occurred, waiting is not required any more
	eventTimeout, // timeout during operation
	eventCompleted, // the operation completed
	eventCancelled, // operatioon cancelled by user
	_eventEnd,
};

DRIVERAPI ELC __stdcall ELCInit();
DRIVERAPI void __stdcall ELCRelease(ELC * ppelc);
DRIVERAPI bool __stdcall ELCSetPort(ELC pelc, int port);
DRIVERAPI bool __stdcall ELCOpen(ELC pelc);
DRIVERAPI int __stdcall ELCPort(ELC pelc);
DRIVERAPI void __stdcall ELCClose(ELC pelc);
DRIVERAPI bool __stdcall ELCStatus(ELC pelc, bool * pfDocumentIsPresent);
DRIVERAPI bool __stdcall ELCAbort(ELC pelc, bool * pfDocumentInside);
DRIVERAPI EventResult __stdcall ELCWaitAsync(ELC pelc, int iTimer);
DRIVERAPI bool __stdcall ELCRead(ELC pelc, int iTimer, char ** ppchRawBuffer, char ** ppchChpnBuffer, bool * pfDocumentInside);
DRIVERAPI bool __stdcall ELCReadAsync(ELC pelc, int iTimer);
DRIVERAPI bool __stdcall ELCReadResult(ELC pelc, char ** ppchRawBuffer, char ** ppchChpnBuffer, bool * pfDocumentInside);
DRIVERAPI bool __stdcall ELCWriteAsync(ELC pelc, const char * pszData, int iTimer);
DRIVERAPI bool __stdcall ELCWriteResult(ELC pelc);
DRIVERAPI bool __stdcall ELCWrite(ELC pelc, const char * pszData, int iTimer);
DRIVERAPI bool __stdcall ELCInitiateDialog(ELC pelc);
DRIVERAPI char __stdcall ELCCR(ELC pelc, int index);
DRIVERAPI DWORD __stdcall ELCSpeed(ELC pelc, DWORD dwBaudRate);
DRIVERAPI void __stdcall ELCSetTimer(ELC pelc, ELCTimer timer, DWORD seconds);
DRIVERAPI bool __stdcall ELCCancelAsync(ELC pelc);

DRIVERAPI void __stdcall LOG(ELC pelc, const char * psz, bool addCRLFBefore);
DRIVERAPI void __stdcall LOGEX(ELC pelc, const char * psz, int value, bool addCRLFBefore);
DRIVERAPI void __stdcall FREE(char ** pp);
DRIVERAPI char * __stdcall CALLOC(size_t size);

#endif