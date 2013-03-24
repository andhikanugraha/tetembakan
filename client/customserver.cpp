/**
 **	File ......... customserver.cpp
 **	Published ....  2007-10-09
 **	Author ....... grymse@alhem.net
**/
/*
Copyright (C) 2007  Anders Hedstrom

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
#ifdef _MSC_VER
#pragma warning(disable:4786)
#endif
#include <ListenSocket.h>

#include "StatusHandler.h"
#include "StatusSocket.h"


static	bool quit = false;

int main()
{
	StatusHandler h;
	ListenSocket<StatusSocket> l(h);

	if (l.Bind(9003))
	{
		exit(-1);
	}
	h.Add(&l);
	h.Select(1,0);
	time_t t = time(NULL);
	while (!quit)
	{
		h.Select(1,0);
		if (time(NULL) - t > 10)
		{
			t = time(NULL);
			h.Update();
			h.Disconnect();
		}
	}
	return 0;
}


