using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    enum BarrierOptionType
    {
        UPOUT, UPIN, DOWNOUT, DOWNIN
    };

    class BarrierOption:Options
    {
        private Double barrier;
        private BarrierOptionType barrierOptionType;

        public double Barrier
        {
            get
            {
                return barrier;
            }

            set
            {
                barrier = value;
            }
        }

        internal BarrierOptionType BarrierOptionType
        {
            get
            {
                return barrierOptionType;
            }

            set
            {
                barrierOptionType = value;
            }
        }

        #region Constructors of the European option
        public BarrierOption(string symbol, ISecurity underlying, DateTime expiryDate, double strikePrice, double historicalVolatility, OptionType type, OptionKind optionKind, Double barrier, BarrierOptionType barrierOptionType)
        {
            this.symbol = symbol;
            this.underlying = underlying;
            this.expiryDate = expiryDate;
            this.strikePrice = strikePrice;
            this.historicalVolatility = historicalVolatility;
            this.type = type;
            this.optionKind = optionKind;
            this.barrier = barrier;
            this.barrierOptionType = barrierOptionType;
        }

        public BarrierOption(String issuer, Exchange[] tradedOnExchange, string symbol, ISecurity underlying, DateTime expiryDate, double strikePrice, double historicalVolatility, String isin, Boolean isTradable, OptionType type, OptionKind optionKind, Double barrier, BarrierOptionType barrierOptionType)
        {
            this.issuer = issuer;
            this.tradedOnExchange = tradedOnExchange;
            this.symbol = symbol;
            this.underlying = underlying;
            this.expiryDate = expiryDate;
            this.strikePrice = strikePrice;
            this.historicalVolatility = historicalVolatility;
            this.isin = isin;
            this.isTradable = isTradable;
            this.type = type;
            this.optionKind = optionKind;
            this.barrier = barrier;
            this.barrierOptionType = barrierOptionType;
        }
        #endregion


        #region Calculation methods 
        // This function is a kind of a dummy finction that uses other 2 function calculate standarddevation and calculate option price to calculate value of greek and option price.
        public override void calulateOptionPriceAndGreeks(long numberOfSimulations, Double interstRate, int numberOfDays, bool antitheticReduction, bool controlVariateReduction, bool multithreading, Double del = 0, ChangeValue change = ChangeValue.NONE, MainWindow form = null, GraphPlotting plot = null)
        {
            Simulator.initializeYieldCurve(interestRate: interstRate);
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
                if (this.type == OptionType.CALL)
                {
                    sum = controlVariateReduction ? (sum + (priceAtEnd[i] != -1 ? Math.Max(priceAtEnd[i] - this.strikePrice, 0) : 0) - Simulator.controlVariateList[i]) : (sum + (priceAtEnd[i] != -1 ? Math.Max(priceAtEnd[i] - this.strikePrice, 0) : 0));
                }
                else if (this.type == OptionType.PUT)
                {
                    sum = controlVariateReduction ? (sum + (priceAtEnd[i] != -1 ? Math.Max(this.strikePrice - priceAtEnd[i], 0) : 0) - Simulator.controlVariateList[i]) : (sum + (priceAtEnd[i] != -1 ? Math.Max(this.strikePrice - priceAtEnd[i], 0) : 0));
                }
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
                if (this.type == OptionType.CALL)
                {
                    sum += Math.Pow(((priceAtEnd[i] != -1 ? Math.Max(priceAtEnd[i] - this.strikePrice, 0) : 0) * Math.Exp(-Simulator.yieldCurve[0].Rate * daysToExpiry / 365.0)) - this.Price, 2);
                }
                else if (this.type == OptionType.PUT)
                {
                    sum += Math.Pow(((priceAtEnd[i] != -1 ? Math.Max(this.strikePrice - priceAtEnd[i], 0) : 0) * Math.Exp(-Simulator.yieldCurve[0].Rate * daysToExpiry / 365.0)) - this.Price, 2);
                }
            }
            this.StandarError = Math.Sqrt(Convert.ToDouble(sum) / (numberOfSimulations - 1)) / Math.Sqrt(numberOfSimulations);
        }

        // This function has the logic which calculates standard Error for the option with Delta Control Variate 
        public void calculateStandardErrorwithcv(long numberOfSimulations, Double[] priceAtEnd, Double daysToExpiry, Boolean antitheticReduction, Double[] controlVariateList)
        {
            Double sum2 = 0;
            numberOfSimulations = antitheticReduction ? 2 * numberOfSimulations : numberOfSimulations;
            Double num = 0, den = 0;
            Double[] payoff = new Double[priceAtEnd.Length];
            for (int i = 0; i < priceAtEnd.Length; i++)
            {
                if (this.type == OptionType.CALL)
                {
                    payoff[i] = (priceAtEnd[i] != -1 ? Math.Max(priceAtEnd[i] - this.strikePrice, 0) : 0);
                }
                else
                {
                    payoff[i] = (priceAtEnd[i] != -1 ? Math.Max(this.strikePrice - priceAtEnd[i], 0) : 0);
                }
            }
            Double pa = payoff.Average(), cva = controlVariateList.Average();
            for (int i = 0; i < priceAtEnd.Length; i++)
            {
                num += (payoff[i] - pa) * (controlVariateList[i] - cva);
                den += Math.Pow((payoff[i] - pa), 2);
            }
            Double beta = num / den;

            for (int i = 0; i < priceAtEnd.Length; i++)
            {
                if (this.type == OptionType.CALL)
                {
                    sum2 += Math.Pow((priceAtEnd[i] != -1 ? Math.Max(priceAtEnd[i] - this.strikePrice, 0) : 0) - beta * controlVariateList[i], 2);
                }
                else if (this.type == OptionType.PUT)
                {
                    sum2 += Math.Pow((priceAtEnd[i] != -1 ? Math.Max(this.strikePrice - priceAtEnd[i], 0) : 0) - beta * controlVariateList[i], 2);
                }
            }
            this.StandarError = Math.Sqrt(((sum2 - Math.Pow(this.price, 2)) * Math.Exp(-2 * Simulator.yieldCurve[0].Rate * (daysToExpiry / 365.0))) / (numberOfSimulations - 1)) / Math.Sqrt(numberOfSimulations);
        }


        #endregion
    }
}
