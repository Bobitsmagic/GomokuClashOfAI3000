using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{
	class Network
	{
		public int InputSize { get { return Neurons[0].Length; } }
		public int OutputSize { get { return Neurons.Last().Length; } }

		public static Random rnd = new Random();
		//[TODO] Dropout

		public Neuron[][] Neurons;

		//Constructor
		public Network(int[] NeuronCount)
		{
			Neurons = new Neuron[NeuronCount.Length][];

			for (int i = 0; i < Neurons.Length; i++)
			{
				Neurons[i] = new Neuron[NeuronCount[i]];
				for (int j = 0; j < NeuronCount[i]; j++)
				{
					Neurons[i][j] = new Neuron(this, i, j);
				}
			}
		}
		public Network(string path)
		{
			Stream s = File.OpenRead(path);
			BinaryReader br = new BinaryReader(s);

			Neurons = new Neuron[br.ReadInt32()][];
			for (int i = 0; i < Neurons.Length; i++)
			{
				Neurons[i] = new Neuron[br.ReadInt32()];
			}

			for (int i = 0; i < Neurons[0].Length; i++)
			{
				Neurons[0][i] = new Neuron(this, 0, i);
			}
			for (int i = 1; i < Neurons.Length; i++)
			{
				for (int j = 0; j < Neurons[i].Length; j++)
				{
					Neurons[i][j] = new Neuron(this, i, j, ref br);
				}
			}

			br.Close();
		}

		//IO
		public void WriteAsFile(string path)
		{
			Stream s = File.Create(path);
			BinaryWriter bw = new BinaryWriter(s);

			//Neurons.Length
			//Neurons[i].Length
			//Neurons[i + 1][j]

			bw.Write(Neurons.Length);
			for (int i = 0; i < Neurons.Length; i++)
			{
				bw.Write(Neurons[i].Length);
			}

			for (int i = 1; i < Neurons.Length; i++)
			{
				for (int j = 0; j < Neurons[i].Length; j++)
				{
					Neurons[i][j].WriteAsFile(ref bw);
				}
			}

			bw.Close();
		}
		private void FlipRndSign()
		{
			Console.Write("Try to jump ");
			int layer = rnd.Next(1, Neurons.Length);
			for (int i = 0; i < Neurons.Length - 1; i++)
			{
				int ni = rnd.Next(Neurons[layer].Length);
				for (int j = 0; j < Neurons[layer].Length; j++)
				{

					if (Neurons[layer][ni].FlipSign())
					{
						Console.WriteLine("Did jump!");
						j = Neurons[layer].Length;
						i = Neurons.Length;
					}

					ni = (ni + 1) % Neurons[layer].Length;
				}
				layer = (layer + 1) % (Neurons.Length - 1) + 1;
			}
		}

		//Overrides
		public void CalcOutput()
		{
			for (int i = 1; i < Neurons.Length; i++)
			{
				for (int j = 0; j < Neurons[i].Length; j++)
				{
					Neurons[i][j].CalcOut();
				}
			}
		}

		public void SetInput(double[] input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				Neurons[0][i].CalcOut(input[i]);
			}
		}

		public double GetOutput(int index)
		{
			return Neurons[Neurons.Length - 1][index].Output;
		}
		public double[] GetOutput()
		{
			double[] ret = new double[OutputSize];

			for (int i = 0; i < OutputSize - 1; i++)
			{
				ret[i] = GetOutput(i);
			}
			ret[OutputSize - 1] = GetOutput(OutputSize - 1);

			return ret;
		}
		public double[] GetOutput(double[] data)
		{
			SetInput(data);

			CalcOutput();

			return GetOutput();
		}

		private double Train(DataSet dataSet)
		{
			double errorSum = 0, errorBuffer;
			for (int i = 0; i < dataSet.SetNumber; i++)
			{
				SetInput(dataSet.InputSet[i]);
				CalcOutput();

				for (int index = 0; index < OutputSize; index++)
				{
					errorBuffer = dataSet.OutputSet[i][index] - GetOutput(index);
					Neurons[Neurons.Length - 1][index].CalcDW(errorBuffer);
					errorSum += errorBuffer * errorBuffer;
				}

				for (int layer = Neurons.Length - 2; layer > 0; layer--)
				{
					for (int index = 0; index < Neurons[layer].Length; index++)
					{
						Neurons[layer][index].CalcDW();
					}
				}
			}

			for (int layer = 1; layer < Neurons.Length; layer++)
			{
				for (int index = 0; index < Neurons[layer].Length; index++)
				{
					Neurons[layer][index].ApplyDW(dataSet.SetNumber);
				}
			}

			return errorSum * 0.5;
		}
		private double Train(DataSet dataSet, List<int> toUse)
		{
			double errorSum = 0, errorBuffer;
			for (int i = 0; i < toUse.Count; i++)
			{
				SetInput(dataSet.InputSet[toUse[i]]);
				CalcOutput();

				for (int index = 0; index < OutputSize; index++)
				{
					errorBuffer = dataSet.OutputSet[toUse[i]][index] - GetOutput(index);
					Neurons[Neurons.Length - 1][index].CalcDW(errorBuffer);
					errorSum += errorBuffer * errorBuffer;
				}

				for (int layer = Neurons.Length - 2; layer > 0; layer--)
				{
					for (int index = 0; index < Neurons[layer].Length; index++)
					{
						Neurons[layer][index].CalcDW();
					}
				}
			}

			for (int layer = 1; layer < Neurons.Length; layer++)
			{
				for (int index = 0; index < Neurons[layer].Length; index++)
				{
					Neurons[layer][index].ApplyDW(dataSet.SetNumber);
				}
			}

			return errorSum * 0.5;
		}

		public double DoSession(DataSet data, int count)
		{
			double ret = 0;
			double StartLR = 0.3, EndLR = 0.01;
			for (int i = 0; i < count; i++)
			{
				Neuron.Lernrate = StartLR - (double)i / count * (StartLR - EndLR);
				ret = Train(data);

				if ((i & 1023) == 0 && false) //
				{
					if (Console.CursorTop != 0) Console.CursorTop = Console.CursorTop - 1;
					Console.WriteLine(i);
				}
			}

			ret = CountErrors(data, 0.3).Count;
			//WriteOutput(data);

			//Utility.G.DrawImage(DrawNetwork(), 512, 0);
			//Utility.G.DrawImage(MapNetwork(5), 0, 0);
			//Utility.G.DrawImage(DrawHyperplanes(data), 256, 0);

			return ret;
		}
		public double DoSession(DataSet data, int count, int sampleSize)
		{
			double ret = 0;
			List<int> ShuffleList = Enumerable.Range(0, data.SetNumber).ToList();
			for (int i = 0; i < count; i++)
			{
				List<int> sample = new List<int>(sampleSize);
				for (int j = 0; j < sampleSize && ShuffleList.Count > 0; j++)
				{
					int buffer = rnd.Next(ShuffleList.Count);

					sample.Add(ShuffleList[buffer]);
					ShuffleList.RemoveAt(buffer);
				}
				ret = Train(data, sample);

				if ((i & 1023) == 0)
				{
					if (Console.CursorTop != 0) Console.CursorTop = Console.CursorTop - 1;
					Console.WriteLine(i);


					//Utility.G.DrawImage(DrawNetwork(), 512, 0);
					//Utility.G.DrawImage(MapNetwork(5), 0, 0);
					//Utility.G.DrawImage(DrawHyperplanes(data), 256, 0);
				}

				if (ShuffleList.Count == 0) ShuffleList = Enumerable.Range(0, data.SetNumber).ToList();
			}


			//WriteOutput(data);

			//Utility.G.DrawImage(DrawNetwork(), 512, 0);
			//Utility.G.DrawImage(MapNetwork(5), 0, 0);
			//Utility.G.DrawImage(DrawHyperplanes(data), 256, 0);

			return ret;
		}


		public List<int> CountErrors(DataSet data, double precision)
		{
			List<int> ret = new List<int>(data.SetNumber / 2);
			for (int i = 0; i < data.SetNumber; i++)
			{
				double[] buffer = GetOutput(data.InputSet[i]);

				for (int j = 0; j < buffer.Length; j++)
				{
					if (Math.Abs(buffer[j] - data.OutputSet[i][j]) > precision)
					{

						ret.Add(i);
						return ret;
						break;
					}
				}
			}

			return ret;
		}
		public void WriteOutput(DataSet data)
		{
			Console.CursorTop = 0;
			Console.CursorLeft = 0;
			StringBuilder s = new StringBuilder(10000);
			for (int i = 0; i < data.SetNumber; i++)
			{
				double[] buffer = GetOutput(data.InputSet[i]);
				s.Append(i.ToString("0000") + " {");

				for (int j = 0; j < InputSize - 1; j++)
				{
					if ((j & 15) == 0) s.Append("\n");
					s.Append((data.InputSet[i][j] / 6 + 0.5).ToString("0"));

					//s.Append(data.InputSet[i][j].ToString(data.InputSet[i][j] >= 0 ? " 0.000, " : "0.000, "));
				}
				//s.Append(data.InputSet[i].Last().ToString(data.InputSet[i].Last() >= 0 ? " 0.000 }" : "0.000 }"));
				s.Append((data.InputSet[i].Last() / 6 + 0.5).ToString("0"));

				s.Append(" -> {");
				for (int j = 0; j < OutputSize - 1; j++)
				{
					s.Append(buffer[j].ToString(buffer[j] >= 0 ? " 0.000, " : "0.000, "));
				}

				s.Append(buffer.Last().ToString(buffer.Last() >= 0 ? " 0.000 }" : "0.000 }") + "\n");
			}

			Console.WriteLine(s);
		}

		public void SetRandomWeigths()
		{
			for (int i = 1; i < Neurons.Length; i++)
			{
				for (int j = 0; j < Neurons[i].Length; j++)
				{
					Neurons[i][j].SetRandomWeights();
				}
			}
		}
	}
}
