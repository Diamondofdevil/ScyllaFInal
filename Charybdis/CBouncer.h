#ifndef __CBOUNCER_H__
#define __CBOUNCER_H__

#include "CMyList.h"
#include "CMySocks.h"

typedef enum {
	CTP_IDLE,
	CTP_INITIALIZED,
	CTP_STARTED,
	CTP_PAUSED,
} CTP_STATE;

class CBouncer;

typedef struct {
	CMySock *pSockRead;
	CMySock *pSockWrite;
	CTP_STATE ThreadState;
	THREAD_HANDLE ThreadHandle;
	CBouncer *pThis;
} ThreadStruct;


#define ACCEPT_TIMEOUT				1000
#define CONNECT_TIMEOUT				10000
#define RECV_TIMEOUT				500
#define BASE_BUFF_SIZE				2048

class CBouncer {
private:
	CMyList<ThreadStruct *> m_Threads;
	THREAD_HANDLE m_MainThread;
	u_long m_ulRemoteAddr;
	int m_iMaxConnections;
	int m_iRemotePort;
	int m_iLocalPort;

	CTP_STATE m_State;

	void FreeThreads( void );

	void ConnectSockets( CMySock *pRead, CMySock *pWrite);

	static ThreadRet THREADFUNC ThreadMain( void *lpvParam );
	static ThreadRet THREADFUNC SocketsThread( void *lpvParam );
public:
	CBouncer();
	~CBouncer();

	CTP_STATE GetGlobalState( void );
	CTP_STATE GetThreadState( int iThread );

	int Start( int iThreads, int iLocalPort, int iRemotePort, const char *pszRemoteHost);
	bool Pause( void );
	bool Stop( void );
};

#endif