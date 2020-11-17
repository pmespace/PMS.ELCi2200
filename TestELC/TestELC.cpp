// TestELC.cpp : Ce fichier contient la fonction 'main'. L'exécution du programme commence et se termine à cet endroit.
//

#include <iostream>
#include <conio.h>
#include <stdio.h>
#include "..\ELCi2200\driver.h"

#define	ESC			0x1B
#define	ONEKB			1024
#define	RESULT		"=> "
#define	OPEN_ERROR	"Error trying to open the device"

static void MenuEntry(char* buffer)
{
	std::cout << std::endl << buffer;
}
static void MenuReply(int i)
{
	char buffer[ONEKB];
	sprintf_s(buffer, sizeof(buffer), "%C", i);
	std::cout << buffer << std::endl;
}
static void MenuClose()
{
	std::cout << "<ESC>" << std::endl;
}
static void ChooseMenuEntry()
{
	std::cout << std::endl << "\t==>";
}
static BOOL TestMenuEntry(char* entries, int entry)
{
	int i;
	char* upper = (char*)calloc(1, strlen(entries) + 2);
	if (NULL != upper)
	{
		// put all options in uppercase
		for (i = 0; i < (int)strlen(entries); i++)
			upper[i] = toupper(entries[i]);
		// add ESC at the end of possible entries
		upper[i] = ESC;
		// return true if entry in inside the set of valid characters, false otherwise
		return NULL != strchr(upper, toupper(entry));
	}
	return false;
}
// return true if a close (ESC) character has been entered
static BOOL GetMenuEntry(char* entries, int* pc)
{
	do
	{
		*pc = _getch();
	} while (!TestMenuEntry(entries, *pc));
	BOOL fClose = TestMenuEntry("", *pc);
	if (!fClose)
		MenuReply(*pc);
	else
		MenuClose();
	return fClose;
}
static void DisplaySuccess(ELC pelc)
{
	std::cout << std::endl << RESULT << "OK" << std::endl;
}
static void DisplayFailed(ELC pelc)
{
	std::cout << std::endl << RESULT << "KO" << std::endl;
}
static void DisplayMessage(ELC pelc, char* psz)
{
	std::cout << std::endl << psz;
}
static void DisplayResult(ELC pelc, const char success, const char* psz)
{
	char dummy[ONEKB];
	DisplaySuccess(pelc);
	std::cout << std::endl;
	for (int i = 1; i <= 3; i++)
	{
		char c = ELCCR(pelc, i);
		if (0x00 != c)
		{
			sprintf_s(dummy, sizeof(dummy), "\t==> CR%d: %X", i, c);
			std::cout << dummy;
			if (1 == i)
				if (success == c)
					std::cout << " - OK" << std::endl;
				else
					std::cout << " - KO" << std::endl;
			else
				std::cout << std::endl;
		}
	}
	if (NULL != psz)
		std::cout << "\t" << psz << std::endl;
}
int main()
{
	int c;
	void* elc;
	BOOL fClose = false, fContinue = true, fOpen = false;
	if (NULL != (elc = ELCInit()))
	{
		while (!fClose && fContinue)
		{
			MenuEntry("U > USB port");
			MenuEntry("S > Serial port");
			ChooseMenuEntry();
			if (!(fClose = GetMenuEntry((char*)"us", &c)))
			{
				if (TestMenuEntry((char*)"s", c))
				{
					MenuEntry("1..9 > Port");
					ChooseMenuEntry();
					if (!(fClose = GetMenuEntry((char*)"0123456789", &c)))
					{
						ELCSetPort(elc, c - '0');
						fContinue = false;
					}
					else
						fClose = false;
				}
				else
				{
					char* ELC_NATIVE_USB_DRIVER = (char*)"USB\\VID_10C4&PID_EA60";
					char* ATEN_USB_DRIVER = (char*)"USB\\VID_0557&PID_2022";
					int port;

					// check all USB drivers
					if (NO_COM_PORT == (port = ELCGetUSBComPort(ATEN_USB_DRIVER)))
						if (NO_COM_PORT == (port = ELCGetUSBComPort(ELC_NATIVE_USB_DRIVER)))
							port = NO_COM_PORT;
					if (NO_COM_PORT != port)
					{
						char dummy[ONEKB];
						sprintf_s(dummy, "Using COM%u", port);
						DisplayMessage(elc, dummy);
						ELCSetPort(elc, port);
						fContinue = false;
					}
				}

			}
			fContinue = false;
		}
		fContinue = true;
		while (!fClose)
		{
			MenuEntry("P > Change transmission speed");
			MenuEntry("T > Test dialog");
			MenuEntry("S > Check status");
			MenuEntry("R > Read check");
			MenuEntry("W > Write check");
			MenuEntry("A > Abort");
			ChooseMenuEntry();
			if (!(fClose = GetMenuEntry((char*)"ptsrwa", &c)))
			{
				if (TestMenuEntry((char*)"p", c))
				{
#define CURRENT " [CURRENT]"
					DWORD rate = ELCSpeed(elc, 0);
					char dummy[ONEKB];
					sprintf_s(dummy, sizeof(dummy), (CBR_2400 == rate ? "%s %s" : "%s"), "2> 2400 baud rate", CURRENT);
					MenuEntry(dummy);
					sprintf_s(dummy, sizeof(dummy), (CBR_4800 == rate ? "%s %s" : "%s"), "4> 4800 baud rate", CURRENT);
					MenuEntry(dummy);
					sprintf_s(dummy, sizeof(dummy), (CBR_9600 == rate ? "%s %s" : "%s"), "9> 9600 baud rate", CURRENT);
					MenuEntry(dummy);
					ChooseMenuEntry();
					if (!GetMenuEntry((char*)"249", &c))
					{
						if (TestMenuEntry((char*)"2", c))
							ELCSpeed(elc, 2400);
						else if (TestMenuEntry((char*)"4", c))
							ELCSpeed(elc, 4800);
						else if (TestMenuEntry((char*)"9", c))
							ELCSpeed(elc, 9600);
					}
					else
					{
						ELCSpeed(elc, 9600);
					}
				}
				else
					if (fOpen = ELCOpen(elc, true))
					{
						if (TestMenuEntry((char*)"t", c))
						{
							if (ELCInitiateDialog(elc))
								DisplaySuccess(elc);
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char*)"s", c))
						{
							BOOL fDocumentReadyToBeRead;
							if (ELCStatus(elc, &fDocumentReadyToBeRead))
								DisplayResult(elc, 0x31, fDocumentReadyToBeRead ? "A document is ready to be read" : "No document ready to be read");
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char*)"a", c))
						{
							BOOL fEjected;
							if (ELCAbort(elc, &fEjected))
								DisplayResult(elc, 0x31, fEjected ? "Document ejected" : "Document not ejected");
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char*)"r", c))
						{
							char raw[ONEKB], chpn[ONEKB];
							BOOL fDocumentIsStillInside;
							HANDLE timerStarted = CreateEvent(NULL, false, false, NULL);
							if (ELCResult::completed == ELCRead(elc,
								10,
								//INFINITE,
								timerStarted, raw, _countof(raw), chpn, _countof(chpn), &fDocumentIsStillInside))
							{
								char dummy[ONEKB * 2];
								sprintf_s(dummy, sizeof(dummy), "CMC7: %s", chpn);
								DisplayResult(elc, 0x31, fDocumentIsStillInside ? "Document still inside the reader" : "Document ejected");
								std::cout << std::endl << RESULT << dummy << std::endl;
							}
							else
							{
								if (ELCResult::timeout == ELCLastAsyncResult(elc))
									DisplayMessage(elc, "TIMEOUT");
								if (ELCResult::cancelled == ELCLastAsyncResult(elc))
									DisplayMessage(elc, "CANCELLED BY USER");
								DisplayFailed(elc);
							}
							CloseHandle(timerStarted);
						}
						else if (TestMenuEntry((char*)"w", c))
						{
							char printed[ONEKB];
							const char* psz = "azertyuiopqsdfghjklmwxcvbnAZERTYUIOPQSDFGHJKLMWXCVBN12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
							HANDLE timerStarted = CreateEvent(NULL, false, false, NULL);
							if (ELCResult::completed == ELCWrite(elc, psz, printed, _countof(printed),
								20,
								//INFINITE,
								timerStarted))
							{
								char buffer[ONEKB * 2];
								DisplayResult(elc, 0x31, "Printing OK");
								sprintf_s(buffer, ONEKB * 2, "SENT: %s (%d)", psz, strlen(psz));
								std::cout << std::endl << RESULT << buffer;
								sprintf_s(buffer, ONEKB * 2, "PRINTED: %s (%d)", printed, strlen(printed));
								std::cout << std::endl << RESULT << buffer;
							}
							else
							{
								if (ELCResult::timeout == ELCLastAsyncResult(elc))
									DisplayMessage(elc, "TIMEOUT");
								if (ELCResult::cancelled == ELCLastAsyncResult(elc))
									DisplayMessage(elc, "CANCELLED BY USER");
								DisplayFailed(elc);
							}
							CloseHandle(timerStarted);
						}
						ELCClose(elc);
					}
					else
						std::cout << std::endl << RESULT << OPEN_ERROR;
			}
		}
		ELCRelease(&elc);
	}
}