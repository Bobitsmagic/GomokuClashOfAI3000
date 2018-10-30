#!/usr/bin/env python3

from AI import AI
from Board import Board

E = "E"
B = "B"
W = "W"

board = Board()
b = AI(board, B)
w = AI(board, W)
ais = [b, w]

saveFile = "../../game.gom"
with open(saveFile, "w") as fh: fh.write("")

turn = 0
turns = -1
winner = None
while winner == None and (turn < turns or turns == -1):
    for ai in ais:
        oldState = open(saveFile, "r").read()

        ai.move()
        ai.board.render()
        print()

        fh = open(saveFile, "w")
        fh.write(board.history)
        fh.close()

        if ai.board.winner != None:
            winner = ai
            break

        stateChanged = False
        while stateChanged is False:
            newState = open(saveFile, "r").read()
            if oldState != newState:
                stateChanged = True

    turn += 1

if winner != None:
    print(winner.stone + " wins!")
    print("History: {}".format(winner.board.history))
