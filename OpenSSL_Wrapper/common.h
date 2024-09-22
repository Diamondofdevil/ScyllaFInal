#pragma once
#include "Stdafx.h"
#define ZLIB
#include <openssl/ssl.h>
#include <openssl/rand.h>
#include <openssl/err.h>

#pragma comment(lib, "ssleay32.dll")
#pragma comment(lib, "libeay32.dll")

using namespace System;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;

#define COMP_ZLIB	2

#define READ_BUFF_SIZE SSL3_RT_MAX_PACKET_SIZE
///Callback if server needs a certificate
RSA* sslTempRSACallback(SSL * ssl, int export, int keylength)
	{
		RSA *rsa = NULL;

		if (rsa == NULL)
			rsa = RSA_generate_key(512, RSA_F4, NULL, NULL);
		return rsa;
	}

namespace OpenSSL_Wrapper
{
	public __value enum ServerCertChainPolicyStatus
    {
		//TODO: change this
        CERT_OK									  =X509_V_OK,
		X509ERR_UNABLE_TO_GET_ISSUER_CERT		  =X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT,
		X509ERR_UNABLE_TO_GET_CRL				  =X509_V_ERR_UNABLE_TO_GET_CRL,
		X509ERR_UNABLE_TO_DECRYPT_CERT_SIGNATURE  =X509_V_ERR_UNABLE_TO_DECRYPT_CERT_SIGNATURE,
		X509ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE   =X509_V_ERR_UNABLE_TO_DECRYPT_CRL_SIGNATURE,
		X509ERR_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY=X509_V_ERR_UNABLE_TO_DECODE_ISSUER_PUBLIC_KEY,
		X509ERR_CERT_SIGNATURE_FAILURE			  =X509_V_ERR_CERT_SIGNATURE_FAILURE,
		X509ERR_CRL_SIGNATURE_FAILURE			  =X509_V_ERR_CRL_SIGNATURE_FAILURE,
		X509ERR_CERT_NOT_YET_VALID				  =X509_V_ERR_CERT_NOT_YET_VALID,
		X509ERR_CERT_HAS_EXPIRED				  =X509_V_ERR_CERT_HAS_EXPIRED,
		X509ERR_CRL_NOT_YET_VALID				  =X509_V_ERR_CRL_NOT_YET_VALID,
		X509ERR_CRL_HAS_EXPIRED					  =X509_V_ERR_CRL_HAS_EXPIRED,
		X509ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD	  =X509_V_ERR_ERROR_IN_CERT_NOT_BEFORE_FIELD,
		X509ERR_ERROR_IN_CERT_NOT_AFTER_FIELD	  =X509_V_ERR_ERROR_IN_CERT_NOT_AFTER_FIELD,
		X509ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD	  =X509_V_ERR_ERROR_IN_CRL_LAST_UPDATE_FIELD,
		X509ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD	  =X509_V_ERR_ERROR_IN_CRL_NEXT_UPDATE_FIELD,
		X509ERR_OUT_OF_MEM						  =X509_V_ERR_OUT_OF_MEM,
		X509ERR_DEPTH_ZERO_SELF_SIGNED_CERT		  =X509_V_ERR_DEPTH_ZERO_SELF_SIGNED_CERT,
		X509ERR_SELF_SIGNED_CERT_IN_CHAIN		  =X509_V_ERR_SELF_SIGNED_CERT_IN_CHAIN,
		X509ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY =X509_V_ERR_UNABLE_TO_GET_ISSUER_CERT_LOCALLY,
		X509ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE	  =X509_V_ERR_UNABLE_TO_VERIFY_LEAF_SIGNATURE,
		X509ERR_CERT_CHAIN_TOO_LONG				  =X509_V_ERR_CERT_CHAIN_TOO_LONG,
		X509ERR_CERT_REVOKED					  =X509_V_ERR_CERT_REVOKED,
		X509ERR_INVALID_CA						  =X509_V_ERR_INVALID_CA,
		X509ERR_PATH_LENGTH_EXCEEDED			  =X509_V_ERR_PATH_LENGTH_EXCEEDED,
		X509ERR_INVALID_PURPOSE					  =X509_V_ERR_INVALID_PURPOSE,
		X509ERR_CERT_UNTRUSTED					  =X509_V_ERR_CERT_UNTRUSTED,
		X509ERR_CERT_REJECTED					  =X509_V_ERR_CERT_REJECTED,
		// 'informational' when looking for issuer cert
		X509ERR_SUBJECT_ISSUER_MISMATCH			  =X509_V_ERR_SUBJECT_ISSUER_MISMATCH,
		X509ERR_AKID_SKID_MISMATCH				  =X509_V_ERR_AKID_SKID_MISMATCH,
		X509ERR_AKID_ISSUER_SERIAL_MISMATCH		  =X509_V_ERR_AKID_ISSUER_SERIAL_MISMATCH,
		X509ERR_KEYUSAGE_NO_CERTSIGN			  =X509_V_ERR_KEYUSAGE_NO_CERTSIGN,
		//application
		X509ERR_APPLICATION_VERIFICATION          =X509_V_ERR_APPLICATION_VERIFICATION,
		X509ERR_COMMON_NAME_MISMATCH              =X509_V_ERR_APPLICATION_VERIFICATION+1
    };

    public __value struct CeriticateInfo
    {
        ServerCertChainPolicyStatus PolStatus;
        Byte                        CertData[];
    };
	///enumerates the different security protocols needed
    public __value enum SecurityProviderProtocol
    {
        PROT_SSL3,
        PROT_TLS1,
		PROT_SSL2,
		PROT_SSL23,
		PROT_NO_SSL,
		//managed by protocol
		PROT_OTHER
    };
}

#pragma unmanaged
struct CHeapBuf
{
	char* m_pBuf;
	int   m_nLen;
	CHeapBuf(int len):m_nLen(len)
	{
		m_pBuf = (char*)malloc(len);
	}
	~CHeapBuf()
	{
		free(m_pBuf);
		m_pBuf = NULL;
	}
};
struct WStringComp
{   // define hash function for strings
    enum { // parameters for hash table
    bucket_size = 4, // 0 < bucket_size
    min_buckets = 8  // min_buckets = 2 ^^ N, 0 < N
	};

	size_t operator()(const std::wstring& s1) const
    { 
        const wchar_t *p = s1.c_str();
        size_t nHash = 0;
        while (*p != '\0')
            nHash = (nHash<<5) + nHash + (*p++);
        return nHash;
    }
    bool operator()(const std::wstring &s1, const std::wstring &s2) const
    { // test if s1 ordered before s2
        return (s1 < s2);
    }
};
#pragma managed