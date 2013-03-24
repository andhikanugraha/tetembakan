#include <StdoutLog.h>
#include "ClientSocket.h"
#include <SocketHandler.h>


int main(int argc,char *argv[])
{
	if (argc < 2)
	{
		printf("Usage: %s <command>\n",*argv);
		return -1;
	}
	SocketHandler h;
	StdoutLog log;
	h.RegStdLog(&log);
	ClientSocket cc(h,argv[1]);

	cc.SetIpv6();
	cc.Open("localhost",40001);
	// Add after Open
	h.Add(&cc);
	h.Select(1,0);
	while (h.GetCount())
	{
		h.Select(1,0);
	}
}


