/**
 **	File ......... displayclient.cpp
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
#include "DisplaySocket.h"
#include <SocketHandler.h>


int main()
{
	SocketHandler h;
	DisplaySocket *p = new DisplaySocket(h);

	p -> SetDeleteByHandler();
	p -> Open("localhost", 9002);
	h.Add(p);
	h.Select(1,0);
	while (h.GetCount())
	{
		h.Select(1,0);
	}
	return 0;
}


