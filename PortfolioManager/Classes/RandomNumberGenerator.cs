using System;

namespace PortfolioManager.Classes
{
	class RandomNumberGenerator
	{
		// This Method  generates normally distributed random variables using the Straight Method.
		// It take 2 input a random value generator and a number whose inital value is assigned 2.
		public static double[] straightMethod(Random var, int num = 2)
		{
			if (num % 2 == 1) { num++;}
			double[] arr = new double[num];
			for (int i = 0; i < num; i = i + 2)
			{
				for (int j = 0; j < 12; j++)
				{
					arr[i] += var.NextDouble();
					arr[i+1] += var.NextDouble();
				}
				arr[i] -= 6;
				arr[i+1] -= 6;
			}
			return arr;
		}

		// This Method  generates normally distributed random variables using the method Box Muller.
		// It take 2 input a random value generator and a number whose inital value is assigned 2.
		public static double[] boxMuller(Random var, int num = 2)
		{
			if (num % 2 == 1) { num++; }
			double[] arr = new double[num];
			for (int j = 0; j < num; j = j + 2)
			{
				double var1 = var.NextDouble();
				double var2 = var.NextDouble();
				arr[j] = Math.Sqrt(-2 * Math.Log(var1)) * Math.Cos(2 * Math.PI * var2);
				arr[j+1] = Math.Sqrt(-2 * Math.Log(var1)) * Math.Sin(2 * Math.PI * var2);
			}
			return arr;
		}

		// This Method  generates normally distributed random variables using the method Polar Rejection.
		// It take 2 input a random value generator and a number whose inital value is assigned 2.
		public static double[] polarRejection(Random var , int num = 2)
		{
			if (num % 2 == 1) { num++; }
			double[] arr = new double[num];
			for (int j = 0; j < num; j = j + 2)
			{
				double var1 = -1 + (2 * var.NextDouble());
				double var2 = -1 + (2 * var.NextDouble());
				while (Math.Pow(var1, 2) + Math.Pow(var2, 2) > 1)
				{
					var1 = -1 + (2 * var.NextDouble());
					var2 = -1 + (2 * var.NextDouble());
				}
				double w = Math.Pow(var1, 2) + Math.Pow(var2, 2);
				double c = Math.Sqrt(-2 * Math.Log(w) / w);
				arr[j] = c * var1;
				arr[j+1] = c * var2;
			}
			return arr;
		}

		// This Method basically generates pair of random values and put them in an array which is later returned.
		// It takes input of rho that is correlation and an array of normally distributed random values.
		public static double[] jointNormal(double rho, double[] arr)
		{
			double[] output = new double[arr.Length];
			for (int i = 0; i < arr.Length; i += 2)
			{
				output[i] = arr[i];
				output[i+1] = (rho * arr[i]) + (Math.Sqrt(1 - Math.Pow(rho, 2)) * arr[i+1]);
			}
			return output;
		}
	}
}