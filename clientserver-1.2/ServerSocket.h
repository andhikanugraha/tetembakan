#ifndef _SERVERSOCKET_H
#define _SERVERSOCKET_H

#include <TcpSocket.h>
#include <ISocketHandler.h>


class ServerSocket : public TcpSocket
{
public:
	ServerSocket(ISocketHandler& );
	~ServerSocket();

	void OnLine(const std::string& );
};




#endif // _SERVERSOCKET_H
