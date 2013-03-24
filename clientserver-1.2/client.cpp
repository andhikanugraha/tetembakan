#include "ClientSocket.h"
#include <SocketHandler.h>


int main(int argc,char *argv[])
{
	if (argc > 1)
	{
		SocketHandler h;
		ClientSocket cc(h,argv[1]);
		cc.Open("127.0.0.1",40001);
		// Add after Open
		h.Add(&cc);
		h.Select(1,0);
		while (h.GetCount())
		{
			h.Select(1,0);
		}
	}
}


