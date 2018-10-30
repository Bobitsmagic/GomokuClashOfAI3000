#!/usr/bin/env python3

import random
from datetime import datetime
random.seed(datetime.now())

from .Board import Board
from .Tactic import Tactic

E = "E"
B = "B"
W = "W"

tactic = Tactic()

class AI:
    def __init__(self, board, stone, saveFile):
        self.board = board
        self.stone = stone
        self.saveFile = saveFile

    def move(self):
        #print("[GOGOGO]: About to move with history: " + self.board.history)
        status = None
        # Always prioritize the middle
        # [TODO] If middle isn't available attempt to play close instead
        x = int(float(self.board.w) / 2)
        y = int(float(self.board.h) / 2)
        status = self.board.place(x, y, self.stone)

        # Attempt a tactic
        if status == None:
            recommendation = tactic.recommend(self.board, self.stone)
            if recommendation is not None:
                status = self.board.place(recommendation[0], recommendation[1], self.stone)

        # Otherwise do something random
        if status == None:
            x = random.randint(0, self.board.w)
            y = random.randint(0, self.board.h)
            status = self.board.place(x, y, self.stone)

        with open(self.saveFile, "w") as fh:
            #print("[GOGOGO]: Writing this history: " + self.board.history)
            fh.write(self.board.history)

        if status == None:
            return self.move()
