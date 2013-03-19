#ifndef _STATUSSOCKET_H
#define _STATUSSOCKET_H

#include <TcpSocket.h>
#include <ISocketHandler.h>


class StatusSocket : public TcpSocket
{
public:
	StatusSocket(ISocketHandler& );

	void Init();

	void OnAccept();
	void InitSSLServer();
};


#endif // _STATUSSOCKET_H
