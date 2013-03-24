/**
 **	File ......... StdinLine.h
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
#ifndef _STDINLINE_H
#define _STDINLINE_H

#include "Stdin.h"


class StdinLine : public Stdin
{
public:
	StdinLine(ISocketHandler&,size_t bufsize = 1000);

	/** characters from stdin, including crlf's */
	virtual void OnLine(const std::string&);

protected:
	void OnRead();

private:
	size_t m_bufsize;
	std::string m_line;
};


#endif // _STDINLINE_H
