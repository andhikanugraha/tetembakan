/**
 **	File ......... threadsafe.cpp
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
#include <stdio.h>
#include <SocketHandler.h>
#include <TcpSocket.h>
#include <Mutex.h>
#include <StdoutLog.h>
#include <ListenSocket.h>
#include <Lock.h>


class InfoSocket : public TcpSocket
{
public:
	InfoSocket(ISocketHandler& h) : TcpSocket(h) {}
	~InfoSocket() {}

	void OnAccept() {
		Send("Welcome, " + GetRemoteAddress() + "\n");
	}
};


class MyHandler : public SocketHandler
{
public:
	MyHandler(IMutex& m,StdoutLog *p) : SocketHandler(m, p) {}
	~MyHandler() {}

	void SendInfo(const std::string& msg) {
		for (socket_m::iterator it = m_sockets.begin(); it != m_sockets.end(); it++)
		{
			InfoSocket *p = dynamic_cast<InfoSocket *>((*it).second);
			if (p)
			{
				p -> Send(msg);
			}
		}
	}
};


class MySocket : public TcpSocket
{
public:
	MySocket(ISocketHandler& h) : TcpSocket(h) {
		SetLineProtocol();
	}
	~MySocket() {}

	void OnAccept() {
		Send("Welcome, " + GetRemoteAddress() + "\n");
#ifdef ENABLE_DETACH
		if (!Detach())
		{
			SetCloseAndDelete();
		}
#endif
	}

#ifdef ENABLE_DETACH
	void OnDetached() {
		Lock lck(MasterHandler().GetMutex());
printf("OnDetached()\n");
		static_cast<MyHandler&>(MasterHandler()).SendInfo(GetRemoteAddress() + ": New connection\n");
	}
#endif

	void OnLine(const std::string& line) {
		Lock lck(MasterHandler().GetMutex());
		static_cast<MyHandler&>(MasterHandler()).SendInfo(GetRemoteAddress() + ": " + line + "\n");
	}
};


int main(int argc,char *argv[])
{
	Mutex lock;
	StdoutLog log;
	MyHandler h(lock, &log);
	ListenSocket<MySocket> l(h);
	if (l.Bind(11001))
	{
		printf("Bind() failed\n");
		exit(-1);
	}
	h.Add(&l);
	ListenSocket<InfoSocket> l2(h);
	if (l2.Bind(11002))
	{
		printf("Bind() failed\n");
		exit(-2);
	}
	h.Add(&l2);
	bool quit = false;
	while (!quit)
	{
		h.Select(1, 0);
	}
}


