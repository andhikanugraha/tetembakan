INCLUDE =	-I../socket
CFLAGS =	-Wall -g -O2 $(INCLUDE) -MD -D_VERSION='"2.3.9.7"' 
CPPFLAGS =	$(CFLAGS) 
LIBS =		-L/usr/local/lib -lpthread

PROGS =		displayserver \
		displayclient \
		statusserver \
		customserver \
		server_errlog \
		put_demo \
		getportdemo \
		pooltest \
		udpserver \
		udpclient \
		httpget2_async_dns \
		httpget2_socks4 \
		resume \
		event \
		threadsafe \
		sslserver \
		sslclient \
		telnet \
		copy \
		send_test

all: 		$(PROGS)

displayserver:	displayserver.o DisplaySocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

displayclient:	displayclient.o DisplaySocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

statusserver:	statusserver.o StatusSocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

customserver:	customserver.o StatusSocket.o StatusHandler.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

server_errlog:	server_errlog.o DisplaySocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

put_demo:	put_demo.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

getportdemo:	getportdemo.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

pooltest:	pooltest.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

udpserver:	udpserver.o UdpTestSocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

udpclient:	udpclient.o UdpTestSocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

httpget2_async_dns: httpget2_async_dns.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

httpget2_socks4: httpget2_socks4.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

resume:		resume.o ResumeSocket.o ResumeSocket2.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

event:		event.o MyEventHandler.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

threadsafe:	threadsafe.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

sslserver:	ssl/statusserver.o ssl/StatusSocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

sslclient:	ssl/sslclient.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

telnet:		telnet.o StdinSocket.o Console.o Stdin.o Stdin2.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

copy:		copy.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

send_test:	send_test.o DisplaySocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

receive_test:	receive_test.o DisplaySocket.o ../socket/libSockets.a 
		g++ -o $@ $^ $(LIBS)

clean:
		rm -f *.o *~ slask $(PROGS) */*~ *.d *.bak */*.o */*~


FILES =		*Socket.h *Socket.cpp \
		*Handler.h *Handler.cpp \
		*server.cpp \
		*client.cpp \
		server_errlog.cpp \
		Stdin.h Stdin.cpp \
		put_demo.cpp \
		getportdemo.cpp \
		pooltest.cpp \
		httpget2_*.cpp \
		resume.cpp \
		MyEvent*.h MyEvent*.cpp \
		event.cpp \
		threadsafe.cpp \
		Makefile \
		Project/*.dsp \
		ssl/*.h ssl/*.cpp \
		ResumeSocket2.h ResumeSocket2.cpp \
		telnet.cpp copy.cpp send_test.cpp \
		StdinLine.h Console.h \
		StdinLine.cpp Console.cpp \
		Stdin2.h Stdin2.cpp

tar:		clean
		tar czf Sockets-tutorial.tar.gz $(FILES)
		/usr/local/bin/tarfix.sh Sockets-tutorial

-include	*.d
