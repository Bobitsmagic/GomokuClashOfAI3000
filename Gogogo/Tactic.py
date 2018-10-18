#!/usr/bin/env python3

import math

E = "E"
B = "B"
W = "W"

class Tactic:
    def __init__(self):
        #self.pattern = [["L", "X", "X", "X", "X"]]
        self.pattern = [
            ["L", "E", "E", "E", "E"],
            ["E", "X", "E", "E", "E"],
            ["E", "E", "X", "E", "E"],
            ["E", "E", "E", "X", "E"],
            ["E", "E", "E", "E", "X"],
        ]

        #m = [
        #["A", "B", "C", "D", "F"],
        #[".", ".", ".", ".", "."],
        #["G", "H", "I", "J", "K"],
        #["L", "M", "N", "O", "P"],
        #["Q", "R", "S", "T", "U"],
        #]
        #m = [
        #[".", ".", "."],
        #]
        #m = [
        #[".", ".", "."],
        #[".", ".", "."],
        #[".", ".", "."],
        #]
        #m = [
        #[".", ".", ".", "."],
        #[".", ".", ".", "."],
        #]
        #m = [[".", ".", ".", ".", ".", ".", "."]]
        #m = self.rotate(m)

    def render(self):
        for row in self.pattern:
            print(row)

    def recommend(self, board, stone):
        bestMatch = None
        bestMatchPercent = 0.0
        for y in range(board.h):
            for x in range(board.w):
                matchPercent = board.patternMatchPercent(self.pattern, x, y, stone)
                if matchPercent > 0.0:
                    if bestMatch == None or matchPercent > bestMatchPercent:
                        bestMatch = [x, y]
                        bestMatchPercent = matchPercent

        # [LEFTOFF] Currently it does not select a random piece that already hasn't been played in the pattern...
        # Get it to do this
        print("Found a match with a percentage of {}".format(bestMatchPercent))
        return bestMatch

    # [TODO] Fix this so that it ACTUALLY works all the time...
    def rotate(self, m):
        theta = math.radians(45)
        s = math.sin(theta)
        c = math.cos(theta)

        w = len(m[0])
        h = len(m)
        transform = []
        cx = int(float(w / 2))
        cy = int(float(h / 2))
        cx = 0
        cy = 0
        for y in range(0 - cy, h - cy):
            for x in range(0 - cx, w - cx):
                px = x
                py = y

                nx = px * c - py * s#px -= cx
                #py -= cy
                ny = px * s + py * c
                px = nx
                py = ny
                #px = nx + cx
                #py = ny + cy
                if px > 0:
                    px = round(math.ceil(px))
                else:
                    px = round(math.floor(px))
                if py > 0:
                    py = round(math.ceil(py))
                else:
                    py = round(math.floor(py))
                #px = round(px)
                #py = round(py)
                print(x, y)
                print(px, py)
                print()
                transform.append([x, y, px, py])

        nwl = 0
        nwh = 0
        nhl = 0
        nhh = 0
        for value in transform:
            if value[2] < nwl:
                nwl = value[2]
            elif value[2] > nwh:
                nwh = value[2]
            if value[3] < nhl:
                nhl = value[3]
            elif value[3] > nhh:
                nhh = value[3]
        nw = nwh - nwl + 1
        nh = nhh - nhl + 1
        print("got {} {}".format(nw, nh))
        rm = [[E] * nw for i in range(nh)]
        for value in transform:
            rm[value[3] - nhl][value[2] - nwl] = m[value[1]][value[0]]

        return rm
