using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku
{
	public partial class Screen : Form
	{
		public Screen()
		{
			InitializeComponent();
		}

		Board b = new Board();
		private void Screen_Load(object sender, EventArgs e)
		{
			b.DoMove(new Position(8, 8));
		}

		private void Screen_Paint(object sender, PaintEventArgs e)
		{
			b.Draw(e.Graphics);
		}

		private void Screen_MouseClick(object sender, MouseEventArgs e)
		{
			//b.ParseMove(e.Location);
			//Refresh();
			//HansAI bob = new HansAI(b);
			//b = bob.FinalBoard;
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
					Console.Write(i++ + " ");
					HansAI bob = new HansAI(b);
					b = bob.FinalBoard;
					Refresh();
				}

				max = Math.Max(i, max);
			}
		}
	}
}
