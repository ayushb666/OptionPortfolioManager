using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    class RangeOption : Options
    {
        #region Constructors of the European option
        public RangeOption(string symbol, ISecurity underlying, DateTime expiryDate, Double strikePrice, double historicalVolatility, OptionType type, OptionKind optionKind)
        {
            this.symbol = symbol;
            this.underlying = underlying;
            this.expiryDate = expiryDate;
            this.historicalVolatility = historicalVolatility;
            this.optionKind = optionKind;
            this.strikePrice = strikePrice;
            this.type = type;
        }

        public RangeOption(String issuer, string symbol, ISecurity underlying, DateTime expiryDate, Double strikePrice, double historicalVolatility, String isin, Boolean isTradable, OptionType type ,OptionKind optionKind)
        {
            this.issuer = issuer;
            this.strikePrice = strikePrice;
            this.symbol = symbol;
            this.underlying = underlying;
            this.expiryDate = expiryDate;
            this.historicalVolatility = historicalVolatility;
            this.isin = isin;
            this.type = type;
            this.isTradable = isTradable;
            this.optionKind = optionKind;
        }
        #endregion


        #region Calculation methods 
        // This function is a kind of a dummy finction that uses other 2 function calculate standarddevation and calculate option price to calculate value of greek and option price.
        public override void calulateOptionPriceAndGreeks(long numberOfSimulations, Double interstRate, int numberOfDays, bool antitheticReduction, bool controlVariateReduction, bool multithreading, Double del = 0, ChangeValue change = ChangeValue.NONE, MainWindow form = null, GraphPlotting plot = null)
        {
            Simulator.initializeYieldCurve();
            Simulator.createRandomNumbers(this, numberOfSimulations, numberOfDays, antitheticReduction);
            Double daysToExpiry = Convert.ToDouble((this.ExpiryDate - DateTime.Today).Days);
            Double[] priceAtEnd = Simulator.getPath(this, numberOfSimulations, numberOfDays, del, change, controlVariateReduction, multithreading, plot: plot);
            Double[] cv = new Double[priceAtEnd.Length];
            Simulator.controlVariateList.CopyTo(cv);
            this.price = calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, controlVariateReduction: controlVariateReduction, multithreading: multithreading);
            if (controlVariateReduction)
            {
                calculateStandardErrorwithcv(numberOfSimulations, priceAtEnd, daysToExpiry, antitheticReduction, cv);
            }
            else
            {
                calculateStandardError(numberOfSimulations, priceAtEnd, daysToExpiry, antitheticReduction);
            }
            greeks.calculateGreeks(numberOfSimulations, numberOfDays, daysToExpiry, this, controlVariateReduction: controlVariateReduction, multithreading: multithreading, form: form);
            Simulator.randomNumbers.Clear();
        }


        // This function has the logic which calculates price of the option.
        public override Double calculateOptionPrice(long numberOfSimulations, int numberOfDays, Double daysToExpiry, Double del = 0, ChangeValue change = ChangeValue.NONE, Boolean controlVariateReduction = false, Boolean multithreading = false)
        {

            double[] priceAtEnd = Simulator.getPath(this, numberOfSimulations, numberOfDays, del, change, controlVariateReduction, multithreading);
            Double daysToExpirey = Convert.ToDouble((this.ExpiryDate - DateTime.Today).Days);
            Double sum = 0;
            for (int i = 0; i < priceAtEnd.Length; i++)
            {

                sum = controlVariateReduction ? (sum + priceAtEnd[i] - Simulator.controlVariateList[i]) : (sum + priceAtEnd[i]);
            }

            Double optionPrice = (sum / numberOfSimulations) * Math.Exp(-Simulator.yieldCurve[0].Rate * daysToExpirey / 365.0);
            if (change == ChangeValue.RATE || change == ChangeValue.TIME) { Simulator.changeValues(this, -1 * del, change); }
            return optionPrice;
        }

        // This function has the logic which calculates standard Error for the option. 
        public void calculateStandardError(long numberOfSimulations, Double[] priceAtEnd, Double daysToExpiry, Boolean antitheticReduction)
        {
            Double sum = 0;
            numberOfSimulations = antitheticReduction ? 2 * numberOfSimulations : numberOfSimulations;

            for (int i = 0; i < priceAtEnd.Length; i++)
            {
                sum += Math.Pow((priceAtEnd[i] * Math.Exp(-Simulator.yieldCurve[0].Rate * daysToExpiry / 365.0)) - this.Price, 2);
            }
            this.StandarError = Math.Sqrt(Convert.ToDouble(sum) / (numberOfSimulations - 1)) / Math.Sqrt(numberOfSimulations);
        }

        // This function has the logic which calculates standard Error for the option with Delta Control Variate 
        public void calculateStandardErrorwithcv(long numberOfSimulations, Double[] priceAtEnd, Double daysToExpiry, Boolean antitheticReduction, Double[] controlVariateList)
        {
            Double sum2 = 0;
            numberOfSimulations = antitheticReduction ? 2 * numberOfSimulations : numberOfSimulations;
            Double num = 0, den = 0;
            Double pa = priceAtEnd.Average(), cva = controlVariateList.Average();
            for (int i = 0; i < priceAtEnd.Length; i++)
            {
                num += (priceAtEnd[i] - pa) * (controlVariateList[i] - cva);
                den += Math.Pow((priceAtEnd[i] - pa), 2);
            }
            Double beta = num / den;

            for (int i = 0; i < priceAtEnd.Length; i++)
            {
                    sum2 += Math.Pow(priceAtEnd[i] - beta * controlVariateList[i], 2);
            }
            this.StandarError = Math.Sqrt(((sum2 - Math.Pow(this.price, 2)) * Math.Exp(-2 * Simulator.yieldCurve[0].Rate * (daysToExpiry / 365.0))) / (numberOfSimulations - 1)) / Math.Sqrt(numberOfSimulations);
        }


        #endregion
    }
}
