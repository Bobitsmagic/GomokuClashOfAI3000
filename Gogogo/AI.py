#!/usr/bin/env python3

import random

from Board import Board
from Tactic import Tactic

E = "E"
B = "B"
W = "W"

tactic = Tactic()

class AI:
    def __init__(self, board, stone):
        self.board = board
        self.stone = stone

    def move(self):
        # Always prioritize the middle
        # [TODO] If middle isn't available attempt to play close instead
        x = int(float(self.board.w) / 2)
        y = int(float(self.board.h) / 2)
        status = self.board.place(x, y, self.stone)
        if status != None:
            return status

        # Attempt a tactic
        recommendation = tactic.recommend(self.board, self.stone)
        if recommendation is not None:
            self.board.place(recommendation[0], recommendation[1], self.stone)

        # Otherwise do something random
        x = random.randint(0, self.board.w)
        y = random.randint(0, self.board.h)
        status = self.board.place(x, y, self.stone)
        if status != None:
            return status
        else:
            self.move()
