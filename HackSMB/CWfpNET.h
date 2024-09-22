#pragma once

#include <utility>
//#include <Ntddndis.h>
#include <winsock2.h>
#include <lmcons.h>
#include <rpcdce.h>					// RPC Support
#include <mgmtapi.h>
#include <psapi.h>			
#include <windows.h>
#include <tchar.h>
#include <cstdlib>
#include <cstdio>
#include <ctime>

#include <iostream>
using namespace std;

#include "commons.h"
#include "WinInfo.h"
#include <Lm.h>
#include <Lmserver.h>

#include <iads.h>			// ADSI Property Methods
#include <adshlp.h>			// ADsGetObject Support

#pragma comment (lib,"oleaut32.lib")
#pragma comment (lib,"activeds") // Active Directory Support
#pragma comment (lib,"adsiid")   // Active Directory Support

#pragma comment (lib, "ws2_32.lib")		// Winsock Support
#pragma comment (lib, "netapi32.lib")
#pragma comment (lib, "mpr.lib")
#pragma comment (lib, "rpcrt4.lib")		// RPC Support
#pragma comment (lib, "version.lib")	// File Version
#pragma comment (lib, "psapi.lib")		// Process Enumeration
#pragma comment (lib, "advapi32.lib")	// File Version

using namespace System;

namespace HackSMB
{

	public __gc class CWfpNET {
	private:
		LPCWSTR GetServerTypeStr( DWORD dwType );
		bool NET_Machines_Users_Groups( DWORD dwLevel, WinInfo* _out, int* indexOut, int maxGet);

		bool ADinitAD();
		bool ADServices_Users_Groups( int iType, WinInfo* _out);
	public:
		CWfpNET( String*, String*, String*, bool bInitAD);
		~CWfpNET();
		bool Disks_get( WinInfo* _out );
		bool EventLog_get( WinInfo* _out);
		bool GroupMembers_get( LPWSTR pwsGroup, groups_*, int iLen);
		bool IPC_Session_Connect( void );
		bool IPC_Session_Disconnect( void );
		bool LocalGroups_get( user_* _out );
		bool OperatingSystem_get( WinInfo* _out );
		bool PasswordPolicy_get( WinInfo* _out );
		bool NetBIOSShares_get( WinInfo* _out );
		bool Services_get( WinInfo* _out );
		bool Groups_get( user_* _out );
		bool Sessions_get( WinInfo* _out );
		bool Time_get( WinInfo* _out );
		bool Transports_get( WinInfo* _out );
		bool Users_get( WinInfo* _out, int* indexOut, int maxGet);
		bool ADGroups_get( WinInfo* _out );
		bool ADOperatingSystem_get( WinInfo* _out );
		bool ADNetBIOSShares_get( WinInfo* _out );
		virtual bool ADServices_get( WinInfo* _out );
	
		bool ADSessions_get( WinInfo* _out );
		bool ADUsers_get( WinInfo* _out );
	};
}