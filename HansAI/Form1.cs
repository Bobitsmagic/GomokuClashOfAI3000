using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Gomoku
{
	public partial class Screen : Form
	{
		public Screen()
		{
			InitializeComponent();
		}

		private Board b = new Board();
		private Network nw = new Network(new int[3] { 15 * 15, 50, 15 * 15 });

		private void Screen_Load(object sender, EventArgs e)
		{
			//b.DoMove(new Position(7, 7));
		}

		private void Screen_Paint(object sender, PaintEventArgs e)
		{
			b?.Draw(e.Graphics);
		}

		private void Screen_MouseClick(object sender, MouseEventArgs e)
		{
			//b.ParseMove(e.Location);
			//Refresh();

			int max = 0;
			while (true)
			{
				b = new Board();
				b.DoMove(new Position(8, 8));
				Refresh();
				HansAI.lost = false;
				int i = 0;
				while (!HansAI.lost)
				{
					Application.DoEvents();

					HansAI bob = new HansAI(b, 5000);
					b = bob.FinalBoard;
					Refresh();

					if (b == null) break;
					bob = new HansAI(b, 5000);
					b = bob.FinalBoard;
					Refresh();
				}

				max = Math.Max(i, max);
			}
		}

		private void BEngine1_Click(object sender, EventArgs e)
		{
			HansAI bob = new HansAI(b, 5000);
			b = bob.FinalBoard;
			Refresh();
		}

		private void BReset_Click(object sender, EventArgs e)
		{
			b = new Board();
			Refresh();
		}

		private void BStatistics_Click(object sender, EventArgs e)
		{
			Random rnd = new Random();

			Stopwatch sw = new Stopwatch();
			sw.Start();
			const int mc = 100;

			int[] count = new int[mc];
			double[] vals = new double[mc];
			
			while(sw.ElapsedMilliseconds < 3 * 60000)
			{
				Board b = new Board();
				b.DoMove(new Position(7, 7));
				for(int i = 0; i< mc && b.Winner == Board.Brick.Empty; i++)
				{
					List<Board> boards = b.GetNearMoves();

					count[i]++;
					vals[i] += boards.Count;

					b = boards[rnd.Next(boards.Count)];
				}
			}

			string s = "";
			for(int i = 0; i < mc; i++)
			{
				if(count[i] == 0)
				{
					vals = vals.Take(i).ToArray();
					Console.WriteLine("Longest: " + i);
					break;
				}
				s += i + "\t" + vals[i] / count[i] + "\n";
			}
			File.WriteAllText(@"C:\Users\Martin\Desktop\out.txt", s);

			Console.WriteLine("Done");
		}
	}
}