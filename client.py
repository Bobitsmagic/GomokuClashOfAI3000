#!/usr/bin/env python3

import socket
import json
import sys
import random

class Client:
    def __init__(self, username = "bot", verbose = False):
        self.verbose = verbose
        self.username = username

        self.sock = None
        self.connect("localhost", 9000)
        self.register()

        if self.sock: self.sock.close()

    def connect(self, address, port):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_address = (address, port)
        if self.verbose:
            print("Connecting to {} port {}".format(*server_address))
        self.sock.connect(server_address)

        # [TODO] Add error checking here for if not connected

    def getData(self):
        # [TODO] Determine if 1024 is not long enough for long games
        data = self.sock.recv(1024).decode()
        try:
            data = json.loads(data)
            return data
        except:
            print("Server did not send proper JSON")
            return None

    def register(self):
        self.registered = False
        try:
            message = json.dumps({"query": "register", "username": self.username})
            self.sock.sendall(message.encode())

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

        finally:
            pass

a = Client(username = str(random.randint(0, 10000)), verbose = True)
#b = Client(username = "b", verbose = True)
