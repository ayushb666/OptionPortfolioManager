using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    class Stock: ISecurity
    {
        #region Variables
        private string issuer;
        private Exchange[] tradedOnExchange;
        private String isin;
        private Double price;
        private String symbol;
        private Boolean isTradable;

        #endregion

        #region Constructors
        public Stock(double price, String symbol = "AAPL")
        {
            this.price = price;
            this.symbol = symbol;
        }

        public Stock(String issuer, Exchange[] tradedOnExchange, String isin, Double price, String symbol, Boolean isTradable)
        {
            this.issuer = issuer;
            this.tradedOnExchange = tradedOnExchange;
            this.isin = isin;
            this.price = price;
            this.symbol = symbol;
            this.isTradable = isTradable;
        }

        #endregion

        #region Getter & Setters
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

        public String Isin
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

        public double Price
        {
            get
            {
                return Math.Round(price, 2);
            }
            set
            {
                this.price = value;
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

        public Exchange[] TradedOnExchange
        {
            get
            {
                return tradedOnExchange;
            }
            set
            {
                this.tradedOnExchange = value;
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

        #endregion
    }
}
