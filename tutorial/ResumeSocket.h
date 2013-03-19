/**
 **	File ......... ResumeSocket.h
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
#ifndef _RESUMESOCKET_H
#define _RESUMESOCKET_H

#include <TcpSocket.h>
#include <ISocketHandler.h>


class ResumeSocket : public TcpSocket
{
public:
	ResumeSocket(ISocketHandler&);
	~ResumeSocket();

	/** When a connect is made, we set the m_b_connected flag */
	void OnConnect();
	/** When the connection attempt fails, we try to reconnect with a new socket instance. 
	    This instance will be deleted, so remember to copy member variables. */
	void OnConnectFailed();
	/** If we are connected (m_b_connected is true), we Reconnect again. */
	void OnDelete();

private:
	ResumeSocket(const ResumeSocket& s) : TcpSocket(s) {} // copy constructor
	ResumeSocket& operator=(const ResumeSocket& ) { return *this; } // assignment operator
	/** Create a new instance and reconnect */
	ResumeSocket *Reconnect();
	bool m_b_connected;
};




#endif // _RESUMESOCKET_H
