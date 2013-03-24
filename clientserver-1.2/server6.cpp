#include "ServerSocket.h"
#include <ListenSocket.h>
#include <StdoutLog.h>
#include <SocketHandler.h>


int main()
{
	SocketHandler h;
	StdoutLog log;
	h.RegStdLog(&log);
	ListenSocket<ServerSocket> l(h);

	l.SetIpv6();
	if (l.Bind(40001,10))
	{
		exit(-1);
	}
	h.Add(&l);
	h.Select(1,0);
	while (h.GetCount())
	{
		h.Select(1,0);
	}
}


