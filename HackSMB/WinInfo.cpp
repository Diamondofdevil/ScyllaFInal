
#include "WinInfo.h"
namespace HackSMB{
WinInfo::WinInfo(void)
{
	plstDisks		= new List<String*>();
	plstEventLog	= new List<events_>();
	plstGroups		= new List<groups_*>();
	plstUsers		= new List<user_>();
	plstGroups		= new List<groups_*>();
	plstSession		= new List<Session_>();
	plstDevs		= new List<NetDev_>();
	plstNbs			= new List<NetBIOS_>();
	plstServ		= new List<Service_>();
}

}