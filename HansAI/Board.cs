using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;

namespace Gomoku
{
	internal class Board : IComparable<Board>
	{
		private static Random rnd = new Random();
		private Position gOffset = new Position(15, 15);
		private const int drawSize = 30;

		public enum Brick : byte
		{
			Empty, White, Black, Error
		}

		private enum LineType : byte
		{
			Length = 3,
			Double = 0, Tripple = 1, Quad = 2, Pent = 3,

			Openness = 12,
			Open = 0, Blocked = 4, Closed = 12,

			Smotheness = 16,
			Smothed = 16
		}

		public const int Sidelength = 15;

		public double Eva = double.NaN;
		public Brick Turn = Brick.White;
		public Brick Winner = Brick.Empty;
		public Position LastMove = new Position(-1, -1);

		private List<Line> whiteL, blackL;

		private	int MoveCount { get { return whiteMoves.Count + blackMoves.Count; } }
		private List<Position> whiteMoves, blackMoves;
		private Brick[,] Field = new Brick[Sidelength, Sidelength];

		public Board()
		{
			whiteL = new List<Line>(20);
			blackL = new List<Line>(20);

			whiteMoves = new List<Position>(200);
			blackMoves = new List<Position>(200);
		}
		public Board(string path)
		{
			string s = File.ReadAllText(path);

			whiteMoves = new List<Position>(200);
			blackMoves = new List<Position>(200);

			for(int i = 0; i < s.Length; i+= 2)
			{
				if ((i & 3) <= 1) whiteMoves.Add(new Position(s[i], s[i + 1]));
				else blackMoves.Add(new Position(s[i], s[i + 1]));
			}

			for (int i = 0; i < MoveCount; i++)
			{
				if ((i & 1) == 0) Field[whiteMoves[i / 2].X, whiteMoves[i / 2].Y] = Brick.White;
				else Field[blackMoves[i / 2].X, blackMoves[i / 2].Y] = Brick.Black;
			}

			if (MoveCount % 2 == 0) Turn = Brick.White;
			else Turn = Brick.Black;

			FindLines();
		}

		public Board(Board old, Position move)
		{
			whiteMoves = new List<Position>(200);
			whiteMoves.AddRange(old.whiteMoves);

			blackMoves = new List<Position>(200);
			blackMoves.AddRange(old.blackMoves);

			Array.Copy(old.Field, Field, Sidelength * Sidelength);
			Turn = old.Turn;

			DoMove(move);
		}

		public void DoMove(Position move)
		{
			if (Turn == Brick.White) whiteMoves.Add(move);
			else blackMoves.Add(move);

			Field[move.X, move.Y] = Turn;
			FindLines();
			LastMove = move;

			Turn = Turn == Brick.White ? Brick.Black : Brick.White;

			Eva = double.NaN;
		}

		public List<Board> GetNearMoves()
		{
			if (Winner != Brick.Empty) return new List<Board>();

			List<Board> list = new List<Board>(50);
			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
					if (list.Count > 0)
					{
						if (list.Last().Winner != Brick.Empty)
						{
							return new List<Board>(1) { list.Last() };
						}
					}

					if (Field[x, y] == Brick.Empty)
					{
						if (x > 0)
						{
							if (Field[x - 1, y] != Brick.Empty)
							{
								list.Add(new Board(this, new Position(x, y)));
								continue;
							}

							if (y > 0)
							{
								if ((byte)Field[x, y - 1] + (byte)Field[x - 1, y - 1] > 0)
								{
									list.Add(new Board(this, new Position(x, y)));
									continue;
								}
							}
							if (y < Sidelength - 1)
							{
								if ((byte)Field[x, y + 1] + (byte)Field[x - 1, y + 1] > 0)
								{
									list.Add(new Board(this, new Position(x, y)));
									continue;
								}
							}
						}

						if (x < Sidelength - 1)
						{
							if (Field[x + 1, y] != Brick.Empty)
							{
								list.Add(new Board(this, new Position(x, y)));
								continue;
							}

							if (y > 0)
							{
								if ((byte)Field[x, y - 1] + (byte)Field[x + 1, y - 1] > 0)
								{
									list.Add(new Board(this, new Position(x, y)));
									continue;
								}
							}
							if (y < Sidelength - 1)
							{
								if ((byte)Field[x, y + 1] + (byte)Field[x + 1, y + 1] > 0)
								{
									list.Add(new Board(this, new Position(x, y)));
									continue;
								}
							}
						}
					}
				}
			}

			return list;
		}

		public List<Board> GetNearMoves(int r)
		{
			if (Winner != Brick.Empty) return new List<Board>();


			bool[,] moves = new bool[Sidelength, Sidelength];
			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
					if(Field[x,y] != Brick.Empty)
					{
						for(int dx = -r; dx < r; dx++)
						{
							if(x + dx >= 0 && x + dx < Sidelength)
							{
								for (int dy = -r; dy < r; dy++)
								{
									if (y + dy >= 0 && y + dy < Sidelength)
									{
										moves[x + dx, y + dy] = true;
									}
								}	
							}
						}
					}
				}
			}

			List<Board> list = new List<Board>(50);
			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
					if (moves[x, y] && Field[x, y] == Brick.Empty)
					{
						list.Add(new Board(this, new Position(x, y)));

						if (list.Last().Winner != Brick.Empty)
						{
							return new List<Board>(1) { list.Last() };
						}
					}
				}
			}

			return list;
		}

		public List<Board> GetAllMoves()
		{
			if (Winner != Brick.Empty)
				return new List<Board>();

			List<Board> list = new List<Board>(50);
			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
					if(Field[x,y] == Brick.Empty)
					{
						list.Add(new Board(this, new Position(x, y)));

						if (list.Last().Winner != Brick.Empty)
						{
							return new List<Board>(1) { list.Last() };
						}
					}
				}
			}

			return list;
		}

		public double Evaluate()
		{
			if (!double.IsNaN(Eva)) return Eva;

			Eva = 0;

			//Lines
			int tCounter = 0;
			foreach (Line line in whiteL)
			{
				Eva += EvaluateLine(line, Turn == Brick.White);
			}

			if (tCounter >= 2) Eva += Turn == Brick.White? 100 : 70;
			tCounter = 0;

			foreach (Line line in blackL)
			{
				Eva -= EvaluateLine(line, Turn == Brick.Black);
			}
			if (tCounter >= 2) Eva -= Turn == Brick.Black ? 100 : 70;

			//Openess
			double avgWx = 0, avgWy = 0, avgBx = 0, avgBy = 0;
			for (int i = 0; i < whiteMoves.Count; i++)
			{
				avgWx += whiteMoves[i].X;
				avgWy += whiteMoves[i].Y;
			}
			for (int i = 0; i < blackMoves.Count; i++)
			{
				avgBx += blackMoves[i].X;
				avgBy += blackMoves[i].Y;
			}
			avgWx /= whiteMoves.Count;
			avgWy /= whiteMoves.Count;

			avgBx /= blackMoves.Count;
			avgBy /= blackMoves.Count;

			double varW = 0, varB = 0;
			for (int i = 0; i < whiteMoves.Count; i++)
			{
				varW += (avgWx - whiteMoves[i].X) * (avgWx - whiteMoves[i].X) +
					(avgWy - whiteMoves[i].Y) * (avgWy - whiteMoves[i].Y);
			}
			for (int i = 0; i < blackMoves.Count; i++)
			{
				varB += (avgBx - blackMoves[i].X) * (avgBx - blackMoves[i].X) +
					(avgBy - blackMoves[i].Y) * (avgBy - blackMoves[i].Y);
			}

			varW /= whiteMoves.Count;
			varB /= blackMoves.Count;

			Eva += (varW - varB) * 0.05;
			Eva += (rnd.NextDouble() * 2 - 1) * 0.1;
			return Eva;

			/* X| open 			blocked		closed	smoothed
			 * 2| 0.6	/0.5	0.2			0		~
			 * 3| 100	/0.6	0.7	 /0.2	-0.1	+= 0.0
			 * 4| 1000	/300	1000 /0.9  	-0.2	1000  /0.9
			 * 5| 10000	/~		10000/~		10000	10000 /~
			 */
			double EvaluateLine(Line l, bool myTurn)
			{
				LineType t = CheckType(l);

				switch (t & LineType.Length)
				{
					case LineType.Double:
						switch (t & LineType.Openness)
						{
							case LineType.Open: return myTurn ? 0.6 : 0.5;
							case LineType.Blocked: return 0.2;
							case LineType.Closed: return 0;
						}
						break;

					case LineType.Tripple:
						switch (t & LineType.Openness)
						{
							case LineType.Open:
								tCounter++;
								return myTurn ? 100 : 0.6;

							case LineType.Blocked: return myTurn ? 0.7 : 0.2;
							case LineType.Closed: return -0.1;
						}
						break;

					case LineType.Quad:

						
						if ((t & LineType.Smotheness) == LineType.Smothed) return myTurn ? 1000 : 0.9;

						switch (t & LineType.Openness)
						{
							case LineType.Open: return myTurn ? 1000 : 300;
							case LineType.Blocked: return myTurn ? 1000: 0.9;
							case LineType.Closed: return -0.2;
						}
						break;

					case LineType.Pent:
						if ((t & LineType.Smotheness) == LineType.Smothed)
							return myTurn ? 10000 : 0.9;

						return 10000;
				}

				return double.NaN;
			}
			LineType CheckType(Line l)
			{
				int count = 0;

				LineType type = (LineType)(l.Count - 2);

				Position pos = l.End + l.Normalized;
				if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
				{
					if (Field[pos.X, pos.Y] == Brick.Empty)
					{
						count++;
						pos += l.Normalized;

						//try extend 1
						if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
						{
							if (Field[pos.X, pos.Y] == Field[l.Start.X, l.Start.Y] && l.Count < 4)
							{
								count--;
								type += 1;
								type |= LineType.Smothed;

								pos += l.Normalized;

								if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
								{
									if (Field[pos.X, pos.Y] == Brick.Empty) count++;
									else if (Field[pos.X, pos.Y] == Field[l.Start.X, l.Start.Y]) type += 1;
									//00X00
								}
							}
						}
					}
				}

				pos = l.Start - l.Normalized;
				if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
				{
					if (Field[pos.X, pos.Y] == Brick.Empty)
					{
						count++;
						pos -= l.Normalized;

						//try extend
						if((type & LineType.Smotheness) != LineType.Smothed)
						{
							if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
							{
								if (Field[pos.X, pos.Y] == Field[l.Start.X, l.Start.Y] && l.Count < 4 && (type & LineType.Smotheness) == LineType.Double)
								{
									count--;
									type += 1;
									type |= LineType.Smothed;

									pos -= l.Normalized;

									if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
									{
										if (Field[pos.X, pos.Y] == Brick.Empty) count++;
										else if (Field[pos.X, pos.Y] == Field[l.Start.X, l.Start.Y]) type += 1;
										//00X00
									}
								}
							}
						}
					}
				}

				if (count == 1) type |= LineType.Blocked;
				if (count == 0) type |= LineType.Closed;

				return type;
			}
		}

		public void FindLines()
		{
			whiteL = new List<Line>(20);
			blackL = new List<Line>(20);
			int t = 1;
			for (int i = 0; i < Sidelength; i++)
			{
				int vCount = int.MinValue, hCount = int.MinValue;

				Brick vBrick = Brick.Empty, hBrick = Brick.Empty;

				for (int j = 0; j < Sidelength; j++)
				{
					if (Field[i, j] == vBrick) vCount++;
					else
					{
						if (vCount > t && vBrick != Brick.Empty)
						{
							if (vCount == 5)
							{
								Winner = vBrick;
							}
							if (vBrick == Brick.White)
							{
								whiteL.Add(new Line(new Position(i, j - vCount), new Position(i, j - 1)));
							}
							else
							{
								blackL.Add(new Line(new Position(i, j - vCount), new Position(i, j - 1)));
							}

							vCount = 0;
						}

						vBrick = Field[i, j];
						if (Field[i, j] != Brick.Empty)
						{
							vCount = 1;
						}
					}

					if (Field[j, i] == hBrick) hCount++;
					else
					{
						if (hCount > t && hBrick != Brick.Empty)
						{
							if (hCount == 5)
							{
								Winner = hBrick;
							}

							if (hBrick == Brick.White)
							{
								whiteL.Add(new Line(new Position(j - hCount, i), new Position(j - 1, i)));
							}
							else
							{
								blackL.Add(new Line(new Position(j - hCount, i), new Position(j - 1, i)));
							}

							hCount = 0;
						}

						hBrick = Field[j, i];
						if (Field[j, i] != Brick.Empty)
						{
							hCount = 1;
						}
					}
				}
			}

			//Diagonal
			for (int i = 1; i < Sidelength * 2; i++)
			{
				if (i == 2 * Sidelength - 1) continue;
				Position pos = new Position(i % Sidelength, 0);
				if (i > Sidelength) pos.Swap();

				Brick b = Brick.Empty;
				int count = int.MinValue;
				while (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
				{
					if (Field[pos.X, pos.Y] == b) count++;
					else
					{
						if (count > 1 && b != Brick.Empty)
						{
							if (count == 5)
							{
								Winner = b;
							}

							if (b == Brick.White)
							{
								whiteL.Add(new Line(new Position(pos.X - count, pos.Y - count), pos - new Position(1, 1)));
							}
							else
							{
								blackL.Add(new Line(new Position(pos.X - count, pos.Y - count), pos - new Position(1, 1)));
							}

							count = 0;
						}

						b = Field[pos.X, pos.Y];
						if (Field[pos.X, pos.Y] != Brick.Empty)
						{
							count = 1;
						}
					}

					pos += new Position(1, 1);
				}

				pos = new Position(i % Sidelength, i > Sidelength ? 0 : Sidelength - 1);
				if (i > Sidelength) pos.Swap();

				b = Brick.Empty;
				count = int.MinValue;
				while (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
				{
					if (Field[pos.X, pos.Y] == b) count++;
					else
					{
						if (count > t && b != Brick.Empty)
						{
							if (count == 5)
							{
								Winner = b;
							}
							if (b == Brick.White)
							{
								whiteL.Add(new Line(new Position(pos.X - count, pos.Y + count), pos - new Position(1, -1)));
							}
							else
							{
								blackL.Add(new Line(new Position(pos.X - count, pos.Y + count), pos - new Position(1, -1)));
							}

							count = 0;
						}

						b = Field[pos.X, pos.Y];
						if (Field[pos.X, pos.Y] != Brick.Empty)
						{
							count = t;
						}
					}

					pos += new Position(1, -1);
				}
			}
		}

		public void Draw(Graphics g)
		{
			int d = 24;

			for (int i = 0; i < Sidelength; i++)
			{
				g.DrawString(i.ToString(), new Font("Arial", 8f), Brushes.Black, i * drawSize + gOffset.X - 3, 0);
				g.DrawString(i.ToString(), new Font("Arial", 8f), Brushes.Black, 0, i * drawSize + gOffset.Y / 2);

				g.DrawLine(i == 4 || i == 10 ? Pens.Black : Pens.Gray, gOffset.X + i * drawSize, gOffset.Y, gOffset.X + i * drawSize, gOffset.Y + drawSize * (Sidelength - 1));
				g.DrawLine(i == 4 || i == 10 ? Pens.Black : Pens.Gray, new Point(gOffset.X, gOffset.Y + i * drawSize), new Point(gOffset.Y + drawSize * (Sidelength - 1), gOffset.Y + i * drawSize));
			}

			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
					if (Field[x, y] == Brick.White) g.FillEllipse(Brushes.White, new RectangleF(x * drawSize + gOffset.X - d / 2, y * drawSize + gOffset.Y - d / 2, d, d));
					if (Field[x, y] == Brick.Black) g.FillEllipse(Brushes.Black, new RectangleF(x * drawSize + gOffset.X - d / 2, y * drawSize + gOffset.Y - d / 2, d, d));
					if (new Position(x, y) == LastMove) g.DrawEllipse(Pens.Blue, new RectangleF(x * drawSize + gOffset.X - d / 2, y * drawSize + gOffset.Y - d / 2, d, d));
				}
			}

			//for (int i = 0; i < whiteL.Count; i++)
			//{
			//	g.DrawLine(Pens.Red, whiteL[i].Start.X * drawSize + gOffset.X, whiteL[i].Start.Y * drawSize + gOffset.Y,
			//		whiteL[i].End.X * drawSize + gOffset.X, whiteL[i].End.Y * drawSize + gOffset.Y);
			//}

			//for (int i = 0; i < blackL.Count; i++)
			//{
			//	g.DrawLine(Pens.Red, blackL[i].Start.X * drawSize + gOffset.X, blackL[i].Start.Y * drawSize + gOffset.Y,
			//		blackL[i].End.X * drawSize + gOffset.X, blackL[i].End.Y * drawSize + gOffset.Y);
			//}
		}

		public void ParseMove(Point p)
		{
			//p -= gOffset;

			if (p.X >= 0 && p.Y >= 0 &&
				p.X < Sidelength * drawSize && p.Y < Sidelength * drawSize)
			{
				Position move = new Position(p.X / drawSize, p.Y / drawSize);
				Console.Write(move);
				DoMove(move);
				Evaluate();
				Console.WriteLine(" Eva: " + Eva.ToString("0.00") + "\n");
				//WriteData();
			}
		}

		public int CompareTo(Board other)
		{
			return Evaluate().CompareTo(other.Evaluate());
		}

		public void WriteData()
		{
			string s = "";

			s += "Turn: " + Turn.ToString() + "\n";
			s += "Winner: " + Winner.ToString() + "\n";
			s += "Eva: " + Eva.ToString("0.000") + "\n";
			for (int y = 0; y < Sidelength; y++)
			{
				for (int x = 0; x < Sidelength; x++)
				{
					switch (Field[x, y])
					{
						case Brick.White: s += "O"; break;
						case Brick.Black: s += "X"; break;
						case Brick.Empty: s += "#"; break;
					}
				}
				s += "\n";
			}

			Console.WriteLine(s);
		}

		public void Save(string path)
		{
			
			File.WriteAllText(path, GetMoveString());
		}
		public string GetMoveString()
		{
			string s = "";
			for (int i = 0; i < MoveCount; i++)
			{
				if ((i & 1) == 0) s += whiteMoves[i / 2].ByranFormat();
				else s += blackMoves[i / 2].ByranFormat();
			}

			return s;
		}

		public override string ToString()
		{
			return LastMove.ToString() + " with " + Eva.ToString("0.000");
		}

		private class BrickList
		{
			public List<int>[] Bricks = new List<int>[Sidelength];

			public BrickList()
			{
				for (int i = 0; i < Sidelength; i++)
				{
					Bricks[i] = new List<int>(Sidelength);
				}
			}

			public void Add(Position p)
			{
				Bricks[p.X].Add(p.Y);
			}

			public bool Contains(Position p)
			{
				return Bricks[p.X].Contains(p.Y);
			}
		}

		private class Line
		{
			public readonly Position Start, End;

			public Position Normalized
			{
				get
				{ return (End - Start).Sign(); }
			}

			public int Length
			{
				get
				{
					if (Start.X - End.X != 0) return Math.Abs(Start.X - End.X) + 1;
					else return Math.Abs(Start.Y - End.Y) + 1;
				}
			}

			public readonly int Count;

			public Line(int x, int y)
			{
				Start = new Position(x, y);
				End = new Position(x, y);
			}

			public Line(Position start, Position end)
			{
				Start = start;
				End = end;

				if (Start.X - End.X != 0) Count = Math.Abs(Start.X - End.X) + 1;
				else Count = Math.Abs(Start.Y - End.Y) + 1;
			}

			public Line(Position start, Position end, int count)
			{
				Start = start;
				End = end;
				Count = count;
			}

			public override string ToString()
			{
				return Start.ToString() + " -> " + End.ToString();
			}
		}
	}

	struct Position : IComparable<Position>
	{
		const string chars = "ABCDEFGHIJKLMNO";

		public sbyte X, Y;

		public Position(int x, int y)
		{
			X = (sbyte)x;
			Y = (sbyte)y;
		}

		public Position(Point p)
		{
			X = (sbyte)p.X;
			Y = (sbyte)p.Y;
		}
		public Position(char x, char y)
		{
			X = (sbyte)chars.IndexOf(x);
			Y = (sbyte)chars.IndexOf(x);
		}

		public int CompareTo(Position other)
		{
			if (X == other.X) return Y.CompareTo(other.Y);

			return X.CompareTo(other.X);
		}

		public Point ToPoint()
		{
			return new Point(X, Y);
		}

		public Position Sign()
		{
			return new Position(Math.Sign(X), Math.Sign(Y));
		}

		public int GausLength()
		{
			return Math.Abs(X) + Math.Abs(Y);
		}

		public double EuclideLength()
		{
			return Math.Sqrt(X * X + Y * Y);
		}

		public double EuclideLengthSquared()
		{
			return X * X + Y * Y;
		}

		public void Swap()
		{
			sbyte b = X;
			X = Y;
			Y = b;
		}

		public string ByranFormat()
		{
			return chars[X].ToString() + chars[Y].ToString();			
		}

		public static Position operator +(Position a, Position b)
		{
			return new Position(a.X + b.X, a.Y + b.Y);
		}

		public static Position operator -(Position a, Position b)
		{
			return new Position(a.X - b.X, a.Y - b.Y);
		}

		public static Position operator *(int s, Position a)
		{
			return new Position(a.X * s, a.Y * s);
		}

		public static bool operator ==(Position a, Position b)
		{
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Position a, Position b)
		{
			return !(a.X == b.X && a.Y == b.Y);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Position))
			{
				return false;
			}

			var position = (Position)obj;
			return X == position.X &&
				   Y == position.Y;
		}

		public override int GetHashCode()
		{
			return X ^ (Y << 8);
		}

		public override string ToString()
		{
			return "{" + X.ToString("00") + " | " + Y.ToString("00") + "}";
		}

	}
}