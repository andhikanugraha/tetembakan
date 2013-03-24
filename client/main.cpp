/**
 **	File ......... send_test.cpp
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
#include <SocketHandler.h>
#include <ListenSocket.h>
#include "DisplaySocket.h"
#include "SendSocket.h"
#include "stdio.h"


int main(int argc, char *argv[])
{
	SocketHandler h;
	std::string text = argc > 1 ? argv[1] : "";
	SendSocket sock(h, text);
	sock.Open("localhost", 12346);
	h.Add(&sock);
	h.Select(1, 0);
}


