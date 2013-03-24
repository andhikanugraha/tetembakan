#include "ServerSocket.h"
#include <ListenSocket.h>
#include <SocketHandler.h>


int main()
{
	SocketHandler h;
	ListenSocket<ServerSocket> l(h);
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


