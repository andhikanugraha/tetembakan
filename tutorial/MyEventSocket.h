/**
 **	File ......... MyEventSocket.h
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
#include <TcpSocket.h>
#include <IEventOwner.h>
#include "MyEventHandler.h"


class MyEventSocket : public TcpSocket,public IEventOwner
{
public:
	MyEventSocket(ISocketHandler& h) : TcpSocket(h), IEventOwner( static_cast<MyEventHandler&>(h) )
	{
		SetLineProtocol(); // we want to receive line callbacks
	}

	void OnLine(const std::string& line)
	{
		if (line == "event")
		{
			// schedule an event 2 seconds ahead in time
			int id = AddEvent(2, 0);
			Send("Scheduled event id " + Utility::l2string(id) + "\n");
		}
	}

	void OnEvent(int id)
	{
		Send("Event id " + Utility::l2string(id) + "\n");
	}
};

