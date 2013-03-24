#ifndef _CLIENTSOCKET_H
#define _CLIENTSOCKET_H

#include <ISocketHandler.h>
#include <TcpSocket.h>


class ClientSocket : public TcpSocket
{
public:
	ClientSocket(ISocketHandler& ,const std::string& );
	~ClientSocket();

	void OnConnect();
	void OnLine(const std::string& );

private:
	std::string m_cmd;
};




#endif // _CLIENTSOCKET_H
