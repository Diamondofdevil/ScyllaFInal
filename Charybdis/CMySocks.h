/** @file CMySocks.h ver 2009.09.10
*	Clase para el manejo de Sockets TCP
*
*	Author: Iker@cuteam.org
*	Revisado y Documentado: Iker@cuteam.org
*
*	Está prohibida la copia o redistribucion de cualquiera de los archivos
*	o de partes de los mismos.
*/

#ifndef __CMYSOCKS_H__
#define __CMYSOCKS_H__

#ifdef WIN32
#undef _WIN32_WINNT
#define _WIN32_WINNT 0x0400
#endif

#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <cstdarg>

#ifdef WIN32
#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>

#pragma comment( lib, "ws2_32.lib")

#define CLOSESOCKET( x )		closesocket( x )

#else
#include <sys/socket.h>
#include <sys/types.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <unistd.h>
#include <setjmp.h>
#include <pthread.h>

#define CLOSESOCKET( x )		close( x )
#endif


#ifndef __COMMON_H__
#include "common.h"
#endif


/**	Common defined Maximum Transfer Unit **/
#define CMS_MTU								1440
/**	Cantidad maxima de interfaces de red */
#define CMS_MAX_INTERFACES					32
/**	Tamaño de la direccion del host */
#define CMS_ADDR_SIZE						32

/**	Tiempo minimo en ms a esperar */
#define CMS_TIMERSTEP						10

/**
*	@brief Clase para el manejo de sockets TCP
*/
class CMySock {
protected:
	/**	Direccion remota a la que se esta conectado o interfaz en la que se escucha */
	u_long m_ulRemoteAddr;
	u_long m_ulLocalAddr;
	/**	Puerto remoto o puerto local a la escucha */
	int m_iRemotePort;
	int m_iLocalPort;
	/**	Variable que indica si se encuentra a la escucha */
	bool m_bListen;
	/**	Descriptor de socket */
	int m_iSock;
	
	int WaitForEvent( long, u_long);
	bool GetProperties( void );
	/**
	*	Conecta al puerto del host especificado con timeout ulTime milisegundos,
	*	en caso de que no se quiera usar debe indicarse 0.
	*	@param pSin as struct sockaddr_in *
	*	@param ulTime as u_long
	*	@return Retorna el exito o fallo de la operacion.
	*/
	bool ConnectTo( struct sockaddr_in *, u_long);
	/**
	*	Acepta una coneccion en el puerto en el que esta escuchando, rellena la 
	*	estructura pSin con la informacion del cliente y retorna el descriptor del socket.
	*	ulTime indica el timeout de la espera, si no se desea usar se debe especificar 0.
	*	@param pSin as struct sockaddr_in *pSin
	*	@param ulTime as u_long
	*	@return Retorna el descriptor de socket del cliente remoto
	*/
	int Accept( struct sockaddr_in *, u_long );
public:
	/**
	*	Constructor por defecto
	*/
	CMySock();
	/**	
	*	Inicializa la clase con el socket, puerto, direccion y estado de escucha especificado
	*	@param iSock as int, Descriptor de socket valido
	*	@param pSin as struct sockaddr_in *, Estructura de la cual se tomara la direccion Ip y el puerto.
	*	@param bListen as bool Especifica si se ecuentra a la escucha o conectado
	*/
	CMySock( int, struct sockaddr_in *, bool);
	/**
	*	Destructor por Defecto
	*/
	~CMySock();
	/**
	*	Cierra el socket y setea los valores por defecto
	*/
	void ShutDown( void );
	/**
	*	Obtiene el actual codigo de error 
	*	@return Retorna el codigo de error correspondiente.
	*/
	int GetErrCode() const;
	/**
	*	Obtiene la direccion ip a la que esta conectado el socket o en la que esta escuchando.
	*	@return Retorna la direccion ip
	*/
	u_long GetLocalAddr( void );
	u_long GetRemoteAddr( void );
	/**
	*	Obtiene el puerto a la escucha o al que esta conectado el socket.
	*	@return Retorna el puerto
	*/
	int GetLocalPort( void );
	int GetRemotePort( void );
	/**
	*	Obtiene el socket asociado a la clase, en caso de que no halla ninguno asociado retorna 0.
	*	@return Retorna el socket asociado a la clase
	*/
	int GetSocket( void );
	/**
	*	Retorna si el socket esta a la escucha o no
	*	@return Retorna true en caso que se encuentre a al escucha o false en lo contrario.
	*/
	bool IsListening( void ) const;
	/**
	*	Retorna si el socket esta activo o no bien, sea a la escucha o conectado
	*	@return Retorna true si se encuentra activo o false en lo contrario
	*/
	bool IsActive( void );
	/**
	*	Espera ulTime milisegundos por datos para leer en el socket
	*	@param ulTime as u_long
	*	@return Retorna true en caso que halla datos para lectura en el socket, false en lo contrario.
	*/
	int IsReadyForRecv( u_long = INFINITE );
	/**
	*	Espera ulTime milisegundos para escribir en el socket asociado
	*	@param ulTime as u_long
	*	@return Retorna true en caso que el socket este o no listo para escritura
	*/
	int IsReadyForSend( u_long = INFINITE );
	/**
	*	Acepta una coneccion en el puerto en el que esta escuchando y devuelve una clase ya
	*	inicializada con la coneccion. Si la direccion del cliente es encontrada en cmlList
	*	la coneccion sera rechazada y se retornara nulo. ulTime indica el timeout de la espera,
	*	si no se desea usar se debe especificar 0.
	*	@see CMyList
	*	@see FindInList
	*	@see Accept
	*	@param ulTime as u_long
	*	@param cmlList as CMyList<u_long>&
	*	@return Retorna un puntero a una nueva clase CMySock ya inicializada.
	*/
	CMySock *Accept( u_long = INFINITE );
	/**
	*	Envia uiSize bytes de pbtInfo e informa el numero de bytes enviados en iSize.
	*	@param pbtInfo as const void *
	*	@param iSize as int *
	*	@return Retorna el exito o fallo de la operacion
	*/
	int Send( const void *, u_int);
	/*
	*	Recibe iSize bytes en pbtInfo, setea el valor de iSize al numero de bytes recibidos.
	*	@param pbtInfo as void *
	*	@param iSize as int *
	*	@return Retorna el exito o fallo de la operacion.
	*/
	int Recv( void *, u_int);
	/**
	*	Recibe un maximo de m_iMaxSize bytes hasta encontrar la marca especificada en pbtMark
	*	si se alcanza m_iMaxSize bytes recibidos o se registra ulTime milisegundos sin encontrar
	*	la marca la fucion falla retorna fallo. El numero de bytes recibidos se informa en iSize
	*	( *ppbtInfo debe ser nulo, si no se desea especificar un timeout ulTime ha de ser 0).
	*	@see m_iMaxSize
	*	@param ppbtInfo as void **
 	*	@param iSize as int *
	*	@param pbtMark as const byte *
	*	@param ulTime as u_long
	*	@return Retorna el exito o fallo de la operacion.
	*/
	int Recv( void *, u_int, const byte *, u_int, u_long = INFINITE);
	/**
	*	Conecta al puerto del host especificado con timeout ulTime milisegundos. (En caso que no
	*	se quiera usar debe indicarse 0.
	*	@see ConnectTo
	*	@param pstrHost as const char *
	*	@param iPort as int
	*	@param ulTime as u_long
	*	@return Retorna el exito o fallo de la operacion
	*/
	bool ConnectTo( const char *, int, u_long = INFINITE);
	/**
	*	Conecta al puerto del host especificado con timeout ulTime milisegundos ( En caso que no
	*	se quiera usar debe indicarse 0.
	*	@see ConnectTo
	*	@param ulAddr as u_long
	*	@param iPort as int
	*	@param ulTime as u_long
	*	@return Retorna el exito o fallo de la operacion
	*/
	bool ConnectTo( u_long, int, u_long = INFINITE);
	/**
	*	Escucha en el puerto especificado en iPort en la interfazes especificada por pszInterface.
	*	(Si se desea escuchar en todas las interfaces bAllInterfaces ha de ser true)
	*	@param iPort as int
	*	@param bAllInterfaces as bool
	*	@param pszInterface as const char *
	*	@return Retorna el exito o fallo de la operacion
	*/
	bool BindPort( int, bool = true, const char * = NULL);
	/**
	*	Resuelve una direccion ip o un nombre de host a formato de red. 
	*	@param pszHost as const char *pszHost
	*	@return Direccion del host en formato de red.
	*/	
	static u_long GetInetAddr( const char * );
};

#endif
