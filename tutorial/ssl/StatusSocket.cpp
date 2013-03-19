#ifdef _WIN32
#pragma warning(disable:4786)
#endif
#include <Utility.h>
#include "StatusSocket.h"
#include "stdio.h"


StatusSocket::StatusSocket(ISocketHandler& h)
:TcpSocket(h)
{
}


void StatusSocket::OnAccept()
{
	Send("Local hostname : " + Utility::GetLocalHostname() + "\n");
	Send("Local address : " + Utility::GetLocalAddress() + "\n");
	Send("Number of sockets in list : " + Utility::l2string(Handler().GetCount()) + "\n");
	Send("\n");
}


void StatusSocket::InitSSLServer()
{
	InitializeContext("session_id", "server.pem", "keypwd", SSLv23_method());
}


void StatusSocket::Init()
{
	if (GetParent() -> GetPort() == 2222)
	{
		EnableSSL();
	}
}


