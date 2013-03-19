/**
 **	File ......... DisplayRawSocket.cpp
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
#include "DisplayRawSocket.h"

DisplayRawSocket::DisplayRawSocket(ISocketHandler& h) : TcpSocket(h)
{
}


void DisplayRawSocket::OnRawData(const char *buf,size_t len)
{
	printf("Read %d bytes:\n",len);
	for (size_t i = 0; i < len; i++)
	{
		printf("%c",isprint(buf[i]) ? buf[i] : '.');
	}
	printf("\n");
}
