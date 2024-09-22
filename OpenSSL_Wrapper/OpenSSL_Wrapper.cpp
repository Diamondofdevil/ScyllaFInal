// This is the main DLL file, Open SSL wrapper for using with c#

#include "stdafx.h"
#include "common.h"
#include "OpenSSL_Wrapper.h"
#include "HTTP_Digest.h";
//just in case -- if not included, include
#include <openssl/ssl.h>
#include <openssl/rand.h>
#include <openssl/err.h>
#include <openssl/tls1.h>

//just in case -- if not loaded load
#pragma comment(lib, "ssleay32.lib")
#pragma comment(lib, "libeay32.lib")

using namespace System;
using namespace System::Net::Sockets;

namespace OpenSSL_Wrapper
{

	void OpenSSL::OpenSSLConnect(OpenSSL_Wrapper::SecurityProviderProtocol prot, Socket *socket, Net::IPEndPoint* ep)
	{
		IntPtr hSocket = socket->Handle;
		struct SSLSOCKETINFO *s = (SSLSOCKETINFO*) malloc(sizeof(struct SSLSOCKETINFO));
		memset(s, 0, sizeof(struct SSLSOCKETINFO));

		SSL *ssl = NULL;
		SSL_CTX *sslContext = NULL;

		//SSLeay_add_ssl_algorithms();
		if (prot == OpenSSL_Wrapper::SecurityProviderProtocol::PROT_SSL2)
			sslContext = SSL_CTX_new(SSLv2_client_method());
		else if (prot == OpenSSL_Wrapper::SecurityProviderProtocol::PROT_SSL3)
			sslContext = SSL_CTX_new(SSLv3_client_method());
		else if(prot == OpenSSL_Wrapper::SecurityProviderProtocol::PROT_TLS1)
			TLSv1_client_method();
		else 
			sslContext = SSL_CTX_new(SSLv23_client_method());
		
		if (sslContext == NULL)
		{
			this->OpenSSLConnect(prot,socket,ep);
			return;	
		}
		// set the compatbility mode
		SSL_CTX_set_options(sslContext, SSL_OP_ALL);
		// we set the default verifiers and dont care for the results
		SSL_CTX_set_default_verify_paths(sslContext);
		SSL_CTX_set_tmp_rsa_callback(sslContext, sslTempRSACallback);
		SSL_CTX_set_verify(sslContext, SSL_VERIFY_NONE, NULL);
		socket->Connect(ep);

		if ((ssl = SSL_new(sslContext)) == NULL)
		{
			throw new Exception("SSL Connection could not be made");
		}
		
		SSL_set_fd(ssl, hSocket.ToInt32());

		if (SSL_connect(ssl) <= 0)
		{
			throw new Exception ("SSL Connect Error");
		}

		SSL_get_cipher(ssl);
		s->id = hSocket.ToInt32();
		s->nUseSSL = 1;
		s->ssl = ssl;
		s->sslContext = sslContext;

		IntPtr iptr = socket->Handle;
		char* buf = (char*) malloc(10);
		_itoa_s(iptr.ToInt32(),buf,10,10);
		hashtbl_insert(ht,buf,s);
		free(buf);


	}
	//void OpenSSL::initHandShake(String* ip, Object* state){}
	void OpenSSL::send(Byte data[], int len, Socket* sock)
	{
		const unsigned char __pin* pBuf = &data[0];

		char* buf = (char*) malloc(10);
		int socket= sock->Handle.ToInt32();
		_itoa_s(socket,buf,10,10);
		struct SSLSOCKETINFO *s = (SSLSOCKETINFO*)hashtbl_get(ht,buf);
		free(buf);

		if ((s != NULL) && s->ssl != NULL && pBuf != 0 && data != NULL && data[0] != 0)
		{
			SSL_write(s->ssl, pBuf, len);
		}
	}
	//There's a fucking bug here that i can't find :S
	int OpenSSL::receive( Byte data[], int length, Socket* sock)
	{
		int nRet = -1;
		Debug::WriteLine("1");
		unsigned char* d = (unsigned char*)malloc(data->Length*2);

		char* buf = (char*) malloc(10);
		Debug::WriteLine("2");
		_itoa_s(sock->Handle.ToInt32(),buf,10,10);
		struct SSLSOCKETINFO *s = (SSLSOCKETINFO*)hashtbl_get(ht,buf);
		Debug::WriteLine("3");
		free(buf);

		do
		{
				nRet = SSL_read(s->ssl, d, length);
		}
	    while (nRet == -1);// && err == SSL_ERROR_SYSCALL);
		Debug::WriteLine("4");
		Marshal::Copy(IntPtr(d), data, 0, length);
		Debug::WriteLine("5");
		return nRet;

	}
	void OpenSSL::disconnect(Socket* sock)
	{
		try{
			int socket = sock->Handle.ToInt32();
			char* buf = (char*) malloc(30);
			_itoa_s(socket,buf,30,10);

			struct SSLSOCKETINFO *s = (SSLSOCKETINFO*)hashtbl_get(ht,buf);
			if(s->ssl != NULL && SSL_ST_OK == SSL_state( s->ssl ) && s->sslContext != NULL)
			{
				SSL_shutdown( s->ssl );
			}
			hashtbl_remove(ht,buf);
			free(buf);
		}catch(...){}
	}
	System::String* OpenSSL::buildHTTPDigest(System::String* algo, System::String* login, System::String* realm, System::String* password, System::String* nonce, System::String* cnonce, System::String* nonceCount, System::String* qop, System::String* opaque, System::String* method, System::String* uri)
	{
		HASHHEX HA1;
		HASHHEX HA2 = "";
		HASHHEX Response;
		char * szAlg = (char*)(void *)Marshal::StringToHGlobalAnsi( algo );
		char * szLogin = (char*)(void *)Marshal::StringToHGlobalAnsi( login );
		char * szRealm = (char*)(void *)Marshal::StringToHGlobalAnsi( realm );
		char * szPassword = (char*)(void *)Marshal::StringToHGlobalAnsi( password );
		char * szNonce = (char*)(void *)Marshal::StringToHGlobalAnsi( nonce );
		char * szCNonce = (char*)(void *)Marshal::StringToHGlobalAnsi( cnonce );
		char * szNonceCount = (char*)(void *)Marshal::StringToHGlobalAnsi( nonceCount );
		char * szQop = (char*)(void *)Marshal::StringToHGlobalAnsi( qop );
		char * szMethod = (char*)(void *)Marshal::StringToHGlobalAnsi( method );
		char * szURI = (char*)(void *)Marshal::StringToHGlobalAnsi( uri );

		char * szOpaque = (char*)(void *)Marshal::StringToHGlobalAnsi( opaque );

		DigestCalcHA1(szAlg, szLogin, szRealm, szPassword, szNonce, szCNonce, HA1);
		DigestCalcResponse(HA1, szNonce, szNonceCount, szCNonce, szQop, szMethod, szURI, HA2, Response);

		//System::String* szAuthorization = "Digest username=\""+login + "\", realm=\""+*realm + "\", nonce=\""+ *nonce + "\", ;
		//m1 whats better, to alloc 100 chars or to get the lenght of all the strings?
		char* szAuthorization = new char[100];
		if ( (!System::String::IsNullOrEmpty(qop)) && (!System::String::IsNullOrEmpty(opaque) ))
			sprintf(szAuthorization, "Digest username=\"%s\", realm=\"%s\", nonce=\"%s\", uri=\"%s\", algorithm=%s, response=\"%s\", qop=%s, nc=00000001, cnonce=\"%s\", opaque=\"%s\"",
                             szLogin, szRealm, szNonce, szURI, szAlg, Response, szQop, szCNonce, szOpaque);
		else if (!System::String::IsNullOrEmpty(qop))
			sprintf(szAuthorization, "Digest username=\"%s\", realm=\"%s\", nonce=\"%s\", uri=\"%s\", algorithm=%s, response=\"%s\", qop=%s, nc=00000001, cnonce=\"%s\"",
                             szLogin, szRealm, szNonce, szURI, szAlg, Response, szQop, szCNonce);
			else if (!System::String::IsNullOrEmpty(opaque))
				sprintf(szAuthorization, "Digest username=\"%s\", realm=\"%s\", nonce=\"%s\", uri=\"%s\", algorithm=%s, response=\"%s\", opaque=\"%s\"",
                             szLogin, szRealm, szNonce, szURI, szAlg, Response, szOpaque);
				else
					sprintf(szAuthorization, "Digest username=\"%s\", realm=\"%s\", nonce=\"%s\", uri=\"%s\", algorithm=%s, response=\"%s\"",
                             szLogin, szRealm, szNonce, szURI, szAlg, Response);
							 
		return new System::String(szLogin);
	}
}