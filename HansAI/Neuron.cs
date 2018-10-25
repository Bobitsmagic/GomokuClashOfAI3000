using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{
	class Neuron
	{
		//Statics
		public static double Lernrate = 0.1, Momentum = 0.03, Sat = -0.01;
		private static Random rnd = new Random();
		//Public Data
		public double Output { get; private set; }
		public double Error { get; private set; }
		public double Saturation { get; private set; }

		public double Bias { get { return bias; } }
		public double[] Weights { get { return weights; } }

		//private Data
		private Network refNetwork;
		private int layer, index;

		private double bias, deltaBias;
		private double[] weights, deltaWeights;

		//Constructor
		public Neuron(Network network, int _layer, int _index)
		{
			refNetwork = network;
			layer = _layer;
			index = _index;

			if (layer != 0)
			{
				weights = new double[network.Neurons[layer - 1].Length];
				deltaWeights = new double[weights.Length];

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
				weights = new double[network.Neurons[layer - 1].Length];
				deltaWeights = new double[weights.Length];
			}

			for (int i = 0; i < weights.Length; i++)
			{
				weights[i] = br.ReadDouble();
				deltaWeights[i] = br.ReadDouble();
			}

			bias = br.ReadDouble();
			deltaBias = br.ReadDouble();
		}

		//IO
		public void WriteAsFile(ref BinaryWriter bw)
		{
			//weigths[], deltaWights[]
			//bias, deltaBias

			for (int i = 0; i < weights.Length; i++)
			{
				bw.Write(weights[i]);
				bw.Write(deltaWeights[i]);
			}

			bw.Write(bias);
			bw.Write(deltaBias);
		}

		//Voids
		public void SetRandomWeights()
		{
			for (int i = 0; i < weights.Length; i++)
			{
				weights[i] = NextGaussian();
				deltaWeights[i] = 0;
			}

			bias = NextGaussian();
			deltaBias = 0;
		}

		public static double NextGaussian(double mean = 1)
		{
			return mean * Math.Sqrt(-2 * Math.Log(rnd.NextDouble())) *
				Math.Sin(2 * Math.PI * rnd.NextDouble());
		}

		double theta = 7;
		public bool FlipSign()
		{
			int wi = rnd.Next(weights.Length + 1);
			for (int i = 0; i <= weights.Length; i++)
			{
				if (wi == weights.Length)
				{
					if (Math.Abs(bias) > theta)
					{
						bias = NextGaussian();
						return true;
					}
				}
				else
				{
					if (Math.Abs(weights[wi]) > theta)
					{
						weights[wi] = NextGaussian();
						return true;
					}
				}

				wi = (wi + 1) % (weights.Length + 1);
			}

			return false;
		}
		public PointF[] GetHyperPlane()
		{
			PointF a = new PointF((float)(Bias / Weights[0]), 0);
			PointF b = new PointF(0, (float)(Bias / Weights[1]));

			return new PointF[] { a, b };
		}

		public void CalcOut()
		{
			double sum = bias;
			for (int i = 0; i < weights.Length; i++)
			{
				sum += refNetwork.Neurons[layer - 1][i].Output * weights[i];
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
					refNetwork.Neurons[layer + 1][i].weights[index] *
					SigmoidDer(refNetwork.Neurons[layer + 1][i].Output);
			}
			double constFactor = Error * SigmoidDer(Output);

			for (int i = 0; i < weights.Length; i++)
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

			for (int i = 0; i < weights.Length; i++)
			{
				deltaWeights[i] += refNetwork.Neurons[layer - 1][i].Output * constFactor
					+ Sat * refNetwork.Neurons[layer - 1][i].Output * SatFuncDer(Output) * Saturation;
			}

			deltaBias += constFactor + Sat * SatFuncDer(Output) * Saturation;
		}

		public void ApplyDW(int testCount)
		{
			for (int i = 0; i < weights.Length; i++)
			{
				weights[i] += Lernrate * deltaWeights[i] / testCount;
				deltaWeights[i] *= Momentum;
			}
			bias += deltaBias * Lernrate / testCount;
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
