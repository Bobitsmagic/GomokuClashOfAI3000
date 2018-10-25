using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{
	public class DataSet
	{
		const double MAxValue = 3;

		//Propertys
		public int SetNumber { get { return InputSet.Length; } }
		public int InputSize { get { return InputSet[0].Length; } }
		public int OutputSize { get { return OutputSet[0].Length; } }

		//Data
		public double[][] InputSet;
		public double[][] OutputSet;

		public string[] Name;
		//Constructor
		/// <summary>
		/// Creates a Dataset from .txt-file.
		/// </summary>
		public DataSet(string path)
		{
			//SetNumber, InputSize, OutputSize, {inputs} # {outputs}

			StreamReader sr = new StreamReader(path);

			InputSet = new double[int.Parse(sr.ReadLine())][];
			OutputSet = new double[InputSet.Length][];

			InputSet[0] = new double[int.Parse(sr.ReadLine())];
			OutputSet[0] = new double[int.Parse(sr.ReadLine())];

			string[] LineBuffer, ValueBuffer;
			for (int i = 0; i < InputSet.Length; i++)
			{
				LineBuffer = sr.ReadLine().Split('#');

				ValueBuffer = LineBuffer[0].Split(',');
				InputSet[i] = new double[InputSize];
				for (int j = 0; j < InputSize; j++)
				{
					InputSet[i][j] = double.Parse(ValueBuffer[j]);
				}

				ValueBuffer = LineBuffer[1].Split(',');
				OutputSet[i] = new double[OutputSize];
				for (int j = 0; j < OutputSize; j++)
				{
					OutputSet[i][j] = double.Parse(ValueBuffer[j]);
				}

			}

			Console.WriteLine("Initialized DataSet on:\n" + path + "\n" + ToString());
		}
		public DataSet(int number, int inSize, int outSize)
		{
			InputSet = new double[number][];
			OutputSet = new double[number][];
			Name = new string[number];

			for (int i = 0; i < number; i++)
			{
				InputSet[i] = new double[inSize];
				OutputSet[i] = new double[outSize];
			}

		}
		public static DataSet XOR()
		{
			DataSet ret = new DataSet(4, 2, 1);
			ret.InputSet[0] = new double[2] { 0, 0 };
			ret.InputSet[1] = new double[2] { 0, 1 };
			ret.InputSet[2] = new double[2] { 1, 0 };
			ret.InputSet[3] = new double[2] { 1, 1 };

			ret.OutputSet[0] = new double[1] { 0 };
			ret.OutputSet[1] = new double[1] { 1 };
			ret.OutputSet[2] = new double[1] { 1 };
			ret.OutputSet[3] = new double[1] { 0 };

			ret.NormalizeInput();
			return ret;

		}
		public static DataSet Easy()
		{
			DataSet ret = new DataSet(2, 1, 1);
			ret.InputSet[0] = new double[1] { -1 };
			ret.InputSet[1] = new double[1] { 1 };

			ret.OutputSet[0] = new double[1] { 0 };
			ret.OutputSet[1] = new double[1] { 1 };

			return ret;
		}

		//Voids
		public void WriteAsFile(string fileName)
		{
			FindMeanVarience(ref InputSet);

			Stream s = File.Create(fileName);
			BinaryWriter bw = new BinaryWriter(s);

			bw.Write(SetNumber);
			bw.Write(InputSize);
			bw.Write(OutputSize);

			for (int i = 0; i < SetNumber; i++)
			{
				for (int j = 0; j < InputSize; j++)
				{
					bw.Write(InputSet[i][j]);
				}

				for (int j = 0; j < OutputSize; j++)
				{
					bw.Write(OutputSet[i][j]);
				}
			}

			bw.Close();
		}

		public void NormalizeInput()
		{
			double[] max = new double[InputSet[0].Length], min = new double[InputSet[0].Length];

			double[] Mean;
			double[] Variance;

			Mean = new double[InputSet[0].Length];
			Variance = new double[InputSet[0].Length];

			for (int i = 0; i < InputSet.Length; i++)
			{
				for (int j = 0; j < InputSet[0].Length; j++)
				{
					max[j] = Math.Max(max[j], InputSet[i][j]);
					min[j] = Math.Min(min[j], InputSet[i][j]);
				}
			}

			for (int j = 0; j < InputSet[0].Length; j++)
			{
				Mean[j] = -(max[j] + min[j]) / 2;
				Variance[j] = MAxValue / (-Mean[j] - min[j]);
			}

			for (int i = 0; i < InputSet.Length; i++)
			{
				for (int j = 0; j < InputSet[0].Length; j++)
				{
					InputSet[i][j] = (InputSet[i][j] + Mean[j]) * Variance[j];
				}
			}

			//Console.WriteLine("Normalized to: \n" + ToString());
		}

		public static void FindMeanVarience(ref double[][] input)
		{
			double[] max = new double[input[0].Length], min = new double[input[0].Length];

			double[] Mean;
			double[] Variance;

			Mean = new double[input[0].Length];
			Variance = new double[input[0].Length];

			for (int i = 0; i < Mean.Length; i++)
			{
				max[i] = double.MinValue;
				min[i] = double.MaxValue;
			}

			for (int i = 0; i < input.Length; i++)
			{
				for (int j = 0; j < input[0].Length; j++)
				{
					max[j] = Math.Max(max[j], input[i][j]);
					min[j] = Math.Min(min[j], input[i][j]);
				}
			}

			for (int j = 0; j < input[0].Length; j++)
			{
				Mean[j] = -(max[j] + min[j]) / 2;
				Variance[j] = MAxValue / (-Mean[j] - min[j]);
			}

			for (int i = 0; i < input.Length; i++)
			{
				for (int j = 0; j < input[0].Length; j++)
				{
					input[i][j] = (input[i][j] + Mean[j]) * Variance[j];
				}
			}
		}

		public static DataSet IririsData(string path)
		{
			StreamReader sr = new StreamReader(path);

			DataSet ret = new DataSet(150, 4, 3);

			string[] buffer;
			for (int i = 0; i < 150; i++)
			{
				buffer = sr.ReadLine().Split(',');
				ret.InputSet[i] = new double[4] { double.Parse(buffer[0]), double.Parse(buffer[1]), double.Parse(buffer[2]), double.Parse(buffer[3]) };
			}

			for (int i = 0; i < 50; i++)
			{
				ret.OutputSet[i] = new double[3] { 1, 0, 0 };
				ret.OutputSet[i + 50] = new double[3] { 0, 1, 0 };
				ret.OutputSet[i + 100] = new double[3] { 0, 0, 1 };
			}

			//Console.WriteLine("Loaded iris.ds:\n" + ret.ToString());
			FindMeanVarience(ref ret.InputSet);
			//Console.WriteLine("Normalized to: \n" + ret.ToString());

			return ret;
		}
		public static DataSet Digits(string path)
		{
			StreamReader sr = new StreamReader(path);

			string[] text = sr.ReadToEnd().Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			DataSet ret = new DataSet(text.Length, 1, 1);
			string[] buffer;
			for (int i = 0; i < ret.SetNumber; i++)
			{
				buffer = text[i].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				ret.InputSet[i] = new double[256];

				for (int j = 0; j < ret.InputSet[i].Length; j++)
				{
					ret.InputSet[i][j] = double.Parse(buffer[j]);
				}

				ret.OutputSet[i] = new double[10];
				for (int j = 256; j < 266; j++)
				{
					ret.OutputSet[i][j - 256] = double.Parse(buffer[j]);
				}
			}

			//Console.WriteLine("Loaded Digits.txt:\n" + ret.ToString());
			FindMeanVarience(ref ret.InputSet);
			//Console.WriteLine("Normalized to: \n" + ret.ToString());

			return ret;
		}

		public void GetRndSet(out double[] input, out double[] output)
		{
			int i = new Random().Next(SetNumber);
			input = InputSet[i];
			output = OutputSet[i];
		}
		//Overrides
		public override string ToString()
		{
			string s = "";
			for (int i = 0; i < SetNumber; i++)
			{
				s += i.ToString() + " {";

				//if (Name != null) s += Name[i] + " | ";
				for (int j = 0; j < InputSize - 1; j++)
				{
					s += InputSet[i][j].ToString(InputSet[i][j] >= 0 ? " 0.000, " : "0.000, ");
				}
				s += InputSet[i].Last().ToString(InputSet[i].Last() >= 0 ? " 0.000 }" : "0.000 }");

				s += " -> {";
				for (int j = 0; j < OutputSize - 1; j++)
				{
					s += OutputSet[i][j].ToString(OutputSet[i][j] >= 0 ? " 0.000, " : "0.000, ");
				}

				s += OutputSet[i].Last().ToString(OutputSet[i].Last() >= 0 ? " 0.000 }" : "0.000 }") + "\n";
			}

			return s;
		}
	}
}
