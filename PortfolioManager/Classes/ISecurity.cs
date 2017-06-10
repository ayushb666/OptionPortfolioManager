using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Classes
{
    interface ISecurity
    {
        string Issuer { get; set; }
        double Price { get; set; }
        string Symbol { get; set; }
        string Isin { get; set; }
        Boolean IsTradable { get; set; }
    }
}
