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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RunWalkProV2
{
    public partial class History : PhoneApplicationPage
    {

        RWPDB.RWPDataContext _db;

        private ObservableCollection<RWPDB.RunHistory> _historyItems;
        public ObservableCollection<RWPDB.RunHistory> RunHistoryItems
        {
            get
            {
                return _historyItems;
            }
            set
            {
                if (_historyItems != value)
                {
                    _historyItems = value;
                    NotifyPropertyChanged("RunHistoryItems");
                }
            }
        }


        public History()
        {
            InitializeComponent();
            BuildAppBar();
            _db = new RWPDB.RWPDataContext(RWPDB.RWPDataContext.DBConnectionString);
            this.DataContext = this;
            GetHistory();
        }

        private void GetHistory()
        {
           var _historyItems = _db.RunHistoryItems.Where(h => h.HistoryItemId > 0).OrderByDescending(h => h.ItemDate);
           RunHistoryItems = new ObservableCollection<RWPDB.RunHistory>(_historyItems);
           lisRunHistory.ItemsSource = RunHistoryItems;
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnHome = new ApplicationBarIconButton(new Uri("/Assets/AppBar/back.png", UriKind.Relative));
            btnHome.Text = "start";
            btnHome.Click += new EventHandler(ho_home);
            ApplicationBar.Buttons.Add(btnHome);
        }

        private void ho_home(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivMain.xaml", UriKind.Relative));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void chkDel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Delete run?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var btn = sender as Button;

                RWPDB.RunHistory rh = btn.DataContext as RWPDB.RunHistory;

                _db.RunHistoryItems.DeleteOnSubmit(rh);
                _db.SubmitChanges();

                GetHistory();
            }
        }

    }
}