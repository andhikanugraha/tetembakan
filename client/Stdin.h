/**
 **	File ......... Stdin.h
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
#ifndef _STDIN_H
#define _STDIN_H

#include "Socket.h"


class Stdin : public Socket
{
public:
	Stdin(ISocketHandler&,size_t bufsize = 1000);

	/** characters from stdin, including crlf's */
	virtual void OnData(const char *,size_t) = 0;

protected:
	void OnRead();

private:
	size_t m_bufsize;
};


#endif // _STDIN_H
