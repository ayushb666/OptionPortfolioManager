using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    class InterestRate
    {
        private DateTime maturity;
        private Double rate;

        public DateTime Maturity
        {
            get
            {
                return maturity;
            }
            set
            {
                this.maturity = value;
            }

        }
        public double Rate
        {
            get
            {
                return rate;
            }
            set
            {
                this.rate = value;
            }

        }

        public InterestRate(DateTime maturity, Double rate)
        {
            this.maturity = maturity;
            this.rate = rate;
        }
    }
}
