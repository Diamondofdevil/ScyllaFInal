// OpenSSL_Wrapper.h

#pragma once
#include "common.h"
#include "uthash.h"
#include "HashTable.h"

using namespace System;
using namespace System::Net::Sockets;
using namespace System::IO;

namespace OpenSSL_Wrapper
{
		/// if needed a certification
		public __value struct ServerCeriticateInfo;

        /// this
        __sealed public __gc class OpenSSL;
		
        //server asked for renegotiation, load new certificate, such as SSLConn->LoadNewClientCredentials
        public __delegate void NewCertificate(OpenSSL* SSLConn);

		///Socket info for using with ssl module
		struct SSLSOCKETINFO {
			int id;                    /* key --> socket id */
			int nUseSSL;
			SSL *ssl;
			SSL_CTX *sslContext;
			UT_hash_handle hh;         /* required for UThash */
			};

		struct SSLSOCKETINFO *psSSLSocketInfo = NULL; 

		public __gc class OpenSSL
		{
			
		public:
			///Constructor vacio
			OpenSSL(){
				SSLeay_add_ssl_algorithms();
			}
			///Destructor vacio
			~OpenSSL(){ }

			///Starts handshake for publickey sharing with ip
			//void initHandShake(String* ip, Object* state);
			///Sends encripted data in data[] of length len through the Socket state
			void send(Byte data[], int len, Socket* state);
			///Receive encripted data in data[] of length len through the Socket state. Return # of bytes reades
			int OpenSSL::receive( Byte data[], int size, Socket* sock);
			///Try disconecting the socket, first closing the ssl connection
			void disconnect(Socket* sock);
			///connects the socket s to the endPoint ep using security (sslv2, sslv3, sslv23 or TSL) defined in prot
			void OpenSSLConnect(OpenSSL_Wrapper::SecurityProviderProtocol prot, Socket* s, Net::IPEndPoint* ep );
			//gets the max chunk data that could send or receive
			__property int get_MaxDataChunkSize()
			{
				return SSL3_RT_MAX_PLAIN_LENGTH;
				
			}
			//builds http digest based on server parameters
			static System::String* buildHTTPDigest(System::String* algo, System::String* login, System::String* realm, System::String* password, System::String* nonce, System::String* cnonce, System::String* nonceCount, System::String* qop, System::String* opaque, System::String* method, System::String* uri);

			
		private:

			//hash table for storing diferent SSLSOCKETINFO structures asigned to each socket
			static HASHTBL* ht =  hashtbl_create(50,NULL);
		
		};
		
}
