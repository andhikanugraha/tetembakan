/**
 **	File ......... udpclient.cpp
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
#include <StdoutLog.h>
#include <SocketHandler.h>
#include "UdpTestSocket.h"
#include "stdio.h"


int main(int argc,char *argv[])
{
	SocketHandler h;
	StdoutLog log;
	h.RegStdLog(&log);
	UdpSocket s(h);

	s.SendTo("localhost", 5000, "Using SendTo with address to send");
	if (!s.Open("127.0.0.1", 5000))
	{
		printf("Exiting...\n");
		exit(-1);
	}
	for (int i = 0; i < argc; i++)
	{
		s.Send(argv[i]);
	}
}


