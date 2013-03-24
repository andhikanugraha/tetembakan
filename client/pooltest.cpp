/**
 **	File ......... pooltest.cpp
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
#include <TcpSocket.h>
#include <ListenSocket.h>
#include <Utility.h>
#include <Parse.h>
#include <HttpGetSocket.h>


class MyHandler : public SocketHandler
{
public:
	MyHandler(StdLog *p) : SocketHandler(p),m_quit(false) {}
	~MyHandler() {}

	void List(TcpSocket *p) {
		for (socket_m::iterator it = m_sockets.begin(); it != m_sockets.end(); it++)
		{
			Socket *p0 = (*it).second;
#ifdef ENABLE_POOL
			if (dynamic_cast<ISocketHandler::PoolSocket *>(p0))
			{
				p -> Send("PoolSocket\n");
			}
			else
#endif
			if (dynamic_cast<HttpGetSocket *>(p0))
			{
				p -> Send("HttpGetSocket\n");
			}
			else
			if (dynamic_cast<TcpSocket *>(p0))
			{
				p -> Send("TcpSocket\n");
			}
			else
			{
				p -> Send("Some kind of Socket\n");
			}
		}
	}
	void SetQuit() { m_quit = true; }
	bool Quit() { return m_quit; }

private:
	bool m_quit;
};


class OrderSocket : public TcpSocket
{
public:
	OrderSocket(ISocketHandler& h) : TcpSocket(h) {
		SetLineProtocol();
	}
	void OnAccept() {
		Send("Cmd (get,quit,list,stop)>");
	}
	void OnLine(const std::string& line) {
		Parse pa(line);
		std::string cmd = pa.getword();
		std::string arg = pa.getrest();
		if (cmd == "get")
		{
			HttpGetSocket *p = new HttpGetSocket(Handler(), arg, "tmpfile.html");
			p -> SetHttpVersion("HTTP/1.1");
			p -> AddResponseHeader("Connection", "keep-alive");
			Handler().Add( p );
			p -> SetDeleteByHandler();
			Send("Reading url '" + arg + "'\n");
		}
		else
		if (cmd == "quit")
		{
			Send("Goodbye!\n");
			SetCloseAndDelete();
		}
		else
		if (cmd == "list")
		{
			static_cast<MyHandler&>(Handler()).List( this );
		}
		else
		if (cmd == "stop")
		{
			static_cast<MyHandler&>(Handler()).SetQuit();
		}
		else
		{
			Send("Huh?\n");
		}
		Send("Cmd>");
	}
};


int main()
{
	StdoutLog log;
	MyHandler h(&log);

	// line server
	ListenSocket<OrderSocket> l3(h);
	l3.Bind(1026);
	h.Add(&l3);

	h.Select(1, 0);
	while (!h.Quit())
	{
		h.Select(1,0);
	}
}


