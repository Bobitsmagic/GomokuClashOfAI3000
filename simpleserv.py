from twisted.internet import reactor, protocol
import json

# [TODO] Figure out if this is the proper global place to place these
verbose = False
activePlayers = []
lobbyPlayers = []
activeMatches = []

def findMatch():
    print("Attempting to find a match")
    if len(lobbyPlayers) >= 2:
        a = lobbyPlayers[0]
        b = lobbyPlayers[1]
        lobbyPlayers.remove(a)
        lobbyPlayers.remove(b)
        activePlayers.append(a)
        activePlayers.append(b)
        createMatch(a, b)

def createMatch(playerA, playerB):
    print("Match started between {} and {}".format(playerA, playerB))

class Server(protocol.Protocol):
    def dataReceived(self, data):
        reply = None

        try:
            data = data.decode()
            data = json.loads(data)
            if verbose: print("Received\n" + str(data))

            if "query" in data:
                if data["query"] == "register":
                    reply = self.processQuery(data)
        except:
            print("Recieved incorrect JSON\n\t" + str(data))
        finally:
            if reply == None:
                reply = json.dumps({"response": "bad"})

            self.sendData(reply)

    def sendData(self, data):
        if verbose: print("Sending\n\t" + data)
        self.transport.write(data.encode())

    def processQuery(self, data):
        action = data["query"]

        if action == "register":
            return self.processQuery_register(data)

        return None

    def processQuery_register(self, data):
        username = data["username"]
        if verbose: print("User attempting to register with name " + username)
        if username not in activePlayers and username not in activePlayers:
            lobbyPlayers.append(username)
            print("User Registered: " + username)
            findMatch()
            return json.dumps({"query": "register", "username": username, "state": "success"})
        else:
            return json.dumps({"query": "register", "username": username, "state": "fail"})

        return None

def main():
    port = 4321
    print("Starting server on port {}".format(port))

    factory = protocol.ServerFactory()
    factory.protocol = Server
    reactor.listenTCP(port, factory)
    reactor.run()

if __name__ == '__main__':
    main()
