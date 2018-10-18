using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Gomoku
{
	class HansAI
	{

		public static bool lost = false;
		static double val = 0;
		public Board FinalBoard;
		public HansAI(Board board)
		{
			lost = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			FinalBoard = AlphaBetaV2.Solve(board); 
				
			sw.Stop();
			Console.WriteLine("Alpha:  "  + FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + " in " + sw.ElapsedMilliseconds.ToString("000 000"));
			if(FinalBoard.LastMove == new Position(-1, -1))
			{
				lost = true;
			}
		}
		public HansAI(Board board, long time)
		{
			lost = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			FinalBoard = Jochen.Predict(board, time);

			sw.Stop();
			Console.WriteLine("Jochen: " + FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + " it: " + Jochen.Iteration.ToString("0 000"));
			if (FinalBoard.LastMove == new Position(-1, -1))
			{
				lost = true;
			}
		}



		private class MinMaxNode
		{
			static int MaxDepth = 6;

			public double Value;
			public bool Maximize;

			public MinMaxNode(Board board, out Board BestMove)
			{
				Maximize = !board.Turn;
				Value = Maximize ? double.MinValue : double.MaxValue;

				List<Board> list = board.GetMoves();
				BestMove = list[0];

				MinMaxNode buffer;

				for (int i = 0; i < list.Count; i++)
				{
					if (Maximize)
					{
						if (Value == 1000) return;
					}
					else if (Value == -1000) return;

					buffer = new MinMaxNode(list[i], !Maximize, 1);

					if (Maximize)
					{
						if (buffer.Value > Value)
						{
							Value = buffer.Value;
							BestMove = list[i];
						}
					}
					else
					{
						if (buffer.Value < Value)
						{
							Value = buffer.Value;
							BestMove = list[i];
						}
					}
				}

				val = Value;
			}
			private MinMaxNode(Board board, bool maximize, int depth)
			{
				if(depth == MaxDepth)
				{
					Value = board.Eva;
					return;
				}

				Maximize = maximize;
				Value = Maximize ? double.MinValue : double.MaxValue;
				List<Board> list = board.GetMoves();

				for (int i = 0; i < list.Count; i++)
				{
					if (Maximize)
					{
						if (Value == 1000) return;
					}
					else if (Value == -1000) return;

					MinMaxNode buffer = new MinMaxNode(list[i], !Maximize, depth + 1);

					if (Maximize)
					{
						if (buffer.Value > Value)
						{
							Value = buffer.Value;
						}
					}
					else
					{
						if (buffer.Value < Value)
						{
							Value = buffer.Value;
						}
					}
				}

				val = Value;

			}
		}

		private class AlphaBetaNode
		{
			static int MaxDepth = 7;

			public double Alpha, Beta;
			public double GetValue { get { return Maximize ? Alpha : Beta; } }
			public bool Maximize;

			public AlphaBetaNode(Board board, out Board BestMove)
			{
				Maximize = !board.Turn;
				Alpha = double.MinValue;
				Beta = double.MaxValue;

				List<Board> list = board.GetMoves();
				list.Sort();
				BestMove = list[0];

				AlphaBetaNode buffer;

				if (Maximize)
				{
					for (int i = list.Count - 1; i >= 0 && Beta > Alpha; i--)
					{
						buffer = new AlphaBetaNode(Alpha, Beta, list[i], !Maximize, 1);
						if (Alpha < buffer.GetValue)
						{
							Alpha = buffer.GetValue;
							val = Alpha;
							BestMove = list[i];
						}
					}
				}
				else
				{
					for (int i = 0; i < list.Count && Beta > Alpha; i++)
					{
						buffer = new AlphaBetaNode(Alpha, Beta, list[i], !Maximize, 1);
						if (Beta > buffer.GetValue)
						{
							Beta = buffer.GetValue;
							val = Beta;
							BestMove = list[i];
						}
					}
				}
			}
			private AlphaBetaNode(double min, double max, Board board, bool maximize, int depth)
			{
				Maximize = maximize;

				Alpha = min;
				Beta = max;

				if (depth == MaxDepth)
				{
					if (Maximize) Alpha = board.Eva;
					else Beta = board.Eva;
				}
				else
				{
					List<Board> list = board.GetMoves();
					AlphaBetaNode buffer;

					list.Sort();
					if (Maximize)
					{
						for (int i = list.Count - 1; i >= 0 && Beta > Alpha; i--)
						{
							buffer = new AlphaBetaNode(Alpha, Beta, list[i], !Maximize, depth + 1);
							if (Alpha < buffer.GetValue)
							{
								Alpha = buffer.GetValue;
							}
						}
					}
					else
					{
						for (int i = 0; i < list.Count && Beta > Alpha; i++)
						{
							buffer = new AlphaBetaNode(Alpha, Beta, list[i], !Maximize, depth + 1);
							if (Beta > buffer.GetValue)
							{
								Beta = buffer.GetValue;
							}
						}
					}

				}

			}
		}
		private static class AlphaBetaV2
		{
			const int MaxDepth = 6;
			static double Value = 0;
			public static Board Solve(Board b)
			{
				Board ret = new Board();

				if (b.Turn) Value = min(MaxDepth, double.MinValue, double.MaxValue, b);
				else Value = max(MaxDepth, double.MinValue, double.MaxValue, b);

				val = Value;

				return ret;

				double max(int tiefe, double alpha, double beta, Board board)
				{
					if (tiefe == 0) return board.Eva;

					double maxWert = alpha;
					List<Board> moves = board.GetMoves();

					moves.Sort();
					moves.Reverse();

					if (moves.Count == 0) return board.Eva;
					for(int i = 0; i < moves.Count; i++)
					{
						double wert = min(tiefe - 1, maxWert, beta, moves[i]);

						if (wert > maxWert)
						{
							maxWert = wert;
							if (maxWert >= beta)
								break;

							if (tiefe == MaxDepth) ret = moves[i]; 
						}
					}
					return maxWert;
				}
				double min(int tiefe, double alpha, double beta, Board board)
				{
					if (tiefe == 0) return board.Eva;

					double minWert = beta;
					List<Board> moves = board.GetMoves();
					moves.Sort();

					if (moves.Count == 0) return board.Eva;
					for (int i = 0; i < moves.Count; i++)
					{
						double wert = max(tiefe - 1, alpha, minWert, moves[i]);

						if (wert < minWert)
						{
							minWert = wert;
							if (minWert <= alpha)
								break;

							if (tiefe == MaxDepth) ret = moves[i];
						}
					}
					return minWert;
				}
			}
		}
		
		private static class Jochen
		{
			const double FACTOR = 3;
			public static int Iteration;
			static Random Rnd;

			public static Board Predict(Board b, long maxTime)
			{
                //Console.WriteLine("Hallo Welt");
				Rnd = new Random();

				long tickstart = Environment.TickCount;

				Root r = new Root(b, !b.Turn);
				Board.Brick won = Board.Brick.Empty;
				Iteration = 0;
				while (Environment.TickCount - tickstart < maxTime && won == Board.Brick.Empty)
				{
					won = r.Visit();
					Iteration++;
				}

				if (won != Board.Brick.Empty)
				{
					Console.WriteLine("Winner is " + won);
					return new Board();
				}

				return r.GetBest();
			}


			class Root : IComparable<Root>
			{

				public double Value;
				public int VCount;
				private bool Maximize;

				public Board.Brick currentWinner { get { return board.Winner; } }
				private bool mixed;

				private Board board;
				private List<Root> moves;

				public Root(Board b, bool max)
				{
					board = b;
					Maximize = max;

					VCount = 0;
					Value = board.Eva;

					mixed = false;
				}

				public Board.Brick Visit()
				{
					if(VCount == 0)
					{
						moves = board.GetMoves().Select(x => new Root(x, !Maximize)).ToList();

						if(moves.Count > 0)
						{
							for(int i = 0; i < moves.Count; i++)
							{
								if (moves[i].currentWinner != Board.Brick.Empty)
								{
									moves = new List<Root>(1) { moves[i] };
									return moves[0].currentWinner;
								}
							}
						}
					}
					else
					{
						//Root best = Maximize ? moves.Max() : moves.Min();
						Root best = moves.Min();
						Board.Brick won = best.Visit();

						if (won != Board.Brick.Empty)
						{
							if (won == (Maximize ? Board.Brick.White : Board.Brick.Black))
							{
								moves = new List<Root>(1) { best };
								return won;
							}
							else
							{
								moves.Remove(best);
							}
						}
					}

					CalcVal();
					VCount++;

					return moves.Count > 0 ? Board.Brick.Empty : Maximize ? Board.Brick.Black : Board.Brick.White;
					
					void CalcVal()
					{
						Value = Maximize ? double.MinValue : double.MaxValue;
						for (int i = 0; i < moves.Count; i++)
						{
							if (Maximize) Value = Math.Max(Value, moves[i].Value);
							else Value = Math.Min(Value, moves[i].Value);
						}
					}
				}

				public Board GetBest()
				{
					Root ret = moves[Rnd.Next(moves.Count)];

					for(int i = 0; i < moves.Count; i++)
					{
						if (Maximize)
						{
							if(moves[i].Value > ret.Value)
							{
								ret = moves[i];
							}
						}
						else
						{
							if (moves[i].Value < ret.Value)
							{
								ret = moves[i];
							}
						}
					}

					val = ret.Value;
					return ret.board;
				}

				public int CompareTo(Root other)
				{
					if (VCount == other.VCount)
					{
						if (Maximize) return Value.CompareTo(other.Value);
						else return -Value.CompareTo(other.Value);
					}
					else return VCount.CompareTo(other.VCount);
				}

				public override string ToString()
				{
					return board.LastMove + ", vCount: " + VCount.ToString("000") + " Value: " + Value.ToString("0.00");
				}
			}
		}
	}
}
