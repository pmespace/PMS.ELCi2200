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

typedef void* ELC;

#define MAX_WRITE_DATA_SIZE 80

enum class ELCTimer
{
	_timeBegin,
	T1,							// in milliseconds
	T2,							// in milliseconds
	TA,							// in milliseconds
	TR,							// in milliseconds
	TD,							// in milliseconds
	TRA,							// in milliseconds
	beforeAbort,				// in milliseconds
	_timeEnd,
};

/// <summary>
/// Result of an asynchronous operation (read or write)
/// </summary>
enum class ELCResult
{
	_eventBegin,
	none, // no event to signal (operation, not completed, not cancelled, not in timeout and no error), wait can carry on
	error, // an error has occurred, waiting is not required any more
	timeout, // timeout during operation
	completed, // the operation completed
	completedWithError, // the operation completed with an error
	cancelled, // operatioon cancelled by user
	_eventEnd,
};

// indicates an invalid COM port
#define NO_COM_PORT						-1
// indicates to wait indefinitely
#define NO_TIMER							-1

DRIVERAPI ELC __stdcall ELCInit();
DRIVERAPI void __stdcall ELCRelease(ELC* ppelc);
DRIVERAPI BOOL __stdcall ELCSetPort(ELC pelc, int port);
DRIVERAPI BOOL __stdcall ELCOpen(ELC pelc);
DRIVERAPI int __stdcall ELCPort(ELC pelc);
DRIVERAPI void __stdcall ELCClose(ELC pelc);
DRIVERAPI BOOL __stdcall ELCStatus(ELC pelc, BOOL* pfDocumentReadyToBeRead);
DRIVERAPI BOOL __stdcall ELCAbort(ELC pelc, BOOL* pfDocumentInside);
DRIVERAPI ELCResult __stdcall ELCWaitAsync(ELC pelc, int iTimer, BOOL* pfTimeout, BOOL* pfCancelled);
DRIVERAPI BOOL __stdcall ELCReadAsync(ELC pelc, int iTimer, HANDLE timerStartedEvent, HANDLE asyncOperationEndedEvent);
DRIVERAPI ELCResult __stdcall ELCReadResult(ELC pelc, char* pchRawBuffer, int sizeRawBuffer, char* pchChpnBuffer, int sizeChpnBuffer, BOOL* pfDocumentInside);
DRIVERAPI BOOL __stdcall ELCWriteAsync(ELC pelc, const char* pszData, char* pchBuffer, const int sizeBuffer, int iTimer, HANDLE timerStartedEvent, HANDLE asyncOperationEndedEvent);
DRIVERAPI ELCResult __stdcall ELCWriteResult(ELC pelc);
DRIVERAPI BOOL __stdcall ELCRead(ELC pelc, int iTimer, HANDLE timerStartedEvent, char* pchRawBuffer, int sizeRawBuffer, char* pchChpnBuffer, int sizeChpnBuffer, BOOL* pfDocumentInside, BOOL* pfTimeout, BOOL* pfCancelled);
DRIVERAPI BOOL __stdcall ELCWrite(ELC pelc, const char* pszData, char* pchBuffer, const int sizeBuffer, int iTimer, HANDLE timerStartedEvent, BOOL* pfTimeout, BOOL* pfCancelled);
DRIVERAPI BOOL __stdcall ELCInitiateDialog(ELC pelc);
DRIVERAPI char __stdcall ELCCR(ELC pelc, int index);
DRIVERAPI DWORD __stdcall ELCSpeed(ELC pelc, DWORD dwBaudRate);
DRIVERAPI void __stdcall ELCSetTimer(ELC pelc, ELCTimer timer, DWORD seconds);
DRIVERAPI BOOL __stdcall ELCCancelAsync(ELC pelc);
DRIVERAPI int __stdcall ELCGetUSBComPort(char* usbDriver);
DRIVERAPI  BOOL __stdcall ELCIsInProgress(ELC pelc);
DRIVERAPI  ELCResult __stdcall  ELCLastAsyncResult(ELC pelc);

#endif