/**	@file common.cpp ver 2008.26.6
*	Funciones comunes entre las clases	
*
*	Author: Iker@cuteam.org
*	Revisado y Documentado: Iker@cuteam.org
*
*   Está prohibida la copia de cualquiera de los archivos, o de partes 
*   de los mismos.
*/
#include "common.h"

bool MyCreateThread( THREAD_HANDLE *pthHandle, THREAD_PROC ThreadProcedure, void *lpvParam)
{
	if (!pthHandle || !ThreadProcedure)
		return false;

#ifdef WIN32
	DWORD dwId;
	
	if (!(*pthHandle = CreateThread( NULL, 0, ThreadProcedure, lpvParam, 0, &dwId)))
		return false;
	return true;
#else
	return pthread_create( pthHandle, NULL, ThreadProcedure, lpvParam) ? false : true;
#endif
}

void MyFinishThread( THREAD_HANDLE *pthHandle )
{
	if (!pthHandle)
		return;

#ifdef WIN32
	DWORD dwExitCode;

	GetExitCodeThread( *pthHandle, &dwExitCode);
	TerminateThread( *pthHandle, dwExitCode);
#else
	pthread_cancel( *pthHandle );
#endif
}

bool WriteExactData( const void *lpvData, u_int uiSize, FILE *pfFile)
{
	u_int uiWrited = 0;
	int iTries = 3;

	if (!lpvData || !uiSize || !pfFile)
		return false;

	do {
		uiWrited = fwrite( ((byte *)lpvData + uiWrited), 1, uiSize - uiWrited, pfFile);
	} while (uiWrited != uiSize && (--iTries));

	return uiWrited != uiSize ? false : true;
}

bool ReadExactData( void *lpvData, u_int uiSize, FILE *pfFile)
{
	u_int uiRead = 0;
	int iTries = 3;

	if (!lpvData || !uiSize || !pfFile)
		return false;

	do {
		uiRead = fread( ((byte *)lpvData + uiRead), 1, uiSize - uiRead, pfFile);
		if ( feof( pfFile ) )
			break;
	} while(uiRead != uiSize && (--iTries));

	return uiRead != uiSize ? false : true;
}

#ifdef WIN32
__inline int gettimeofday( struct timeval *tvTime, struct timezone *tzTime)
{
    LARGE_INTEGER liInt;
    __int64 i64Time = 0;
	FILETIME ftTime;

    if (!tvTime)
		return 0;

    GetSystemTimeAsFileTime( &ftTime );

    liInt.LowPart	= ftTime.dwLowDateTime;
    liInt.HighPart	= ftTime.dwHighDateTime;

	i64Time	= liInt.QuadPart;
    i64Time	-= 116444736000000000;
    i64Time	/= 10;

    tvTime->tv_sec	= (u_long)(i64Time / 1000000);
    tvTime->tv_usec	= (u_long)(i64Time % 1000000);

    return 0;
}
#endif

__inline u_long DiffTime( struct timeval *tvTime1, struct timeval *tvTime2)
{
	u_long ulRet = 0;

	if (!tvTime1 || !tvTime2)
		return 0;

	ulRet = ((tvTime1->tv_sec <= tvTime2->tv_sec) ? (tvTime2->tv_sec - tvTime1->tv_sec): -(tvTime1->tv_sec - 60) + tvTime2->tv_sec) * 1000;
	ulRet += ((tvTime1->tv_usec <= tvTime2->tv_usec) ? (tvTime2->tv_usec - tvTime1->tv_usec) : -(tvTime1->tv_usec - 1000000) + tvTime2->tv_usec) / 1000;
	ulRet -= tvTime1->tv_usec > tvTime2->tv_usec ? 1000 : 0;

	return ulRet;
}

u_long GetAndDiffTime( struct timeval *tvTime )
{
	timeval tvTime2;

	gettimeofday( &tvTime2, NULL);

	return DiffTime( &( *tvTime  ), &tvTime2);
}

u_int UrlEncode( const char *pszSrc, char *pszDst)
{
	char *pszAlHex = ALPHA_HEX_STR;
	u_int uiLen = 0;

	if (!pszSrc || !pszDst)
		return 0;

	while( *pszSrc ) {
		if (!ISALPHANUM( *pszSrc ) && *pszSrc != '-' && *pszSrc != '_' && *pszSrc != '.' && *pszSrc != '~') {
			*pszDst++ = '%';
			*pszDst++ = pszAlHex[((byte)*pszSrc) >> 4];
			*pszDst++ = pszAlHex[((byte)*pszSrc) ^ ((((byte)*pszSrc) >> 4) << 4)];
			++pszSrc;
			uiLen += 3;
			continue;
		}
		*pszDst++ = *pszSrc++;
		++uiLen;
	}

	*pszDst = 0;

	return uiLen;
}

u_int UrlDecode( const char *pszSrc, char *pszOut)
{
	u_int uiLen = 0;

	if (!pszSrc || !pszOut)
		return 0;

	while( *pszSrc ) {
         if (*pszSrc == '%'){
			 if (!ISHEXDIGIT(*(pszSrc + 1)) || !ISHEXDIGIT(*(pszSrc + 2)))
				 return 0;	// Bad format of str

			 *pszOut 	= *(++pszSrc) >= 'A' ? ((*pszSrc++ - 'A') << 4) + 0xA0 : (*pszSrc++ - '0') << 4;
			 *pszOut++ 	+= *pszSrc >= 'A' ? (*pszSrc++ - 'A') + 0x0A : (*pszSrc++ - '0');
			 ++uiLen;
			 continue;
         }
         *pszOut++ = *pszSrc++;
		 ++uiLen;
   }

   *pszOut = 0;

   return uiLen;
}

int StrToHex( const char *pszSrc, char *pszDst)
{
	char *pszAlHex = ALPHA_HEX_STR;
	int iRet = 0;

	if (!pszSrc || !pszDst)
		return 0;

	while ( *pszSrc ) {
		*pszDst++ = pszAlHex[((byte)*pszSrc) >> 4];
		*pszDst++ = pszAlHex[((byte)*pszSrc) ^ ((((byte)*pszSrc) >> 4) << 4)];
		++pszSrc;
		iRet += 2;
	}

	*pszDst = 0;

	return iRet;
}

int HexToStr( const char *pszSrc, char *pszDst)
{
	int iRet = 0;

	if (!pszSrc || !pszDst)
		return 0;

	while ( *pszSrc && *(pszSrc + 1)) {
		 if (!ISHEXDIGIT( *pszSrc ) || !ISHEXDIGIT(*(pszSrc + 1)))
			 return 0;	// Bad format of str
		*pszDst 	= *pszSrc >= 'A' ? ((*pszSrc++ - 'A') << 4) + 0xA0 : (*pszSrc++ - '0') << 4;
		*pszDst++ 	+= *pszSrc >= 'A' ? (*pszSrc++ - 'A') + 0x0A : (*pszSrc++ - '0');
		++iRet;
	}

	*pszDst = 0;

	return iRet;
}