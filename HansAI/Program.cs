using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Gomoku
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Random rnd = new Random();

			string path = args[0];
			bool starts = args[1] == "B";
			int time = int.Parse(args[2]);
			bool debug = args[3] == "1";
			//string path = Console.ReadLine();
			//bool starts = Console.ReadLine() == "B";

			if (debug) Console.WriteLine("[HansAI]: Color: " + (starts ? "Black" : "White") + ", TextPath: " + path);

			Board b = new Board();
			if (starts)
			{
				b.DoMove(new Position(6, 6));
				File.WriteAllText(path, b.GetMoveString());
				if (debug) Console.WriteLine("[HansAI]: Starting: TextFileChangedTo " + b.GetMoveString());
			}

			string lastGame = b.GetMoveString();


			while (true)
			{

				while (CheckFile()) Thread.Sleep(50);


				b = new Board(path);
				if (debug) Console.WriteLine("[HansAI]: Reading " + lastGame);

				//b.WriteData();

				HansAI bob = new HansAI(b, time);
				if (bob.FinalBoard == null)
				{
					if (debug) Console.WriteLine("###################Alarm##############");

					List<Board> boards = b.GetNearMoves(2);

					b = boards[rnd.Next(boards.Count)];
				}
				else b = bob.FinalBoard;

				//b.WriteData();
				lastGame = b.GetMoveString();

				while (!WriteFile()) Thread.Sleep(10);

				if (debug) Console.WriteLine("[HansAI]: TextFileChangedTo " + lastGame);
			}


			bool CheckFile()
			{
				try
				{
					return File.ReadAllText(path) == lastGame;
				}
				catch (Exception e)
				{
					//Console.WriteLine("[HansAI]: Catched: " + e.ToString());
					return false;
				}
			}

			bool WriteFile()
			{
				try
				{
					File.WriteAllText(path, lastGame);
					return true;
				}
				catch (Exception e)
				{
					//Console.WriteLine("[HansAI]: Catched: " + e.ToString());
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