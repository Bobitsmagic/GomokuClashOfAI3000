#!/usr/bin/env python3

import os
import time
import subprocess

from Gogogo.Board import Board
from Gogogo.AI import AI

saveFile = "../game.gom"
saveFile = os.path.abspath(saveFile)

with open(saveFile, "w") as fh: fh.write("")

# [TODO] Make this randomly select between stone colors instead of hardcoded
gogogo_board = Board()
#gogogo = AI(gogogo_board, "B", saveFile = saveFile, verbose = True)
gogogo = AI(gogogo_board, "B", saveFile = saveFile, verbose = True)
#HansAI = AI(gogogo_board, "W", saveFile = saveFile)

HansAI_loc = os.path.abspath("./HansAI/bin/Release/Gomoku.exe")
difficulty = 100
command = HansAI_loc + " " + saveFile + " " + "W" + " " + str(difficulty)
process = subprocess.Popen(command, shell = True)
HansAI = None

b = gogogo
w = HansAI

turn = 0
turns = -1
winner = None
while winner == None and (turn < turns or turns == -1):
    gameState = open(saveFile, "r").read()

    """
    start = os.stat(saveFile).st_mtime
    if b is not None: b.move()
    playerMoved = False
    while playerMoved == False:
        now = os.stat(saveFile).st_mtime
        if now - start != 0: playerMoved = True
    with open(saveFile, "r") as fh: gameState = fh.read()
    if b is not None:
        board = Board()
        board.processCode(gameState)
        b.board = board

    start = os.stat(saveFile).st_mtime
    if w is not None: w.move()
    playerMoved = False
    while playerMoved == False:
        now = os.stat(saveFile).st_mtime
        if now - start != 0: playerMoved = True
    with open(saveFile, "r") as fh: gameState = fh.read()
    if w is not None:
        board = Board()
        board.processCode(gameState)
        w.board = board

    board = Board()
    board.processCode(gameState)
    board.render()
    """

    # [TODO] Make this work elegantly so that we can randomize B/W initializing

    b.move()
    with open(saveFile, "r") as fh: gameState = fh.read()

    start = os.stat(saveFile).st_mtime
    playerMoved = False
    while playerMoved == False:
        try:
            now = os.stat(saveFile).st_mtime
        except Exception:
            pass
        if now - start != 0: playerMoved = True

    with open(saveFile, "r") as fh: gameState = fh.read()
    board = Board()
    board.processCode(gameState)
    b.board = board

    board.render()

    if board.winner != None:
        winner = board.winner
        break

    turn += 1

if winner == b:
    print("Black is the victor!")
else:
    print("White is the victor!")