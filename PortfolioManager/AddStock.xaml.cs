using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using PortfolioManager.Model;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PortfolioManager
{
    /// <summary>
    /// Interaction logic for AddStock.xaml
    /// </summary>
    public partial class AddStock : MetroWindow
    {
        private Double lastTradedPrice;
        private Double historicalVolatility;
        Boolean checker1 = false;
        Boolean checker2 = false;
        public List<InstrumentsDB> Instruments { get; set; }
        private static DataModelContainer model = new DataModelContainer();

        public AddStock()
        {
            InitializeComponent();
            long idTemp = model.SecurityTypeDBs.Where(x => x.TypeName == "Stocks").Select(x => x.Id).First();
            Instruments = model.InstrumentsDBs.Where(x => x.SecurityTypeId == idTemp).ToList();
            cbSymbolToBeDeleted.DataContext = Instruments;
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            model.InstrumentsDBs.Add(new InstrumentsDB()
            {
                Symbol = this.tbSymbol.Text.ToString().ToUpper(),
                SecurityTypeId = model.SecurityTypeDBs.Where(x => x.TypeName == "Stocks").Select(x => x.Id).First()
            });
            model.StockDBs.Add(new StockDB()
            {
                ISIN = this.tbIsin.Text.ToString().ToUpper(),
                Issuer = this.tbIssuer.Text.ToString(),
                Symbol = this.tbSymbol.Text.ToString().ToUpper(),
                IsTradable = true,
                LastTradedPrice = this.lastTradedPrice,
                HistoricalVolatility = this.historicalVolatility/100
            });
            model.SaveChangesAsync();
            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbLastTradedPrice_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbLastTradedPrice.Text, out this.lastTradedPrice))
            {
                tbLastTradedPrice.BorderBrush = Brushes.Red;
                this.checker1 = false;
            }
            else
            {
                if (this.lastTradedPrice <= 0)
                {
                    tbLastTradedPrice.BorderBrush = Brushes.Red;
                    this.checker1 = false;
                }
                else
                {
                    tbLastTradedPrice.BorderBrush = Brushes.White;
                    this.checker1 = true;
                }
                buttonEnabler();
            }
        }

        private void tbHistoricalVolatility_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbHistoricalVolatility.Text, out this.historicalVolatility))
            {
                tbHistoricalVolatility.BorderBrush = Brushes.Red;
                this.checker2 = false;
            }
            else
            {
                if (this.historicalVolatility <= 0)
                {
                    tbHistoricalVolatility.BorderBrush = Brushes.Red;
                    this.checker2 = false;
                }
                else
                {
                    tbHistoricalVolatility.BorderBrush = Brushes.White;
                    this.checker2 = true;
                }
                buttonEnabler();
            }
        }

        private void buttonEnabler()
        {
            if (this.checker1 && this.checker2)
            {
                bAdd.IsEnabled = true;
            }
            else
            {
                bAdd.IsEnabled = false;
            }
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            var options = model.OptionsDBs.Where(x => x.UnderlyingID == (Int64)cbSymbolToBeDeleted.SelectedValue);
            foreach(var option in options)
            {
                model.OrderBookDBs.RemoveRange(model.OrderBookDBs.Where(x => x.InstrumentsId == model.InstrumentsDBs.Where(y => y.Symbol == option.Symbol).Select(z=>z.Id).FirstOrDefault()));
                model.InstrumentsDBs.Remove(model.InstrumentsDBs.Where(x=> x.Symbol == option.Symbol).FirstOrDefault());
                model.OptionsDBs.Remove(option);
            }
            model.OrderBookDBs.RemoveRange(model.OrderBookDBs.Where(v=>v.InstrumentsId == (model.InstrumentsDBs.Where(x => x.Symbol == model.StockDBs.Where(y => y.Id == (Int64)cbSymbolToBeDeleted.SelectedValue).Select(z => z.Symbol).FirstOrDefault()).Select(w=>w.Id).FirstOrDefault())));
            model.InstrumentsDBs.Remove(model.InstrumentsDBs.Where(x => x.Symbol == model.StockDBs.Where(y => y.Id == (Int64)cbSymbolToBeDeleted.SelectedValue).Select(z => z.Symbol).FirstOrDefault()).FirstOrDefault());
            model.StockDBs.Remove(model.StockDBs.Where(x => x.Id == (Int64)cbSymbolToBeDeleted.SelectedValue).FirstOrDefault());
            model.SaveChangesAsync();
            this.Close();
        }

        private void cbSymbolToBeDeleted_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bDelete.IsEnabled = true;
        }
    }
}
