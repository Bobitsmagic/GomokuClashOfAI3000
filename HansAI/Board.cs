using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

		public const int Sidelength = 17;

		public double Eva = double.NaN;
		public Brick Turn = Brick.White;
		public Brick Winner = Brick.Empty;
		public Position LastMove = new Position(-1, -1);

		private List<Line> whiteL, blackL;
		public Brick[,] Field = new Brick[Sidelength, Sidelength];

		public Board()
		{
			whiteL = new List<Line>(20);
			blackL = new List<Line>(20);
		}

		public Board(Board old, Position move)
		{
			Array.Copy(old.Field, Field, Sidelength * Sidelength);
			Turn = old.Turn;

			DoMove(move);
		}

		public void DoMove(Position move)
		{
			Field[move.X, move.Y] = Turn;
			FindLines();
			LastMove = move;

			Turn = Turn == Brick.White ? Brick.Black : Brick.White;
		}

		public List<Board> GetMoves()
		{
			if (Winner != Brick.Empty) return new List<Board>();

			List<Board> list = new List<Board>(50);
			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
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

					if (list.Count > 0)
					{
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

			double v = 0;

			int tCounter = 0;
			foreach (Line line in whiteL)
			{
				v += EvaluateLine(line);
			}

			if (tCounter >= 2) v += 10;
			tCounter = 0;

			foreach (Line line in blackL)
			{
				v -= EvaluateLine(line);
			}
			if (tCounter >= 2) v -= 10;

			Eva = v + (rnd.NextDouble() * 2 - 1) * 0;
			return Eva;

			double EvaluateLine(Line l)
			{
				LineType t = CheckType(l);

				switch (t & LineType.Length)
				{
					case LineType.Double:
						switch (t & LineType.Openness)
						{
							case LineType.Open: return 3;
							case LineType.Blocked: return 0.4;
							case LineType.Closed: return 0.01;
						}
						break;

					case LineType.Tripple:
						switch (t & LineType.Openness)
						{
							case LineType.Open:
								tCounter++;
								return 10;

							case LineType.Blocked: return 1;
							case LineType.Closed: return 0.02;
						}
						break;

					case LineType.Quad:
						if ((t & LineType.Smotheness) == LineType.Smothed)
							return 4;

						switch (t & LineType.Openness)
						{
							case LineType.Open: return 100;
							case LineType.Blocked: return 9;
							case LineType.Closed: return 0.03;
						}
						break;

					case LineType.Pent:
						if ((t & LineType.Smotheness) == LineType.Smothed)
							return 8;

						return 1234;
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

						//try extend
						if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
						{
							if (Field[pos.X, pos.Y] == Field[l.Start.X, l.Start.Y])
							{
								type += 1;
								type |= LineType.Smothed;
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
						if (pos.X >= 0 && pos.Y >= 0 && pos.X < Sidelength && pos.Y < Sidelength)
						{
							if (Field[pos.X, pos.Y] == Field[l.Start.X, l.Start.Y])
							{
								type += 1;
								type |= LineType.Smothed;
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

				g.DrawLine(i == 5 || i == 11 ? Pens.Black : Pens.Gray, gOffset.X + i * drawSize, gOffset.Y, gOffset.X + i * drawSize, gOffset.Y + drawSize * (Sidelength - 1));
				g.DrawLine(i == 5 || i == 11 ? Pens.Black : Pens.Gray, new Point(gOffset.X, gOffset.Y + i * drawSize), new Point(gOffset.Y + drawSize * (Sidelength - 1), gOffset.Y + i * drawSize));
			}

			for (int x = 0; x < Sidelength; x++)
			{
				for (int y = 0; y < Sidelength; y++)
				{
					if (Field[x, y] == Brick.White) g.FillEllipse(Brushes.White, new RectangleF(x * drawSize + gOffset.X - d / 2, y * drawSize + gOffset.Y - d / 2, d, d));
					if (Field[x, y] == Brick.Black) g.FillEllipse(Brushes.Black, new RectangleF(x * drawSize + gOffset.X - d / 2, y * drawSize + gOffset.Y - d / 2, d, d));
					if(new Position(x,y) == LastMove) g.DrawEllipse(Pens.Blue, new RectangleF(x * drawSize + gOffset.X - d / 2, y * drawSize + gOffset.Y - d / 2, d, d));
				}
			}

			for (int i = 0; i < whiteL.Count; i++)
			{
				g.DrawLine(Pens.Red, whiteL[i].Start.X * drawSize + gOffset.X, whiteL[i].Start.Y * drawSize + gOffset.Y,
					whiteL[i].End.X * drawSize + gOffset.X, whiteL[i].End.Y * drawSize + gOffset.Y);
			}

			for (int i = 0; i < blackL.Count; i++)
			{
				g.DrawLine(Pens.Red, blackL[i].Start.X * drawSize + gOffset.X, blackL[i].Start.Y * drawSize + gOffset.Y,
					blackL[i].End.X * drawSize + gOffset.X, blackL[i].End.Y * drawSize + gOffset.Y);
			}
		}

		public void ParseMove(Point p)
		{
			//p -= gOffset;

			if (p.X >= 0 && p.Y >= 0 &&
				p.X < Sidelength * drawSize && p.Y < Sidelength * drawSize)
			{
				Position move = new Position(p.X / drawSize, p.Y / drawSize);
				Console.WriteLine(move + "\n");
				DoMove(move);
				Eva = Evaluate();

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
			//for (int y = 0; y < Sidelength; y++)
			//{
			//	for(int x = 0; x < Sidelength; x++)
			//	{
			//		switch (Field[x, y])
			//		{
			//			case Brick.White: s += "O"; break;
			//			case Brick.Black: s += "X"; break;
			//			case Brick.Empty: s += "#"; break;
			//		}
			//	}
			//	s += "\n";
			//}

			Console.WriteLine(s);
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

	internal struct Position : IComparable<Position>
	{
		public short X, Y;

		public Position(int x, int y)
		{
			X = (short)x;
			Y = (short)y;
		}

		public Position(Point p)
		{
			X = (short)p.X;
			Y = (short)p.Y;
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

		public void Swap()
		{
			short b = X;
			X = Y;
			Y = b;
		}

		public static Position operator +(Position a, Position b)
		{
			return new Position(a.X + b.X, a.Y + b.Y);
		}

		public static Position operator -(Position a, Position b)
		{
			return new Position(a.X - b.X, a.Y - b.Y);
		}

		public static Position operator *(Position a, int s)
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