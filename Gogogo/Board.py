#!/usr/bin/env python3

E = "E"
B = "B"
W = "W"
U = "U"
icons = {E: ". ", B: "B ", W: "W "}
ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

class Board:
    def __init__(self):
        self.w = 15
        self.h = 15
        self.board = [[E] * self.w for i in range(self.h)]
        self.history = ""

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
        if self.getStone(x, y) != E:
            return None

        self.board[y][x] = stone
        self.history = self.history + ALPHABET[x] + ALPHABET[y] + stone
        return True

    def processCode(self, code):
        n = 3
        if type(code) != str or len(code) % 3 != 0:
            print("[WARN] Code must be a string with length in multiples of {}".format(n))
            return None

        snippets = [code[i * n:(i + 1) * n] for i in range(len(code) + n - 1) // n]
        for snippet in snippets:
            x = ALPHABET.index(snippet[0].upper())
            y = ALPHABET.index(snippet[1].upper())
            s = snippet[2]

            if self.place(x, y, s) == None:
                return None

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
