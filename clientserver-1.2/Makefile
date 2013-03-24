VERSION =	1.2

INCLUDE =	-I/usr/local/include/Sockets 
CFLAGS =	-Wall -g -O2 $(INCLUDE) -MD `Sockets-config`
CPPFLAGS =	$(CFLAGS)
LIBS =		-L/usr/local/lib -lSockets -lpthread -lssl -lcrypto

PROGS =		server client server6 client6

all:		$(PROGS)

server:		ServerSocket.o server.o
		g++ -o $@ $^ $(LIBS)

client:		ClientSocket.o client.o
		g++ -o $@ $^ $(LIBS)

server6:	ServerSocket.o server6.o
		g++ -o $@ $^ $(LIBS)

client6:	ClientSocket.o client6.o
		g++ -o $@ $^ $(LIBS)

clean:
		rm -f *.o *~ *.d slask $(PROGS)

install:	all
		install --strip $(PROGS) /usr/local/bin

-include	*.d

docs:
		./mkdot.sh

tar:		clean
		tar czf clientserver-$(VERSION).tar.gz *.h *.cpp Makefile
		/usr/local/bin/tarfix.sh clientserver-$(VERSION)
		cp clientserver-$(VERSION).tar.gz /usr/local/apache/www.alhem.net/htdocs/project/example5


