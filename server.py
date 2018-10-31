#!/usr/bin/env python3

import socket
import json
import sys

class Server:
    def __init__(self):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_address = ("localhost", 9000)
        print("Starting server on {} port {}".format(*server_address))
        self.sock.bind(server_address)
        self.sock.listen(1)

        self.activePlayers = []
        self.activeGames = []

    def run(self):
        while True:
            print("Waiting for a connection...\n")
            connection = None

            try:
                connection, client_address = self.sock.accept()
                # [TODO] Associate this connection with whatever use that is
                # This allows the server to close broken connections
                print("Connection from " + str(client_address))

                # [TODO] Determine if 1024 is not long enough for long games
                data = connection.recv(1024).decode()
                data = json.loads(data)

                reply = None

                if "query" in data:
                    if data["query"] == "register":
                        reply = self.processQuery(data)

                if reply == None:
                    print("Failed to process request: " + str(data))
                    reply = json.dumps({"response": "bad"})

                print("Sending: " + reply)
                connection.sendall(reply.encode())

            except KeyboardInterrupt:
                print("Interrupt received, stopping...")
                if connection: connection.close()
                break

            finally:
                if connection: connection.close()

    def processQuery(self, data):
        action = data["query"]

        if action == "register":
            return self.processQuery_register(data)

        return None

    def processQuery_register(self, data):
        username = data["username"]
        print("User attempting to register with name " + username)
        if username not in self.activePlayers:
            self.activePlayers.append(username)
            return json.dumps({"query": "register", "username": username, "state": "success"})
        else:
            return json.dumps({"query": "register", "username": username, "state": "fail"})

        return None

server = Server()
server.run()
