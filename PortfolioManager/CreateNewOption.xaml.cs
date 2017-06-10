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
    /// Interaction logic for CreateNewOption.xaml
    /// </summary>
    public partial class CreateNewOption : MetroWindow
    {
        public List<InstrumentsDB> Instruments { get; set; }
        public List<OptionKindDB> OptionKind { get; set; }
        private static DataModelContainer model = new DataModelContainer();
        private Boolean checker7 = false;
        private Boolean checker2 = false;
        private Boolean checker3 = false;
        private Boolean checker1 = false;
        private Boolean checker8 = false;
        private Boolean checker4 = false;
        private Boolean checker5 = false;
        private Boolean checker6 = false;
        private Boolean checker9 = false;
        private Double rebateBarrier;
        private DateTime maturityDate;
        private Double lastTradedPrice;
        private List<InstrumentsDB> toBeDeleted { get; set; }
        private String type;
        private Double strike;

        public CreateNewOption()
        {
            InitializeComponent();
            this.dtMaturityDate.DisplayDateStart = DateTime.Now;
            long idTemp = model.SecurityTypeDBs.Where(x => x.TypeName == "Stocks").Select(x => x.Id).First();
            Instruments = model.InstrumentsDBs.Where(x => x.SecurityTypeId == idTemp).ToList();
            cbUnderlying.DataContext = Instruments;
            idTemp = model.SecurityTypeDBs.Where(x => x.TypeName == "Options").Select(x => x.Id).First();
            toBeDeleted = model.InstrumentsDBs.Where(x => x.SecurityTypeId == idTemp).ToList();
            cbSymbolToBeDeleted.DataContext = toBeDeleted;
            OptionKind = model.OptionKindDBs.ToList();
            cboptionKinds.DataContext = OptionKind;
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            model.InstrumentsDBs.Add(new InstrumentsDB()
            {
                Symbol = this.tbSymbol.Text.ToString().ToUpper(),
                SecurityTypeId = model.SecurityTypeDBs.Where(x => x.TypeName == "Options").Select(x => x.Id).First()
            });
            model.OptionsDBs.Add(new OptionsDB()
            {
                ISIN = tbIsin.Text.ToString().ToUpper(),
                Issuer = tbIssuer.Text.ToString(),
                Symbol = tbSymbol.Text.ToString().ToUpper(),
                UnderlyingID = (Int64)cbUnderlying.SelectedValue,
                StrikePrice = this.strike,
                IsTradable = true,
                MaturityDate = this.maturityDate,
                OptionType = this.type,
                LastTradedPrice = this.lastTradedPrice,
                OptionKindID = (Int64)cboptionKinds.SelectedValue,
                Rebate = this.checker4?this.rebateBarrier:-1,
                Barrier = this.checker5?this.rebateBarrier:-1,
                BarrierOptionType = this.checker6?((ComboBoxItem)cbBarrierOptionKind.SelectedValue).Name.ToString():"Not Valid"
            });
            model.SaveChangesAsync();
            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dtMaturityDate_SelectedDateChanged(object sender, TimePickerBaseSelectionChangedEventArgs<DateTime?> e)
        {
            if (this.dtMaturityDate.SelectedDate.Value < DateTime.Today.AddDays(1))
            {
                this.dtMaturityDate.BorderBrush = Brushes.Red;
                this.checker7 = false;
            }
            else
            {
                this.maturityDate = this.dtMaturityDate.SelectedDate.Value;
                this.dtMaturityDate.BorderBrush = Brushes.White;
                this.checker7 = true;
            }
            buttonEnabler();
        }

        private void tbStrikePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbStrikePrice.Text, out this.strike))
            {
                tbStrikePrice.BorderBrush = Brushes.Red;
                this.checker2 = false;
            }
            else
            {
                if (this.strike <= 0)
                {
                    tbStrikePrice.BorderBrush = Brushes.Red;
                    this.checker2 = false;
                }
                else
                {
                    tbStrikePrice.BorderBrush = Brushes.White;
                    this.checker2 = true;
                }
                buttonEnabler();
            }
        }

        private void buttonEnabler()
        {
            if (this.checker1 && this.checker2 && this.checker3 && this.checker7 && this.checker8)
            {
                tbSymbol.Text = model.StockDBs.Where(x=>x.Id== (Int64)cbUnderlying.SelectedValue).Select(x=>x.Symbol). First() + this.maturityDate.Year + this.maturityDate.Month + ((Boolean)call.IsChecked?"C":"P") + this.strike;
                bAdd.IsEnabled = true;
            }
            else
            {
                bAdd.IsEnabled = false;
            }
        }

        private void tbLastTradedPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbLastTradedPrice.Text, out this.lastTradedPrice))
            {
                tbLastTradedPrice.BorderBrush = Brushes.Red;
                this.checker3 = false;
            }
            else
            {
                if (this.lastTradedPrice <= 0)
                {
                    tbLastTradedPrice.BorderBrush = Brushes.Red;
                    this.checker3 = false;
                }
                else
                {
                    tbLastTradedPrice.BorderBrush = Brushes.White;
                    this.checker3 = true;
                }
                buttonEnabler();
            }
        }

        private void cbUnderlying_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.checker1 = true;
        }

        private void cboptionKinds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((Int64)cboptionKinds.SelectedValue)
            {
                case 1:
                    tbRebateBarrier.IsEnabled = false;
                    cbBarrierOptionKind.IsEnabled = false;
                    this.checker4 = false;
                    this.checker5 = false;
                    this.checker6 = false;
                    break;
                case 2:
                    tbRebateBarrier.IsEnabled = true;
                    cbBarrierOptionKind.IsEnabled = true;
                    lRebateBarrier.Content = "Barrier";
                    this.checker4 = false;
                    this.checker5 = true;
                    this.checker6 = true;
                    break;
                case 3:
                    tbRebateBarrier.IsEnabled = true;
                    cbBarrierOptionKind.IsEnabled = false;
                    this.checker4 = true;
                    this.checker5 = false;
                    this.checker6 = false;
                    break;
                case 4:
                    tbRebateBarrier.IsEnabled = false;
                    cbBarrierOptionKind.IsEnabled = false;
                    this.checker4 = false;
                    this.checker5 = false;
                    this.checker6 = false;
                    break;
                case 5:
                    tbRebateBarrier.IsEnabled = false;
                    cbBarrierOptionKind.IsEnabled = false;
                    this.checker4 = false;
                    this.checker5 = false;
                    this.checker6 = false;
                    break;
                case 6:
                    tbRebateBarrier.IsEnabled = false;
                    cbBarrierOptionKind.IsEnabled = false;
                    this.checker4 = false;
                    this.checker5 = false;
                    this.checker6 = false;
                    break;
                default:
                    break;
            }
        }

        private void call_Click(object sender, RoutedEventArgs e)
        {
            if ((Boolean)call.IsChecked)
            {
                this.type = "CALL";
                this.checker8 = true;
            }
            buttonEnabler();
        }

        private void put_Click(object sender, RoutedEventArgs e)
        {
            if ((Boolean)put.IsChecked)
            {
                this.type = "PUT";
                this.checker8 = true;
            }
            buttonEnabler();
        }

        private void tbRebateBarrier_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbRebateBarrier.Text, out this.rebateBarrier))
            {
                tbRebateBarrier.BorderBrush = Brushes.Red;
                this.checker9 = false;
            }
            else
            {
                if (this.lastTradedPrice <= 0)
                {
                    tbRebateBarrier.BorderBrush = Brushes.Red;
                    this.checker9 = false;
                }
                else
                {
                    tbRebateBarrier.BorderBrush = Brushes.White;
                    this.checker9 = true;
                }
                buttonEnabler();
            }
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            model.OrderBookDBs.RemoveRange(model.OrderBookDBs.Where(x=>x.InstrumentsId == (Int64)cbSymbolToBeDeleted.SelectedValue));
            model.InstrumentsDBs.Remove(model.InstrumentsDBs.Where(x=> x.Id == (Int64)cbSymbolToBeDeleted.SelectedValue).FirstOrDefault());
            model.OptionsDBs.Remove(model.OptionsDBs.Where(x=> x.Symbol == model.InstrumentsDBs.Where(y => y.Id == (Int64)cbSymbolToBeDeleted.SelectedValue).Select(z => z.Symbol).First()).FirstOrDefault());
            model.SaveChangesAsync();
            this.Close();
        }

        private void cbSymbolToBeDeleted_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bDelete.IsEnabled = true;
        }

    }
}
