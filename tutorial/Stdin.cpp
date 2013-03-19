/**
 **	File ......... Stdin.cpp
 **	Published ....  2004-07-03
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
#include <errno.h>
#include "ISocketHandler.h"
#include "Stdin.h"
#ifdef _WIN32
#include <io.h>
#endif


Stdin::Stdin(ISocketHandler& h,size_t bufsize) : Socket(h)
,m_bufsize(bufsize)
{
	Attach( STDIN_FILENO );
	Handler().ISocketHandler_Add(this, true, false);
}


void Stdin::OnRead()
{
	char buf[m_bufsize];
	int n = read(GetSocket(), buf, m_bufsize - 1); //recv(0, buf, 1000, 0);
	if (n == -1)
	{
		Handler().LogError(this, "OnRead", errno, strerror(errno), LOG_LEVEL_FATAL);
		SetCloseAndDelete();
		return;
	}
	OnData(buf, n);
}


