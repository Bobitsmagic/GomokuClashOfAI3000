from __future__ import print_function
from twisted.internet import reactor, protocol
import json, time, random

port = 4321

class Client(protocol.Protocol):
    def connectionMade(self):
        self.username = str(random.randint(0, 10000))
        self.registered = False
        self.register()

    def dataReceived(self, data):
        print("Got\n\t" + str(data))
        # [TODO] Find a proper place to close this out
        #self.transport.loseConnection()

    def connectionLost(self, reason):
        print("Connection lost")

    def register(self):
        message = json.dumps({"query": "register", "username": self.username})
        self.transport.write(message.encode())

        """
        data = self.getData()
        if data == None:
            return False

        if "query" not in data:
            if self.verbose:
                print("Failed to receive proper reply from server:\n\t" + str(data))
            return False

        if data["query"] == "register" and data["state"] == "success":
            if self.verbose:
                print("Registered user " + self.username)
            self.registered = True
        else:
            if self.verbose:
                print("Unable to register user " + self.username)
            return False
        """

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
