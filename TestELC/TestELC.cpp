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

static void MenuEntry(char * buffer)
{
	std::cout << std::endl << buffer;
}
static void MenuReply(int i)
{
	char buffer[ONEKB];
	sprintf_s(buffer, ONEKB, "%C", i);
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
static bool TestMenuEntry(char * entries, int entry)
{
	int i;
	char * upper = (char *)calloc(1, strlen(entries) + 2);
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
static bool GetMenuEntry(char * entries, int * pc)
{
	do
	{
		*pc = _getch();
	}
	while (!TestMenuEntry(entries, *pc));
	bool fClose = TestMenuEntry("", *pc);
	if (!fClose)
		MenuReply(*pc);
	else
		MenuClose();
	return fClose;
}
static void DisplaySuccess(ELC pelc)
{
	std::cout << std::endl << RESULT << "OK";// << std::endl;
}
static void DisplayFailed(ELC pelc)
{
	std::cout << std::endl << RESULT << "KO";// << std::endl;
}
static void DisplayResult(ELC pelc, const char success, const char * psz)
{
	char buffer[ONEKB];
	DisplaySuccess(pelc);
	std::cout << std::endl;
	for (int i = 1; i <= 3; i++)
	{
		char c = ELCCR(pelc, i);
		if (0x00 != c)
		{
			sprintf_s(buffer, ONEKB, "\t==> CR%d: %X", i, c);
			std::cout << buffer;
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
	void * elc;
	char buffer[ONEKB];
	bool fClose = false, fContinue = true, fOpen = false;
	if (NULL != (elc = ELCInit()))
	{
		while (!fClose && fContinue)
		{
			MenuEntry("U > USB port");
			MenuEntry("S > Serial port");
			ChooseMenuEntry();
			if (!(fClose = GetMenuEntry((char *)"us", &c)))
			{
				if (TestMenuEntry((char *)"s", c))
				{
					MenuEntry("1..9 > Port");
					ChooseMenuEntry();
					if (!(fClose = GetMenuEntry((char *)"0123456789", &c)))
					{
						ELCSetPort(elc, c - '0');
						fContinue = false;
					}
					else
						fClose = false;
				}
				else
					fContinue = false;
			}
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
			if (!(fClose = GetMenuEntry((char *)"ptsrwa", &c)))
			{
				if (TestMenuEntry((char *)"p", c))
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
					if (!GetMenuEntry((char *)"249", &c))
					{
						if (TestMenuEntry((char *)"2", c))
							ELCSpeed(elc, 2400);
						else if (TestMenuEntry((char *)"4", c))
							ELCSpeed(elc, 4800);
						else if (TestMenuEntry((char *)"9", c))
							ELCSpeed(elc, 9600);
					}
					else
					{
						ELCSpeed(elc, 9600);
					}
				}
				else
					if (fOpen = ELCOpen(elc))
					{
						if (TestMenuEntry((char *)"t", c))
						{
							if (ELCInitiateDialog(elc))
								DisplaySuccess(elc);
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char *)"s", c))
						{
							bool fPresent;
							if (ELCStatus(elc, &fPresent))
								DisplayResult(elc, 0x31, fPresent ? "A document is ready to be read" : "No document ready to be read");
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char *)"a", c))
						{
							bool fEjected;
							if (ELCAbort(elc, &fEjected))
								DisplayResult(elc, 0x31, fEjected ? "No paper inside the reader" : "Paper not ejected");
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char *)"r", c))
						{
							char * pchRaw, * pchChpn;
							int uiRead;
							bool fRead, fDocumentInside;
							if (ELCRead(elc, 0, &pchRaw, &pchChpn, &fDocumentInside))
							{
								//char dummy[ONEKB];
								//sprintf_s(dummy, ONEKB, "%s - %s", fRead ? "Readinf was successfull" : "Reading has failed", fDocumentInside ? "Document still inside" : "Document ejected");
								//DisplayResult(elc, 0x31, dummy);
								//sprintf_s(buffer, ONEKB, "%s (%d)", pb, uiRead);
								//std::cout << std::endl << RESULT << buffer;
							}
							else
								DisplayFailed(elc);
						}
						else if (TestMenuEntry((char *)"w", c))
						{
							DWORD dw;
							if (ELCWrite(elc, "Hello World", 30))
							{
								//DisplayResult(elc);
								//sprintf_s(buffer, ONEKB, "%s (%d)", pb, dw);
								//std::cout << std::endl << RESULT << "ECRIT: " << buffer;
							}
							else
								DisplayFailed(elc);
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