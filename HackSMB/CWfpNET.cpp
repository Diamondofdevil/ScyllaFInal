#include "CWfpNET.h"

using namespace System::Runtime::InteropServices;

namespace HackSMB
{

	CWfpNET::CWfpNET(String* strIP, String* strUsr, String* strPass, bool bInitAD)
	{
		char* szStr = NULL;

		memset( &wfNode, 0x00, sizeof( wfphost_t ));
		//strIP is NEVER NULL!
	
		szStr = (char*)(void *)Marshal::StringToHGlobalAnsi( strIP );

		if(String::IsNullOrEmpty( strUsr ) == false)
			wfNode.wsUsr = (WCHAR *)(void *)Marshal::StringToCoTaskMemUni( strUsr );
		
		if(String::IsNullOrEmpty( strPass ) == false)
			wfNode.wsPass = (WCHAR *)(void *)Marshal::StringToCoTaskMemUni( strPass );

		strcpy( wfNode.szComputer, szStr);

		MultiByteToWideChar(CP_ACP, MB_PRECOMPOSED, wfNode.szComputer, -1, wfNode.wsComputer, UNCLEN);

		strcpy( wfNode.szIPAddr, wfNode.szComputer);
		
		if (bInitAD != NULL)
			ADinitAD();
	}

	bool CWfpNET::ADinitAD()
	{
		// FIXME: Currently ADSI_Shares only returns 1 share.
		IADsFileShare *pShares = NULL;	
		IADsCollection *pColl = NULL;
		IADsContainer *pCont = NULL;
		IEnumVARIANT *pEnum = NULL;
		LPWSTR pwsADSPath = NULL;
		IDispatch *pDisp = NULL;
		IUnknown *pUnk = NULL;
		HRESULT hrRes = 0;//0;
		ULONG ulFetch = 0;
		VARIANT vVar;
		BSTR bStr;

		pwsADSPath = new WCHAR[MAX_ADS_PATH + 1]; 

		if (pwsADSPath == NULL)
			return false;

		memset( pwsADSPath, 0x00, (MAX_ADS_PATH + 1 * sizeof( WCHAR )));
	
		_snwprintf_s( pwsADSPath, MAX_ADS_PATH, 26, L"WinNT://%S/LanmanServer", wfNode.szIPAddr);

		hrRes = ADsGetObject( pwsADSPath, IID_IADsContainer, (void**)&pCont);

		if (FAILED( hrRes ) != false) {
failed:
			delete [] pwsADSPath;
			return false;
		}

		hrRes = pCont->get__NewEnum( &pUnk );
	
		if (FAILED( hrRes ) != false)
			goto failed;

		hrRes = pUnk->QueryInterface( IID_IEnumVARIANT, (void**)&pEnum);
	
		if (FAILED( hrRes ) != false)
			goto failed;

		// Now Enumerate

		VariantInit( &vVar );
	
		pEnum->Reset();
	
		hrRes = pEnum->Next( 1, &vVar, &ulFetch);
	
		
		while(SUCCEEDED( hrRes ) == true && ulFetch > 0) {
			if (ulFetch == 1) {
				pDisp = V_DISPATCH( &vVar );

				pDisp->QueryInterface( IID_IADsFileShare , (void**)&pShares);

				pShares->get_HostComputer( &bStr );

				SysFreeString( bStr );

				pShares->get_Name( &bStr );
				
				SysFreeString( bStr );

				pShares->get_Description( &bStr );
				
				SysFreeString( bStr );

				pShares->Release();
			}
			VariantClear( &vVar );
			
			pDisp = NULL;
			hrRes = pEnum->Next( 1, &vVar, &ulFetch);
	};
	
	delete [] pwsADSPath;
	
	if (pDisp != NULL)
		pDisp->Release();

	if (pEnum != NULL)
		pEnum->Release();
	
	if (pUnk != NULL)
		pUnk->Release();
	
	if(pColl != NULL)
		pColl->Release();

	return true;
}

CWfpNET::~CWfpNET(void)
{
}

bool CWfpNET::Groups_get(user_* _out) 
{
	LPLOCALGROUP_USERS_INFO_0 pBuf = NULL;
   DWORD dwLevel = 0;
   DWORD dwFlags = LG_INCLUDE_INDIRECT ;
   DWORD dwPrefMaxLen = MAX_PREFERRED_LENGTH;
   DWORD dwEntriesRead = 0;
   DWORD dwTotalEntries = 0;
   NET_API_STATUS nStatus;


	WCHAR* szStr = NULL;
    szStr =(WCHAR *)(void *)Marshal::StringToCoTaskMemUni( _out->strName );

   nStatus = NetUserGetLocalGroups(wfNode.wsComputer,
                                   szStr,
                                   dwLevel,
                                   dwFlags,
                                   (LPBYTE *) &pBuf,
                                   dwPrefMaxLen,
                                   &dwEntriesRead,
                                   &dwTotalEntries);
  
   if (nStatus == NERR_Success)
   {
      LPLOCALGROUP_USERS_INFO_0 pTmpBuf;
      DWORD i;
      DWORD dwTotalCount = 0;
	  if(_out->plstGroups == NULL)
	  	_out->plstGroups = new List<String*>();
      if ((pTmpBuf = pBuf) != NULL)
      {
         for (i = 0; i < dwEntriesRead; i++)
         {
            if (pTmpBuf == NULL)
            {
               return false;
            }
			
			_out->plstGroups->Add(new String(pTmpBuf->lgrui0_name));

            pTmpBuf++;
            dwTotalCount++;
         }
      }
      
   }
   else
      return false;
   
   if (pBuf != NULL)
      NetApiBufferFree(pBuf);

   return true;
}

LPCWSTR CWfpNET::GetServerTypeStr( DWORD dwType )
{
	if (dwType & SV_TYPE_SQLSERVER)
		return SV_TYPE_SQLSERVER_STR;

	if (dwType & SV_TYPE_DOMAIN_CTRL)
		return SV_TYPE_DOMAIN_CTRL_STR;

	if (dwType & SV_TYPE_DOMAIN_BAKCTRL)
		return SV_TYPE_DOMAIN_BAKCTRL_STR;

	if (dwType & SV_TYPE_SERVER_NT)
		return SV_TYPE_SERVER_NT_STR;

	if (dwType & SV_TYPE_NT)
		return SV_TYPE_NT_STR;

	if (dwType & SV_TYPE_WORKSTATION)
		return SV_TYPE_WORKSTATION_STR;

	if (dwType & SV_TYPE_SERVER)
		return SV_TYPE_SERVER_STR;

	if (dwType & SV_TYPE_TIME_SOURCE)
		return SV_TYPE_TIME_SOURCE_STR;

	if (dwType & SV_TYPE_AFP)
		return SV_TYPE_AFP_STR;

	if (dwType & SV_TYPE_DOMAIN_MEMBER)
		return SV_TYPE_DOMAIN_MEMBER_STR;

	if (dwType & SV_TYPE_LOCAL_LIST_ONLY)
		return SV_TYPE_LOCAL_LIST_ONLY_STR;

	if (dwType & SV_TYPE_PRINTQ_SERVER)
		return SV_TYPE_PRINTQ_SERVER_STR;

	if (dwType & SV_TYPE_DIALIN_SERVER)
		return SV_TYPE_DIALIN_SERVER_STR;

	if (dwType & SV_TYPE_XENIX_SERVER)
		return SV_TYPE_XENIX_SERVER_STR;

	if (dwType & SV_TYPE_POTENTIAL_BROWSER)
		return SV_TYPE_POTENTIAL_BROWSER_STR;

	if (dwType & SV_TYPE_MASTER_BROWSER)
		return SV_TYPE_MASTER_BROWSER_STR;

	if (dwType & SV_TYPE_BACKUP_BROWSER)
		return SV_TYPE_BACKUP_BROWSER_STR;

	if (dwType & SV_TYPE_DOMAIN_MASTER)
		return SV_TYPE_DOMAIN_MASTER_STR;

#if _WIN32_WINNT > 0x0500
	if (dwType & SV_TYPE_POTENTIAL_BROWSER)
		return SV_TYPE_POTENTIAL_BROWSER_STR;
#endif

	if (dwType & SV_TYPE_TERMINALSERVER)
		return SV_TYPE_TERMINALSERVER_STR;

	if (dwType & SV_TYPE_CLUSTER_NT)
		return SV_TYPE_CLUSTER_NT_STR;

	if (dwType & SV_TYPE_NOVELL)
		return SV_TYPE_NOVELL_STR;

	if (dwType & SV_TYPE_WINDOWS)
		return SV_TYPE_WINDOWS_STR;

	if (dwType & SV_TYPE_WFW)
		return SV_TYPE_WFW_STR;

	return NULL;
}

bool CWfpNET::OperatingSystem_get(WinInfo* _out)
{
	LPSERVER_INFO_101 pBuf	= NULL;
	LPWKSTA_INFO_102 pwBuf  = NULL;
	NET_API_STATUS nStatus  = NULL;
	unsigned long ulRes = 0;
	
	// The NetServerGetInfo function retrieves current configuration information
	// for the specified server.
	// No special group membership is required for level 100 or level 101 calls.

	nStatus = NetServerGetInfo( wfNode.wsComputer, 101,(LPBYTE *)&pBuf);

	if (nStatus != NERR_Success) {
		if(pBuf != NULL)
			NetApiBufferFree( pBuf );
		return false;
	}

	_out->os.iMajor = pBuf->sv101_version_major;
	_out->os.iMinor = pBuf->sv101_version_minor;

	_out->os.strType = GetServerTypeStr( pBuf->sv101_type );

		
	if(pBuf->sv101_comment != NULL)
		_out->os.strComment = new String( pBuf->sv101_comment );

	if(pBuf != NULL)
		NetApiBufferFree( pBuf );

	return true; 
}

bool CWfpNET::NetBIOSShares_get( WinInfo* _out )
{
	DWORD i = 0, entriesread = 0, resume_handle = 0, totalentries = 0;
	PSHARE_INFO_1 pBuf = NULL, pTmpBuf = NULL;
	NET_API_STATUS nStatus  = NULL;
	bool accessible = false;
	NETRESOURCE nr;
	WCHAR tmp;
	
	// The NetShareEnum function retrieves information about each shared
	// resource on a server.
	// No special group membership is required for level 0 or level 1 calls.

	do {
		nStatus = NetShareEnum( wfNode.wsComputer, 1, (LPBYTE *) &pBuf,
			0xFFFFFFFF, &entriesread, &totalentries, &resume_handle);

		if (nStatus != ERROR_SUCCESS && nStatus != ERROR_MORE_DATA) {
			return false;
		}

		if((pTmpBuf = pBuf) != NULL) {

			for ( i = 0; i < entriesread; ++i) {
				NetBIOS_ nb = NetBIOS_();
				
				nb.strName = pTmpBuf->shi1_netname;
				nb.strComment = pTmpBuf->shi1_remark;

				if (pTmpBuf->shi1_type == STYPE_DISKTREE) {
					accessible = false;
					nr.dwType = RESOURCETYPE_ANY;
					
					nr.lpLocalName = NULL;
					
					nr.lpProvider = NULL;

					if (WNetAddConnection2(&nr, NULL, NULL, FALSE) == NO_ERROR) {
						accessible = true;
						IntPtr ptr = Marshal::StringToHGlobalUni(nb.strName);
						WNetCancelConnection2((LPWSTR)ptr.ToPointer() ,CONNECT_UPDATE_PROFILE, TRUE);
						Marshal::FreeHGlobal(ptr);
					}
					nb.strType = L"Disk";
				}
				
				else if(pTmpBuf->shi1_type == STYPE_DEVICE)
					nb.strType = L"Communication Device";
				else if(pTmpBuf->shi1_type == STYPE_IPC)
					nb.strType = L"IPC";
				else if(pTmpBuf->shi1_type == STYPE_PRINTQ)
					nb.strType = L"Print Queue";
				
				pTmpBuf++;
				_out->plstNbs->Add(nb);
			}
		}
		
		if(pBuf != NULL) {
			NetApiBufferFree(pBuf);
			pBuf = NULL;
		}
	} while (nStatus == ERROR_MORE_DATA);
	return true;
}

bool CWfpNET::PasswordPolicy_get(WinInfo* _out)
{
	USER_MODALS_INFO_0 *pBuf0 = NULL;
	USER_MODALS_INFO_3 *pBuf3 = NULL;
	NET_API_STATUS nStatus = NULL;

	nStatus = NetUserModalsGet( wfNode.wsComputer, 0,(LPBYTE *)&pBuf0);

	if(nStatus != NERR_Success){
		nStatus = NULL; //no se q mas poner xD
		return false;
	}
	if (pBuf0 != NULL) {
		_out->history.iMinLen = pBuf0->usrmod0_min_passwd_len;
		_out->history.iMinAge = pBuf0->usrmod0_min_passwd_age/86400;
		_out->history.iMaxAge = pBuf0->usrmod0_max_passwd_age/86400;
		_out->history.iForceLogOff = pBuf0->usrmod0_force_logoff;
		_out->history.iPassHistLen = pBuf0->usrmod0_password_hist_len;
		NetApiBufferFree( pBuf0 );

     }
 
	nStatus = NetUserModalsGet( wfNode.wsComputer, 3,(LPBYTE *)&pBuf3);

	//	REMUEVA ESTA SHIT Y RETORNE EN VEZ DE USAR ELSE
	if(nStatus != NERR_Success){
		pBuf0 = NULL;
		return false;
	}
	
	if(pBuf3 != NULL) {
		_out->history.iAttempts = pBuf3->usrmod3_lockout_threshold;
		_out->history.iTimeBetweenFail = pBuf3->usrmod3_lockout_observation_window;
		_out->history.iLockDuration = pBuf3->usrmod3_lockout_duration / 60;
		NetApiBufferFree( pBuf3 );
	}
	return true;
}

bool CWfpNET::Services_get(WinInfo* _out)
{
	DWORD numServices = 0, sizeNeeded = 0, resume = 0;
	LPENUM_SERVICE_STATUS service_status = NULL;
	WCHAR *pwsDir = NULL;
	SC_HANDLE scm;
	DWORD i	= 0;
	
	pwsDir = new WCHAR[100];

	if (pwsDir == NULL)
		return false;

	_snwprintf_s( pwsDir, 100, 100, L"\\\\%s", wfNode.wsComputer);
	
	scm = OpenSCManager( pwsDir, 0, SC_MANAGER_ALL_ACCESS);

	delete []pwsDir;

	if(scm == NULL)
		return false;
	
	EnumServicesStatus(scm,	SERVICE_WIN32, SERVICE_STATE_ALL, // use SERVICE_STATE_ALL to see both
		0, 0, &sizeNeeded, &numServices, &resume);
	
	if(GetLastError() != ERROR_MORE_DATA)
		return false;

	
	if((service_status = (LPENUM_SERVICE_STATUS) HeapAlloc(GetProcessHeap(), 0, sizeNeeded)) == NULL)
		return false;
			
		// Get the status records. Making an assumption
		// here that no new services get added during
		// the allocation (could lock the database to
		// guarantee that...)
	resume = 0;
		// EnumServicesStatusEx supersedes EnumServicesStatus and also returns PID
		// but is for Win2k or XP only
	if(!EnumServicesStatus(scm, SERVICE_WIN32, SERVICE_STATE_ALL,
		service_status, sizeNeeded, &sizeNeeded, &numServices, &resume))
	{
		CloseServiceHandle( scm );

		if(service_status != NULL)
			HeapFree(GetProcessHeap(), 0, service_status);

		return false;
	}
	
	for(i=0; i < numServices; i++) {
		Service_ ser = Service_();
		ser.strDisplayName = new String(service_status[i].lpDisplayName);
		ser.strServiceName = new String(service_status[i].lpServiceName);
		
		switch(service_status[i].ServiceStatus.dwCurrentState) {
		case SERVICE_CONTINUE_PENDING:
			ser.strStatus = new String(L"continue pending.");
			break;
		case SERVICE_PAUSE_PENDING:
			ser.strStatus = new String(L"pause pending.");
			break;
		case SERVICE_PAUSED:
			ser.strStatus = new String(L"paused.");
			break;
		case SERVICE_RUNNING:
			ser.strStatus = new String(L"running.");
			break;
		case SERVICE_START_PENDING:
			ser.strStatus = new String(L"start pending.");
			break;
		case SERVICE_STOP_PENDING:
			ser.strStatus = new String(L"stop pending.");
			break;
		case SERVICE_STOPPED:
			ser.strStatus = new String(L"stopped.");
			break;
		}

		_out->plstServ->Add(ser);
	}
			
	if(service_status != NULL)
		HeapFree(GetProcessHeap(), 0, service_status);

	CloseServiceHandle(scm);
	return true;
}


bool CWfpNET::Sessions_get(WinInfo* _out)  {
		LPSESSION_INFO_10 pBuf = NULL, pTmpBuf = NULL;
	DWORD dwLevel = 10,	dwPrefMaxLen = MAX_PREFERRED_LENGTH,
		dwEntriesRead = 0, dwTotalEntries = 0, dwResumeHandle = 0,
		i = 0;
	LPWSTR pszClientName = NULL, pszUserName = NULL;
	NET_API_STATUS nStatus = NULL;

	// The NetSessionEnum function provides information about sessions
	// established on a server.
	// No special group membership is required for level 0 or level 10 calls.

	do 
	{
		nStatus = NetSessionEnum( wfNode.wsComputer, pszClientName, pszUserName,
			dwLevel, (LPBYTE*)&pBuf, dwPrefMaxLen, &dwEntriesRead, &dwTotalEntries,
			&dwResumeHandle);
		if(!(nStatus == NERR_Success) && !(nStatus == ERROR_MORE_DATA))
		{		
			return false;
		}
		if((pTmpBuf = pBuf) != NULL)
		{
			for(i = 0; i < dwEntriesRead; i++)
			{
				if(pTmpBuf == NULL)
					break;
				Session_ s = Session_();
				s.strConnection_Machine_Name = pTmpBuf->sesi10_cname;
				s.strUsername = pTmpBuf->sesi10_username;
				s.iSecConnected = pTmpBuf->sesi10_time;
				s.iSecIdle = pTmpBuf->sesi10_idle_time;
				_out->plstSession->Add(s);
				pTmpBuf++;
			}
		}
			
		if(pBuf != NULL)
		{
			NetApiBufferFree(pBuf);
			pBuf = NULL;
		}
	}while (nStatus == ERROR_MORE_DATA); // end do

	if (pBuf != NULL)
		NetApiBufferFree(pBuf);
	  
	return true;
}

bool CWfpNET::Users_get(WinInfo* _out, int* indexOut, int maxGet) {
	int index;
	if(!NET_Machines_Users_Groups(ENUM_USERS, _out, &index, maxGet))
		return false;
	else{
		indexOut = (int*)malloc(sizeof(int));
		*indexOut = index;
		return true;
	}
}

bool CWfpNET::NET_Machines_Users_Groups(DWORD level, WinInfo* _out, int* indexOut, int maxGet)
{
	NET_DISPLAY_USER *ndu		= NULL;
	NET_DISPLAY_MACHINE *ndm	= NULL;
	NET_DISPLAY_GROUP *ndg		= NULL;
	NET_API_STATUS nStatus		= NULL;
	DWORD read = 0, Index = 0, i = 0;
	//indexOut = (int*)malloc(sizeof(int));
	*indexOut = 0;
	void *pBuf;

	do
	{
		pBuf = NULL;
		nStatus = NetQueryDisplayInformation(wfNode.wsComputer, level, Index, 100,
			MAX_PREFERRED_LENGTH, &read, &pBuf);
		if (nStatus != ERROR_MORE_DATA && nStatus != ERROR_SUCCESS)
		{			
			return false;
		}

		switch (level)
		{
			case ENUM_USERS: // users
			
				for(i = 0, ndu = (NET_DISPLAY_USER *)pBuf; i < read; ++ i, ++ ndu )
				{
					user_ usr = user_();
					usr.plstFlags = new List<String*>();
					usr.strName = ndu->usri1_name;
					usr.strComment = ndu->usri1_comment;
					usr.iUser_ID = ndu->usri1_user_id;
					usr.strFull_Name = ndu->usri1_full_name;

					if(ndu->usri1_flags & UF_SCRIPT)
						usr.plstFlags->Add(L"The logon script executed. This value must be set for LAN Manager 2.0 or Windows NT.");

					if (ndu->usri1_flags & UF_ACCOUNTDISABLE)
						usr.plstFlags->Add(L"The user's account is disabled.");
					
					if (ndu->usri1_flags & UF_HOMEDIR_REQUIRED)
						usr.plstFlags->Add(L"The home directory is required. Windows NT/2000 ignores this value.");
					
					if (ndu->usri1_flags & UF_PASSWD_NOTREQD)
						usr.plstFlags->Add(L"No password is required.");
					
					if (ndu->usri1_flags & UF_PASSWD_CANT_CHANGE )
						usr.plstFlags->Add(L"The user cannot change the password.");
					
					if (ndu->usri1_flags & UF_LOCKOUT)
						usr.plstFlags->Add(L"The Account is currently locked out.");
					
					if (ndu->usri1_flags & UF_DONT_EXPIRE_PASSWD)
						usr.plstFlags->Add(L"Password does not expire.");
						
					#if _WIN32_WINNT > 0x0500
					if (ndu->usri1_flags & UF_TRUSTED_FOR_DELEGATION)
						usr.plstFlags->Add(L"The account is enabled for delegation.");
					
					if (ndu->usri1_flags & UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED)
						usr.plstFlags->Add(L"The user's password is stored under reversible encryption in the Active Directory.");
					
					if (ndu->usri1_flags & UF_NOT_DELEGATED)
						usr.plstFlags->Add(L"Marks the account as \"sensitive\"; other users cannot act as delegates of this user account.");
						
					if (ndu->usri1_flags & UF_SMARTCARD_REQUIRED)
						usr.plstFlags->Add(L"Requires the user to log on to the user account with a smart card.");
						
					if (ndu->usri1_flags & UF_USE_DES_KEY_ONLY)
						usr.plstFlags->Add(L"Restrict this principal to use only Data Encryption Standard (DES) encryption types for keys.");
						
					if (ndu->usri1_flags & UF_DONT_REQUIRE_PREAUTH)
						usr.plstFlags->Add(L"This account does not require Kerberos preauthentication for logon.");
					
					if (ndu->usri1_flags & UF_PASSWORD_EXPIRED)
						usr.plstFlags->Add(L"The user's password has expired.");
					#endif
					_out->plstUsers->Add(usr);
				}
				if (read > 0)
					Index = ((NET_DISPLAY_USER *)pBuf)[read - 1].usri1_next_index;
				break;
			
			case ENUM_MACHINES: // machines. Not used in scylla
				for (i = 0, ndm = (NET_DISPLAY_MACHINE *) pBuf; i < read; ++ i, ++ ndm )
				{
//					m_output.Format(_T("\t%S %S\n"),ndm->usri2_name, ndm->usri2_flags );
				}
				break;
				//Deprecated in scylla
			case ENUM_GROUPS: // groups
			{	
				for (i = 0, ndg = (NET_DISPLAY_GROUP *) pBuf; i < read; ++ i, ++ ndg )
				{
					groups_* g = new groups_();

					g->strName = ndg->grpi3_name;
					g->strComment = ndg->grpi3_comment;

					g->plstUsers = new List<String*>();
					GroupMembers_get(ndg->grpi3_name, g,wcslen(ndg->grpi3_name));
					_out->plstGroups->Add(g);

				}
				// take the last element's next_index
				//usri1_next_index
				if (read > 0)
					Index = ((NET_DISPLAY_GROUP *)pBuf)[read - 1].grpi3_next_index;
				break;
			}
		}

		if (pBuf != NULL)
			NetApiBufferFree(pBuf);
		*indexOut = Index;
	} while (nStatus == ERROR_MORE_DATA && maxGet < Index);
	return true;
}
const WCHAR* DELIM = L"-:-";

bool CWfpNET::Disks_get(WinInfo* _out) //Enumerate Disks
{
	const int ENTRY_SIZE   = 3; // Drive letter, colon, NULL
	LPWSTR pBuf            = NULL;
	DWORD dwLevel          = 0; // level must be zero
	DWORD dwPrefMaxLen     = (DWORD)-1;
	DWORD dwEntriesRead    = 0;
	DWORD dwTotalEntries   = 0;
	NET_API_STATUS nStatus = NULL;

	// The NetServerDiskEnum function retrieves a list of disk drives on a server.
	// Only members of the Administrators or Account Operators local group can
	// successfully execute the NetServerDiskEnum function on a remote computer.
	nStatus = NetServerDiskEnum(wfNode.wsComputer, dwLevel, (LPBYTE *) &pBuf,
        dwPrefMaxLen, &dwEntriesRead, &dwTotalEntries, NULL);
   
	if(nStatus != NERR_Success)
	{
		return false;
	}
	LPWSTR pTmpBuf;
	if((pTmpBuf = pBuf) != NULL)
	{
		DWORD i;
		DWORD dwTotalCount = 0;
       
		// Loop through the entries.
       
		for(i = 0; i < dwEntriesRead; i++)
		{
			//assert(pTmpBuf != NULL);
			if(pTmpBuf == NULL)
				break;
            
			_out->plstDisks->Add(pTmpBuf);			

			pTmpBuf += ENTRY_SIZE;
			dwTotalCount++;
		}
		NetApiBufferFree(pBuf);
	}

	return true;
}

bool CWfpNET::EventLog_get(WinInfo* _out)
{
	HANDLE h;
    EVENTLOGRECORD *pevlr; 
    BYTE bBuffer[8192]; 
    DWORD dwRead, dwNeeded, dwThisRecord = 0;
	char *cp;
	char *pSourceName;
	char *pComputerName;

    if((h = OpenEventLog((LPCWSTR)wfNode.szComputer, _T("Security"))) == NULL)
	{
		return false;
	}
    
    pevlr = (EVENTLOGRECORD *)&bBuffer; 
 
    // Opening the event log positions the file pointer for this 
    // handle at the beginning of the log. Read the records 
    // sequentially until there are no more. 
	_out->plstEventLog = new List<events_>();

    while(ReadEventLog(h,                // event log handle 
		EVENTLOG_FORWARDS_READ |  // reads forward 
        EVENTLOG_SEQUENTIAL_READ, // sequential read 
        0,			  // ignored for sequential reads 
        pevlr,        // pointer to buffer 
        8192,		  // size of buffer 
        &dwRead,      // number of bytes read 
        &dwNeeded))   // bytes in next record 
    {
        while(dwRead > 0) 
        { 
            // Print the event identifier, type, and source name. 
            // The source name is just past the end of the 
            // formal structure. 
			events_ ne;
			dwThisRecord++;
			ne.iID = pevlr->EventID;

			ne.dtTimeGen = DateTime(pevlr->TimeGenerated);
			ne.dtTimeWritten = DateTime(pevlr->TimeWritten);

			switch(pevlr->EventType)
			{
				case EVENTLOG_ERROR_TYPE: ne.strType = (L"Error Event"); break;
				case EVENTLOG_WARNING_TYPE: ne.strType = (L"Warning Event"); break;
				case EVENTLOG_INFORMATION_TYPE: ne.strType = (L"Information Event"); break;
				case EVENTLOG_AUDIT_SUCCESS: ne.strType = (L"Success Audit Event"); break;
				case EVENTLOG_AUDIT_FAILURE: ne.strType = (L"Failure Audit Event"); break;
				default: ne.strType = (L"Unknown"); break;
			}
           
			cp = (char *)pevlr;
			cp += sizeof(EVENTLOGRECORD);

			pSourceName = cp;
			cp += strlen(cp)+1;

			pComputerName = cp;
			cp += strlen(cp)+1;

			ne.strSourceName = pSourceName;
			ne.strComputerName = pComputerName;
            dwRead -= pevlr->Length; 
            pevlr = (EVENTLOGRECORD *)((LPBYTE) pevlr + pevlr->Length);
			_out->plstEventLog->Add(ne);
        } 
		
        pevlr = (EVENTLOGRECORD *) &bBuffer; 
	}
	
	return true;
}

bool CWfpNET::GroupMembers_get(LPWSTR Group,groups_* g, int iLen) // Enumerate Group Memberships
{
	NET_API_STATUS nStatus       = NULL;
	LPGROUP_USERS_INFO_0 pBuf    = NULL,
		pTmpBuf                  = NULL;
	DWORD i                      = 0,
		entriesread              = 0,
		totalentries			 = 0;
	
	do
	{
		WCHAR* strGr = (WCHAR*)malloc(sizeof(WCHAR)*iLen);
		wcscpy(strGr,Group);
		WCHAR* server = (WCHAR*)malloc(sizeof(WCHAR)*17);
		wcscpy(server,L"\\\\");
		wcscat(server,wfNode.wsComputer);

		nStatus = NetGroupGetUsers(server,strGr,1,(LPBYTE *) &pBuf,
			MAX_PREFERRED_LENGTH, &entriesread, &totalentries, NULL);
	
		if(nStatus != NERR_Success && nStatus != ERROR_MORE_DATA)
		{
			return false;
		}
		if((pTmpBuf = pBuf) != NULL)
		{
			for(i = 0; i < entriesread; i++)
			{
				if (pTmpBuf == NULL)
					break;
				try{
					g->plstUsers->Add(new String(pTmpBuf->grui0_name));
				}catch(...){}
				pTmpBuf++;
			}
		}
		if(pBuf != NULL)
		{
			NetApiBufferFree(pBuf);
			pBuf = NULL;
		}

	} while (nStatus==ERROR_MORE_DATA);
	
	return true;
}
bool CWfpNET::IPC_Session_Connect(void) // Establish NULL IPC$ Sessions
{
	NETRESOURCE nr;
	DWORD nStatus = 0;
	TCHAR RemoteResource[23]; // UNC Name length (17) + \\IPC$\0 (6) = 23 
	
	wcscpy(RemoteResource,L"\\\\");
	wcscat(RemoteResource, wfNode.wsComputer);
	wcscat(RemoteResource, L"\\IPC$");
	
	nr.dwType				= RESOURCETYPE_ANY;
	nr.lpLocalName			= NULL;
	nr.lpProvider			= NULL;
	nr.lpRemoteName			= (LPWSTR)RemoteResource;

	// First attempt: Use currently logged in user
	nStatus = WNetAddConnection3(NULL,
			&nr,
			wfNode.wsPass, // username
			wfNode.wsUsr, // password
			0);

	if(nStatus == NO_ERROR)
		return(true);
	
	LPSTR LaUsr = new char[40];
	LPSTR LaPass = new char[40];
	WideCharToMultiByte(CP_ACP,MB_PRECOMPOSED,wfNode.wsUsr,-1,LaUsr,-1,NULL,NULL);
	WideCharToMultiByte(CP_ACP,MB_PRECOMPOSED,wfNode.wsPass,-1,LaPass,-1,NULL,NULL);
	nStatus = WNetAddConnection3(NULL,
		&nr,
		(LPTSTR) LaUsr,
		(LPTSTR) LaPass,
		0);
	
	if(nStatus != NO_ERROR)
	{
		return (false);
	}
	return (true);
}

bool CWfpNET::IPC_Session_Disconnect(void) // Disconnect NULL IPC$ Sessions
{
	DWORD nStatus = 0;
	nStatus = WNetCancelConnection2((LPCWSTR)wfNode.szComputer,0,1);

	if(nStatus == NO_ERROR)
		return true;
	else
		return false;
}

bool CWfpNET::LocalGroups_get(user_* _out) // Enumerate Groups
{

	LPLOCALGROUP_USERS_INFO_0 pBuf = NULL;
   DWORD dwLevel = 0;
   DWORD dwFlags = LG_INCLUDE_INDIRECT ;
   DWORD dwPrefMaxLen = MAX_PREFERRED_LENGTH;
   DWORD dwEntriesRead = 0;
   DWORD dwTotalEntries = 0;
   NET_API_STATUS nStatus;


	WCHAR* szStr = NULL;
    szStr =(WCHAR *)(void *)Marshal::StringToCoTaskMemUni( _out->strName );
   //
   // Call the NetUserGetLocalGroups function 
   //  specifying information level 0.
   //
   //  The LG_INCLUDE_INDIRECT flag specifies that the 
   //   function should also return the names of the local 
   //   groups in which the user is indirectly a member.
   //
   nStatus = NetUserGetLocalGroups(wfNode.wsComputer,
                                   szStr,
                                   dwLevel,
                                   dwFlags,
                                   (LPBYTE *) &pBuf,
                                   dwPrefMaxLen,
                                   &dwEntriesRead,
                                   &dwTotalEntries);
   //
   // If the call succeeds,
   //
   if (nStatus != NERR_Success)
   {
	   return false;
   }
      LPLOCALGROUP_USERS_INFO_0 pTmpBuf;
      DWORD i;
      DWORD dwTotalCount = 0;
	  if(_out->plstGroups == NULL)
	  	_out->plstGroups = new List<String*>();
      if ((pTmpBuf = pBuf) != NULL)
      {
         for (i = 0; i < dwEntriesRead; i++)
         {
            _out->plstGroups->Add(new String(pTmpBuf->lgrui0_name));
            pTmpBuf++;
            dwTotalCount++;
         }
      }
   //
   // Free the allocated memory.
   //
   if (pBuf != NULL)
      NetApiBufferFree(pBuf);

   return true;
}

bool CWfpNET::Time_get(WinInfo* _out) // Obtain Date and Time
{
	LPTIME_OF_DAY_INFO pTOD = NULL;
	NET_API_STATUS nStatus = NULL;
	DWORD mindiff = 0, hourdiff = 0;
	
	// The NetRemoteTOD function returns the time of day information from
	// a specified server.
	// No special group membership is required to successfully execute the
	// NetRemoteTOD function.

	nStatus = NetRemoteTOD(wfNode.wsComputer, (LPBYTE *)&pTOD);
	
	if(nStatus != NERR_Success)
	{
		return false;
	}
		if(pTOD != NULL)
		{
			_out->dtDate = DateTime(pTOD->tod_year, pTOD->tod_month, pTOD->tod_day, pTOD->tod_hours, pTOD->tod_mins, pTOD->tod_secs);
		}

	if(pTOD != NULL)
      NetApiBufferFree(pTOD);

	return true;
}

bool CWfpNET::Transports_get(WinInfo* _out)
{
	NET_API_STATUS nStatus			  = NULL;
	LPSERVER_TRANSPORT_INFO_1 pBuf	  = NULL,
		pTmpBuf						  = NULL;
	DWORD dwEntriesRead				  = 0;
	DWORD dwTotalEntries			  = 0;
	DWORD dwResumeHandle			  = 0;
	DWORD i	= 0;
	int j = 0;

	// The NetServerTransportEnum function supplies information about
	// transport protocols that are managed by the server.
	// No special group membership is required to successfully execute
	// the NetServerTransportEnum function.
		
	do 
	{
		nStatus = NetServerTransportEnum(wfNode.wsComputer, 1, (LPBYTE *)&pBuf,
			MAX_PREFERRED_LENGTH, &dwEntriesRead, &dwTotalEntries, &dwResumeHandle);
		// If the call succeeds,
		if((nStatus != NERR_Success) && (nStatus != ERROR_MORE_DATA))
		{
			return false;
		}
		if((pTmpBuf = pBuf) != NULL)
		{
			// Loop through the entries;
			//  process access errors.
	          		
			for(i = 0; i < dwEntriesRead; i++)
			{
				if(pTmpBuf == NULL)
					return false;

				NetDev_ dev = NetDev_();
				dev.strAddress = new String((LPSTR)pTmpBuf->svti1_transportaddress);
				dev.strDomain = new String(pTmpBuf->svti1_domain);
				dev.strNetAddress= new String(pTmpBuf->svti1_networkaddress);
				dev.strName = new String(pTmpBuf->svti1_transportname);
				_out->plstDevs->Add(dev);
				
					/*if(options.optionmacaddress)
						if(wcscmp(pTmpBuf->svti1_transportname,L"\\Device\\NetbiosSmb") != 0)  
						{
							/*tmp.Format("%S", pTmpBuf->svti1_networkaddress);
							if(MACAddress.GetSize() > 0) 
							{
								bool exists = false;
								for(j = 0; j <= MACAddress.GetUpperBound(); j++)
								{	
									if(strcmp(tmp, MACAddress[j]) == 0)
										exists = true;
								}
								if(!exists)
									MACAddress.Add(tmp);
							}	
							else
								MACAddress.Add(tmp);
						}*/
				pTmpBuf++;
			}
			
			if(pBuf != NULL)
			{
				NetApiBufferFree(pBuf);
				pBuf = NULL;
			}
		}
	}while(nStatus == ERROR_MORE_DATA);
		return true;
}

bool CWfpNET::ADNetBIOSShares_get(WinInfo* _out)
{
	// FIXME: Currently ADSI_Shares only returns 1 share.
	IADsFileShare *pShares = NULL;	
	HRESULT hr = 0;//0;
	LPWSTR  adsPath = new WCHAR[37]; 
	IADsContainer *pCont = NULL;
	IADsCollection *pColl = NULL;
	IUnknown *pUnk = NULL;
	IEnumVARIANT *pEnum = NULL;
	ULONG lFetch = 0;
	IDispatch *pDisp = NULL;
	BSTR bstr;
	VARIANT var;
	
	wcscpy(adsPath,L"WinNT://");
	wcscat(adsPath,wfNode.wsComputer);
	wcscat(adsPath,L"/LanmanServer");
	
	hr = ADsGetObject(adsPath, IID_IADsContainer, (void**)&pCont);

	if(FAILED(hr))
	{
		delete [] adsPath;
		return false;
	}

	
	hr=ADsBuildEnumerator(pCont, &pEnum);
	while(hr == S_OK)
	{
		hr = ADsEnumerateNext(pEnum, 1, &var, &lFetch);
		if (lFetch == 1)    
		{
			pDisp = V_DISPATCH(&var);
			pDisp->QueryInterface(IID_IADsFileShare , (void**)&pShares);
			pShares->get_HostComputer(&bstr);
			NetBIOS_ nb = NetBIOS_();
			nb.strHost = bstr;

			SysFreeString(bstr);

			pShares->get_Name(&bstr);
			nb.strName = bstr;
			 
			SysFreeString(bstr);

			pShares->get_Description(&bstr);
			nb.strComment = bstr;
			SysFreeString(bstr);
			pShares->Release();
			_out->plstNbs->Add(nb);
		}
		VariantClear(&var);
		pDisp=NULL;
		//hr = pEnum->Next(1, &var, &lFetch);
	};
	
	delete [] adsPath;
	
	if(pDisp)
		pDisp->Release();
	if(pEnum)
		pEnum->Release();
	if(pUnk)
		pUnk->Release();
	if(pColl)
		pColl->Release();
	return true;
}

bool CWfpNET::ADGroups_get(WinInfo* _out)
{
	if(!ADServices_Users_Groups(ENUM_GROUPS,_out))
		return false;
	else
		return true;
}

bool CWfpNET::ADOperatingSystem_get(WinInfo* _out)
{
	IADsComputer *pComp = NULL;
	LPWSTR  pwszBindingString = new WCHAR[100]; 
	HRESULT hr = 0;//0;
	BSTR bstr;
	
	wcscpy(pwszBindingString,L"WinNT://");
	wcscat(pwszBindingString,wfNode.wsComputer);
	wcscat(pwszBindingString,L",computer");

	hr = ADsGetObject(pwszBindingString,IID_IADsComputer,(void**)&pComp);
	if(FAILED(hr))
	{
		delete[] pwszBindingString;
		return false;
	}
			
	if(!FAILED(pComp->get_OperatingSystem(&bstr))){
		_out->os.strADName = new String(bstr);
	}
	SysFreeString(bstr);

	if(!FAILED(	pComp->get_Division(&bstr)))
		_out->os.strADOrg = new String(bstr);
	
	SysFreeString(bstr);
			
	if(!FAILED(pComp->get_Processor(&bstr)))
		_out->os.strADProcessor = new String(bstr);
	
	SysFreeString(bstr);

	if(!FAILED(pComp->get_ProcessorCount(&bstr)))
		_out->os.strADProcessor = new String(bstr);
	SysFreeString(bstr);

	if(!FAILED(pComp->get_GUID(&bstr)))
		_out->os.strADguid = new String(bstr);

	SysFreeString(bstr);
			
	if(!FAILED(pComp->get_Owner(&bstr)))
		_out->os.strADOwner = new String(bstr);
	SysFreeString(bstr);

	pComp->Release();
	delete[] pwszBindingString;
	return true;
}

bool CWfpNET::ADServices_get(WinInfo* _out)
{
	if(!ADServices_Users_Groups(ENUM_GROUPS, _out))
		return false;
	else
		return true;
}

bool CWfpNET::ADSessions_get(WinInfo* _out) {
	IADsFileServiceOperations *pFso = NULL;	
	IADsSession *pSes				= NULL;
	LONG seconds					= 0;
	HRESULT hr						= 0;//0;
	LPWSTR  adsPath = new WCHAR[37]; 
	LPWKSTA_INFO_102 pwBuf			= NULL;
	IADsCollection *pColl			= NULL;
	IUnknown *pUnk					= NULL;
	IEnumVARIANT *pEnum				= NULL;
	ULONG lFetch					= 0;
	IDispatch *pDisp				= NULL;
	BSTR bstr;
	VARIANT var;
//CString tmp, session;
		
	_snwprintf_s(adsPath, 36, 36, L"WinNT://%S/LanmanServer", wfNode.szIPAddr);
	hr = ADsGetObject(adsPath, IID_IADsFileServiceOperations, (void**)&pFso);
	if(FAILED(hr))
	{
		delete [] adsPath;
		return false;
	}
	
	hr = pFso->Sessions(&pColl);
	pFso->Release();

	// Now to enumerate sessions. 
	hr = pColl->get__NewEnum(&pUnk);
	if (FAILED(hr))
	{
		delete [] adsPath;
		return false;
	}
			
	pColl->Release();

	hr = pUnk->QueryInterface(IID_IEnumVARIANT,(void**)&pEnum);
	if (FAILED(hr))
	{
		delete [] adsPath;
		return false;
	}
	pUnk->Release();

	// Now Enumerate

	VariantInit(&var);
	hr = pEnum->Next(1, &var, &lFetch);
	while(hr == S_OK)
	{
		if (lFetch == 1)    
		{
			Session_ s = Session_();
			pDisp = V_DISPATCH(&var);
			pDisp->QueryInterface(IID_IADsSession, (void**)&pSes);
			pSes->get_Computer(&bstr);
			s.strConnection_Machine_Name = bstr;
			   
			SysFreeString(bstr);
			pSes->get_User(&bstr);
			s.strUsername = bstr;

			SysFreeString(bstr);

			pSes->get_ConnectTime(&seconds);
			s.iSecConnected = seconds;

			pSes->get_IdleTime(&seconds);
			s.iSecIdle = seconds;
			pSes->Release();
		}
		VariantClear(&var);
		pDisp=NULL;
		hr = pEnum->Next(1, &var, &lFetch);
	};
	
	delete [] adsPath;
	hr = pEnum->Release();
	return true;
}

bool CWfpNET::ADUsers_get(WinInfo* _out)
{
	if(!ADServices_Users_Groups(ENUM_USERS,_out))
		return false;
	else
		return true;
}

bool CWfpNET::ADServices_Users_Groups(int enumtype, WinInfo* _out)
{
	IADsContainer * pIADsCont = NULL;
	LPWSTR pwszBindingString = new WCHAR[33];
	LPWSTR pwszFilter = NULL;
	IDispatch *pDispatch = NULL;
	IADs *pIADs	= NULL;
	VARIANT vFilter, Variant;
	unsigned long hr = 0;
	IEnumVARIANT *pEnumVariant = NULL; // Ptr to the IEnumVariant Interface
	ULONG ulElementsFetched = 0;    // Number of elements fetched
	BSTR bsResult, bsResult2, bsResult3;
	_snwprintf_s(pwszBindingString, 32, 32, L"WinNT://%S,computer", wfNode.szIPAddr);
	hr = ADsGetObject(pwszBindingString, IID_IADsContainer,(void **)&pIADsCont);

	if(SUCCEEDED(hr))
	{
		VariantInit(&vFilter);
		// Build a Variant of array type, using the filter passed
		hr = ADsBuildVarArrayStr(&pwszFilter, 1, &vFilter);		
		if (SUCCEEDED(hr))
		{
			// Set the filter for the results of the Enum
			hr = pIADsCont->put_Filter(vFilter);
			if (SUCCEEDED(hr))
			{
				// Builds an enumerator interface- this will be used 
				// to enumerate the objects contained in the IADsContainer 
				hr = ADsBuildEnumerator(pIADsCont,&pEnumVariant);
				// While no errors- Loop through and print the data
				while (SUCCEEDED(hr) && hr != S_FALSE) 
				{
					// Object comes back as a VARIANT holding an IDispatch *
					hr = ADsEnumerateNext(pEnumVariant,1,&Variant,&ulElementsFetched);
					if(hr != S_FALSE) 
					{ 
						pDispatch = Variant.pdispVal;
						// QI the Variant's IDispatch * for the IADs interface
						hr = pDispatch->QueryInterface(IID_IADs,(VOID **) &pIADs) ;
 						if (SUCCEEDED(hr))
						{
							// Print some information about the object
							pIADs->get_Class(&bsResult);
							pIADs->get_Name(&bsResult2);
						
							pIADs->get_GUID(&bsResult3);
							if(wcswcs(bsResult,L"Service") != 0)
							{
								Service_ s = Service_();
								s.strDisplayName = bsResult2;
								s.strServiceName = bsResult3;
								_out->plstServ->Add(s);
							}
								
							if(wcswcs(bsResult,L"Group") != 0)
							{
								for(int i = 0; i< _out->plstGroups->Count; i++)
								{
									groups_ *u = _out->plstGroups->Item[i];
									if(u->strName->Contains(bsResult2))
									{
										((groups_*)_out->plstGroups->Item[i])->strGUID = bsResult3;
										goto out;
									}
								}
								groups_ *g = new groups_();
								g->strName = bsResult2;
								g->strGUID = bsResult3;
								_out->plstGroups->Add(g);
							}
							
							if(wcswcs(bsResult,L"User") != 0)
							{
								for(int i = 0; i< _out->plstUsers->Count; i++)
								{
									user_ u = _out->plstUsers->Item[i];
									if(u.strName->Contains(bsResult2))
									{
										_out->plstUsers->RemoveAt(i);
										u.strLonguid = bsResult3;
										_out->plstUsers->Add(u);
										goto out;
									}
								}
								user_ u = user_();
								u.strName = bsResult2;
								u.strComment = bsResult3;
								_out->plstUsers->Add(u);
							}
						out:
							SysFreeString(bsResult);
							SysFreeString(bsResult2);
							SysFreeString(bsResult3);
							pIADs->Release();
							pIADs = NULL;
						}
					}
				}
 
				// Since the hr from iteration was lost, free 
				// the interface if the ptr is != NULL
				if(pEnumVariant)
				{
					pEnumVariant->Release();
					pEnumVariant = NULL;
				}
				VariantClear(&Variant);
			}
		}
		VariantClear(&vFilter);
	}

	delete[] pwszBindingString;
	return true;
}



}