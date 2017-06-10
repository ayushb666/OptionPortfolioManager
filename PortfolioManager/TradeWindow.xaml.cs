using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MahApps.Metro.Controls;
using PortfolioManager.Model;
using System.Windows.Controls;
using System.Windows.Media;


namespace PortfolioManager
{
    /// <summary>
    /// Interaction logic for TradeWindow.xaml
    /// </summary>
    public partial class TradeWindow : MetroWindow
    {
        private static DataModelContainer model = new DataModelContainer();
        public List<SecurityTypeDB> InstrumentTypeData { get; set; }
        public List<InstrumentsDB> Instruments { get; set; }
        private Double price;
        private Int64 quantity;
        Boolean checker1 = false;
        Boolean checker2 = false;

        public TradeWindow()
        {
            InitializeComponent();
            InstrumentTypeData = model.SecurityTypeDBs.ToList();
            cbInstrumentType.DataContext = InstrumentTypeData;
        }

        private void cbInstrumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Instruments = model.InstrumentsDBs.Where(x => x.SecurityTypeId == (Int64)cbInstrumentType.SelectedValue).ToList();
            cbInstrument.DataContext = Instruments;
        }

        private void tbPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbPrice.Text, out this.price))
            {
                tbPrice.BorderBrush = Brushes.Red;
                this.checker1 = false;
            }
            else
            {
                if (this.price <= 0)
                {
                    tbPrice.BorderBrush = Brushes.Red;
                    this.checker1 = false;
                }
                else
                {
                    tbPrice.BorderBrush = Brushes.White;
                    this.checker1 = true;
                }
                buttonEnabler();
            }
        }

        private void tbQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Int64.TryParse(this.tbQuantity.Text, out this.quantity))
            {
                tbQuantity.BorderBrush = Brushes.Red;
                this.checker2 = false;
            }
            else
            {
                if (this.quantity <= 0)
                {
                    tbQuantity.BorderBrush = Brushes.Red;
                    this.checker2 = false;
                }
                else
                {
                    tbQuantity.BorderBrush = Brushes.White;
                    this.checker2 = true;
                }
                buttonEnabler();
            }
        }

        private void buttonEnabler()
        {
            if (this.checker1 && this.checker2)
            {
                bBuy.IsEnabled = true;
                bSell.IsEnabled = true;
            }
            else
            {
                bBuy.IsEnabled = false;
                bSell.IsEnabled = false;
            }
        }

        private void bBuy_Click(object sender, RoutedEventArgs e)
        {
            model.OrderBookDBs.Add(new OrderBookDB()
            {
                Position = "BUY",
                Quantity = this.quantity,
                TimeStamp = DateTime.Now,
                Price = this.price,
                InstrumentsId = (Int64)cbInstrument.SelectedValue
            });
            model.SaveChangesAsync();
            this.Close();
        }

        private void bSell_Click(object sender, RoutedEventArgs e)
        {
            model.OrderBookDBs.Add(new OrderBookDB()
            {
                Position = "SELL",
                Quantity = this.quantity,
                TimeStamp = DateTime.Now,
                Price = this.price,
                InstrumentsId = (Int64)cbInstrument.SelectedValue
            });
            model.SaveChangesAsync();
            this.Close();
        }
    }
}
