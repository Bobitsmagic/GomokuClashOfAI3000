#!/usr/bin/env python3

import os
import subprocess

from Gogogo.Board import Board
from Gogogo.AI import AI

saveFile = "../game.gom"
saveFile = os.path.abspath(saveFile)
with open(saveFile, "w") as fh: fh.write("")

# [TODO] Make this randomly select between stone colors instead of hardcoded
gogogo_board = Board()
gogogo = AI(gogogo_board, "B", saveFile = saveFile)
#HansAI = AI(gogogo_board, "W", saveFile = saveFile)

HansAI_loc = os.path.abspath("./HansAI/bin/Release/Gomoku.exe")
command = HansAI_loc + " " + saveFile + " " + "W"
process = subprocess.Popen(command, shell = True)
HansAI = None

b = gogogo
w = HansAI

turn = 0
turns = 1
winner = None
board = Board()
board.render(history = False)
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

    b.move()
    with open(saveFile, "r") as fh: gameState = fh.read()
    board = Board()
    board.processCode(gameState)
    b.board = board

    start = os.stat(saveFile).st_mtime
    playerMoved = False
    while playerMoved == False:
        now = os.stat(saveFile).st_mtime
        if now - start != 0: playerMoved = True

    if board.winner != None:
        winner = board.winner
        break

    turn += 1
