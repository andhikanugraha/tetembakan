/**
 **	File ......... StatusHandler.cpp
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
#include <stdarg.h>
#include "StatusSocket.h"
#include "StatusHandler.h"
#include "stdio.h"
#include "string.h"


StatusHandler::StatusHandler()
:SocketHandler()
{
}


#define SIZE 5000
void StatusHandler::tprintf(TcpSocket *p,const char *format, ...)
{
	va_list ap;
	size_t n;
	char tmp[SIZE];

	va_start(ap,format);
#ifdef _WIN32
	n = vsprintf(tmp,format,ap);
#else
	n = vsnprintf(tmp,SIZE - 1,format,ap);
#endif
	va_end(ap);

	p -> SendBuf(tmp, strlen(tmp));
}


void StatusHandler::List(TcpSocket *sendto)
{
	tprintf(sendto, "Handler Socket List\n");
	for (socket_m::iterator it = m_sockets.begin(); it != m_sockets.end(); it++)
	{
		Socket *p = (*it).second;
		TcpSocket *p3 = dynamic_cast<TcpSocket *>(p);
		StatusSocket *p4 = dynamic_cast<StatusSocket *>(p);
		tprintf(sendto, " %s:%d",p -> GetRemoteAddress().c_str(),p -> GetRemotePort());
		tprintf(sendto, "  %s  ",p -> Ready() ? "Ready" : "NOT_Ready");
		if (p4)
		{
			tprintf(sendto, "StatusSocket");
		}
		tprintf(sendto, "\n");
		tprintf(sendto, "  Uptime:  %d days %02d:%02d:%02d\n",
			p -> Uptime() / 86400,
			(p -> Uptime() / 3600) % 24,
			(p -> Uptime() / 60) % 60,
			p -> Uptime() % 60);
		if (p3)
		{
			tprintf(sendto, "    Bytes Read: %9lu\n",p3 -> GetBytesReceived());
			tprintf(sendto, "    Bytes Sent: %9lu\n",p3 -> GetBytesSent());
		}
	}
	tprintf(sendto, "\n");
}


void StatusHandler::Update()
{
	for (socket_m::iterator it = m_sockets.begin(); it != m_sockets.end(); it++)
	{
		Socket *p0 = (*it).second;
		StatusSocket *p = dynamic_cast<StatusSocket *>(p0);
		if (p)
		{
			List(p);
		}
	}
}


void StatusHandler::Disconnect()
{
	for (socket_m::iterator it = m_sockets.begin(); it != m_sockets.end(); it++)
	{
		Socket *p0 = (*it).second;
		TcpSocket *p = dynamic_cast<TcpSocket *>(p0);
		if (p && p -> Uptime() > 60 )
		{
			tprintf(p, "Goodbye\n");
			p -> SetCloseAndDelete();
		}
	}
}


