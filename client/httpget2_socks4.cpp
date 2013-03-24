/**
 **	File ......... httpget2_socks4.cpp
 **	Published ....  2004-04-07
 **	Author ....... grymse@alhem.net
**/
/*
Copyright (C) 2004  Anders Hedstrom

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
#ifdef _WIN32
#define strcasecmp stricmp
#endif
#include <HttpGetSocket.h>
#include <SocketHandler.h>


void Get(SocketHandler& h,const std::string& url_in)
{
	HttpGetSocket *s = new HttpGetSocket(h, url_in);
	s -> SetDeleteByHandler();
	h.Add(s);
}


int main(int argc,char *argv[])
{
	SocketHandler h;

	// enable socks4 client
#ifdef ENABLE_SOCKS4
	h.SetSocks4Host("somehost.com");
	h.SetSocks4Port(1080);
	h.SetSocks4Userid("myname.net");
	h.SetSocks4TryDirect(false);
#endif

	for (int i = 1; i < argc; i++)
	{
		Get(h,argv[i]);
	}
	h.Select(1,0);
	while (h.GetCount())
	{
		h.Select(1,0);
	}
}


