//#include <stdio.h>

#include "ServerSocket.h"




ServerSocket::ServerSocket(ISocketHandler& h)
:TcpSocket(h)
{
	SetLineProtocol();
}


ServerSocket::~ServerSocket()
{
}


void ServerSocket::OnLine(const std::string& cmd)
{
	if (cmd == "QUIT")
	{
		Send("BYE\n\n");
		SetCloseAndDelete();
	}
	else
	if (cmd == "TIME")
	{
		time_t t = time(NULL);
		struct tm* tp = localtime(&t);
		char datetime[40];
		sprintf(datetime,"%d-%02d-%02d %02d:%02d:%02d",
			tp -> tm_year + 1900,
			tp -> tm_mon + 1,
			tp -> tm_mday,
			tp -> tm_hour,
			tp -> tm_min,
			tp -> tm_sec);
		std::string reply = "TIME: ";
		reply += datetime;
		reply += "\n";
		for (int i = 0; i < 10; i++)
			Send(reply);
		Send("\n");
	}
	else
	if (cmd == "HELP")
	{
		Send("Commands supported: QUIT TIME HELP\n\n");
	}
	else
	{
		Send("404 Not found\n\n");
	}
}


