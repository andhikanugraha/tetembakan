//#include <stdio.h>

#include "ClientSocket.h"




ClientSocket::ClientSocket(ISocketHandler& h,const std::string& cmd)
:TcpSocket(h)
,m_cmd(cmd)
{
	SetLineProtocol();
}


ClientSocket::~ClientSocket()
{
}


void ClientSocket::OnConnect()
{
	Send(m_cmd + "\n");
}


void ClientSocket::OnLine(const std::string& line)
{
	printf("Reply from server: '%s'\n",line.c_str());
	if (line.size())
	{
	}
	else
	{
		SetCloseAndDelete();
	}
}


