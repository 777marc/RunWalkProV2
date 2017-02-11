using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RunWalkProV2.Models.Data;
using RunWalkProV2.Models;

namespace RunWalkProV2
{
    public partial class Settings : PhoneApplicationPage
    {
        RWPDB.RWPDataContext _RWPDB;
        RWPDB.UserSetting _userSettings;

        public Settings()
        {
            // get db
            _RWPDB = new RWPDB.RWPDataContext(RWPDB.RWPDataContext.DBConnectionString);
            _userSettings = _RWPDB.UserSettings.FirstOrDefault();

            InitializeComponent();
            BuildAppBar();
            initializeLists();
            LoadUserSettings();
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnHome = new ApplicationBarIconButton(new Uri("/Assets/AppBar/back.png", UriKind.Relative));
            btnHome.Text = "start";
            btnHome.Click += new EventHandler(go_home);
            ApplicationBar.Buttons.Add(btnHome);
        }

        private void initializeLists()
        {
            // unit
            List<string> units = new List<string>() { "Miles", "Kilometers" };
            rlpUnits.ItemsSource = units;

            List<string> activityType = new List<string>() { "Run", "Walk" };
            rlpStartWith.ItemsSource = activityType;

            List<string> alertType = new List<string>() { "Voice", "Beep" };
            rlpAlertType.ItemsSource = alertType;
        }

        private void go_home(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivMain.xaml", UriKind.Relative));
        }

        private void LoadUserSettings()
        {


            // other
            rlpAlertType.SelectedValue = UserSettings.AlertType;
            rlpStartWith.SelectedValue = UserSettings.StartWith;
            rlpUnits.SelectedValue = UserSettings.UnitOfMeasure;
            txtCalories.Text = UserSettings.CaloriesPerUnit.ToString();
            chkAllowLocation.IsChecked = UserSettings.LocationConsent;
        }




        private void rlpAlertType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _userSettings.AlertType = rlpAlertType.SelectedValue.ToString();
            UserSettings.AlertType = rlpAlertType.SelectedValue.ToString();
            _RWPDB.SubmitChanges();
        }

        private void rlpStartWith_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _userSettings.StartWith = rlpStartWith.SelectedValue.ToString();
            UserSettings.StartWith = rlpStartWith.SelectedValue.ToString();
            _RWPDB.SubmitChanges();
        }

        private void btnCalcCalories_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Coming soon!");
        }

        private void rlpUnits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _userSettings.UnitOfMeasure = rlpUnits.SelectedValue.ToString();
            UserSettings.UnitOfMeasure = rlpUnits.SelectedValue.ToString();
            _RWPDB.SubmitChanges();
        }

        private void rtxtCalories_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_userSettings.CaloriesPerUnit == Convert.ToInt32(txtCalories.Text))
                return;

            if (txtCalories.Text == "")
            {
                txtCalories.Text = "0";
            }

            _userSettings.CaloriesPerUnit = Convert.ToInt32(txtCalories.Text);
            _RWPDB.SubmitChanges();
        }

    }
}