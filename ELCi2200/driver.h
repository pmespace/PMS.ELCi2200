#pragma once

#ifndef DRIVER_H
#define DRIVER_H

#include "exports.h"
#include "windows.h"

//**********
// use MONEYLINE commands
#define MONEYLINE
//**********

typedef void* ELC;

#define MAX_WRITE_DATA_SIZE 80

enum class ELCTimer
{
	_timeBegin = -1,
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
	_eventBegin = -1,
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
#define NO_TIMER							0

#ifdef __cplusplus
extern "C" {
#endif
	DRIVERAPI ELC DRIVERCALL ELCInit();
	DRIVERAPI void DRIVERCALL ELCRelease(ELC* ppelc);
	DRIVERAPI BOOL DRIVERCALL ELCSetPort(ELC pelc, int port);
	DRIVERAPI BOOL DRIVERCALL ELCOpen(ELC pelc, BOOL useLog);
	DRIVERAPI int DRIVERCALL ELCPort(ELC pelc);
	DRIVERAPI void DRIVERCALL ELCClose(ELC pelc);
	DRIVERAPI BOOL DRIVERCALL ELCStatus(ELC pelc, BOOL* pfDocumentReadyToBeRead);
	DRIVERAPI BOOL DRIVERCALL ELCAbort(ELC pelc, BOOL* pfDocumentInside);
	DRIVERAPI ELCResult DRIVERCALL ELCWaitAsync(ELC pelc, int iTimer);
	DRIVERAPI BOOL DRIVERCALL ELCReadAsync(ELC pelc, int iTimer, HANDLE timerStartedEvent, HANDLE asyncOperationEndedEvent);
	DRIVERAPI ELCResult DRIVERCALL ELCReadAsyncResult(ELC pelc, char* pchRawBuffer, int sizeRawBuffer, char* pchChpnBuffer, int sizeChpnBuffer, BOOL* pfDocumentInside);
	DRIVERAPI BOOL DRIVERCALL ELCWriteAsync(ELC pelc, const char* pszData, char* pchBuffer, const int sizeBuffer, int iTimer, HANDLE timerStartedEvent, HANDLE asyncOperationEndedEvent);
	DRIVERAPI ELCResult DRIVERCALL ELCWriteAsyncResult(ELC pelc);
	DRIVERAPI ELCResult DRIVERCALL ELCRead(ELC pelc, int iTimer, HANDLE timerStartedEvent, char* pchRawBuffer, int sizeRawBuffer, char* pchChpnBuffer, int sizeChpnBuffer, BOOL* pfDocumentInside);
	DRIVERAPI ELCResult DRIVERCALL ELCWrite(ELC pelc, const char* pszData, char* pchBuffer, const int sizeBuffer, int iTimer, HANDLE timerStartedEvent);
	DRIVERAPI BOOL DRIVERCALL ELCInitiateDialog(ELC pelc);
	DRIVERAPI char DRIVERCALL ELCCR(ELC pelc, int index);
	DRIVERAPI DWORD DRIVERCALL ELCSpeed(ELC pelc, DWORD dwBaudRate);
	DRIVERAPI void DRIVERCALL ELCSetTimer(ELC pelc, ELCTimer timer, DWORD seconds);
	DRIVERAPI BOOL DRIVERCALL ELCCancelAsync(ELC pelc);
	DRIVERAPI int DRIVERCALL ELCGetUSBComPort(char* usbDriver);
	DRIVERAPI  BOOL DRIVERCALL ELCIsInProgress(ELC pelc);
	DRIVERAPI  ELCResult DRIVERCALL  ELCLastAsyncResult(ELC pelc);
	DRIVERAPI void DRIVERCALL ELCSetLogFile(ELC pelc, BOOL f);
#ifdef __cplusplus
}
#endif


#endif