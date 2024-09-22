/** @file CMySocks.h ver 2009.09.10
*	Clase para el manejo de Sockets TCP
*
*	Author: Iker@cuteam.org
*	Revisado y Documentado: Iker@cuteam.org
*
*	Está prohibida la copia o redistribucion de cualquiera de los archivos
*	o de partes de los mismos.
*/
#include "CMySocks.h"


CMySock::CMySock()
{
#ifdef WIN32
	WSADATA wsa;

	WSAStartup( MAKEWORD( 2, 2), &wsa);
#endif

	m_iSock			= 0;
	m_bListen		= false;
	m_iLocalPort	= 0;
	m_iRemotePort	= 0;
	m_ulLocalAddr	= INADDR_NONE;
	m_ulRemoteAddr	= INADDR_NONE;
}

CMySock::CMySock( int iSock, struct sockaddr_in *pSin, bool bListen)
{
#ifdef WIN32
	WSADATA wsa;

	WSAStartup( MAKEWORD( 2, 2), &wsa);
#endif

	if (iSock <= 0 || pSin->sin_addr.s_addr == INADDR_NONE) {
		m_iSock			= 0;
		m_bListen		= false;
		m_iLocalPort	= 0;
		m_iRemotePort	= 0;
		m_ulLocalAddr	= INADDR_NONE;
		m_ulRemoteAddr	= INADDR_NONE;
		return;
	}

	m_iSock			= iSock;
	m_bListen		= bListen;
	
	if (GetProperties() == false)
		ShutDown();
}

CMySock::~CMySock()
{
	ShutDown();
}

int CMySock::GetErrCode( void ) const
{
#ifdef WIN32
	return WSAGetLastError();
#else
	return errno;
#endif
}

bool CMySock::IsListening( void ) const
{
	return m_bListen;
}

bool CMySock::IsActive( void )
{
	if (WaitForEvent( FD_CLOSE, 0) != 0)
		return false;

	return true;
}

bool CMySock::GetProperties( void )
{
	int iStLen = sizeof( sockaddr_in );
	struct sockaddr_in siAux;

	if (m_bListen == true)
		return true;

	if (getsockname( m_iSock, (struct sockaddr *)&siAux, &iStLen) != 0)
		return false;

	m_ulLocalAddr	= siAux.sin_addr.s_addr;
	m_iLocalPort	= ntohs( siAux.sin_port );

	iStLen = sizeof( sockaddr_in );

	if (getpeername( m_iSock, (struct sockaddr *)&siAux, &iStLen) != 0)
		return false;

	m_ulRemoteAddr	= siAux.sin_addr.s_addr;
	m_iRemotePort	= ntohs( siAux.sin_port );

	return true;
}

int CMySock::WaitForEvent( long lEvent, u_long ulTime)
{
#ifdef WIN32
	DWORD dwBlock = 0, dwRet = 0;
	WSAEVENT wsEvent = NULL;
	int iRet = 0;

	wsEvent = WSACreateEvent();

	if (wsEvent == WSA_INVALID_EVENT)
		return -1;

	iRet = WSAEventSelect( m_iSock, wsEvent, lEvent);
	
	if (iRet == SOCKET_ERROR) {
		WSACloseEvent( wsEvent );
		return -1;
	}

	dwRet = WSAWaitForMultipleEvents( 1, &wsEvent, TRUE, ulTime, FALSE);

	WSAEventSelect( m_iSock, wsEvent, 0);
	WSACloseEvent( wsEvent );

	ioctlsocket( m_iSock, FIONBIO, &dwBlock);
	
	if (dwRet == WSA_WAIT_TIMEOUT)
		return 0;

	if (dwRet == WSA_WAIT_EVENT_0)
		return 1;

	return -1;
#else
	struct timeval timeout;
	fd_set fdAsync;
	int iAux = 0;

	FD_ZERO( &fdAsync );
	FD_SET( m_iSock, &fdAsync );

	timeout.tv_sec		= !ulTime ? 0 : (ulTime / 1000);
	timeout.tv_usec		= !ulTime ? 500 : ((ulTime % 1000) * 100);
	
	if (lEvent == FD_CLOSE) {
		if (select( m_iSock, &rfd, NULL, NULL, &timeout) == -1)
			return false;
		
		if (FD_ISSET( m_iSock, &rfd) == 0)
			return false;
		
		ioctl( m_iSock, FIONREAD, &iAux);
		
		return iAux == 0 ? false : true;
	} else if (lEvent == FD_READ || lEvent == FD_ACCEPT) {
		if (select( m_iSock, &fdAsync, NULL, NULL, &timeout) == -1)
			return false;
	} else if (lEvent == FD_WRITE) {
		if (select( m_iSock, NULL, &fdAsync, NULL, &timeout) == -1)
			return false;
	}

	return FD_ISSET( m_iSock, &fdAsync ) == 0 ? false : true;
#endif
}

int CMySock::IsReadyForRecv( u_long ulTime )
{
	return WaitForEvent( FD_READ, ulTime);
}

int CMySock::IsReadyForSend( u_long ulTime )
{
	return WaitForEvent( FD_WRITE, ulTime);
}

u_long CMySock::GetLocalAddr( void )
{
	return m_ulLocalAddr;
}

u_long CMySock::GetRemoteAddr( void )
{
	return m_ulRemoteAddr;
}

int CMySock::GetLocalPort( void )
{
	return m_iLocalPort;
}

int CMySock::GetRemotePort( void )
{
	return m_iRemotePort;
}

int CMySock::GetSocket( void )
{
	return m_iSock;
}

int CMySock::Send( const void *lpvData, u_int uiSize)
{
	const char *lpszPtr = (const char *)lpvData;
	int iLen = 0, iRes = 0;
	u_int uiSend = 0;

	if (lpszPtr == NULL || uiSize == 0)
		return 0;

	do {
		iLen = (uiSize - uiSend) > CMS_MTU ? CMS_MTU : (uiSize - uiSend);

		iRes = send( m_iSock, lpszPtr + uiSend, iLen, 0);

		if (iRes <= 0)
			return iRes;
		
		uiSend += iRes;

	} while(uiSend != uiSize);

	return uiSend;
}

int CMySock::Recv( void *lpvData, u_int uiSize)
{
	char *lpszPtr = (char *)lpvData;
	int iRes = 0;

	if (lpszPtr == NULL || uiSize == 0)
		return 0;

	iRes = recv( m_iSock, lpszPtr, (int)uiSize, 0);

	return iRes;
}

int CMySock::Recv( void *lpvData, u_int uiSize, const byte *pbtStr, u_int uiStrSize, u_long ulTime)
{
	char *lpszPtr = (char *)lpvData;
	u_int uiAt = 0, uiBytes = 0;
	u_long ulElapsed = 0;
	timeval tvInit;
	char cTmp = 0;
	int iRes = 0;

	if (lpszPtr == NULL || uiSize == 0)
		return 0;

	if (pbtStr == NULL || uiStrSize == 0 || uiStrSize >= uiSize)
		return 0;

	gettimeofday( &tvInit, NULL);

	do {
		if (ulTime != INFINITE) {
			ulElapsed = GetAndDiffTime( &tvInit );
			
			iRes = IsReadyForRecv( (ulTime - ulElapsed) );

			if (iRes == 0)
				continue;

			if (iRes == -1)
				return -1;
		}

		iRes = recv( m_iSock, &cTmp, 1, 0);
		
		if (iRes <= 0)
			return iRes;

		if (uiBytes > uiSize)
			return 0;

		lpszPtr[uiBytes] = cTmp;

		++uiBytes;
		
		if (cTmp != pbtStr[uiAt]) {
			uiAt = 0;
			continue;
		}

		++uiAt;

		if (uiAt == uiStrSize)
			return uiBytes;
		
	} while(ulElapsed < ulTime);

	return 0;
}

bool CMySock::ConnectTo( const char *pszHost, int iPort, u_long ulTime)
{
	u_long ulAddr;

	if (pszHost == NULL)
		return false;

	ulAddr = GetInetAddr( pszHost );

	if (ulAddr == INADDR_NONE)
		return false;

	return ConnectTo( ulAddr, iPort, ulTime);
}

bool CMySock::ConnectTo( u_long ulAddr, int iPort, u_long ulTime)
{
	struct sockaddr_in siSin;

	if (ulAddr == INADDR_NONE || iPort <=0 || iPort > 65535)
		return false;

	m_iSock = socket( AF_INET, SOCK_STREAM, 0);

	if (m_iSock == INVALID_SOCKET) {
		m_iSock = 0;
		return false;
	}

	siSin.sin_family		= AF_INET;
	siSin.sin_port			= htons( iPort );
	siSin.sin_addr.s_addr	= ulAddr;

	if (ConnectTo( &siSin, ulTime) == false) {
		ShutDown();
		return false;
	}

	m_bListen	= false;

	if (GetProperties() == false) {
		ShutDown();
		return false;
	}

	return true;
}

bool CMySock::ConnectTo( struct sockaddr_in *pSin, u_long ulTime)
{
	if (ulTime == INFINITE) {
		if (connect( m_iSock, ( struct sockaddr * )pSin, sizeof( struct sockaddr_in )) == SOCKET_ERROR)
			return false;
		return true;
	}
#ifdef WIN32
	int iStLen = sizeof( sockaddr_in );
	struct sockaddr_in siTmp;
	WSAEVENT wsEvent;
	DWORD dwRet = 0;
	int iRes = 0;

	wsEvent = WSACreateEvent();

	if (wsEvent == WSA_INVALID_EVENT)
		return false;

	iRes = WSAEventSelect( m_iSock, wsEvent, FD_CONNECT);

	if (iRes == SOCKET_ERROR) {
		WSACloseEvent( wsEvent );
		return false;
	}

	connect( m_iSock, ( struct sockaddr* )pSin, sizeof( struct sockaddr_in ));

	iRes = WSAWaitForMultipleEvents( 1, &wsEvent, TRUE, ulTime, FALSE);

	WSAEventSelect( m_iSock, wsEvent, 0);
	WSACloseEvent( wsEvent );

	if (iRes != 0)
		return false;
	
	if (dwRet == WSA_WAIT_TIMEOUT)
		return false;

	if (dwRet == WSA_WAIT_EVENT_0)
		return true;

	if (getpeername( m_iSock, ( struct sockaddr * )&siTmp, &iStLen) != 0)
		return false;

	return true;
#else
	int flags = 0, error = 0, ret = 0;
	socklen_t len = sizeof( error );
    fd_set rset, wset;
    timeval timeout;
    
    timeout.tv_sec		= !ulTime ? 0 : (ulTime / 1000);
	timeout.tv_usec		= !ulTime ? 500 : ((ulTime % 1000) * 100);
    
    //clear out descriptor sets for select
    //add socket to the descriptor sets
    FD_ZERO( &rset );
    FD_SET( m_iSock, &rset);

    wset = rset;    //structure assignment ok
    
    //set socket nonblocking flag
	flags = fcntl( m_iSock, F_GETFL, 0);

    if (flags < 0)
        return false;
    
    if (fcntl( m_iSock, F_SETFL, flags | O_NONBLOCK) < 0)
        return false;
    
    //initiate non-blocking connect
	ret = connect( m_iSock, ( struct sockaddr * )pSin, sizeof( struct sockaddr_in ));

    if (ret < 0 ) {
        if (errno != EINPROGRESS)
            return false;
	}

    if (ret == 0)    //then connect succeeded right away
        goto done;

	ret = select( m_iSock + 1, &rset, &wset, NULL, &timeout)
    
    //we are waiting for connect to complete now
    if (ret < 0)
        return false;
    else if(ret == 0){   //we had a timeout
        errno = ETIMEDOUT;
        return false;
    }

    //we had a positivite return so a descriptor is ready
    if (FD_ISSET( m_iSock, &rset) == 0 && FD_ISSET( m_iSock, &wset) == 0)
		return false;
	
	if (getsockopt( m_iSock, SOL_SOCKET, SO_ERROR, &error, &len) < 0)
        return false;

    if (error != 0){  //check if we had a socket error
        errno = error;
        return false;
    }

done:
    //put socket back in blocking mode
    if (fcntl( m_iSock, F_SETFL, flags) < 0)
        return false;

    return true;
#endif
}
 
bool CMySock::BindPort( int iPort, bool bAllInterfaces, const char *pszInterface)
{
	u_long ulAddr = INADDR_ANY;
	struct sockaddr_in siSin;
	int iRes = 0;

	if (iPort <= 0 || iPort > 65535 || (bAllInterfaces == false && pszInterface == NULL))
		return false;

	if (bAllInterfaces == false) {
		ulAddr = GetInetAddr( pszInterface );
		if (ulAddr == INADDR_NONE)
			return false;
	}

	m_iSock = socket( AF_INET, SOCK_STREAM, 0);

	if (m_iSock == INVALID_SOCKET)
		return false;

	memset( &siSin, 0x00, sizeof( struct sockaddr_in ));

	siSin.sin_family		= AF_INET;
	siSin.sin_port			= htons( iPort );
	siSin.sin_addr.s_addr	= ulAddr;

	iRes = bind( m_iSock, (struct sockaddr *)&siSin, sizeof( sockaddr_in ));

	if (iRes < 0) {
		ShutDown();
		return false;
	}

	iRes = listen( m_iSock, 5);

	if (iRes == SOCKET_ERROR) {
		ShutDown();
		return false;
	}

	m_bListen		= true;
	m_iLocalPort	= iPort;
	m_ulLocalAddr	= siSin.sin_addr.s_addr;

	if (GetProperties() == false) {
		ShutDown();
		return false;
	}

	return true;
}

CMySock *CMySock::Accept( u_long ulTime )
{
	struct sockaddr_in siSin;
	CMySock *pcmsRet = NULL;
	int iNewSock = 0;

	if (IsListening() == false)
		return NULL;

	iNewSock = Accept( &siSin, ulTime);

	if (iNewSock == SOCKET_ERROR)
		return NULL;

	pcmsRet = new CMySock( iNewSock, &siSin, false);

	if (pcmsRet == NULL) {
		CLOSESOCKET( iNewSock );
		return NULL;
	}

	return pcmsRet;
}

int CMySock::Accept( struct sockaddr_in *psiSin, u_long ulTime)
{
	int iStLen = sizeof( sockaddr_in );
	int iRes = 0;

	if (!psiSin)
		return INVALID_SOCKET;

	memset( psiSin, 0x00, sizeof( struct sockaddr_in ));

	if (ulTime == INFINITE)
		return accept( m_iSock, ( struct sockaddr * )psiSin, &iStLen);

	iRes = WaitForEvent( FD_ACCEPT, ulTime);

	if (iRes != 1)
		return SOCKET_ERROR;

	return accept( m_iSock, ( struct sockaddr * )psiSin, &iStLen);
}

void CMySock::ShutDown( void )
{
	if (m_iSock > 0)
		CLOSESOCKET( m_iSock );

	m_iSock			= 0;
	m_bListen		= false;
	m_iLocalPort	= 0;
	m_iRemotePort	= 0;
	m_ulLocalAddr	= INADDR_NONE;
	m_ulRemoteAddr	= INADDR_NONE;
}

u_long CMySock::GetInetAddr( const char *pszHost )
{
	struct hostent *he;
	WSADATA wsa;

	WSAStartup( MAKEWORD( 2, 2), &wsa);

	if (pszHost == NULL)
		return INADDR_NONE;

	he = gethostbyname( pszHost );

	if (he == NULL)
		return inet_addr( pszHost );

	return (( struct in_addr* )( he->h_addr ))->s_addr;
}