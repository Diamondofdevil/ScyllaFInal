#include <cstdio>
#include "CBouncer.h"

//#pragma comment( lib, "ws2_32.lib")

void usage( void )
{
	printf("Usage:\r\n");
	printf("\tCarybdis.exe srcport dstport dsthost\r\n");
}

int main(int argc, char** argv) 
{
	int iSrcPort = 0, iDstPort = 0;
	CBouncer BouncerMain;
	int iRet = 0;

	if (argc != 4) {
		usage();
		return 0;
	}

	iSrcPort	= atoi( argv[1] );
	iDstPort	= atoi( argv[2] );

	iRet = BouncerMain.Start( 1, iSrcPort, iDstPort, argv[3]);

	if (iRet != 0) {
		printf("Error, verify your parameters and bind local ports\r\n");
		return 0;
	}

	while (BouncerMain.GetGlobalState() != CTP_IDLE);

	return (0);
}
