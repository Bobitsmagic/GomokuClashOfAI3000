#!/usr/bin/env python3

import os
import time
import subprocess

from Gogogo.Board import Board
from Gogogo.AI import AI

for game in range(100):
    saveFile = "../game.gom"
    saveFile = os.path.abspath(saveFile)

    with open(saveFile, "w") as fh: fh.write("")

    # [TODO] Make this randomly select between stone colors instead of hardcoded
    gogogo_board = Board()
    #gogogo = AI(gogogo_board, "B", saveFile = saveFile, verbose = True)
    gogogo = AI(gogogo_board, "B", saveFile = saveFile, verbose = True)
    #HansAI = AI(gogogo_board, "W", saveFile = saveFile)

    HansAI_loc = os.path.abspath("./HansAI/bin/Release/Gomoku.exe")
    difficulty = 1000
    command = HansAI_loc + " " + saveFile + " " + "W" + " " + str(difficulty)
    process = subprocess.Popen(command, shell = True)
    HansAI = None

    b = gogogo
    w = HansAI

    turn = 0
    turns = -1
    winner = None
    winnerCode = ""
    while winner == None and (turn < turns or turns == -1):
        gameState = open(saveFile, "r").read()

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

        #board.render()

        if board.winner != None:
            winner = board.winner
            winnerCode = board.history
            break

        turn += 1
    
    scoreFile = "../score.txt"
    scoreFile = os.path.abspath(scoreFile)
    with open(scoreFile, "a") as fh:
        if winner == b:
            print("Black is the victor!")
            fh.write("Winner B {}\n".format(winnerCode))
        else:
            print("White is the victor!")
            fh.write("Winner W {}\n".format(winnerCode))
