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
			//string path = Console.ReadLine();
			//bool starts = Console.ReadLine() == "B";

			Console.WriteLine("[HansAI]: Color: " + (starts ? "Black" : "White") + ", TextPath: " + path);

			Board b = new Board();
			if (starts)
			{
				b.DoMove(new Position(6, 6));
				File.WriteAllText(path, b.GetMoveString());
				Console.WriteLine("[HansAI]: Starting: TextFileChangedTo " + b.GetMoveString());
			}

			string lastGame = b.GetMoveString();


			while (true)
			{

				while (CheckFile()) Thread.Sleep(50);

				b = new Board(path);
				//b.WriteData();

				HansAI bob = new HansAI(b, 2000);
				b = bob.FinalBoard;

				//b.WriteData();
				lastGame = b.GetMoveString();

				File.WriteAllText(path, lastGame);
				Console.WriteLine("[HansAI]: TextFileChangedTo " + lastGame);
			}

			bool CheckFile()
			{
				try
				{
					return File.ReadAllText(path) == lastGame;
				}
				catch(Exception e)
				{
					Console.WriteLine("[HansAI]: Catched: " + e.ToString());
					return false;
				}
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