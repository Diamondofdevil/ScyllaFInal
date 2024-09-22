#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace HackSMB{

	public __value struct events_{
	public:
		int iID;
		DateTime dtTimeGen;
		DateTime dtTimeWritten;
		String* strType;
		String* strSourceName;
		String* strComputerName;
	};

	public __gc class groups_{
	public:
		List<String *> *plstUsers;
		String* strName;
		String* strComment;
		String* strGUID;
	};

	public __value struct user_{
		String* strName;
		String* strComment;
		List<String *> *plstFlags; //? // WTF?
		List<String *> *plstGroups;
		String* strFull_Name;
		String* strLonguid;
		int iUser_ID;
	};

	public __value struct transport_{
	public:
		String* strAddress;
		String* strDomain;
		String* strNetAddress;
	};

	public __value struct OS_{
	public:
		int iMinor;
		int iMajor;
		String* strType;
		String* strComment;
		String* strADName;
		String* strADOrg;
		String* strADProcessor;
		int iADProcCount;
		String* strADguid;
		String* strADOwner;
	};

	public __value struct History_{
		int iMinLen;
		int iMaxAge;
		int iMinAge;
		int iForceLogOff;
		int iPassHistLen;
		int iAttempts;
		int iTimeBetweenFail; //secs
		int iLockDuration; //mins
	};

	public __value struct NetBIOS_{
		String* strName;
		String* strType;
		String* strHost;
		String* strComment;
	};

	public __value struct Service_{
		String* strDisplayName;
		String* strServiceName;
		String* strStatus;
	};

	public __value struct Session_{
		String* strConnection_Machine_Name;
		String* strUsername;
		int iSecIdle;
		int iSecConnected;
	};

	public __value struct NetDev_{
		String* strAddress;
		String* strDomain;
		String* strNetAddress;
		String* strName;
	};

	public __gc class WinInfo{
	public:
		WinInfo( void );
		
		List<user_> *plstUsers;
		List<String *> *plstDisks;
		List<events_> *plstEventLog;
		List<groups_ *> *plstGroups;
		OS_ os;
		History_ history;
		List<NetBIOS_> *plstNbs;
		List<Service_> *plstServ;
		List<Session_> *plstSession;
		DateTime dtDate;
		List<NetDev_> *plstDevs;
	};
}