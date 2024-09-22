#include "CBouncer.h"

CBouncer::CBouncer()
{
	m_State				= CTP_IDLE;
	m_iLocalPort		= -1;
	m_iRemotePort		= -1;
	m_ulRemoteAddr		= INADDR_NONE;
	m_iMaxConnections	= 0;
	
	m_MainThread		= nullptr;
}

CBouncer::~CBouncer()
{
	FreeThreads();
}

void CBouncer::FreeThreads( void )
{
	ThreadStruct *pAux = NULL;
	int iAt = 0;

	m_State	= CTP_IDLE;

	Sleep( 1000 );

	MyFinishThread( &m_MainThread );

	for ( ; iAt < m_Threads.GetSize(); ++iAt) {
		pAux = m_Threads[iAt];

		if (pAux == nullptr)
			continue;

		MyFinishThread( &pAux->ThreadHandle );

		delete pAux;

		pAux = nullptr;
	}

	m_Threads.RemoveAll();
}

CTP_STATE CBouncer::GetGlobalState( void )
{
	return m_State;
}

CTP_STATE CBouncer::GetThreadState( int iThread )
{
	ThreadStruct *pAux = nullptr;

	if (iThread >= m_Threads.GetSize())
		return CTP_IDLE;

	pAux = m_Threads[iThread];

	if (pAux == nullptr)
		return CTP_IDLE;

	return pAux->ThreadState;
}

ThreadRet THREADFUNC CBouncer::SocketsThread( void *lpvParam )
{
	ThreadStruct *pMe = (ThreadStruct *)lpvParam;
	byte btBuff[BASE_BUFF_SIZE + 1];
	CBouncer *pThis = pMe->pThis;
	int iRecv = 0, iSend = 0;

	pMe->ThreadState	= CTP_IDLE;

	while(pMe->pSockRead->IsActive() == true && pMe->pSockWrite->IsActive() == true) {
		if (pMe->pSockRead->IsReadyForRecv( RECV_TIMEOUT ) == true) {
			memset( btBuff, 0x00, BASE_BUFF_SIZE + 1);

			iRecv = pMe->pSockRead->Recv( btBuff, BASE_BUFF_SIZE);

			if (iRecv <= 0)
				break;

			iSend = pMe->pSockWrite->Send( btBuff, iRecv);

			if (iRecv != iSend)
				break;	
		}

		if (pMe->pSockWrite->IsReadyForRecv( RECV_TIMEOUT ) == true) {
			memset( btBuff, 0x00, BASE_BUFF_SIZE + 1);

			iRecv = pMe->pSockWrite->Recv( btBuff, BASE_BUFF_SIZE);

			if (iRecv <= 0)
				break;

			iSend = pMe->pSockRead->Send( btBuff, iRecv);

			if (iRecv != iSend)
				break;	
		}

		Sleep( 1 );
	}

	delete pMe->pSockRead;
	delete pMe->pSockWrite;

	pMe->ThreadState	= CTP_IDLE;

	return 0;
}

void CBouncer::ConnectSockets( CMySock *pRead, CMySock *pWrite)
{
	ThreadStruct *pAux = nullptr;

	pAux = new ThreadStruct;

	pAux->pSockRead		= pRead;
	pAux->pSockWrite	= pWrite;
	pAux->pThis			= this;

	MyCreateThread( &pAux->ThreadHandle, SocketsThread, (void *)pAux);

	m_Threads.AddNode( pAux );
}

ThreadRet THREADFUNC CBouncer::ThreadMain( void *lpvParam )
{
	CBouncer *pThis = (CBouncer *)lpvParam;
	CMySock *pClient = nullptr;
	CMySock *pServer = nullptr;
	CMySock MainSock;

	if (MainSock.BindPort( pThis->m_iLocalPort ) == false) {
		pThis->m_State = CTP_IDLE;
		return (ThreadRet)0;
	}

	pThis->m_State	= CTP_STARTED;

	while (pThis->GetGlobalState() != CTP_IDLE) {
		if (pThis->GetGlobalState() == CTP_PAUSED) {
			SLEEP( 10 );
			continue;
		}
		
		pClient = MainSock.Accept( ACCEPT_TIMEOUT );

		if (pClient == nullptr)
			continue;

		pServer = new CMySock();

		if (pServer->ConnectTo( pThis->m_ulRemoteAddr, pThis->m_iRemotePort, CONNECT_TIMEOUT) == false) {
			delete pClient;
			delete pServer;
			continue;
		}

		pThis->ConnectSockets( pClient, pServer);
	}

	pThis->m_State	= CTP_IDLE;

	return (ThreadRet)0;
}

int CBouncer::Start( int iThreads, int iLocalPort, int iRemotePort, const char *pszRemoteHost)
{
	if (GetGlobalState() != CTP_IDLE || iThreads <= 0)
		return -1;

	m_ulRemoteAddr = CMySock::GetInetAddr( pszRemoteHost );

	if (m_ulRemoteAddr == INADDR_NONE || iLocalPort <= 0 || iLocalPort >= 65535 || iRemotePort <= 0 || iRemotePort > 65535)
		return -1;

	m_iLocalPort		= iLocalPort;
	m_iRemotePort		= iRemotePort;
	m_iMaxConnections	= iThreads;

	m_State = CTP_INITIALIZED;

	if (MyCreateThread( &m_MainThread, ThreadMain, (void *)this) == false) {
		FreeThreads();
		return -1;
	}

	return 0;
}

bool CBouncer::Pause( void )
{
	if (GetGlobalState() != CTP_STARTED || GetGlobalState() != CTP_PAUSED)
		return -1;

	m_State = CTP_PAUSED;

	return 0;
}

bool CBouncer::Stop( void )
{
	if (GetGlobalState() == CTP_IDLE)
		return -1;

	FreeThreads();

	m_State = CTP_IDLE;

	return 0;
}