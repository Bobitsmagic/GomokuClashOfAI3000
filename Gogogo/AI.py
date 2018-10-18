#!/usr/bin/env python3

import random

from Board import Board

E = "E"
B = "B"
W = "W"

class AI:
    def __init__(self, board, stone):
        self.board = board
        self.stone = stone

    def move(self):
        # Always prioritize the middle
        x = int(float(self.board.w) / 2)
        y = int(float(self.board.h) / 2)
        status = self.board.place(x, y, self.stone)
        if status != None:
            return status

        x = random.randint(0, self.board.w)
        y = random.randint(0, self.board.h)
        status = self.board.place(x, y, self.stone)
        if status != None:
            return status
        else:
            self.move()
