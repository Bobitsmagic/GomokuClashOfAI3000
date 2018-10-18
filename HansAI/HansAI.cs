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
			Console.WriteLine(FinalBoard.LastMove.ToString() + " with " + val.ToString("0.000") + " in " + sw.ElapsedMilliseconds.ToString("000 000"));
			if(FinalBoard.LastMove == new Position(-1, -1))
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
	}
}
