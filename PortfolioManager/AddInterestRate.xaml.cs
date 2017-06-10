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
    /// Interaction logic for AddInterestRate.xaml
    /// </summary>
    public partial class AddInterestRate : MetroWindow
    {
        private static DataModelContainer model = new DataModelContainer();
        private Double interestRate;

        public AddInterestRate()
        {
            InitializeComponent();
            this.dtTenor.DisplayDateStart = DateTime.Now;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            model.InterestRateDBs.Add(new InterestRateDB()
            {
                Tenor = dtTenor.SelectedDate.Value,
                Rate = this.interestRate/100,
            });
            model.SaveChangesAsync();
            this.Close();
        }
        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tbInterestRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Double.TryParse(this.tbInterestRate.Text, out this.interestRate))
            {
                tbInterestRate.BorderBrush = Brushes.Red;
                Add.IsEnabled = false;
            }
            else
            {
                if (this.interestRate <= 0)
                {
                    tbInterestRate.BorderBrush = Brushes.Red;
                    Add.IsEnabled = false;
                }
                else
                {
                    tbInterestRate.BorderBrush = Brushes.White;
                    Add.IsEnabled = true;
                }
            }
        }
    }
}
