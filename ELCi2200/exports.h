//#pragma once

#ifndef EXPORTS_H
#define EXPORTS_H

#ifdef DRIVER_USE_AS_DLL
#	undef DRIVER_USE_AS_LIB
#else
#	define DRIVER_USE_AS_LIB
#endif

#ifdef DRIVER_USE_AS_DLL

#	ifdef DRIVER_EXPORTS
#		define DRIVERAPI __declspec(dllexport)
#	else
#		define DRIVERAPI extern __declspec(dllimport)
#	endif

#endif

#ifdef DRIVER_USE_AS_LIB

#	ifdef DRIVER_EXPORTS
#		define DRIVERAPI
#	else
#		define DRIVERAPI extern
#	endif

#endif

#define DRIVERCALL __stdcall

#endif