/**
 **	File ......... ResumeSocket.cpp
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
#include "ResumeSocket.h"




ResumeSocket::ResumeSocket(ISocketHandler& h)
:TcpSocket(h)
,m_b_connected(false)
{
	// initial connection timeout setting
	SetConnectTimeout(15);
}


ResumeSocket::~ResumeSocket()
{
}


void ResumeSocket::OnConnect()
{
	m_b_connected = true;
}


ResumeSocket *ResumeSocket::Reconnect()
{
	std::auto_ptr<SocketAddress> ad = GetClientRemoteAddress();
	ResumeSocket *p = new ResumeSocket(Handler());
	p -> SetDeleteByHandler();
	p -> Open(*ad);
	Handler().Add(p);
	return p;
}


void ResumeSocket::OnConnectFailed()
{
	ResumeSocket *p = Reconnect();
	// modify connection timeout setting
	p -> SetConnectTimeout(5);
}


void ResumeSocket::OnDelete()
{
	if (m_b_connected)
	{
		Reconnect();
	}
}


