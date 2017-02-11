using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RunWalkProV2.Models;
using RunWalkProV2.Models.Data;
using System.Windows.Threading;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;
using NExtra.Geo;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using RunWalkProV2.Resources;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RunWalkProV2.Helpers;
using Windows.ApplicationModel.Store;
using Microsoft.Phone.Tasks;

namespace RunWalkProV2
{
    public partial class PivMain : PhoneApplicationPage
    {
        private GeoCoordinateWatcher _watcher;
        private MapPolyline _line;
        private DispatcherTimer _mainTimer;
        private double _kilometres;
        private double _miles;
        private long _previousPositionChangeTick;
        private bool _starting = false;

        private RWPDB.RWPDataContext _RWPDB;

        public PivMain()
        {
            InitializeComponent();
            checkRemoveAdsLicesnse();
            BuildLocalizedApplicationBar();
            setUpMap();
            InitializeTimer();
            _watcher.Start();

            // get db
            _RWPDB = new RWPDB.RWPDataContext(RWPDB.RWPDataContext.DBConnectionString);
            // load user setting from db
            getUserSetting();

            // location consent
            if(!UserSettings.LocationConsent)
            {
                GetLocationPermission();
            }

            // set up main screen
            if(SessionWorkout.StartTime == DateTime.MinValue)
            {
             
                txtState.Text = AppResources.AppReady;
                txtMainTimer.Text = AppResources.InitialTimerSetting;
                setButtonState("stop");
            }

            // load goals
            LoadGoals();
            setButtonState(App.ButtonState);


        }

        private void checkRemoveAdsLicesnse()
        {
            var licenseInformation = CurrentApp.LicenseInformation;

            if (licenseInformation.ProductLicenses["RWP_NoAds"].IsActive)
            {
                App.SHOWADS = false;
            }
            else
            {
                App.SHOWADS = true;
            }
        }


        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton btnStart = new ApplicationBarIconButton(new Uri("/Assets/AppBar/play.png", UriKind.Relative));
            btnStart.Text = "start";
            btnStart.Click += new EventHandler(start);
            ApplicationBar.Buttons.Add(btnStart);

            ApplicationBarIconButton btnPause = new ApplicationBarIconButton(new Uri("/Assets/AppBar/pause.png", UriKind.Relative));
            btnPause.Text = "pause";
            btnPause.Click += new EventHandler(pause);
            ApplicationBar.Buttons.Add(btnPause);

            ApplicationBarIconButton btnStop = new ApplicationBarIconButton(new Uri("/Assets/AppBar/stop.png", UriKind.Relative));
            btnStop.Text = "stop";
            btnStop.Click += new EventHandler(stop);
            ApplicationBar.Buttons.Add(btnStop);

            ApplicationBarMenuItem mnuSettings = new ApplicationBarMenuItem();
            mnuSettings.Text = "settings";
            mnuSettings.Click += new EventHandler(goto_settings);
            ApplicationBar.MenuItems.Add(mnuSettings);

            ApplicationBarMenuItem mnuRWPOnline = new ApplicationBarMenuItem();
            mnuRWPOnline.Text = "Run Walk PRO Online";
            mnuRWPOnline.Click += new EventHandler(goto_rwponline);
            ApplicationBar.MenuItems.Add(mnuRWPOnline);

            ApplicationBarMenuItem mnuHistory = new ApplicationBarMenuItem();
            mnuHistory.Text = "history";
            mnuHistory.Click += new EventHandler(goto_history);
            ApplicationBar.MenuItems.Add(mnuHistory);

            ApplicationBarMenuItem mnuAbout = new ApplicationBarMenuItem();
            mnuAbout.Text = "About Us";
            mnuAbout.Click += new EventHandler(goto_about);
            ApplicationBar.MenuItems.Add(mnuAbout);

            if(App.SHOWADS)
            { 
                ApplicationBarMenuItem mnuDonate = new ApplicationBarMenuItem();
                mnuDonate.Text = "Remove Ads";
                mnuDonate.Click += new EventHandler(goto_donate);
                ApplicationBar.MenuItems.Add(mnuDonate);
            }

            ApplicationBarMenuItem mnuHelp = new ApplicationBarMenuItem();
            mnuHelp.Text = "Online Help";
            mnuHelp.Click += new EventHandler(goto_help);
            ApplicationBar.MenuItems.Add(mnuHelp);

        }

        private void goto_help(object sender, EventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://runwalkpro.azurewebsites.net/wphelp", UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void getUserSetting()
        {
            IQueryable<RWPDB.UserSetting> uset = _RWPDB.UserSettings.Where(u => u.UserSettingItemId > 0);

            if(uset.Count() == 0) // initial load
            {
                RWPDB.UserSetting us = new RWPDB.UserSetting();

                us.WalkInterval = 60;
                us.RunInterval = 120;
                us.AlertType = "Voice";
                us.CaloriesPerUnit = 125;
                us.LocationConsent = false;
                us.StartWith = "Run";
                us.UnitOfMeasure = "Miles";
                us.GoalTimeInSeconds = 0;
                us.GoalPaceInSeconds = 0;
                us.GoalDistance = 0;

                _RWPDB.UserSettings.InsertOnSubmit(us);
                _RWPDB.SubmitChanges();
            }

            RWPDB.UserSetting userset = _RWPDB.UserSettings.FirstOrDefault(u => u.UserSettingItemId > 0);

            UserSettings.WalkInterval = userset.WalkInterval;
            UserSettings.RunInterval = userset.RunInterval;
            UserSettings.StartWith = userset.StartWith;
            UserSettings.AlertType = userset.AlertType;
            UserSettings.CaloriesPerUnit = userset.CaloriesPerUnit;
            UserSettings.LocationConsent = userset.LocationConsent;
            UserSettings.UnitOfMeasure = userset.UnitOfMeasure;
            UserSettings.PaceGoal = userset.GoalPaceInSeconds;
            UserSettings.TimeGoal = userset.GoalTimeInSeconds;
            UserSettings.DistanceGoal = userset.GoalDistance;

            // intervals
            TimeSpan runInterval = TimeSpan.FromSeconds(UserSettings.RunInterval);
            radTimeSpanPickerRun.EmptyContent = string.Format("{0:D2}:{1:D2}:{2:D2}", runInterval.Hours, runInterval.Minutes, runInterval.Seconds);
            radTimeSpanPickerRun.Value = runInterval;

            TimeSpan walkInterval = TimeSpan.FromSeconds(UserSettings.WalkInterval);
            radTimeSpanPickerWalk.EmptyContent = string.Format("{0:D2}:{1:D2}:{2:D2}", walkInterval.Hours, walkInterval.Minutes, walkInterval.Seconds);
            radTimeSpanPickerWalk.Value = walkInterval;   

        }

        private void LoadGoals()
        {
            if (UserSettings.DistanceGoal != 0)
            {
                txtDistanceGoal.Text = UserSettings.DistanceGoal.ToString();
            }
            if (UserSettings.TimeGoal != 0)
            {
                rtpTimeGoal.Value = TimeSpan.FromSeconds(UserSettings.TimeGoal);
            }
            if (UserSettings.PaceGoal != 0)
            {
                rtpPaceGoal.Value = TimeSpan.FromSeconds(UserSettings.PaceGoal);
            }
        }

        private void setUpMap()
        {
            // create a line which illustrates the run
            _line = new MapPolyline();
            _line.StrokeColor = Colors.Red;
            _line.StrokeThickness = 5;
            Map.MapElements.Add(_line);

            //Map.CartographicMode = MapCartographicMode.Aerial;
                        
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);

            _watcher.PositionChanged += Watcher_PositionChanged;
            _watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
        }

        private void GetLocationPermission()
        {
            RWPDB.UserSetting us = _RWPDB.UserSettings.FirstOrDefault();

            string privacyNotice = "This app accesses your phone's location. Your location is only used " +
                                   "to provide pace and map data for your workout and is never shared with " +
                                   "anyone.  Is it okay for Run Walk PRO to use your location? If not, click " +
                                   "cancel and the application will close.";

            MessageBoxResult result =
                MessageBox.Show(privacyNotice,
                "Location",
                MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                UserSettings.LocationConsent = true;
                us.LocationConsent = true;
                _RWPDB.SubmitChanges();
                setUpMap();
                txtBlkLocationServiceNotify.Text = "";
            }
            else
            {
                UserSettings.LocationConsent = false;
                us.LocationConsent = false;
                _RWPDB.SubmitChanges();
                txtBlkLocationServiceNotify.Text = "location serverices are turned off.";
                Application.Current.Terminate();
            }
            

        }

        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => GeoStatusChanged(e));
        }

        void GeoStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Disabled)
            {
                App.LOCATIONSERVICEAVAILABLE = false;
            }
            else
            {
                App.LOCATIONSERVICEAVAILABLE = true;
            }

            CheckLocationServices();
        }

        private void CheckLocationServices()
        {
            if (!App.LOCATIONSERVICEAVAILABLE)
            {
                txtBlkLocationServiceNotify.Text = "Location services is off on your phone. Map will not function.";
            }
            else
            {
                if (UserSettings.LocationConsent)
                {
                    txtBlkLocationServiceNotify.Text = "";
                }
            }
        }

        private void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {

            if (SessionWorkout.StartTime == DateTime.MinValue)
                return;

            var coord = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            DistanceConverter dc = new DistanceConverter();
            int caloriesPerUnit;

            caloriesPerUnit = UserSettings.CaloriesPerUnit;

            if (_line.Path.Count > 0)
            {
                var previousPoint = _line.Path.Last();
                var distance = coord.GetDistanceTo(previousPoint);
                var millisPerKilometer = (1000.0 / distance) * (System.Environment.TickCount - _previousPositionChangeTick);
                _kilometres += distance / 1000.0;

                _miles = dc.ConvertKilometersToMiles(_kilometres);
                var millisPerMile = (1000.0 / _miles) * (System.Environment.TickCount - _previousPositionChangeTick);
                

                if (UserSettings.UnitOfMeasure == "Miles")
                {
                    TimeSpan currenttime = TimeSpan.FromSeconds(SessionWorkout.BaseSeconds);
                    TimeSpan pace = CalculatePPD(currenttime, _miles);
                    txtBlkPaceMain.Text = pace.ToString(@"mm\:ss");
                    SetPaceColor(pace);
                    txtBlkDistanceMain.Text = string.Format("{0:f2}", _miles);
                    txtBlkCaloriesMain.Text = string.Format("{0:f0}", _miles * caloriesPerUnit);
                    txtPacePer.Text = "Mins./Mile";
                    // save current distance
                    SessionWorkout.Distance = _miles;
                }
                else
                {
                    TimeSpan currenttime = TimeSpan.FromSeconds(SessionWorkout.BaseSeconds);
                    TimeSpan pace = CalculatePPD(currenttime, _kilometres);
                    SetPaceColor(pace);    
                    txtBlkPaceMain.Text = pace.ToString(@"mm\:ss");
                    txtBlkDistanceMain.Text = string.Format("{0:f2}", _kilometres);
                    txtBlkCaloriesMain.Text = string.Format("{0:f0}", _kilometres * caloriesPerUnit);
                    // save current distance
                    SessionWorkout.Distance = _kilometres;
                }

                PositionHandler handler = new PositionHandler();
                var heading = handler.CalculateBearing(new Position(previousPoint), new Position(coord));
                Map.SetView(coord, Map.ZoomLevel, heading, MapAnimationKind.Parabolic);

            }
            else
            {
                Map.Center = coord;
            }

            _line.Path.Add(coord);
            _previousPositionChangeTick = System.Environment.TickCount;

            checkDistanceGoal();

        }

        private void checkDistanceGoal()
        {
            if (UserSettings.DistanceGoal == 0)
                return;

            double percentdone = 0;
            percentdone = ((SessionWorkout.Distance / UserSettings.DistanceGoal) * 100);

            if (percentdone > 100)
                return;

            gaugeDistance.Value = percentdone;
        }

        private void SetPaceColor(TimeSpan pace)
        {
            if (UserSettings.PaceGoal < pace.TotalSeconds)
            {
                txtBlkPaceMain.Foreground = new SolidColorBrush(Colors.Red);               
            }
            else
            {
                txtBlkPaceMain.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        public TimeSpan CalculatePPD(TimeSpan time, double distance)
        {
            return new TimeSpan(0, 0, (int)(time.TotalSeconds / distance));
        }

        private void start(object sender, EventArgs e)
        {
            if (SessionWorkout.StartTime == null || SessionWorkout.StartTime == DateTime.MinValue)
            {
                _starting = true;
                SessionWorkout.StartTime = DateTime.Now;

                if (UserSettings.StartWith == "Walk")
                {
                    SessionWorkout.IsRunning = false;
                }
                else
                {
                    SessionWorkout.IsRunning = true;
                }

            }

            // keep track of how much
            if (SessionWorkout.PausedTime != DateTime.MinValue)
            {
                TimeSpan span = (SessionWorkout.PausedTime - DateTime.Now);
                SessionWorkout.SecondsPaused += span.TotalSeconds;
                SessionWorkout.PausedTime = DateTime.MinValue;
            }


            _watcher.Start();
            _mainTimer.Start();
            setButtonState("start");
            enableMenu(false);
        }

        private void pause(object sender, EventArgs e)
        {
            _mainTimer.Stop();
            _watcher.Stop();
            SessionWorkout.PausedTime = DateTime.Now;
            setButtonState("pause");
        }

        private void stop(object sender, EventArgs e)
        {
            _mainTimer.Stop();
            _watcher.Stop();

            MessageBoxResult result =
            MessageBox.Show("Are you sure you want to reset your workout data?","Reset Workout", MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
            {
                return;
            }

            SaveWorkout();

            SessionWorkout.StartTime = DateTime.MinValue;
            SessionWorkout.PausedTime = DateTime.MinValue;
            SessionWorkout.SecondsPaused = 0;
            SessionWorkout.BaseSeconds = 0;
            SessionWorkout.Distance = 0;
            SessionWorkout.IntervalStartTime = DateTime.MinValue;
            SessionWorkout.IntervalElapsedTime = 0;
            SessionWorkout.CurentIntervalDuration = 0;
            SessionWorkout.IsRunning = false;

            txtState.Text = AppResources.AppReady;
            txtMainTimer.Text = AppResources.InitialTimerSetting;
            _miles = 0;
            _kilometres = 0;

            txtBlkLocationServiceNotify.Text = "";
            txtBlkPaceMain.Text = AppResources.InitialPace;
            txtBlkPaceMain.Foreground = new SolidColorBrush(Colors.White);               
            txtBlkDistanceMain.Text = AppResources.InitialDistance;
            txtBlkCaloriesMain.Text = AppResources.InitialCalories;
            setButtonState("stop");
            setUpMap();
            gaugeTime.Value = 0;
            gaugeDistance.Value = 0;
            _starting = true;
            enableMenu(true);

        }

        private void SaveWorkout()
        {

            string shortPace;
            TimeSpan pace = CalculatePPD(TimeSpan.FromSeconds(SessionWorkout.BaseSeconds), SessionWorkout.Distance);
            shortPace = pace.ToString(@"mm\:ss");

            RWPDB.RunHistory rh = new RWPDB.RunHistory
            {
                Time = SessionWorkout.BaseSeconds,
                TimeForDisplay = txtMainTimer.Text,
                Distance = SessionWorkout.Distance,
                ItemDate = DateTime.Now,
                Felt = "Good",
                Pace = shortPace,
                RunType = "Regular"
            };

            _RWPDB.RunHistoryItems.InsertOnSubmit(rh);
            _RWPDB.SubmitChanges();

            if(UserSettings.RunWalkPROToken != "" || UserSettings.RunWalkPROToken != null)
            {
                SaveWorkoutOnline(rh);
            }
        }

        private void SaveWorkoutOnline(RWPDB.RunHistory rh)
        {

            double duration;
            duration = rh.Time;

            double distanceRaw = rh.Distance;
            double Distance = Math.Round(distanceRaw, 2);


            TimeSpan currentValue = TimeSpan.FromSeconds(duration);
            string woTime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                            currentValue.Hours,
                                            currentValue.Minutes,
                                            currentValue.Seconds);

            RWPWorkout rwpwo = new RWPWorkout
            {
                Title = "I ran " + Distance.ToString() + " " + UserSettings.UnitOfMeasure + " in " + woTime,
                RunType = "Regular",
                Distance = Distance,
                Time = Convert.ToInt32(duration),
                Felt = "Good",
                ItemDate = DateTime.Now.ToString(),
                Temp = 75,
                UserName = UserSettings.RunWalkPROUserName,
                Note = "Uploaded from my Windows Phone."
            };

            var rwpwojson = JsonConvert.SerializeObject(rwpwo, Formatting.None);

            // post workout to RWP Online
            var rwpresult = PostMyWorkOut(App.RWPPostURL + UserSettings.RunWalkPROToken, rwpwojson);

        }

        public async Task<string> PostMyWorkOut(string postURI, string woData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postURI);
            request.ContentType = "application/json";
            request.Method = HttpMethod.Post;
            request.ContentLength = woData.Length;

            using (StreamWriter requeststream = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                requeststream.Write(woData);
                requeststream.Flush();
                requeststream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {

            if (SessionWorkout.IntervalStartTime == DateTime.MinValue)
            {
                SessionWorkout.IntervalStartTime = DateTime.Now;
            }

            DateTime currentTime = DateTime.Now;
            TimeSpan elapsedSeconds = (currentTime - SessionWorkout.IntervalStartTime);

            SessionWorkout.IntervalElapsedTime = elapsedSeconds.TotalSeconds;

            if (SessionWorkout.IntervalElapsedTime >= SessionWorkout.CurentIntervalDuration)
            {

                SessionWorkout.IntervalStartTime = DateTime.Now;

                if(!_starting)
                {
                    if(SessionWorkout.IsRunning)
                    {
                        SessionWorkout.IsRunning = false;
                    }
                    else
                    {
                        SessionWorkout.IsRunning = true;
                    }
                }

                playAlert(_starting);

                setState(SessionWorkout.IsRunning);

                SessionWorkout.IntervalElapsedTime = 0;
            }

            TimeSpan currentValue = DateTime.Now.AddSeconds(SessionWorkout.SecondsPaused) - SessionWorkout.StartTime;

            txtMainTimer.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                                currentValue.Hours,
                                                currentValue.Minutes,
                                                currentValue.Seconds);


            SessionWorkout.BaseSeconds = currentValue.TotalSeconds;

            double intTimeRemaining = SessionWorkout.CurentIntervalDuration - SessionWorkout.IntervalElapsedTime;


            TimeSpan currentIntervalValue = TimeSpan.FromSeconds(intTimeRemaining);

            txtBlkLocationServiceNotify.Text = "Interval Time Remaining: " + string.Format("{0:D2}:{1:D2}",
                                                                                            currentIntervalValue.Minutes,
                                                                                            currentIntervalValue.Seconds);

            checkTimeGoal(currentValue.Seconds);

        }

        private void checkTimeGoal(double currentSeconds)
        {
            if (UserSettings.TimeGoal == 0)
                return;

            double percentagedone = 0;
            percentagedone = ((currentSeconds / UserSettings.TimeGoal) * 100);

            if (percentagedone > 100)
                return;

            gaugeTime.Value = percentagedone;
        }

        private void goto_settings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void goto_rwponline(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RWPLogin.xaml", UriKind.Relative));
        }

        private void goto_history(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/History.xaml", UriKind.Relative));
        }

        private void goto_about(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void goto_donate(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Donate.xaml", UriKind.Relative));
        }

        private void InitializeTimer()
        {
            _mainTimer = new DispatcherTimer();
            _mainTimer.Interval = TimeSpan.FromMilliseconds(100);
            _mainTimer.Tick += OnTimerTick;
        }

        private void playAlert(bool start = false)
        {
            string soundfile;

            if(start)
            {
                soundfile = "Assets/Sound/begin.wav";
                _starting = false;
            }
            else
            {
                switch (UserSettings.AlertType)
                {
                    case "Beep":
                        soundfile = "Assets/Sound/warn.wav";
                        break;

                    case "Voice":
                        if (SessionWorkout.IsRunning)
                        {
                            soundfile = "Assets/Sound/timetorun.wav";
                        }
                        else
                        {
                            soundfile = "Assets/Sound/timetowalk.wav";
                        }
                        break;

                    default:
                        soundfile = "Assets/Sound/warn.wav";
                        break;
                }
            }

            Stream stream = TitleContainer.OpenStream(soundfile);
            SoundEffect effect = SoundEffect.FromStream(stream);
            FrameworkDispatcher.Update();
            effect.Play();
        }

        private void setState(bool isRunning)
        {

            if (isRunning)
            {
                txtState.Text = "RUN";
                SessionWorkout.IsRunning = true;
                SessionWorkout.CurentIntervalDuration = UserSettings.RunInterval;
            }
            else
            {
                txtState.Text = "WALK";
                SessionWorkout.IsRunning = false;
                SessionWorkout.CurentIntervalDuration = UserSettings.WalkInterval;
            }

        }

        private void setButtonState(string btnstate)
        {

            if (btnstate == "stop")
            {
                foreach (var button in ApplicationBar.Buttons)
                {
                    if (((ApplicationBarIconButton)button).Text == "stop")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "start")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "pause")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }
                }
            }

            if (btnstate == "start")
            {
                foreach (var button in ApplicationBar.Buttons)
                {
                    if (((ApplicationBarIconButton)button).Text == "stop")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "start")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "pause")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }
                }
            }

            if (btnstate == "pause")
            {
                foreach (var button in ApplicationBar.Buttons)
                {
                    if (((ApplicationBarIconButton)button).Text == "pause")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = false; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "stop")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }

                    if (((ApplicationBarIconButton)button).Text == "start")
                    {
                        ((ApplicationBarIconButton)button).IsEnabled = true; // disables the button
                    }
                }

            }

            App.ButtonState = btnstate;

        }

        private void enableMenu(bool enabled)
        {
            ApplicationBar.IsMenuEnabled = enabled;
        }

        private void myMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "5aabb3ca-e645-4f6e-a7cc-669baebbe7fd";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "TaDJwHRsPJkH4FAbG1ZRsg";
        }

        private void rtpTimeGoal_ValueChanged(object sender, Telerik.Windows.Controls.ValueChangedEventArgs<object> args)
        {
            if(rtpTimeGoal.Value != null)
            {
                RWPDB.UserSetting us = _RWPDB.UserSettings.FirstOrDefault();
                us.GoalTimeInSeconds = rtpTimeGoal.Value.Value.TotalSeconds;
                UserSettings.TimeGoal = rtpTimeGoal.Value.Value.TotalSeconds;
                _RWPDB.SubmitChanges();
            }
        }

        private void rtpPaceGoal_ValueChanged(object sender, Telerik.Windows.Controls.ValueChangedEventArgs<object> args)
        {
            if(rtpPaceGoal.Value != null)
            {
                RWPDB.UserSetting us = _RWPDB.UserSettings.FirstOrDefault();
                us.GoalPaceInSeconds = rtpPaceGoal.Value.Value.TotalSeconds;
                UserSettings.PaceGoal = rtpPaceGoal.Value.Value.TotalSeconds;
                _RWPDB.SubmitChanges();
            }
        }

        private void txtTimeGoal_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtDistanceGoal.Text != null || txtDistanceGoal.Text != "")
            {
                RWPDB.UserSetting us = _RWPDB.UserSettings.FirstOrDefault();
                double goalDistance;
                double.TryParse(txtDistanceGoal.Text, out goalDistance);
                us.GoalDistance = goalDistance;
                UserSettings.DistanceGoal = goalDistance;
                _RWPDB.SubmitChanges();

            }
        }
        
        private void radTimeSpanPickerRun_ValueChanged(object sender, Telerik.Windows.Controls.ValueChangedEventArgs<object> args)
        {
            RWPDB.UserSetting _userSettings = new RWPDB.UserSetting();
            _userSettings = _RWPDB.UserSettings.FirstOrDefault();

            if (radTimeSpanPickerRun.Value.HasValue)
            {
                _userSettings.RunInterval = radTimeSpanPickerRun.Value.Value.TotalSeconds;
                UserSettings.RunInterval = radTimeSpanPickerRun.Value.Value.TotalSeconds;
                _RWPDB.SubmitChanges();
            }
        }

        private void radTimeSpanPickerWalk_ValueChanged(object sender, Telerik.Windows.Controls.ValueChangedEventArgs<object> args)
        {
            RWPDB.UserSetting _userSettings;
            _userSettings = _RWPDB.UserSettings.FirstOrDefault();

            if (radTimeSpanPickerWalk.Value.HasValue)
            {
                _userSettings.WalkInterval = radTimeSpanPickerWalk.Value.Value.TotalSeconds;
                UserSettings.WalkInterval = radTimeSpanPickerWalk.Value.Value.TotalSeconds;
                _RWPDB.SubmitChanges();
            }
        }
        
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if(App.ButtonState == "start")
            {
                MessageBoxResult result =
                MessageBox.Show("Using the phone back button will cause you to reset your workout data. Do you want to do that?", "Reset Workout", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    SaveWorkout();
                    return;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void ads1_Loaded(object sender, RoutedEventArgs e)
        {
            if(App.SHOWADS)
            {
                ads1.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ads1.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ads2_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.SHOWADS)
            {
                ads2.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ads2.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

    }

}