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

			if (val.ToString().Length > 30)
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
			if (Jochen.Tree.Best != null)
			{
				FinalBoard = Jochen.Tree.Best.board;
				//Console.WriteLine(FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + "\n");
			}
			else lost = true;
			sw.Stop();

			//Console.WriteLine("Jochen: " + board.Turn.ToString() + " " + FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + "\n");
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
					List<Board> moves = board.GetNearMoves();

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
					List<Board> moves = board.GetNearMoves();
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
			private const int BaseDepth = 1;
			public static int Deepness = 0;
			private static Stopwatch sw;

			private static int ITCount;
			private const double Factor = .14;

			public static Root Tree;

			private static double MoveCount(int x)
			{
				return 83.32779931622647 - 74.47931529411333 * Math.Exp(-0.030021967172434064 * x);
			}
			public static void Initialize(Board b)
			{
				sw = new Stopwatch();
				sw.Start();

				Deepness = 0;

				ITCount = 1;
				Tree = new Root(b, 0);
				//Console.WriteLine("depth 1 in " + sw.ElapsedMilliseconds.ToString("0,000"));

				//if (Tree.Winner != Board.Brick.Empty) Console.WriteLine("The Winner is " + Tree.Winner);
				//else Console.WriteLine("Best so far: "  + Tree.Best.ToString());

				if(Tree.Best != null)
				{
					val = Tree.Best.Evaluation;
				}
			}

			public static void Extend(long time)
			{
				ITCount = 0;
				while (sw.ElapsedMilliseconds < time && Tree.Winner == Board.Brick.Empty)
				{
					Tree.Visit();
					ITCount++;

					if (Tree.moves == null) break;
					if (Tree.moves.Count <= 1) break;
				}

				if (Tree.Best != null)
				{
					val = Tree.Best.Evaluation;
				}

				// Console.Write("Deepness: " + Deepness.ToString());
			}

			public class Root : IComparable<Root>
			{
				public bool Maximize { get { return board.Turn == Board.Brick.White; } }

				public Board.Brick Winner;
				public Root Best;
				public double Evaluation;
				public readonly Board board;

				private int Count;

				private double worst, range;

				private readonly int depth;
				public List<Root> moves;

				public Root(Board b, int d)
				{
					Count = 1;

					board = b;
					depth = d;

					if (depth > Deepness) Deepness = depth;

					Winner = board.Winner;

					Evaluation = board.Evaluate();

					if(depth == 0)
					{
						Console.WriteLine("###################Alarm##############");
						List<Board> boards;

						boards = board.GetNearMoves(2);

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
								}
							}
						}
					}
				}

				public void FindBest()
				{
					Best = moves[0];
					worst = Best.Evaluation;
					for (int i = 1; i < moves.Count; i++)
					{
						if (Maximize)
						{
							if (moves[i].Evaluation > Best.Evaluation) Best = moves[i];
							if (moves[i].Evaluation < worst) worst = moves[i].Evaluation;
						}
						else
						{
							if (moves[i].Evaluation < Best.Evaluation) Best = moves[i];
							if (moves[i].Evaluation > worst) worst = moves[i].Evaluation;
						}
					}
					range = Best.Evaluation - worst;
					Evaluation = Best.Evaluation;
				}

				public bool Visit()
				{
					if (Count == 1)
					{
						List<Board> boards;

						boards = board.GetNearMoves(2);

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
					}
					else
					{
						double best = moves[0].GetUCB1(worst, range);
						int index = 0;

						for (int i = 0; i < moves.Count; i++)
						{
							if (moves[i].GetUCB1(worst, range) > best)
							{
								best = moves[i].GetUCB1(worst, range);
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
					}

					FindBest();

					Count++;
					return false;
				}

				public double GetUCB1(double min, double range)
				{
					return (Evaluation - min) / range / Count + Factor * Math.Sqrt(Math.Log(ITCount) / Count);
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