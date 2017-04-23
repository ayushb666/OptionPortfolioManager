using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;


namespace PortfolioManager.Classes
{
    public enum ChangeValue { UPRICE, TIME, RATE, VOLATILITY, NONE };

    static class Simulator
    {
        public static List<Double> controlVariateList = new List<Double>();
        public static List<InterestRate> yieldCurve = new List<InterestRate>();
        public static List<Double[]> randomNumbers = new List<Double[]>();
        private static readonly Object lck = new Object();


        // This function initializes yield curve. Currently it is not of much use and would make changes.
        #region Make Yield Curve
        public static void initializeYieldCurve(Double interestRate = 0.05)
        {
            yieldCurve.Clear();
            for (int i = 0; i < 10; i++)
            {
                yieldCurve.Add(new InterestRate(DateTime.Today.AddYears(i), interestRate));
            }
        }
        #endregion


        //This function creates an path of the stock which we assume for brownian geometric motion.
        #region Create Path for Underlying
        public static Double[] getPath(Options option, long noOfSimulations, int numberOfDays, Double del, ChangeValue change, bool controlVariateReduction, bool multithreading = true, GraphPlotting plot = null)
        {
            changeValues(option, del, change);
            Double[] priceAtEnd = new Double[noOfSimulations];
            int numberofThreads = multithreading ? Environment.ProcessorCount : 1;
            Double dt = Convert.ToDouble((option.ExpiryDate - DateTime.Today).Days) / (365 * numberOfDays);
            if (controlVariateReduction) { controlVariateList.Clear(); }
            int day = 0;
            Parallel.ForEach(randomNumbers, new ParallelOptions { MaxDegreeOfParallelism = numberofThreads }, randomList =>
               {
                   Double[] simulatedPathForOneSimulation = new Double[numberOfDays + 1];
                   simulatedPathForOneSimulation[0] = option.Underlying.Price;
                   double controlVariate = 0;
                   for (int time = 1; time <= numberOfDays; time++)
                   {
                       try
                       {
                           simulatedPathForOneSimulation[time] = simulatedPathForOneSimulation[time - 1] * Math.Exp(((yieldCurve[0].Rate - (Math.Pow(option.HistoricalVolatility, 2) / 2.0)) * (dt)) + option.HistoricalVolatility * Math.Sqrt(dt) * randomList[time - 1]);
                           if (controlVariateReduction)
                           {
                               Double delta = OptionDelta(simulatedPathForOneSimulation[time - 1], option.StrikePrice, yieldCurve[0].Rate, option.HistoricalVolatility, (numberOfDays - time) * dt);

                               if (option.Type == OptionType.PUT) { delta = delta - 1; }

                               controlVariate += delta * (simulatedPathForOneSimulation[time] - (simulatedPathForOneSimulation[time - 1] * Math.Exp(yieldCurve[0].Rate * dt)));
                           }
                       }
                       catch (Exception e)
                       {
                           throw e;
                       }
                   }
                   lock (lck)
                   {
                       if (plot != null && day<=50) { plot.addNewSeries(simulatedPathForOneSimulation); }
                       switch (option.OptionKind)
                       {
                           case OptionKind.ASIAN:
                               priceAtEnd[day++] = simulatedPathForOneSimulation.Average();
                               break;
                           case OptionKind.BARRIER:
                               switch (((BarrierOption)option).BarrierOptionType)
                               {
                                   case BarrierOptionType.UPOUT:
                                       if (simulatedPathForOneSimulation.Max() > ((BarrierOption)option).Barrier) { priceAtEnd[day++] = -1; }
                                       else { priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1]; }
                                       break;
                                   case BarrierOptionType.UPIN:
                                       if (simulatedPathForOneSimulation.Max() <= ((BarrierOption)option).Barrier) { priceAtEnd[day++] = -1; }
                                       else { priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1]; }
                                       break;
                                   case BarrierOptionType.DOWNOUT:
                                       if (simulatedPathForOneSimulation.Min() < ((BarrierOption)option).Barrier) { priceAtEnd[day++] = -1; }
                                       else { priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1]; }
                                       break;
                                   case BarrierOptionType.DOWNIN:
                                       if (simulatedPathForOneSimulation.Min() >= ((BarrierOption)option).Barrier) { priceAtEnd[day++] = -1; }
                                       else { priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1]; }
                                       break;
                                   default:
                                       break;
                               }
                               break;
                           case OptionKind.DIGITAL:
                               priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1];
                               break;
                           case OptionKind.EUROPEAN:
                               priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1];
                               break;
                           case OptionKind.LOOKBACK:
                               switch (option.Type)
                               {
                                   case OptionType.CALL:
                                       priceAtEnd[day++] = simulatedPathForOneSimulation.Max();
                                       break;
                                   case OptionType.PUT:
                                       priceAtEnd[day++] = simulatedPathForOneSimulation.Min();
                                       break;
                                   default:
                                       break;
                               }
                               break;
                           case OptionKind.RANGE:
                               priceAtEnd[day++] = simulatedPathForOneSimulation.Max() - simulatedPathForOneSimulation.Min();
                               break;
                           default:
                               priceAtEnd[day++] = simulatedPathForOneSimulation[simulatedPathForOneSimulation.Length - 1];
                               break;
                       }
                       if (controlVariateReduction) { controlVariateList.Add(controlVariate); }
                   }
               });

            if (change != ChangeValue.RATE && change != ChangeValue.TIME) { changeValues(option, -1 * del, change); }
            return priceAtEnd;
        }
        #endregion

        //This function make minor changes in the value of input like strikeprice, price of underlying etc so that they can be used for calculating value of greeks
        #region Change value for calculating greeks
        public static void changeValues(Options option, Double del, ChangeValue change)
        {
            switch (change)
            {
                case ChangeValue.UPRICE:
                    option.Underlying.Price += del;
                    break;
                case ChangeValue.TIME:
                    option.ExpiryDate = option.ExpiryDate.AddDays(del);
                    break;
                case ChangeValue.RATE:
                    yieldCurve[0].Rate += del;
                    break;
                case ChangeValue.VOLATILITY:
                    option.HistoricalVolatility += del;
                    break;
                case ChangeValue.NONE:
                    break;
                default:
                    break;
            }
        }

        #endregion

        // This function creates random number and then save those random numbers in a file so that they can be used later for further use.
        #region Create and Save Random Numbers
        public static void createRandomNumbers(Options option, long noOfSimulations, int numberOfDays, Boolean antitheticReduction)
        {
            noOfSimulations = antitheticReduction ? noOfSimulations / 2 : noOfSimulations;
            Parallel.For(0, noOfSimulations, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, i =>
              {
                  double[] rn = RandomNumberGenerator.polarRejection(new Random((int)(i + DateTime.Now.Millisecond) + Environment.TickCount), numberOfDays);
                  double[] rnnew = null;
                  if (antitheticReduction)
                  {
                      rnnew = new Double[rn.Length];
                      for (int j = 0; j < rn.Length; j++) { rnnew[j] = -1 * rn[j]; }
                  }
                  lock (lck)
                  {
                      randomNumbers.Add(rn);
                      if (antitheticReduction) { randomNumbers.Add(rnnew); }
                  }
              });
        }

        #endregion

        private static Double OptionDelta(double S, double K, double R, double vol, double t)
        {
            double z = (Math.Log(S / K) + ((R + Math.Pow(vol, 2)) * (t))) / (vol * Math.Sqrt(t));
            return NormalDistribution.CumDensity(z);
        }

    }
}
