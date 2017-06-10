using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    class Greeks
    {
        #region variables
        private Double delta;
        private Double theta;
        private Double gamma;
        private Double vega;
        private Double rho;

        #endregion

        #region Getters & Setters
        public double Delta
        {
            get
            {
                return delta;
            }

            set
            {
                delta = value;
            }
        }

        public double Theta
        {
            get
            {
                return theta;
            }

            set
            {
                theta = value;
            }
        }

        public double Gamma
        {
            get
            {
                return gamma;
            }

            set
            {
                gamma = value;
            }
        }

        public double Vega
        {
            get
            {
                return vega;
            }

            set
            {
                vega = value;
            }
        }

        public double Rho
        {
            get
            {
                return rho;
            }

            set
            {
                rho = value;
            }
        }
        #endregion

        // This function is used to caculate greeks. It takes as an input an option and then make use of calculate option price present in option class to calulate value of greeks
        #region Function to calculate greeks
        public void calculateGreeks(long numberOfSimulations, int numberOfDays, Double daysToExpiry,Options option, Double delta = 1, Boolean controlVariateReduction = false, Boolean multithreading = false, MainWindow form = null)
        {
            Double delgamma1 = option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, delta, ChangeValue.UPRICE, controlVariateReduction: controlVariateReduction, multithreading: multithreading);
            Double delgamma2 = option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, -1 * delta, ChangeValue.UPRICE, controlVariateReduction: controlVariateReduction, multithreading: multithreading);

            this.delta = (delgamma1 - delgamma2)/ (2 * delta);

            this.theta = -365 * (option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, delta, ChangeValue.TIME, controlVariateReduction: controlVariateReduction, multithreading: multithreading) - option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, -1 * delta, ChangeValue.TIME, controlVariateReduction: controlVariateReduction, multithreading: multithreading)) / (2 * delta);

            this.gamma = (delgamma1 + delgamma2 - 2 * option.Price) / Math.Pow(delta, 2);

            this.vega = 100 * (option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, delta / 100.0, ChangeValue.VOLATILITY, controlVariateReduction: controlVariateReduction, multithreading: multithreading) - option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, -1 * delta / 100.0, ChangeValue.VOLATILITY, controlVariateReduction: controlVariateReduction, multithreading: multithreading)) / (2 * delta);

            this.rho = 100 * (option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, delta / 100, ChangeValue.RATE, controlVariateReduction: controlVariateReduction, multithreading: multithreading) - option.calculateOptionPrice(numberOfSimulations, numberOfDays, daysToExpiry, -1 * delta / 100.0, ChangeValue.RATE, controlVariateReduction: controlVariateReduction, multithreading: multithreading)) / (2 * delta);
        }

        #endregion
    }
}
