#include <TcpSocket.h>
#include <ISocketHandler.h>

class Tracker : public TcpSocket
{
public:
	Tracker(ISocketHandler& );

	void OnRead();
private:
	int handShake();
	int createRoom(int peerID);
	std::pair<int,int>* listRoom();
	void joinRoom(int peerID, int roomID);
	void quitRoom(int peerID);
	
	void keepAlive();
	int findLowestPeerID();
	
	SocketHandler* peerHandler;
	
};
