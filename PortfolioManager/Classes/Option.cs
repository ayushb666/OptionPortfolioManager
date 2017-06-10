using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    enum OptionType
    {
        CALL, PUT
    };

    enum OptionKind
    {
        ASIAN, BARRIER, DIGITAL, EUROPEAN, LOOKBACK, RANGE
    };

    class Options : ISecurity
    {
        #region Variable Name
        protected String issuer;
        protected Double price = -1;
        protected String symbol;
        protected ISecurity underlying;
        protected DateTime expiryDate;
        protected Double strikePrice;
        protected Double historicalVolatility;
        protected String isin;
        protected Boolean isTradable;
        protected OptionType type;
        protected Double standardError;
        protected OptionKind optionKind;
        protected Greeks greeks = new Greeks();
        #endregion

        #region Properties
        public string Issuer
        {
            get
            {
                return issuer;
            }
            set
            {
                this.issuer = value;
            }

        }


        public string Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                this.symbol = value;
            }

        }

        public string Isin
        {
            get
            {
                return isin;
            }
            set
            {
                this.isin = value;
            }

        }

        public bool IsTradable
        {
            get
            {
                return isTradable;
            }
            set
            {
                this.isTradable = value;
            }

        }

        public ISecurity Underlying
        {
            get
            {
                return underlying;
            }
            set
            {
                this.underlying = value;
            }

        }

        public DateTime ExpiryDate
        {
            get
            {
                return expiryDate;
            }
            set
            {
                this.expiryDate = value;
            }

        }

        public double StrikePrice
        {
            get
            {
                return strikePrice;
            }
            set
            {
                this.strikePrice = value;
            }

        }

        public double HistoricalVolatility
        {
            get
            {
                return historicalVolatility;
            }
            set
            {
                this.historicalVolatility = value;
            }

        }

        public OptionType Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }
        }

        public double StandarError
        {
            get
            {
                return standardError;
            }
            set
            {
                this.standardError = value;
            }

        }

        public double Price
        {
            get
            {
                if (this.price == -1)
                {
                    throw new NotFiniteNumberException("Price Not Set Yet. First calculate the Option Price");
                }
                else
                {
                    return this.price;
                }
            }
            set
            {
                throw new NotSupportedException("You are not allowed to set an option Value");
            }
        }

        public Greeks Greeks
        {
            get
            {
                return greeks;
            }

            set
            {
                greeks = value;
            }
        }

        public OptionKind OptionKind
        {
            get
            {
                return optionKind;
            }

            set
            {
                optionKind = value;
            }
        }


        #endregion

        #region Functions
        virtual public void calulateOptionPriceAndGreeks(long numberOfSimulations, Double intersRate, int steps, bool antitheticReduction, bool controlVariateReduction, bool multithreading, Double del = 0, ChangeValue change = ChangeValue.NONE, MainWindow form = null, GraphPlotting plot = null)
        {
        }

        virtual public Double calculateOptionPrice(long numberOfSimulations, int numberOfDays, Double daysToExpiry, Double del = 0, ChangeValue change = ChangeValue.NONE, Boolean controlVariateReduction = false, Boolean multithreading = false)
        {
            return 0;
        }
        #endregion
    }
}
