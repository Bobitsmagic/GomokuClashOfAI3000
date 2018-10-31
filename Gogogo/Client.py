from __future__ import print_function
from twisted.internet import reactor, protocol
import json, time, random

port = 4321
verbose = 1

class Client(protocol.Protocol):
    def connectionMade(self):
        self.username = str(random.randint(0, 10000))
        self.registered = False
        self.register()

        self.inMatch = False
        self.matchStones = {}

    def dataReceived(self, data):
        # [TODO] Find a proper place to close this out
        #self.transport.loseConnection()

        try:
            data = data.decode()
            data = json.loads(data)
            if verbose > 1: print("Received\n\t" + str(data))

            if "query" in data:
                return self.processQuery(data)
        except:
            print("Recieved incorrect JSON, or could not process it\n\t" + str(data))
        finally:
            pass

    def sendData(self, data):
        if verbose > 1: print("Sending\n\t" + str(data))
        data = json.dumps(data)
        data = data.encode()
        self.transport.write(data)

    def processQuery(self, data):
        query = data["query"]

        if query == "register":
            self.processQuery_register(data)
        if query == "match":
            self.processQuery_match(data)
        if query == "move":
            self.processQuery_move(data)

    def connectionLost(self, reason):
        print("Connection lost")

    def register(self):
        if self.registered:
            print("[ERROR] Cannot register twice!")
            return

        self.sendData({"query": "register", "username": self.username})

    def processQuery_register(self, data):
        username = data["username"]
        if username != self.username: return
        state = data["state"]
        if state != "success": return

        self.registered = True
        print("Registered as: " + username)

    def processQuery_match(self, data):
        self.inMatch = True

        players = data["players"]
        for player in players:
            stone = data[player]
            self.matchStones[player] = stone

        # [TODO] Get this to render all information of the match
        print("Entered match ({})".format(self.matchStones[self.username]))

    def processQuery_move(self, data):
        # [TODO] Here is where we interface with the A.I.
        move = "GG"
        print("Making move " + move)
        self.sendData({"query" : "move", "username" : self.username, "stone" : self.matchStones[self.username], "move" : move})

class Factory(protocol.ClientFactory):
    protocol = Client

    def clientConnectionFailed(self, connector, reason):
        print("Connection failed - goodbye!")
        reactor.stop()

    def clientConnectionLost(self, connector, reason):
        print("Connection lost - goodbye!")
        reactor.stop()

def main():
    f = Factory()
    reactor.connectTCP("localhost", port, f)
    reactor.run()

if __name__ == '__main__':
    main()
