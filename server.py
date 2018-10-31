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
        self.playerA = playerA
        self.playerB = playerB
        self.stones = {}
        self.stones[self.playerA] = playerAStone
        self.stones[self.playerB] = playerBStone
        if self.stones[self.playerA] == "B": self.currentPlayer = self.playerA
        if self.stones[self.playerB] == "B": self.currentPlayer = self.playerB

        self.history = ""

        thread = threading.Thread(target = self.run, args = ())
        thread.daemon = True
        thread.start()

    def run(self):
        time.sleep(1)   # [TODO] This is a bad hack to sync client response, do it correctly instead
        self.requestMove()

    def requestMove(self):
        sockets[self.currentPlayer].move(self)

    def processMove(self, data):
        player = data["username"]
        move = data["move"]

        if player == self.currentPlayer:
            # [TODO] Have board check if it's a valid move
            self.history = self.history + move
            print("Game history is now " + self.history)
            if False: pass
            elif self.currentPlayer == self.playerA: self.currentPlayer = self.playerB
            elif self.currentPlayer == self.playerB: self.currentPlayer = self.playerA
            self.requestMove()
        else:
            print("Incorrect move response of\n\t" + str(data))

        #print("match got " + str(data))

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
            sockets[playerA].joinMatch(match)
            sockets[playerB].joinMatch(match)
            activeMatches.append(match)

class Server(protocol.Protocol):
    def __init__(self):
        self.match = None

    def dataReceived(self, data):
        try:
            data = data.decode()
            data = json.loads(data)
            if verbose > 1: print("Received\n\t" + str(data))

            if "query" in data:
                self.processQuery(data)
        except:
            print("Recieved incorrect JSON, or could not process it\n\t" + str(data))

    def sendData(self, data):
        if verbose > 1: print("Sending\n\t" + str(data))
        data = json.dumps(data)
        data = data.encode()
        self.transport.write(data)

    def processQuery(self, data):
        query = data["query"]

        if query == "register":
            self.processQuery_register(data)
        if query == "move":
            self.processQuery_move(data)

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

    def processQuery_move(self, data):
        self.match.processMove(data)

    def joinMatch(self, match):
        self.match = match
        self.sendData({"query" : "match", "players" : [match.playerA, match.playerB], match.playerA : match.stones[match.playerA], match.playerB : match.stones[match.playerB]})

    def move(self, match):
        self.sendData({"query" : "move", "stone" : match.stones[match.currentPlayer]})

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
