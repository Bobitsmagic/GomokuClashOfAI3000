#!/usr/bin/env python3

import random

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

        fh = open(self.saveFile, "w")
        fh.write(self.board.history)
        fh.close()

        if status == None:
            return self.move()
