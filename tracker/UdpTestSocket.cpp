/**
 **	File ......... UdpTestSocket.cpp
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
#include <Utility.h>
#include "UdpTestSocket.h"
#include "stdio.h"


UdpTestSocket::UdpTestSocket(ISocketHandler& h)
:UdpSocket(h)
{
}


void UdpTestSocket::OnRawData(const char *p,size_t l,struct sockaddr *sa_from,socklen_t sa_len)
{
	if (sa_len == sizeof(struct sockaddr_in)) // IPv4
	{
		struct sockaddr_in sa;
		memcpy(&sa,sa_from,sa_len);
		ipaddr_t a;
		memcpy(&a,&sa.sin_addr,4);
		std::string ip;
		Utility::l2ip(a,ip);
		printf("Received %d bytes from: %s:%d\n", l,ip.c_str(),ntohs(sa.sin_port));
		printf("%s\n",static_cast<std::string>(p).substr(0,l).c_str());
	}
#ifdef ENABLE_IPV6
#ifdef IPPROTO_IPV6
	else
	if (sa_len == sizeof(struct sockaddr_in6)) // IPv6
	{
		struct sockaddr_in6 sa;
		memcpy(&sa,sa_from,sa_len);
		std::string ip;
		Utility::l2ip(sa.sin6_addr,ip);
		printf("(IPv6) Received %d bytes from: %s:%d\n", l,ip.c_str(),ntohs(sa.sin6_port));
		printf("%s\n",static_cast<std::string>(p).substr(0,l).c_str());
	}
#endif
#endif
}
