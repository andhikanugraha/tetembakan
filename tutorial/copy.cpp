/**
 **	File ......... copy.cpp
 **	Published ....  2007-10-09
 **	Author ....... grymse@alhem.net
**/
/*
Copyright (C) 2007  Anders Hedstrom

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
#include <TcpSocket.h>
#include <SocketHandler.h>
#include <ListenSocket.h>
#include <Utility.h>

#define BUFSZ 60000

static	bool quit = false;


class CopySocket : public TcpSocket
{
public:
	CopySocket(ISocketHandler& h) : TcpSocket(h, BUFSZ, BUFSZ), m_out(NULL), m_b_server(false) {}
	CopySocket(ISocketHandler& h, const std::string& filename) : TcpSocket(h, BUFSZ, BUFSZ), m_filename(filename), m_out(NULL), m_b_server(false) {}
	~CopySocket() {}

	void OnConnect() {
		Utility::GetTime(&m_start);
		size_t x = 0;
		for (size_t i = 0; i < m_filename.size(); i++)
			if (m_filename[i] == '/')
				x = i + 1;
		Send(m_filename.substr(x) + "\n");
		FILE *fil = fopen(m_filename.c_str(), "rb");
		if (fil)
		{
			char buf[BUFSZ];
			int n = fread(buf, 1, BUFSZ, fil);
			while (n > 0)
			{
				SendBuf(buf, n);
				n = fread(buf, 1, BUFSZ, fil);
			}
			fclose(fil);
		}
		SetCloseAndDelete();
	}

	void OnAccept() {
		Utility::GetTime(&m_start);
		m_b_server = true;
		SetLineProtocol();
		DisableInputBuffer();
	}

	void OnLine(const std::string& line) {
		m_out = fopen(line.c_str(), "wb");
		SetLineProtocol(false);
		DisableInputBuffer();
	}

	void OnDelete() {
		struct timeval stop;
		Utility::GetTime(&stop);
		stop.tv_sec -= m_start.tv_sec;
		stop.tv_usec -= m_start.tv_usec;
		if (stop.tv_usec < 0)
		{
			stop.tv_usec += 1000000;
			stop.tv_sec -= 1;
		}
		double t = stop.tv_usec;
		t /= 1000000;
		t += stop.tv_sec;
		printf("OnDelete: %s\n", m_b_server ? "SERVER" : "CLIENT");
		printf("  Time: %ld.%06ld (%f)\n", stop.tv_sec, stop.tv_usec, t);
		double r = GetBytesReceived();
		printf("  bytes in: %lld (%f Mbytes/sec)\n", GetBytesReceived(), r / t / 1000000);
		double s = GetBytesSent();
		printf("  bytes out: %lld (%f Mbytes/sec)\n", GetBytesSent(), s / t / 1000000);
		printf("\n");
		if (m_out)
			fclose(m_out);
		if (!m_b_server)
			quit = true;
	}

	void OnRawData(const char *buf, size_t len) {
		if (m_out)
			fwrite(buf, 1, len, m_out);
	}

private:
	std::string m_filename;
	FILE *m_out;
	bool m_b_server;
	struct timeval m_start;
};


int main(int argc, char *argv[])
{
	std::string host = "127.0.0.1";
	int port = 12344;
	std::string filename;

	for (int i = 1; i < argc; i++)
	{
		if (!strcmp(argv[i], "-host") && i < argc - 1)
			host = argv[++i];
		else
		if (!strcmp(argv[i], "-port") && i < argc - 1)
			port = atoi(argv[++i]);
		else
		if (!strcmp(argv[i], "-h"))
		{
			fprintf(stderr, "Usage: %s [-host <host>] [-port <port>] [<file to send>]\n", *argv);
			fprintf(stderr, "  Will run as host only if <file to send> isn't specified.\n");
			fprintf(stderr, "  host default: 127.0.0.1\n");
			fprintf(stderr, "  port default: 12344\n");
			return 0;
		}
		else
			filename = argv[i];
	}

	SocketHandler h;
	ListenSocket<CopySocket> l(h);
	if (l.Bind( port ) != 0)
	{
		fprintf(stderr, "Bind() port %d failed - exiting\n", port);
		return -1;
	}
	h.Add(&l);
	if (filename.size())
	{
		CopySocket *sock = new CopySocket(h, filename);
		sock -> SetDeleteByHandler();
		sock -> Open(host, port);
		h.Add(sock);
	}
	else
	{
		fprintf(stderr, "Starting as server only, listening on port %d\n", port);
	}
	while (!quit)
	{
		h.Select(5, 0);
	}
	return 0;
}


