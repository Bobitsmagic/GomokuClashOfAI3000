#!/usr/bin/env python3

import random

ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
standardAllowedItems = ["."]

class Grid:
    def __init__(self, w, h, code = ""):
        self.data = []
        self.w = w
        self.h = h

        for y in range(self.h):
            row = []
            for x in range(self.w):
                row.append(".")
            self.data.append(row)

        for offset in range(0, len(code), 3):
            snippet = code[offset : offset + 3]
            self.place(snippet)

    def resized(self, w, h, xOffset = 0, yOffset = 0):
        if w < self.w:
            print("Cannot be smaller w")
            return None
        if h < self.h:
            print("Cannot be smaller h")
            return None

        retVal = Grid(w, h)

        for y in range(self.h):
            for x in range(self.w):
                nx = x + xOffset
                ny = y + yOffset
                if nx >= retVal.w:
                    return None
                if ny >= retVal.h:
                    return None
                retVal.place(ALPHABET[nx] + ALPHABET[ny] + self.data[y][x])

        return retVal

    def pan(self, grid, allowOverlap = False, allowedItems = standardAllowedItems):
        retVal = []

        for y in range(grid.h):
            for x in range(grid.w):
                panned = self.resized(grid.w, grid.h, x, y)
                if panned is None:
                    continue
                if allowOverlap == False and panned.overlaps(grid, allowedItems = allowedItems):
                    continue
                retVal.append(panned)

        return retVal

    def render(self):
        for y in range(self.h):
            for x in range(self.w):
                print("{} ".format(self.data[y][x]), end = "")
            print()

    @property
    def serialized(self):
        retVal = ""
        for y in range(self.h):
            for x in range(self.w):
                retVal = retVal + self.data[y][x]
        return retVal

    def overlaps(self, grid, allowedItems = standardAllowedItems):
        seed = self.serialized
        target = grid.serialized
        for x in range(len(seed)):
            if seed[x] != target:
                if target[x] not in allowedItems and seed[x] not in allowedItems:
                    return True
        return False

    def place(self, code):
        x = ALPHABET.index(code[0])
        y = ALPHABET.index(code[1])
        i = code[2]
        self.data[y][x] = i

class Tactic:
    def __init__(self, code, grid, target):
        self.allowedItems = [".", target]
        self.available = []
        self.code = code

        self.w = 0
        self.h = 0
        for offset in range(0, len(code), 3):
            snippet = self.code[offset : offset + 3]
            x = ALPHABET.index(snippet[0]) + 1
            y = ALPHABET.index(snippet[1]) + 1
            # [TODO] Do max() here instead
            if x > self.w:
                self.w = x
            if y > self.h:
                self.h = y

        self.sync(grid)

    def sync(self, grid):
        self.available = Grid(self.w, self.h, self.code).pan(grid, allowedItems = self.allowedItems)

    def render():
        pass

class Move:
    def __init__(self, loc):
        self.x = ALPHABET.index(loc[0])
        self.y = ALPHABET.index(loc[1])
        self.available = []

    def render(self):
        print("{}, {}".format(self.x, self.y))
        for available in self.available:
            available.render()
            print()

    def addTactic(self, tactic):
        for available in tactic.available:
            item = available.data[self.y][self.x]
            if item in ["s", "S"]:
                self.available.append(available)

    # [TODO] Determine if score() should prune things, or if it should be done instead with a sync() function
    def score(self, grid, target):
        retVal = 1          # Default score indicating you can place here at the very least

        invalidTactics = []

        # Cannot occupy an already occupied space
        if grid.data[self.y][self.x] != ".":
            return 0

        for available in self.available:
            collision = available.overlaps(grid, [".", target])
            if collision:
                invalidTactics.append(available)
                continue

            # Now we go through each spot in grid and tally target matchs
            for y in range(grid.h):
                for x in range(grid.w):
                    if available.data[y][x] not in ["s", "S"]:
                        continue
                    item = grid.data[y][x]
                    if item == target:
                        retVal += 1

        # For optimization purposes we should prune all invalid tactics
        for invalidTactic in invalidTactics:
            self.available.remove(invalidTactic)

        # Plus 1 for ever potential tactic that is still available for use
        retVal += len(self.available)

        return retVal

board = Grid(15, 15)
target = "W"
tactics = []
tactics.append(Tactic("AASBAsCAsDAsEAs", board, target))    # L -> R
tactics.append(Tactic("AAsBAsCAsDAsEAS", board, target))    # L <- R
tactics.append(Tactic("AASABsACsADsAEs", board, target))    # U -> D
tactics.append(Tactic("AAsABsACsADsAES", board, target))    # U <- D
tactics.append(Tactic("AASBBsCCsDDsEEs", board, target))    # UL -> DR
tactics.append(Tactic("AAsBBsCCsDDsEES", board, target))    # UL <- DR
tactics.append(Tactic("EASDBsCCsBDsAEs", board, target))    # UR -> DL
tactics.append(Tactic("EAsDBsCCsBDsAES", board, target))    # UR <- DL
tactics.append(Tactic("AAsBBsCCSBDsAEs", board, target))    # V facing right
tactics.append(Tactic("CAsBBsACSBDsCEs", board, target))    # V facing left
tactics.append(Tactic("CASBBsDBsACsECs", board, target))    # V facing up
tactics.append(Tactic("AAsEAsBBsDBsCCS", board, target))    # V facing down

moves = []
for y in range(board.h):
    for x in range(board.w):
        loc = ALPHABET[x] + ALPHABET[y]
        move = Move(loc)
        for tactic in tactics:
            move.addTactic(tactic)
        moves.append(move)

while True:
    data = input("Move? ")
    data = data.upper()
    board.place(data)

    """
    for y in range(board.h):
        for x in range(board.w):
            move = moves[y * board.w + x]
            score = move.score(board, target)
            print("{}\t".format(score), end = "")
        print()
    """

    highscore = 0
    goodMoves = []
    for move in moves:
        score = move.score(board, target)
        if score > highscore:
            highscore = score
            goodMoves = []
        if score == highscore:
            goodMoves.append(move)

    selection = random.SystemRandom().choice(goodMoves)
    loc = ALPHABET[selection.x] + ALPHABET[selection.y]
    print("{}\t{}, {}\t{}".format(score, selection.x, selection.y, loc))