using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using System.IO;
using RunWalkProV2.Helpers;
using RunWalkProV2.Models;
using RunWalkProV2.Models.Data;

namespace RunWalkProV2
{
    public partial class RWPLogin : PhoneApplicationPage
    {

        private RWPDB.RWPDataContext _RWPDB;

        public RWPLogin()
        {
            InitializeComponent();
            checkRWPOnlineConnection();
            BuildAppBar();
            _RWPDB = new RWPDB.RWPDataContext(RWPDB.RWPDataContext.DBConnectionString);

            radBusyInd.Visibility = System.Windows.Visibility.Collapsed;

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


        private void cmdLogin_Click(object sender, RoutedEventArgs e)
        {

            bool connecting = true;
            setVisibility(connecting);

            string address = "http://rwpapiv2.azurewebsites.net/api/Mob?";
            string userNamePrefix = "username=";
            string pwPrefix = "&pw=";

            if (txtUserName.Text != "" && txtPassword.Password != "")
            {
                address = address + userNamePrefix + txtUserName.Text + pwPrefix + txtPassword.Password;
                fetchLoginData(address);
            }
        }

        private async void fetchLoginData(string myInfoURI)
        {
            bool connecting = false;

            try
            {
                string x = await fetchLoginDataCall(myInfoURI);

                if (x.IndexOf("-1") == -1)
                {

                    RWPDB.UserSetting us = _RWPDB.UserSettings.FirstOrDefault();

                    string token = x.Substring(1, x.Length - 2);

                    UserSettings.RunWalkPROToken = token;
                    UserSettings.RunWalkPROUserName = txtUserName.Text;

                    us.RunWalkPROUserName = txtUserName.Text;
                    us.RunWalkPROToken = token;

                    _RWPDB.SubmitChanges();

                    txtErrors.Text = "You are now connected!";
                    txtRWPStatus.Text = "Status: Connected as " + UserSettings.RunWalkPROUserName;
                    
                    checkRWPOnlineConnection();

                }
                else
                {
                    txtErrors.Text = "Invalid login credentials.  Try again.";
                    txtRWPStatus.Text = "Status: Not Connected";
                }

            }
            catch (Exception ex)
            {
                txtErrors.Text = ex.Message.ToString();
            }
            finally
            {
                setVisibility(connecting);
            }
        }

        private async Task<string> fetchLoginDataCall(string feedURI)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(feedURI);
            request.Method = HttpMethod.Get;
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        private void btnRWPDisconnect_Click(object sender, RoutedEventArgs e)
        {
            RWPDB.UserSetting us = _RWPDB.UserSettings.FirstOrDefault();

            UserSettings.RunWalkPROUserName = null;
            UserSettings.RunWalkPROToken = null;

            us.RunWalkPROUserName = "";
            us.RunWalkPROToken = "";

            _RWPDB.SubmitChanges();

            txtRWPStatus.Text = "Status: Not Connected";
            txtErrors.Text = "";
            checkRWPOnlineConnection();
        }

        private void checkRWPOnlineConnection()
        {

            if (UserSettings.RunWalkPROToken != null && UserSettings.RunWalkPROUserName != null)
            {
                cmdLogin.Visibility = System.Windows.Visibility.Collapsed;
                btnRWPDisconnect.Visibility = System.Windows.Visibility.Visible;
                txtRWPStatus.Text = "Status: Connected as " + UserSettings.RunWalkPROUserName;
            }
            else
            {
                cmdLogin.Visibility = System.Windows.Visibility.Visible;
                btnRWPDisconnect.Visibility = System.Windows.Visibility.Collapsed;
                txtRWPStatus.Text = "Status: Not Connected";
            }
        }

        private void setVisibility(bool connecting)
        {

            if(connecting)
            {
                cmdLogin.Visibility = System.Windows.Visibility.Collapsed;
                btnRWPDisconnect.Visibility = System.Windows.Visibility.Collapsed;
                txtWebsite.Visibility = System.Windows.Visibility.Collapsed;
                txtSignUpMessage.Visibility = System.Windows.Visibility.Collapsed;
                txtRWPStatus.Visibility = System.Windows.Visibility.Collapsed;

                radBusyInd.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                cmdLogin.Visibility = System.Windows.Visibility.Visible;
                btnRWPDisconnect.Visibility = System.Windows.Visibility.Visible;
                txtWebsite.Visibility = System.Windows.Visibility.Visible;
                txtSignUpMessage.Visibility = System.Windows.Visibility.Visible;
                txtRWPStatus.Visibility = System.Windows.Visibility.Visible;

                radBusyInd.Visibility = System.Windows.Visibility.Collapsed;
            }

        }
    }
}