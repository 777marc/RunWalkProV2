using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace RunWalkProV2
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            BuildAppBar();
            txtWebsite.NavigateUri = new Uri("http://runwalkpro.azurewebsites.net/wphelp");

        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnHome = new ApplicationBarIconButton(new Uri("/Assets/AppBar/back.png", UriKind.Relative));
            btnHome.Text = "home";
            btnHome.Click += new EventHandler(ho_home);
            ApplicationBar.Buttons.Add(btnHome);
        }

        private void ho_home(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/PivMain.xaml", UriKind.Relative));
        }


        private void btnRate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rateTask = new MarketplaceReviewTask();
                rateTask.Show();
            }
            catch
            {
                MessageBox.Show("There was a problem connecting to the marketplace. Please try again later.");
            }
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            string emailacct = "support@alminasoftware.com";

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Message From Run Walk PRO App";
            emailComposeTask.Body = "insert your message here";
            emailComposeTask.To = emailacct;

            emailComposeTask.Show();
        }
    }
}