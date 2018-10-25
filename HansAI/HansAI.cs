using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gomoku
{
	internal class HansAI
	{
		public static bool lost = false;
		public static bool Version = false;
		private static double val = 0;
		public Board FinalBoard;

		public HansAI(Board board)
		{
			lost = false;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			FinalBoard = AlphaBetaV2.Solve(board);

			if(val.ToString().Length > 30)
			{

			}

			sw.Stop();
			Console.WriteLine("Alpha:  " + FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + " in " + sw.ElapsedMilliseconds.ToString("000 000") + "\n");
			if (FinalBoard.LastMove == new Position(-1, -1))
			{
				lost = true;
			}
		}

		public HansAI(Board board, long time)
		{
			lost = false;
			Stopwatch sw = new Stopwatch();

			sw.Start();
			Jochen.Initialize(board);
			Jochen.Extend(time);

			FinalBoard = Jochen.Tree.Best.board;
			sw.Stop();

			Console.WriteLine("Jochen: " + board.Turn.ToString() + " " + FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + "\n");
			if (FinalBoard.LastMove == new Position(-1, -1))
			{
				lost = true;
			}
		}

		private static class AlphaBetaV2
		{
			private const int MaxDepth = 6;
			private static double Value = 0;

			public static Board Solve(Board b)
			{
				Board ret = new Board();

				if (b.Turn == Board.Brick.Black) Value = min(MaxDepth, double.MinValue, double.MaxValue, b);
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
					for (int i = 0; i < moves.Count; i++)
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
			private const int MaxDepth = 4;
			private const int BaseDepth = 2;
			public static int Deepness = 0;
			private static Stopwatch sw;

			private static int ITCount;
			private const double Factor = 4.66920;

			public static Root Tree;

			public static void Initialize(Board b)
			{
				sw = new Stopwatch();
				sw.Start();

				Deepness = 0;

				ITCount = 1;
				Tree = new Root(b, 0);
				//Console.WriteLine("depth 2 in " + sw.ElapsedMilliseconds.ToString("0,000"));

				if (Tree.Winner != Board.Brick.Empty) Console.WriteLine("The Winner is " + Tree.Winner);
				//else Console.WriteLine("Best so far: "  + Tree.Best.ToString());

				val = Tree.Best.Evaluation;
			}

			public static void Extend(long time)
			{
				ITCount = 0;
				while (sw.ElapsedMilliseconds < time && Tree.Winner == Board.Brick.Empty)
				{
					Tree.Visit();
					ITCount++;
				}
				val = Tree.Best.Evaluation;

				Console.WriteLine("Deepness: " + Deepness.ToString());
			}

			public class Root : IComparable<Root>
			{
				public bool Maximize { get { return board.Turn == Board.Brick.White; } }

				public Board.Brick Winner;
				public Root Best;
				public double Evaluation;
				public readonly Board board;

				private int Count;

				private readonly int depth;
				private List<Root> moves;

				public Root(Board b, int d)
				{
					Count = 1;

					board = b;
					depth = d;

					if (depth > Deepness) Deepness = depth;


					Winner = board.Winner;

					Evaluation = board.Evaluate();

					if (depth <= BaseDepth && Winner == Board.Brick.Empty)
					{
						Count++;
						List<Board> boards;

						boards = board.GetMoves();

						boards.Sort();
						if (Maximize) boards.Reverse();

						moves = new List<Root>(boards.Count);

						for (int i = 0; i < boards.Count; i++)
						{
							Root r = new Root(boards[i], depth + 1);

							if (r.Winner != Board.Brick.Empty)
							{
								if (r.Winner == board.Turn)
								{
									Winner = board.Turn;
									Best = r;
									moves = null;
									return;
								}
							}
							else moves.Add(r);
						}

						if (moves.Count == 0)
						{
							Winner = board.Turn == Board.Brick.White ? Board.Brick.Black : Board.Brick.White;
						}
						else
						{
							FindBest();
						}
					}
				}

				public void FindBest()
				{
					Best = moves[0];
					for (int i = 1; i < moves.Count; i++)
					{
						if (Maximize)
						{
							if (moves[i].Evaluation > Best.Evaluation) Best = moves[i];
						}
						else
						{
							if (moves[i].Evaluation < Best.Evaluation) Best = moves[i];
						}
					}

					Evaluation = Best.Evaluation;
				}

				public bool Visit()
				{
					if(Count == 1)
					{
						List<Board> boards;

						boards = board.GetMoves();

						boards.Sort();
						if (Maximize) boards.Reverse();

						moves = new List<Root>(boards.Count);

						for (int i = 0; i < boards.Count; i++)
						{
							Root r = new Root(boards[i], depth + 1);

							if (r.Winner != Board.Brick.Empty)
							{
								if (r.Winner == board.Turn)
								{
									Winner = board.Turn;
									Best = r;
									moves = null;
									return true;
								}
							}
							else moves.Add(r);
						}

						if (moves.Count == 0)
						{
							Winner = board.Turn == Board.Brick.White ? Board.Brick.Black : Board.Brick.White;
							return true;
						}

						FindBest();
					}
					else
					{
						double best = moves[0].GetUCB1();
						int index = 0;

						for(int i = 0; i < moves.Count; i++)
						{
							if(moves[i].GetUCB1() > best)
							{
								best = moves[i].GetUCB1();
								index = i;
							}
						}

						if (moves[index].Visit())
						{
							if (moves[index].Winner == board.Turn)
							{
								Winner = board.Turn;
								Best = moves[index];
								moves = null;
								return true;
							}
							else moves.RemoveAt(index);
						}

						if (moves.Count == 0)
						{
							Winner = board.Turn == Board.Brick.White ? Board.Brick.Black : Board.Brick.White;
							return true;
						}

						FindBest();
					}

					Count++;
					return false;
				}

				public double GetUCB1()
				{						//Cuz depth + 1
					return Evaluation * (Maximize ? -1 : 1) / Count + Factor * Math.Sqrt(Math.Log(ITCount) / Count);
				}

				public int CompareTo(Root other)
				{
					return Evaluation.CompareTo(other.Evaluation);
				}

				public override string ToString()
				{
					return depth.ToString() + " -> " + board.LastMove.ToString() + " with " + Evaluation.ToString("000.000");
				}
			}
		}
	}
}