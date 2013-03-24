/**
 **	File ......... telnet.cpp
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
#include <TcpSocket.h>
#include <Socket.h>

#include "StdinLine.h"
#include "Console.h"


class tSocket : public TcpSocket
{
public:
	tSocket(ISocketHandler& h) : TcpSocket(h) {}
	~tSocket() {}

	void OnConnect() {
		printf("Connected\n");
	}
	void OnLine(const std::string& line) {
		printf("%s\n", line.c_str());
	}

private:
};

int main(int argc,char *argv[])
{
	if (argc < 2)
	{
		printf("Usage: %s <hostname> [<port>]\n", *argv);
	}
	SocketHandler h;
	tSocket sock(h);
	int port = argc > 2 ? atoi(argv[2]) : 23;
	sock.Open(argv[1], port);
	h.Add(&sock);
	Console console(h);
	h.Add(&console);
	console.SetSock(&sock);
	bool quit = false;
	while (!quit)
	{
		h.Select(1,0);
printf(".");
fflush(stdout);
	}
}


