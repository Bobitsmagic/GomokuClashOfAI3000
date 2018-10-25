﻿using System;
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

		private void Screen_Load(object sender, EventArgs e)
		{
			//b.DoMove(new Position(8, 8));
		}

		private void Screen_Paint(object sender, PaintEventArgs e)
		{
			b.Draw(e.Graphics);
		}

		private void Screen_MouseClick(object sender, MouseEventArgs e)
		{
			b.ParseMove(e.Location);
			Refresh();
			HansAI bob = new HansAI(b, 10000);
			b = bob.FinalBoard;
			Refresh();

			//int max = 0;
			//while (true)
			//{
			//	b = new Board();
			//	b.DoMove(new Position(8, 8));
			//	Refresh();
			//	HansAI.lost = false;
			//	int i = 0;
			//	while (!HansAI.lost)
			//	{
			//		Application.DoEvents();

			//		Console.Write(i++ + " ");
			//		HansAI bob = new HansAI(b);
			//		b = bob.FinalBoard;
			//		Refresh();

			//		Console.Write(i++ + " ");
			//		bob = new HansAI(b, 5000);
			//		b = bob.FinalBoard;
			//		Refresh();
			//	}

			//	max = Math.Max(i, max);
			//}
		}

		private void BEngine1_Click(object sender, EventArgs e)
		{
			HansAI bob = new HansAI(b, 10000);
		}
	}
}