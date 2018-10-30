#!/usr/bin/env python3

import os

from Gogogo.Board import Board
from Gogogo.AI import AI

saveFile = "../game.gom"
saveFile = os.path.abspath(saveFile)
with open(saveFile, "w") as fh: fh.write("")

# [TODO] Make this randomly select between stone colors instead of hardcoded
board = Board()
gogogo = AI(board, "B", saveFile = saveFile)
HansAI = AI(board, "W", saveFile = saveFile)
b = gogogo
w = HansAI

turn = 0
turns = -1
winner = None
board.render(history = False)
while winner == None and (turn < turns or turns == -1):
    gameState = open(saveFile, "r").read()

    b.move()
    playerMoved = False
    while playerMoved == False:
        currentState = open(saveFile, "r").read()
        if currentState != gameState:
            playerMoved = True

    w.move()
    playerMoved = False
    while playerMoved == False:
        currentState = open(saveFile, "r").read()
        if currentState != gameState:
            playerMoved = True

    board.render()

    if board.winner != None:
        winner = board.winner
        break

    turn += 1
