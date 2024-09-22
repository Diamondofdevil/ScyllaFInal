/**	@file common.h ver 2008.26.6
*	Funciones comunes entre las clases	
*
*	Author: Iker@cuteam.org
*	Revisado y Documentado: Iker@cuteam.org
*
*   Está prohibida la copia de cualquiera de los archivos, o de partes 
*   de los mismos.
*/
#ifndef __COMMON_H__
#define __COMMON_H__

/**	Define null afford to C++ std */
#ifndef NULL
#define NULL 0
#endif

/**	basic types define */
typedef unsigned char byte;
/**	basic types define */
typedef unsigned int u_int;
/**	basic types define */
typedef unsigned long u_long;
/**	basic types define */
typedef unsigned short u_short;

/**	Macro for create a short from 2 bytes MAKESHORT( LOWPART, HIPART ) */
#define MKSHORT( x, y)							(short)((x) | ((y) << 8))
/**	Macro for create a int from 2 shorts MAKEINT( LOWPART, HIPART ) */
#define MKINT( x, y)							(int)((x) | ((y) << 16 ))
/**	Macro that use the macro for int casting to long */
#define MKLONG( x, y)							(long)MKINT( (x), (y))
/**	Macro for obtain the low byte of x */
#define LOWBYTE( x )		 					(byte)(x)
/**	Macro for obtain the high byte of x( int, short, long ) */
#define HIGHBYTE( x )							(byte)((x) >> ((sizeof( (x) ) - 1) * 8))
/**	Macro for obtain the low word of x */
#define LOWWORD( x )							(short)(x)
/**	Macro for obtain the high word of x */
#define HIGHWORD( x )							(short)((x) >> 16)
/**	Alphabet for hexadecimal conversion */
#define ALPHA_HEX_STR							"0123456789ABCDEF"
/**	Macro for verify if is a hexadecimal char */
#define ISHEXDIGIT( x )							(((byte)(tolower( (x) ) - 'a') < 6 ) || ((byte)((x) - '0')) < 10)
/**	Macro for verify if is a alphanumeric char */
#define ISALPHANUM( x )							(((byte)(tolower( (x) ) - 'a') < 26) || ((byte)((x) - '0')) < 10)

#ifndef _CSTDIO_
#include <cstdio>
#endif

#ifndef _CCTYPE_
#include <cctype>
#endif

#ifdef WIN32

#ifndef _WINDOWS_
#include <windows.h>
#endif

#ifdef MAX_PATH
#undef MAX_PATH
#endif

/**	Max path cross paltform value */
#define MAX_PATH			256

/**	Directory separator */
#define DIR_CHAR			'\\'

/**	Define Critical Section type */
typedef CRITICAL_SECTION	SECTION_HANDLE;

/**	Critical Sections macro */
#define INIT_SECTION( x )	InitializeCriticalSection( (x) )
/**	Critical Sections macro */
#define DELETE_SECTION( x )	DeleteCriticalSection( (x) )
/**	Critical Sections macro */
#define ENTER_SECTION( x )	EnterCriticalSection( (x) )
/**	Critical Sections macro */
#define TRY_ENTER_SECTION( x )	TryEnterCriticalSection( (x) )
/**	Critical Sections macro */
#define LEAVE_SECTION( x )	LeaveCriticalSection( (x) )

/**	Handle for thread management */
typedef HANDLE				THREAD_HANDLE;
/**	Return type for thread procedure */
typedef DWORD				ThreadRet;
/**	Thread Procedure type function */
#define THREADFUNC			WINAPI
/**	Macro for Sleep */
#define SLEEP( x )			Sleep( (x) )

/**	
*	Struct for gettimeofday on windows, be carefull with collisions in winsock define
*/

#ifndef _WINSOCKAPI_

/**
*	Struct for tiome management
*/
struct timeval{
	/**	secs */
	long tv_sec;
	/**	microsegs */
	long tv_usec;
};

#endif

/**	issue for compatibility in cross platform */
struct timezone{
	int	tz_minuteswest;	
	int	tz_dsttime;	 
};

/**
*	Windows gettimeofday implementation
*	@param tvTime as struct timeval *
*	@param tzTime as struct timezone *
*	@return Always return 0.
*/
_inline int gettimeofday( struct timeval *, struct timezone *);

#else
#include <pthread.h>
#include <sys/time.h>

#ifdef MAX_PATH
#undef MAX_PATH
#endif

/**	Max path cross paltform value */
#define MAX_PATH			256
/**	Directory separator */
#define DIR_CHAR			'/'

/**	Define Critical Section type in linux */
typedef pthread_mutex_t		SECTION_HANDLE;

/**	Critical Sections macro */
#define INIT_SECTION( x )	pthread_mutex_t n = PTHREAD_MUTEX_INITIALIZER; *(x) = n
/**	Critical Sections macro */
#define DELETE_SECTION( x )	(x)
/**	Critical Sections macro */
#define ENTER_SECTION( x )	pthread_mutex_lock( (x) )
/**	Critical Sections macro */
#define TRY_ENTER_SECTION( x )	pthread_mutex_trylock( (x) ) ? 0 : 1
/**	Critical Sections macro */
#define LEAVE_SECTION( x )	pthread_mutex_unlock( (x) )

/**	Handle for thread management */
typedef pthread_t			THREAD_HANDLE;
/**	Return type for thread procedure */
typedef void *				ThreadRet;
/**	Thread Procedure type function */
#define THREADFUNC			__cdecl
/**	Macrpo for Sleep */
#define SLEEP( x )			usleep( (x) * 1000 )

#endif

/**	Thread Procedure prototype for MyCreateThread */
typedef ThreadRet (THREADFUNC *THREAD_PROC)( void *);

/**
*	Cross-platform Function for creation thread
*	Create the thread with the procedure ThreadProcdeure and lpvParam like argument
*	and set the handle to pthHandle
*	@param pthHandle as THREAD_HANDLE *
*	@param ThreadProcedure as THREAD_PROC
*	@param lpvParam as void *
*	@return Returns the result of the operation.
*/
bool MyCreateThread( THREAD_HANDLE *pthHandle, THREAD_PROC ThreadProcedure, void *lpvParam);

/**
*	Cross-platform Function for finish thread
*	Finish a thread identify with the descriptor specified in pthHandle
*	@param pthHandle as THREAD_HANDLE *
*/
void MyFinishThread( THREAD_HANDLE *pthHandle);

/**
*	Write uiSize bytes of lpvData to the file specified by pfFile.
*	@param lpvData as const void *
*	@param uiSize as u_int
*	@param pfFile as FILE *
*	@return Returns true if write all bytes or false in otherwise
*/
bool WriteExactData( const void *, u_int, FILE *);

/**
*	Read uiSize bytes to lpvData from the file specified by pfFile.
*	@param lpvData as void *
*	@param uiSize as u_int
*	@param pfFile as FILE *
*	@return Returns true if read all bytes or false in otherwise
*/
bool ReadExactData( void *, u_int, FILE *);

/**
*	Obtain the current time and minus with tvTime
*	@param tvTime as timeval *
*	@return Returns CurrentTime - tvTime in ms
*/
u_long GetAndDiffTime( struct timeval *tvTime );

/**
*	Minus tvTime2 with tvTime1
*	@param tvTime1 as timeval *
*	@param tvTime2 as timeval *
*	@return Returns tvTime2 - tvTime1 in ms
*/
__inline u_long DiffTime( struct timeval *tvTime1, struct timeval *tvTime2);

/**
*	Apply a url encode to pszSrc and write the result in pszDst
*	@param pszSrc as const char *
*	@param pszDst as char *
*	@return Returns the size of the string encoded or 0 in fail
*/
u_int UrlEncode( const char *pszSrc, char *pszDst);

/**
*	Decode the string pointed by pszSrc to pszDst
*	@param pszSrc as const chr *
*	@param szDst as char *
*	@return Returns the size the string encoded or 0 in fail
*/
u_int UrlDecode( const char *pszSrc, char *pszDst);

int StrToHex( const char *pszSrc, char *pszDst);
int HexToStr( const char *pszSrc, char *pszDst);

#endif