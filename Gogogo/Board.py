#!/usr/bin/env python3

E = "E"
B = "B"
W = "W"
U = "U"
icons = {E: ". ", B: "B ", W: "W "}
ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

class Board:
    def __init__(self, verbose = False):
        self.w = 15
        self.h = 15
        self.board = [[E] * self.w for i in range(self.h)]
        self.history = ""
        self.verbose = verbose

    def render(self, history = True):
        for y in range(self.h):
            for x in range(self.w):
                stone = self.getStone(x, y)
                icon = "? "
                if stone in icons:
                    icon = icons[stone]
                print(icon, end = "")
            print()
        if history: print(self.history)
        print()

    def getStone(self, x, y):
        if x < 0 or y < 0 or x >= self.w or y >= self.h:
            return U

        stone = self.board[y][x]
        if stone not in [E, B, W]:
            return U

        return stone

    def place(self, x, y, stone):
        if self.getStone(x, y) != E: return None

        #existing = self.board[y][x]
        #if existing != E:
        #    print("[ERROR] Space not empty, cheating detected!!!")
        #    return False

        self.board[y][x] = stone

        self.history = self.history + ALPHABET[x] + ALPHABET[y]
        return True

    def processCode(self, code):
        #print("[GOGOGO]: Processing code: " + code)
        # [TODO] Get this to more elegantly process code
        stone = B
        for d in range(0, len(code), 2):
            x = ALPHABET.index(code[d + 0].upper())
            y = ALPHABET.index(code[d + 1].upper())
            #print("1st {} 2nd {}".format(code[d + 0], code[d + 1]))

            if self.place(x, y, stone) == None:
                return None

            if False: pass
            elif stone == B: stone = W
            elif stone == W: stone = B
        #print("[GOGOGO]: This was the code processed: " + self.history)

    @property
    def winner(self):
        for y in range(self.h):
            for x in range(self.w):
                for delta in [[1, 0], [0, 1], [1, 1], [1, -1]]:
                    for stone in [B, W]:
                        if self.adjacentMatches(stone, x, y, delta[0], delta[1], 5) == stone:
                            return stone
        return None

    def adjacentMatches(self, stone, x, y, deltaX, deltaY, n):
        if n == 0:
            return stone

        if stone != self.getStone(x, y):
            return None

        return self.adjacentMatches(stone, x + deltaX, y + deltaY, deltaX, deltaY, n - 1)

    def patternMatchPercent(self, pattern, x, y, expectStone):
        retVal = 0.0

        w = len(pattern[0])
        h = len(pattern)
        for cy in range(h):
            for cx in range(w):
                current = pattern[cy][cx]
                if current != "X" and current != "L":
                    continue

                stone = self.getStone(x + cx, y + cy)
                if stone != expectStone and stone != E:
                    return 0.0
                elif stone == expectStone:
                    retVal += 1.0

        return retVal
