/**
 **	File ......... StdinSocket.cpp
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
#include <errno.h>

#include "StdinSocket.h"
#include "StdLog.h"
#include <ISocketHandler.h>
#ifdef _WIN32
#include <io.h>
#endif


StdinSocket::StdinSocket(ISocketHandler& h)
:Socket(h)
{
	Attach( STDIN_FILENO );
	Handler().ISocketHandler_Add(this, true, false);
}


StdinSocket::~StdinSocket()
{
}


void StdinSocket::OnRead()
{
	char buf[1000];
	int n = read(GetSocket(), buf, 999);
printf("read: %d\n", n);
	if (n == -1)
	{
		Handler().LogError(this, "OnRead", errno, strerror(errno), LOG_LEVEL_FATAL);
		SetCloseAndDelete();
		return;
	}
	for (size_t i = 0; i < (size_t)n; i++)
	{
		switch (buf[i])
		{
		case 13:
			break;
		case 10:
			OnLine(m_line);
			m_line = "";
			break;
		default:
			m_line += buf[i];
		}
	}
}


