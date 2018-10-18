#!/usr/bin/env python3

from AI import AI
from Board import Board
from Tactic import Tactic

E = "E"
B = "B"
W = "W"

board = Board()
b = AI(board, B)
w = AI(board, W)

turn = 0
turns = 1000
winner = None
while winner == None and turn < turns:
    for ai in [b, w]:
        ai.move()
        ai.board.render()
        print()

        if ai.board.winner != None:
            winner = ai
            break

    turn += 1

if winner != None:
    print(winner.stone + " wins!")
    print("History: {}".format(winner.board.history))

tactic = Tactic()
tactic.render()
