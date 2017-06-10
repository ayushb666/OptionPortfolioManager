using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using PortfolioManager.Classes;
using System.Windows.Threading;
using PortfolioManager.Model;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace PortfolioManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Variables
        private Double underlyingPrice;
        private DateTime maturityDate;
        private Double strike;
        private Double vol;
        private OptionType type;
        private Double rate;
        private Double rebate;
        private Int64 simulationNumber;
        private static readonly Object lck = new Object();
        private Int32 steps;
        private Boolean controlVariateEnabled;
        private Boolean multithreadingEnabled;
        private BarrierOptionType barrierOptiont;
        private Boolean anitheticReductionEnabled;
        private Stopwatch sw = new Stopwatch();
        private GraphPlotting graphPlot = null;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private Boolean checker1 = false;
        private Boolean checker2 = false;
        private Boolean checker3 = false;
        private Boolean checker4 = false;
        private Boolean checker5 = false;
        private Boolean checker6 = false;
        private Boolean checker7 = false;
        private Boolean checker8 = false;
        private Boolean checker9 = false;
        private static DataModelContainer model = new DataModelContainer();
        private OptionKind kind;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.dtMaturityDate.DisplayDateStart = DateTime.Now;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            var trades = model.OrderBookDBs.ToList();
            dataGrid.DataContext = trades;
        }

        #region Price Calulator Page

        #region Buttons
        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            this.sw.Reset(); this.sw.Start();
            graphPlot = new GraphPlotting();
            this.DataContext = graphPlot;
            this.graphs.IsEnabled = true;
            if (this.simulationNumber > 500 && this.steps > 500) { this.graphs.ToolTip = "Graph may take time to load "; }
            else { this.graphs.ToolTip = ""; }
            ISecurity underlying = new Stock(this.underlyingPrice);
            Options option = null;
            switch (this.kind)
            {
                case OptionKind.ASIAN:
                    option = new AsianOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind);
                    break;
                case OptionKind.BARRIER:
                    option = new BarrierOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind, this.rebate, this.barrierOptiont);
                    break;
                case OptionKind.DIGITAL:
                    option = new DigitalOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind, this.rebate);
                    break;
                case OptionKind.EUROPEAN:
                    option = new EuropeanOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind);
                    break;
                case OptionKind.LOOKBACK:
                    option = new LookbackOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind);
                    break;
                case OptionKind.RANGE:
                    option = new RangeOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind);
                    break;
                default:
                    option = new EuropeanOption(underlying.Symbol, underlying, this.maturityDate, this.strike, this.vol, this.type, this.kind);
                    break;
            }
            try
            {
                Task work = Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke(() => this.progress.IsActive = true);
                    option.calulateOptionPriceAndGreeks(this.simulationNumber, this.rate, this.steps, this.anitheticReductionEnabled, this.controlVariateEnabled, this.multithreadingEnabled, plot: this.graphPlot);
                    this.sw.Stop();
                    this.Dispatcher.Invoke(() => { display(option); this.progress.IsActive = false; });
                });
            }
            catch (Exception message)
            {
                MessageBox.Show(message.Message.ToString() + "\n" + message.StackTrace.ToString());
            }
        }
        #endregion

        #region Function that sets the value of labels of Option price , Standard Error & Greeks
        private void display(Options option)
        {
            result.Visibility = Visibility.Visible;
            price.Visibility = Visibility.Visible;
            lPricePlot.Content = lPrice.Content = "$" + Math.Round(option.Price, 4).ToString();
            error.Visibility = Visibility.Visible;
            lErrorPlot.Content = lError.Content = Math.Round(option.StandarError, 4).ToString();
            delta.Visibility = Visibility.Visible;
            lDeltaPlot.Content = lDelta.Content = Math.Round(option.Greeks.Delta, 4).ToString();
            theta.Visibility = Visibility.Visible;
            lThetaPlot.Content = lTheta.Content = Math.Round(option.Greeks.Theta, 4).ToString();
            gamma.Visibility = Visibility.Visible;
            lGammaPlot.Content = lGamma.Content = Math.Round(option.Greeks.Gamma, 4).ToString();
            vega.Visibility = Visibility.Visible;
            lVegaPlot.Content = lVega.Content = Math.Round(option.Greeks.Vega, 4).ToString();
            rho.Visibility = Visibility.Visible;
            lRhoPlot.Content = lRho.Content = Math.Round(option.Greeks.Rho, 4).ToString();
            timeTaken.Visibility = Visibility.Visible;
            lTimeTakenPlot.Content = lTimeTaken.Content = this.sw.Elapsed.Hours.ToString() + ":" + this.sw.Elapsed.Minutes.ToString() + ":" + this.sw.Elapsed.Seconds.ToString() + ":" + this.sw.Elapsed.Milliseconds.ToString();
            processorCount.Visibility = Visibility.Visible;
            lProcessorCountPlot.Content = lProcessorCount.Content = Environment.ProcessorCount.ToString();
        }
        #endregion

        #region Error Checkers
        private void tvCurrentPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tvCurrentPrice.Text, out this.underlyingPrice))
            {
                tvCurrentPrice.BorderBrush = Brushes.Red;
                this.checker1 = false;
            }
            else
            {
                if (this.underlyingPrice <= 0)
                {
                    tvCurrentPrice.BorderBrush = Brushes.Red;
                    this.checker1 = false;
                }
                else
                {
                    tvCurrentPrice.BorderBrush = Brushes.White;
                    this.checker1 = true;
                }
                buttonEnabler();
            }
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

        private void tbInterestRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbInterestRate.Text, out this.rate))
            {
                tbInterestRate.BorderBrush = Brushes.Red;
                this.checker3 = false;
            }
            else
            {
                if (this.rate < 0)
                {
                    tbInterestRate.BorderBrush = Brushes.Red;
                    this.checker3 = false;
                }
                else
                {
                    tbInterestRate.BorderBrush = Brushes.White;
                    this.rate = this.rate / 100;
                    this.checker3 = true;
                }
                buttonEnabler();
            }
        }

        private void tbVolatility_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbVolatility.Text, out this.vol))
            {
                tbVolatility.BorderBrush = Brushes.Red;
                this.checker4 = false;
            }
            else
            {
                if (this.vol < 0)
                {
                    tbVolatility.BorderBrush = Brushes.Red;
                    this.checker4 = false;
                }
                else
                {
                    tbVolatility.BorderBrush = Brushes.White;
                    this.vol = this.vol / 100;
                    this.checker4 = true;
                }
                buttonEnabler();
            }
        }

        private void tbNumberOfSimulations_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Int64.TryParse(this.tbNumberOfSimulations.Text, out this.simulationNumber))
            {
                tbNumberOfSimulations.BorderBrush = Brushes.Red;
                this.checker5 = false;
            }
            else
            {
                if (this.simulationNumber <= 0)
                {
                    tbNumberOfSimulations.BorderBrush = Brushes.Red;
                    this.checker5 = false;
                }
                else
                {
                    tbNumberOfSimulations.BorderBrush = Brushes.White;
                    this.checker5 = true;
                    simulationNumber = 2 * (simulationNumber / 2);
                }
                buttonEnabler();
            }
        }

        private void tbnumberOfScenarios_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Int32.TryParse(this.tbnumberOfScenarios.Text, out this.steps))
            {
                tbnumberOfScenarios.BorderBrush = Brushes.Red;
                this.checker6 = false;
            }
            else
            {
                if (this.steps <= 0)
                {
                    tbnumberOfScenarios.BorderBrush = Brushes.Red;
                    this.checker6 = false;
                }
                else
                {
                    tbnumberOfScenarios.BorderBrush = Brushes.White;
                    this.checker6 = true;
                }
                buttonEnabler();
            }
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

        private void call_Click(object sender, RoutedEventArgs e)
        {
            if ((Boolean)call.IsChecked)
            {
                this.type = OptionType.CALL;
                this.checker8 = true;
            }
            buttonEnabler();
        }

        private void put_Click(object sender, RoutedEventArgs e)
        {
            if ((Boolean)put.IsChecked)
            {
                this.type = OptionType.PUT;
                this.checker8 = true;
            }
            buttonEnabler();
        }

        private void antitheticReduction_Click(object sender, RoutedEventArgs e)
        {
            this.anitheticReductionEnabled = (Boolean)this.antitheticReduction.IsChecked;
        }

        private void cbcontrolVariate_Click(object sender, RoutedEventArgs e)
        {
            this.controlVariateEnabled = (Boolean)this.cbcontrolVariate.IsChecked;
        }

        private void multithreading_Click(object sender, RoutedEventArgs e)
        {
            this.multithreadingEnabled = (Boolean)this.multithreading.IsChecked;
        }

        private void tbRebate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbRebate.Text, out this.rebate))
            {
                tbRebate.BorderBrush = Brushes.Red;
                this.checker9 = false;
            }
            else
            {
                if (this.rebate <= 0)
                {
                    tbRebate.BorderBrush = Brushes.Red;
                    this.checker9 = false;
                }
                else
                {
                    tbRebate.BorderBrush = Brushes.White;
                    this.checker9 = true;
                }
                buttonEnabler();
            }
        }

        private void buttonEnabler()
        {
            if (this.checker1 && this.checker2 && this.checker3 && this.checker4 && this.checker5 && this.checker6 && this.checker7 && this.checker8 && this.checker9)
            {
                calculate.IsEnabled = true;
            }
            else
            {
                calculate.IsEnabled = false;
            }
        }

        #endregion

        #region Option Kind Selecter
        private void optionKinds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String optionKindValue = ((ComboBoxItem)this.optionKinds.SelectedValue).Name;
            switch (optionKindValue)
            {
                case "asian":
                    this.extra.Visibility = Visibility.Hidden;
                    this.dpBarrierOptionType.Visibility = Visibility.Hidden;
                    this.checker9 = true;
                    this.tbStrikePrice.IsEnabled = true;
                    this.put.IsEnabled = true;
                    this.call.IsEnabled = true;
                    this.kind = OptionKind.ASIAN;
                    break;
                case "barrier":
                    this.extra.Visibility = Visibility.Visible;
                    this.dpBarrierOptionType.Visibility = Visibility.Visible;
                    this.lrebate.Content = "Barrier: ";
                    this.tbStrikePrice.IsEnabled = true;
                    this.put.IsEnabled = true;
                    this.call.IsEnabled = true;
                    this.kind = OptionKind.BARRIER;
                    break;
                case "digital":
                    this.extra.Visibility = Visibility.Visible;
                    this.dpBarrierOptionType.Visibility = Visibility.Hidden;
                    this.lrebate.Content = "Rebate: ";
                    this.tbStrikePrice.IsEnabled = true;
                    this.put.IsEnabled = true;
                    this.call.IsEnabled = true;
                    this.kind = OptionKind.DIGITAL;
                    break;
                case "european":
                    if (this.extra != null) { this.extra.Visibility = Visibility.Hidden; }
                    if (this.dpBarrierOptionType != null) { this.dpBarrierOptionType.Visibility = Visibility.Hidden; }
                    if (this.tbStrikePrice != null && this.put != null && this.call != null)
                    {
                        this.tbStrikePrice.IsEnabled = true;
                        this.put.IsEnabled = true;
                        this.call.IsEnabled = true;
                    }
                    this.checker9 = true;
                    this.kind = OptionKind.EUROPEAN;
                    break;
                case "lookback":
                    this.extra.Visibility = Visibility.Hidden;
                    this.dpBarrierOptionType.Visibility = Visibility.Hidden;
                    this.checker9 = true;
                    this.tbStrikePrice.IsEnabled = true;
                    this.put.IsEnabled = true;
                    this.call.IsEnabled = true;
                    this.kind = OptionKind.LOOKBACK;
                    break;
                case "range":
                    this.extra.Visibility = Visibility.Hidden;
                    this.dpBarrierOptionType.Visibility = Visibility.Hidden;
                    this.checker9 = true;
                    this.kind = OptionKind.RANGE;
                    break;
                default:
                    break;
            }
        }

        private void barrierOptionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String barrierOptionTyp = ((ComboBoxItem)this.barrierOptionType.SelectedValue).Name;
            switch (barrierOptionTyp)
            {
                case "downout":
                    this.barrierOptiont = BarrierOptionType.DOWNOUT;
                    break;
                case "downin":
                    this.barrierOptiont = BarrierOptionType.DOWNIN;
                    break;
                case "upout":
                    this.barrierOptiont = BarrierOptionType.UPOUT;
                    break;
                case "upin":
                    this.barrierOptiont = BarrierOptionType.UPIN;
                    break;
                default:
                    break;
            }

        }

        #endregion

        #region time
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.currentTime.Content = DateTime.Now.ToShortTimeString();
        }
        #endregion

        #endregion

        #region Portfolio Page
        private void bAddNewStock_Click(object sender, RoutedEventArgs e)
        {
            AddStock newStockWindow = new AddStock();
            newStockWindow.Show();
        }

        private void bAddNewOption_Click(object sender, RoutedEventArgs e)
        {
            CreateNewOption newOptionWindow = new CreateNewOption();
            newOptionWindow.Show();
        }

        private void bTrade_Click(object sender, RoutedEventArgs e)
        {
            TradeWindow newTradeWindow = new TradeWindow();
            newTradeWindow.Show();
        }

        private void bRefresh_Click(object sender, RoutedEventArgs e)
        {
            var orderBook = model.OrderBookDBs.ToList();
            dataGrid.DataContext = orderBook;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double profit = 0, delta = 0, theta = 0, gamma = 0, vega = 0, rho = 0;
            foreach(OrderBookDB trade in dataGrid.SelectedItems) {
                profit += (Double)trade.ProfitLoss;
                delta += (Double)trade.Delta;
                theta += (Double)trade.Theta;
                gamma += (Double)trade.Gamma;
                vega += (Double)trade.Vega;
                rho += (Double)trade.Rho;
            }
            lvProfitnLoss.Content = profit.ToString();
            lvTotaldelta.Content = delta.ToString();
            lvTotalTheta.Content = theta.ToString();
            lvTotalGamma.Content = gamma.ToString();
            lvTotalVega.Content = vega.ToString();
            lvTotalRho.Content = rho.ToString();
        }

        private void bPriceOptionBook_Click(object sender, RoutedEventArgs e)
        {
            graphPlot = new GraphPlotting();
            graphs.DataContext = graphPlot;
            Task work = Task.Factory.StartNew(() =>
            {
                var orderBook = model.OrderBookDBs.ToList();
                foreach (var trade in orderBook)
                {
                    if (trade.InstrumentsDB.SecurityTypeDB.TypeName.Equals("Stocks"))
                    {
                        var instrument = model.StockDBs.Where(x => x.Symbol == trade.InstrumentsDB.Symbol).First();
                        trade.FairPrice = trade.Price;
                        trade.Delta = trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity;
                        trade.ProfitLoss = 0;
                        trade.Theta = 0;
                        trade.Gamma = 0;
                        trade.Vega = 0;
                        trade.Rho = 0;
                        lock (lck)
                        {
                            model.SaveChanges();
                        }
                    }
                    else if (trade.InstrumentsDB.SecurityTypeDB.TypeName.Equals("Options"))
                    {
                        var instrument = model.OptionsDBs.Where(x => x.Symbol == trade.InstrumentsDB.Symbol).First();
                        Options option = null;
                        ISecurity underlying = new Stock(instrument.StockDB.LastTradedPrice);
                        switch (instrument.OptionKindDB.OptionKindName)
                        {
                            case "Asian Option":
                                option = new AsianOption(instrument.Symbol, underlying, instrument.MaturityDate, (Double)instrument.StrikePrice, instrument.StockDB.HistoricalVolatility, instrument.OptionType == "CALL" ? OptionType.CALL : OptionType.PUT, OptionKind.ASIAN);
                                break;
                            case "Barrier Option":
                                option = new BarrierOption(instrument.Symbol, underlying, instrument.MaturityDate, (Double)instrument.StrikePrice, instrument.StockDB.HistoricalVolatility, instrument.OptionType == "CALL" ? OptionType.CALL : OptionType.PUT, OptionKind.BARRIER, (Double)instrument.Barrier, (instrument.BarrierOptionType == "downout" ? BarrierOptionType.DOWNOUT : (instrument.BarrierOptionType == "downin" ? BarrierOptionType.DOWNIN : (instrument.BarrierOptionType == "upout" ? BarrierOptionType.UPOUT : BarrierOptionType.UPIN))));
                                break;
                            case "Digital Option":
                                option = new DigitalOption(instrument.Symbol, underlying, instrument.MaturityDate, (Double)instrument.StrikePrice, instrument.StockDB.HistoricalVolatility, instrument.OptionType == "CALL" ? OptionType.CALL : OptionType.PUT, OptionKind.DIGITAL, (Double)instrument.Rebate);
                                break;
                            case "European Option":
                                option = new EuropeanOption(instrument.Symbol, underlying, instrument.MaturityDate, (Double)instrument.StrikePrice, instrument.StockDB.HistoricalVolatility, instrument.OptionType == "CALL" ? OptionType.CALL : OptionType.PUT, OptionKind.EUROPEAN);
                                break;
                            case "Lookback Option":
                                option = new LookbackOption(instrument.Symbol, underlying, instrument.MaturityDate, (Double)instrument.StrikePrice, instrument.StockDB.HistoricalVolatility, instrument.OptionType == "CALL" ? OptionType.CALL : OptionType.PUT, OptionKind.LOOKBACK);
                                break;
                            case "Range Option":
                                option = new RangeOption(instrument.Symbol, underlying, instrument.MaturityDate, (Double)instrument.StrikePrice, instrument.StockDB.HistoricalVolatility, instrument.OptionType == "CALL" ? OptionType.CALL : OptionType.PUT, OptionKind.RANGE);
                                break;
                            default:
                                break;
                        }
                        try
                        {
                            option.calulateOptionPriceAndGreeks(1000, 0.05, (option.ExpiryDate - DateTime.Now).Days, true, option.OptionKind == OptionKind.EUROPEAN ? true : false, true, plot: graphPlot);
                            trade.FairPrice = Math.Round(option.Price, 4);
                            trade.ProfitLoss = Math.Round((Double)trade.FairPrice - (Double)trade.Price, 4) * (trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity);
                            trade.Delta = Math.Round(option.Greeks.Delta, 4) * (trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity);
                            trade.Theta = Math.Round(option.Greeks.Theta, 4) * (trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity);
                            trade.Gamma = Math.Round(option.Greeks.Gamma, 4) * (trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity);
                            trade.Vega = Math.Round(option.Greeks.Vega, 4) * (trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity);
                            trade.Rho = Math.Round(option.Greeks.Rho, 4) * (trade.Position == "BUY" ? trade.Quantity : -1 * trade.Quantity);
                            lock (lck)
                            {
                                model.SaveChanges();
                            }
                        }
                        catch (Exception message)
                        {
                            MessageBox.Show(message.Message.ToString() + "\n" + message.StackTrace.ToString());
                        }
                    }
                }
                this.Dispatcher.Invoke(() => {
                    dataGrid.DataContext = orderBook;
                });
            });
        }

        private void bAddRate_Click(object sender, RoutedEventArgs e)
        {
            AddInterestRate newInterestRateWindow = new AddInterestRate();
            newInterestRateWindow.Show();
        }
    }
    #endregion
}