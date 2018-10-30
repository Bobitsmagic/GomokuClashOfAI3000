using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Gomoku
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			string path = args[0];
			bool starts = args[1] == "B";

			Board b = new Board();
			if (starts) b.DoMove(new Position(6, 6));

			string lastGame = b.GetMoveString();
			while (true)
			{
				while (File.ReadAllText(path) == lastGame) Thread.Sleep(50);
				b = new Board(path);

				HansAI bob = new HansAI(b, 2000);
				b = bob.FinalBoard;

				lastGame = b.GetMoveString();

				File.WriteAllText(path, lastGame);
			}
		}


		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		//[STAThread]
		//private static void Main()
		//{
		//	Application.EnableVisualStyles();
		//	Application.SetCompatibleTextRenderingDefault(false);
		//	Application.Run(new Screen());
		//}
	}
}