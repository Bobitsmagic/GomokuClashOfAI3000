from twisted.internet import reactor, protocol
import json, time, threading

# [TODO] Figure out if this is the proper global place to place these
verbose = 1
activePlayers = []
lobbyPlayers = []
activeMatches = []

sockets = {}

class Match(object):
    def __init__(self, playerA, playerAStone, playerB, playerBStone):
        sockets[playerA].createMatch(playerA, playerAStone, playerB, playerBStone, playerB)
        sockets[playerB].createMatch(playerA, playerAStone, playerB, playerBStone, playerA)

        self.playerA = playerA
        self.playerAStone = playerAStone
        self.playerB = playerB
        self.playerBStone = playerBStone
        if self.playerAStone == "B": self.currentPlayer = self.playerA
        if self.playerBStone == "B": self.currentPlayer = self.playerB

        thread = threading.Thread(target = self.run, args = ())
        thread.daemon = True
        thread.start()

    def run(self):
        print("Match started!!!")

class MatchmakingSystem(object):
    def __init__(self, interval = 0.5):
        self.interval = interval

        thread = threading.Thread(target = self.run, args = ())
        thread.daemon = True
        thread.start()

    def run(self):
        print("Matchmaking System has started searching for matches at interval of {} seconds".format(self.interval))
        while True:
            self.findMatch()
            time.sleep(self.interval)

    def findMatch(self):
        if len(lobbyPlayers) >= 2:
            playerA = lobbyPlayers[0]
            playerB = lobbyPlayers[1]
            lobbyPlayers.remove(playerA)
            lobbyPlayers.remove(playerB)
            activePlayers.append(playerA)
            activePlayers.append(playerB)

            print("Match found between {} and {}".format(playerA, playerB))
            # [TODO] Make this randomized
            playerAStone = "B"
            playerBStone = "W"
            match = Match(playerA, playerAStone, playerB, playerBStone)

class Server(protocol.Protocol):
    def dataReceived(self, data):
        reply = None

        try:
            data = data.decode()
            data = json.loads(data)
            if verbose > 1: print("Received\n\t" + str(data))

            if "query" in data:
                if data["query"] == "register":
                    reply = self.processQuery(data)
        except:
            print("Recieved incorrect JSON\n\t" + str(data))
        finally:
            if reply == None:
                reply = {"response": "bad"}

            self.sendData(reply)

    def sendData(self, data):
        if verbose > 1: print("Sending\n\t" + str(data))
        data = json.dumps(data)
        data = data.encode()
        self.transport.write(data)

    def processQuery(self, data):
        query = data["query"]

        if query == "register":
            return self.processQuery_register(data)

        return None

    def processQuery_register(self, data):
        username = data["username"]
        if verbose > 1: print("User attempting to register with name " + username)
        if username not in activePlayers and username not in activePlayers:
            lobbyPlayers.append(username)
            sockets[username] = self
            print("User Registered: " + username)
            return {"query": "register", "username": username, "state": "success"}
        else:
            return {"query": "register", "username": username, "state": "fail"}

        return None

    def createMatch(self, playerA, playerAStone, playerB, playerBStone, opponent):
        self.sendData({"query" : "match", playerA : playerAStone, playerB : playerBStone, "opponent" : opponent})

def main():
    port = 4321
    print("Starting server on port {}".format(port))
    matchmakingSystem = MatchmakingSystem()

    factory = protocol.ServerFactory()
    factory.protocol = Server
    reactor.listenTCP(port, factory)
    reactor.run()

if __name__ == '__main__':
    main()
