# **GomokuClashOfAI3000**

## ***Worlds Best Gomoku AIs***

## Rules

- 15 x 15 Field (Grid, creating 15x15 possible moves)

- Time Controlled Engine with arbitrary millisecond mode
  A.I.s are encouraged to reply with random moves prior to running out of time

- Black always starts

- Move Format:
  - xLetter yLetter, e.g. GHBCFF...
  - Implied to be in Black White Black White etc...

- Serialization of game states:
  - Winner [Gogogo|HansAI] as [BW] HISTORYFOLLOWS...\n

- Coordination between A.I.s will be accomplished through echo reply w/ server
  - TCP localhost:4321

## Engines

Gogogo (Pard0x13's engine)

HansAI (Bobitsmagic's engine)

Server
  - Server which will arbitrate games between A.I.s
  - All client side code will be provided by each respective A.I. in it's natural language

## Server Interaction

All communication will occur with the server via TCP sockets via JSON

Client -> Server Register User
  * {"query" : "register", "username" : username}

Server -> Client Register User Succeed|Fail
  * {"query" : "register", "username" : username, "state" : "success|fail"}
  - Upon registering Server will begin looking for game matches right away
  - Client should be prepared to handle a game being found directly after this

Server -> Client Game Found
  * {"query" : "game", "username" : username, "state" : "success|fail"}
  * On success this will also include {"B" : blackUsername, "W" : whiteUsername}
  - Failure may be because timeouts, or any other Serverside issue
  - Keep in mind Server will ask for moves, Client will not automatically provide one

Server -> Client Move
  * {"query" : "move", "username" : username, "stone" : "B|W", "location" : "XX"}
  - User will provide move if this is their color
  - If it is not their color they will process opponents move
  - Server will begin counting down time upon sending this message

Client -> Server Move
  * {"query" : "move", "username" : username, "location" : "XX"}

Server -> Client Winner
  * {"query" : "winner", "username" : username}
  - Username will be the username of the A.I. that won, or "TIE" for a tie
  - Clients should spin down and terminate upon reciept of this message

Server -> Client Disconnect
  * {"query" : "disconnect", "username" : username}
  - Implies that for whatever reason the Server has terminated Client's session
  - If received by Client then Client should terminate, even midgame

Let it be known that the Server will arbitrate games and reply with failures if rules are broken
Additional information about broken rules may be provided and established in detail at a later time

## Potential Future Updates

Ability to process more than two colored stones

Ability to have boards larger than restricted A-Z

Ability to play in 3 dimensions

Server to process online games outside of localhost
  Postential security conserns

# Possible TCP Socket Frameworks

C#
  https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-client-socket-example

Python
  https://twistedmatrix.com/documents/current/core/examples/
