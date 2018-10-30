using System;
using System.IO;

namespace Gomoku
{
	internal class Neuron
	{
		//Statics
		public static double Lernrate = 0.1, Momentum = 0.03, Sat = -0.01;

		private static Random rnd = new Random();

		//Public Data
		public double Output { get; private set; }

		public double Error { get; private set; }
		public double Saturation { get; private set; }

		public double Bias { get; private set; }
		public double[] Weights { get; }

		//private Data
		private Network refNetwork;

		private int layer, index;

		private double deltaBias;
		private double[] deltaWeights;

		//Constructor
		public Neuron(Network network, int _layer, int _index)
		{
			refNetwork = network;
			layer = _layer;
			index = _index;

			if (layer != 0)
			{
				Weights = new double[network.Neurons[layer - 1].Length];
				deltaWeights = new double[Weights.Length];

				SetRandomWeights();
			}
		}

		public Neuron(Network network, int _layer, int _index, ref BinaryReader br)
		{
			refNetwork = network;
			layer = _layer;
			index = _index;

			if (layer != 0)
			{
				Weights = new double[network.Neurons[layer - 1].Length];
				deltaWeights = new double[Weights.Length];
			}

			for (int i = 0; i < Weights.Length; i++)
			{
				Weights[i] = br.ReadDouble();
				deltaWeights[i] = br.ReadDouble();
			}

			Bias = br.ReadDouble();
			deltaBias = br.ReadDouble();
		}

		//IO
		public void WriteAsFile(ref BinaryWriter bw)
		{
			for (int i = 0; i < Weights.Length; i++)
			{
				bw.Write(Weights[i]);
				bw.Write(deltaWeights[i]);
			}

			bw.Write(Bias);
			bw.Write(deltaBias);
		}

		//Voids
		public void SetRandomWeights()
		{
			for (int i = 0; i < Weights.Length; i++)
			{
				Weights[i] = NextGaussian();
				deltaWeights[i] = 0;
			}

			Bias = NextGaussian();
			deltaBias = 0;
		}

		public static double NextGaussian(double mean = 1)
		{
			return mean * Math.Sqrt(-2 * Math.Log(rnd.NextDouble())) *
				Math.Sin(2 * Math.PI * rnd.NextDouble());
		}

		private double theta = 7;

		public bool FlipSign()
		{
			int wi = rnd.Next(Weights.Length + 1);
			for (int i = 0; i <= Weights.Length; i++)
			{
				if (wi == Weights.Length)
				{
					if (Math.Abs(Bias) > theta)
					{
						Bias = NextGaussian();
						return true;
					}
				}
				else
				{
					if (Math.Abs(Weights[wi]) > theta)
					{
						Weights[wi] = NextGaussian();
						return true;
					}
				}

				wi = (wi + 1) % (Weights.Length + 1);
			}

			return false;
		}

		public void CalcOut()
		{
			double sum = Bias;
			for (int i = 0; i < Weights.Length; i++)
			{
				sum += refNetwork.Neurons[layer - 1][i].Output * Weights[i];
			}

			Output = Sigmoid(sum);
			Saturation = SatFunc(Output);

			//Console.WriteLine("CalcOut on layer {0}, index {1}: in={2} out = {3}", layer, index, sum.ToString("0.000"), Output.ToString("0.000"));
		}

		public void CalcOut(double input)
		{
			Output = Sigmoid(input);
			Saturation = SatFunc(Output);

			//Console.WriteLine("CalcOut on layer {0}, index {1}: in={2} out = {3}", layer, index, input.ToString("0.000"), Output.ToString("0.000"));
		}

		public void CalcDW()
		{
			//error * SigmoidDer(output) * lernrate
			Error = 0;
			for (int i = 0; i < refNetwork.Neurons[layer + 1].Length; i++)
			{
				//w * phi'(eingabe) * error
				Error += refNetwork.Neurons[layer + 1][i].Error *
					refNetwork.Neurons[layer + 1][i].Weights[index] *
					SigmoidDer(refNetwork.Neurons[layer + 1][i].Output);
			}
			double constFactor = Error * SigmoidDer(Output);

			for (int i = 0; i < Weights.Length; i++)
			{
				deltaWeights[i] += refNetwork.Neurons[layer - 1][i].Output * constFactor
									+ Sat * refNetwork.Neurons[layer - 1][i].Output * SatFuncDer(Output) * Saturation;
			}

			deltaBias += constFactor + Sat * SatFuncDer(Output) * Saturation;
		}

		public void CalcDW(double error)
		{
			//error * SigmoidDer(output) * lernrate
			Error = error;
			double constFactor = Error * SigmoidDer(Output);

			for (int i = 0; i < Weights.Length; i++)
			{
				deltaWeights[i] += refNetwork.Neurons[layer - 1][i].Output * constFactor
					+ Sat * refNetwork.Neurons[layer - 1][i].Output * SatFuncDer(Output) * Saturation;
			}

			deltaBias += constFactor + Sat * SatFuncDer(Output) * Saturation;
		}

		public void ApplyDW(int testCount)
		{
			for (int i = 0; i < Weights.Length; i++)
			{
				Weights[i] += Lernrate * deltaWeights[i] / testCount;
				deltaWeights[i] *= Momentum;
			}
			Bias += deltaBias * Lernrate / testCount;
			deltaBias *= Momentum;
		}

		public static double Sigmoid(double x)
		{
			return 1 / (1 + (double)Math.Exp(-x));
		}

		public static double SigmoidDer(double y)
		{
			return y * (1 - y);
		}

		public static double SigmoidInv(double y)
		{
			return Math.Log(-y / (y - 1));
		}

		public static double SatFunc(double y)
		{
			return 1 - 4 * SigmoidDer(y);
		}

		public static double SatFuncDer(double y)
		{
			return -4 * SigmoidDer(y) * (-2 * y + 1);
		}
	}
}