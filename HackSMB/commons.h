#pragma once

#include <cstring>
#include <lmcons.h>
using namespace std;

#define MAX_IP_ADDR			16

typedef struct _wfphost{
	WCHAR* wsUsr;
	WCHAR* wsPass;
	WCHAR* wsDNS;
	WCHAR* wsNetBIOS;
	WCHAR* wsDomain;
	char szIPAddr[MAX_IP_ADDR];
	unsigned long ulRes;
	char szComputer[UNCLEN + 1];
	WCHAR wsComputer[UNCLEN + 1];
}wfphost_t, *pwfphost_t;

wfphost_t wfNode;

#define REQ_DATASIZE			32		// Echo Request Data size
#define ICMP_ECHOREQ			8		// Echo request query
#define	ICMP_ECHOREPLY			0	 // echo reply 
#define MAXHOSTNAMELEN			256
#define MIN_ICMP_PACKET_SIZE	8   //minimum 8 byte icmp packet (just header)
#define ENUM_USERS				1
#define ENUM_MACHINES			2
#define ENUM_GROUPS				3
#define ENUM_SERVICES			4
#define F_READY					0x1L // Non-blocking connect Initialized 
#define F_CONNECTING			0x2L // Non-blocking connect in progress 
#define F_READING				0x4L // Non-blocking complete; now reading
#define F_DONE					0x8L // Non-blocking done
//#define BUFFSIZE		2048
#define HASHLEN					41  // MD5=32, SHA-1=40 + 1
#define MAX_ADS_PATH			36


enum _smb_access {
	SMB_ADSI,
	SMB_NET,
	SMB_WMI,
};

enum _scan_type {
	SCAN_HOST,
	SCAN_RANGE,
	SCAN_LIST,
	SCAN_NEIGHBORHOOD,
	SCAN_PROCESSES,
	SCAN_FILES
};

#define WF_OPT_BINDINGS				0x0001L
#define WF_OPT_OSVER				0x0002L
#define WF_OPT_PENSHARETEST			0x0004L
#define WF_OPT_PING					0x0008L
#define WF_OPT_REGISTRY				0x0010L
#define WF_OPT_SERVICES				0x0020L
#define WF_OPT_SESSIONS				0x0040L
#define WF_OPT_SHARES				0x0080L
#define WF_OPT_SID					0x0100L
#define WF_OPT_TRACE				0x0200L
#define WF_OPT_GROUPS				0x0400L
#define WF_OPT_USERS				0x0800L
#define WF_OPT_MAC_ADDR				0x1000L
#define WF_OPT_MD5					0x2000L
#define WF_OPT_NO_DIR_RECURSE		0x4000L

#define GET_OPT(x, y) (((x) & (y)) == (y) ? 1 : 0)

typedef struct _wfpoptions {
	// Scan Input Types
	WCHAR wcHost;
	char szList[MAX_PATH + 1];
	// Scan Types
	_smb_access smb_type;
	_scan_type scan_type;
	// Scan Options
	unsigned long ulOpts;
	unsigned int uiMax_Connections;
	unsigned int uiRetries;
	unsigned int uiTimeout;
}wfpoptions_t, *pwfpoptions_t;

//	Types of server

#define SV_TYPE_SQLSERVER_STR			L"SQL Server"
#define SV_TYPE_DOMAIN_CTRL_STR			L"Primary Domain Controller"
#define SV_TYPE_DOMAIN_BAKCTRL_STR		L"Backup Domain Controller"
#define SV_TYPE_SERVER_NT_STR			L"NT Member Server"
#define SV_TYPE_NT_STR					L"NT Workstation"
#define SV_TYPE_WORKSTATION_STR			L"LAN Manager Workstation"
#define	SV_TYPE_SERVER_STR				L"LAN Manager Server"
#define SV_TYPE_TIME_SOURCE_STR			L"Time Source"
#define SV_TYPE_AFP_STR					L"Apple File Protocol Server"
#define SV_TYPE_DOMAIN_MEMBER_STR		L"LAN Manager 2.x domain member"
#define SV_TYPE_LOCAL_LIST_ONLY_STR		L"Servers maintained by the browser"
#define SV_TYPE_PRINTQ_SERVER_STR		L"Server sharing print queue"
#define SV_TYPE_DIALIN_SERVER_STR		L"Dial-in Server"
#define SV_TYPE_XENIX_SERVER_STR		L"Xenix"
#define SV_TYPE_POTENTIAL_BROWSER_STR	L"Potential Browser"
#define SV_TYPE_MASTER_BROWSER_STR		L"Master Browser"
#define SV_TYPE_BACKUP_BROWSER_STR		L"Backup Browser"
#define SV_TYPE_DOMAIN_MASTER_STR		L"Domain Master Browser"

#if _WIN32_WINNT > 0x0500
#define SV_TYPE_TERMINALSERVER_STR		L"Terminal Server"
#endif

#define SV_TYPE_CLUSTER_NT_STR			L"Cluster"
#define SV_TYPE_NOVELL_STR				L"Novell Netware Server"
#define SV_TYPE_WINDOWS_STR				L"Windows 9x or Me Workstation"
#define SV_TYPE_WFW_STR					L"Windows for Workgroups Workstation"


