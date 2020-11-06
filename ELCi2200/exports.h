//#pragma once

#ifndef EXPORTS_H
#define EXPORTS_H

#ifdef __EXPORTS
#define DRIVERAPI __declspec(dllexport)
#else
#define DRIVERAPI __declspec(dllimport)
#endif

#endif