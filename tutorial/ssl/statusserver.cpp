#ifdef _WIN32
#pragma warning(disable:4786)
#endif
#include <SocketHandler.h>
#include <ListenSocket.h>

#include "StatusSocket.h"


static	bool quit = false;

int main()
{
	SocketHandler h;
	ListenSocket<StatusSocket> l(h);

	if (l.Bind(2222))
	{
		exit(-1);
	}
	Utility::ResolveLocal();
	h.Add(&l);
	h.Select(1,0);
	while (!quit)
	{
		h.Select(1,0);
	}
	return 0;
}


