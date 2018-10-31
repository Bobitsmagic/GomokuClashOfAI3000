#!/usr/bin/env python3

import socket
import json
import sys

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

server_address = ("localhost", 9000)
print("Starting server on {} port {}".format(*server_address))
sock.bind(server_address)
sock.listen(1)

activePlayers = []

while True:
    print("Waiting for a connection...\n")
    connection = None

    try:
        connection, client_address = sock.accept()
        print("Connection from " + str(client_address))

        data = connection.recv(1024).decode()
        data = json.loads(data)

        reply = None

        if "action" in data:
            if data["action"] == "register":
                username = data["username"]
                print("User attempting to register with name " + username)
                if username not in activePlayers:
                    activePlayers.append(username)
                    reply = json.dumps({"action": "register", "username": username, "state": "success"})
                else:
                    reply = json.dumps({"action": "register", "username": username, "state": "fail"})

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
