using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunWalkProV2.Models.Data
{
    public class RWPDB
    {
        public class RWPDataContext : DataContext
        {
            // Specify the connection string as a static, used in main page and app.xaml.
            public static string DBConnectionString = "Data Source=isostore:/RWPDB.sdf";

            // Pass the connection string to the base class.
            public RWPDataContext(string connectionString)
                : base(connectionString)
            { }

            
            public Table<RunHistory> RunHistoryItems;

            public Table<UserSetting> UserSettings;
        }

        [Table]
        public class RunHistory : INotifyPropertyChanged, INotifyPropertyChanging
        {
            // Define ID: private field, public property and database column.
            private int _historyItemId;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int HistoryItemId
            {
                get
                {
                    return _historyItemId;
                }
                set
                {
                    if (_historyItemId != value)
                    {
                        NotifyPropertyChanging("HistoryItemId");
                        _historyItemId = value;
                        NotifyPropertyChanged("HistoryItemId");
                    }
                }
            }

            private DateTime _itemDate;
            [Column]
            public DateTime ItemDate
            {
                get
                {
                    return _itemDate;
                }
                set
                {
                    if (_itemDate != value)
                    {
                        NotifyPropertyChanging("ItemDate");
                        _itemDate = value;
                        NotifyPropertyChanged("ItemDate");
                    }
                }
            }

            private double _distance;
            [Column]
            public double Distance
            {
                get
                {
                    return _distance;
                }
                set
                {
                    if (_distance != value)
                    {
                        NotifyPropertyChanging("Distance");
                        _distance = value;
                        NotifyPropertyChanged("Distance");
                    }
                }
            }

            private double _time;
            [Column]
            public double Time
            {
                get
                {
                    return _time;
                }
                set
                {
                    if (_time != value)
                    {
                        NotifyPropertyChanging("Time");
                        _time = value;
                        NotifyPropertyChanged("Time");
                    }
                }
            }

            private string _runType;
            [Column]
            public string RunType
            {
                get
                {
                    return _runType;
                }
                set
                {
                    if (_runType != value)
                    {
                        NotifyPropertyChanging("RunType");
                        _runType = value;
                        NotifyPropertyChanged("RunType");
                    }
                }
            }

            private double _temp;
            [Column]
            public double Temp
            {
                get
                {
                    return _temp;
                }
                set
                {
                    if (_temp != value)
                    {
                        NotifyPropertyChanging("Temp");
                        _temp = value;
                        NotifyPropertyChanged("Temp");
                    }
                }
            }

            private string _note;
            [Column]
            public string Note
            {
                get
                {
                    return _note;
                }
                set
                {
                    if (_note != value)
                    {
                        NotifyPropertyChanging("Note");
                        _note = value;
                        NotifyPropertyChanged("Note");
                    }
                }
            }

            private string _title;
            [Column]
            public string Title
            {
                get
                {
                    return _title;
                }
                set
                {
                    if (_title != value)
                    {
                        NotifyPropertyChanging("Title");
                        _title = value;
                        NotifyPropertyChanged("Title");
                    }
                }
            }

            private string _felt;
            [Column]
            public string Felt
            {
                get
                {
                    return _felt;
                }
                set
                {
                    if (_felt != value)
                    {
                        NotifyPropertyChanging("Felt");
                        _felt = value;
                        NotifyPropertyChanged("Felt");
                    }
                }
            }

            private string _unit;
            [Column]
            public string Unit
            {
                get
                {
                    return _unit;
                }
                set
                {
                    if (_unit != value)
                    {
                        NotifyPropertyChanging("Unit");
                        _unit = value;
                        NotifyPropertyChanged("Unit");
                    }
                }
            }

            private string _pace;
            [Column]
            public string Pace
            {
                get
                {
                    return _pace;
                }
                set
                {
                    if (_pace != value)
                    {
                        NotifyPropertyChanging("Pace");
                        _pace = value;
                        NotifyPropertyChanged("Pace");
                    }
                }
            }

            private string _timeForDisplay;
            [Column]
            public string TimeForDisplay
            {
                get
                {
                    return _timeForDisplay;
                }
                set
                {
                    if (_timeForDisplay != value)
                    {
                        NotifyPropertyChanging("TimeForDisplay");
                        _timeForDisplay = value;
                        NotifyPropertyChanged("TimeForDisplay");
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            // Used to notify the page that a data context property changed
            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanging Members

            public event PropertyChangingEventHandler PropertyChanging;

            // Used to notify the data context that a data context property is about to change
            private void NotifyPropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            #endregion

        }

        [Table]
        public class UserSetting : INotifyPropertyChanged, INotifyPropertyChanging
        {
            private int _userSettingItemId;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
            public int UserSettingItemId
            {
                get
                {
                    return _userSettingItemId;
                }
                set
                {
                    if (_userSettingItemId != value)
                    {
                        NotifyPropertyChanging("UserSettingItemId");
                        _userSettingItemId = value;
                        NotifyPropertyChanged("UserSettingItemId");
                    }
                }
            }

            private int _caloriesPerUnit;
            [Column]
            public int CaloriesPerUnit
            {
                get
                {
                    return _caloriesPerUnit;
                }
                set
                {
                    if (_caloriesPerUnit != value)
                    {
                        NotifyPropertyChanging("CaloriesPerUnit");
                        _caloriesPerUnit = value;
                        NotifyPropertyChanged("CaloriesPerUnit");
                    }
                }
            }

            private double _walkInterval;
            [Column]
            public double WalkInterval
            {
                get
                {
                    return _walkInterval;
                }
                set
                {
                    if (_walkInterval != value)
                    {
                        NotifyPropertyChanging("WalkInterval");
                        _walkInterval = value;
                        NotifyPropertyChanged("WalkInterval");
                    }
                }
            }

            private double _runInterval;
            [Column]
            public double RunInterval
            {
                get
                {
                    return _runInterval;
                }
                set
                {
                    if (_runInterval != value)
                    {
                        NotifyPropertyChanging("RunInterval");
                        _runInterval = value;
                        NotifyPropertyChanged("RunInterval");
                    }
                }
            }

            private string _unitOfMeasure;
            [Column]
            public string UnitOfMeasure
            {
                get
                {
                    return _unitOfMeasure;
                }
                set
                {
                    if (_unitOfMeasure != value)
                    {
                        NotifyPropertyChanging("UnitOfMeasure");
                        _unitOfMeasure = value;
                        NotifyPropertyChanged("UnitOfMeasure");
                    }
                }
            }

            private string _startWith;
            [Column]
            public string StartWith
            {
                get
                {
                    return _startWith;
                }
                set
                {
                    if (_startWith != value)
                    {
                        NotifyPropertyChanging("StartWith");
                        _startWith = value;
                        NotifyPropertyChanged("StartWith");
                    }
                }
            }

            private string _alertType;
            [Column]
            public string AlertType
            {
                get
                {
                    return _alertType;
                }
                set
                {
                    if (_alertType != value)
                    {
                        NotifyPropertyChanging("AlertType");
                        _alertType = value;
                        NotifyPropertyChanged("AlertType");
                    }
                }
            }

            private bool _locationConsent;
            [Column]
            public bool LocationConsent
            {
                get
                {
                    return _locationConsent;
                }
                set
                {
                    if (_locationConsent != value)
                    {
                        NotifyPropertyChanging("LocationConsent");
                        _locationConsent = value;
                        NotifyPropertyChanged("LocationConsent");
                    }
                }
            }

            private string _runWalkPROUserName;
            [Column]
            public string RunWalkPROUserName
            {
                get
                {
                    return _runWalkPROUserName;
                }
                set
                {
                    if (_runWalkPROUserName != value)
                    {
                        NotifyPropertyChanging("RunWalkPROUserName");
                        _runWalkPROUserName = value;
                        NotifyPropertyChanged("RunWalkPROUserName");
                    }
                }
            }

            private string _runWalkPROToken;
            [Column]
            public string RunWalkPROToken
            {
                get
                {
                    return _runWalkPROToken;
                }
                set
                {
                    if (_runWalkPROToken != value)
                    {
                        NotifyPropertyChanging("RunWalkPROToken");
                        _runWalkPROToken = value;
                        NotifyPropertyChanged("RunWalkPROToken");
                    }
                }
            }

            private double _goalTimeInSeconds;
            [Column]
            public double GoalTimeInSeconds
            {
                get
                {
                    return _goalTimeInSeconds;
                }
                set
                {
                    if (_goalTimeInSeconds != value)
                    {
                        NotifyPropertyChanging("GoalTimeInSeconds");
                        _goalTimeInSeconds = value;
                        NotifyPropertyChanged("GoalTimeInSeconds");
                    }
                }
            }

            private double _goalPaceInSeconds;
            [Column]
            public double GoalPaceInSeconds
            {
                get
                {
                    return _goalPaceInSeconds;
                }
                set
                {
                    if (_goalPaceInSeconds != value)
                    {
                        NotifyPropertyChanging("GoalPaceInSeconds");
                        _goalPaceInSeconds = value;
                        NotifyPropertyChanged("GoalPaceInSeconds");
                    }
                }
            }

            private double _goalDistance;
            [Column]
            public double GoalDistance
            {
                get
                {
                    return _goalDistance;
                }
                set
                {
                    if (_goalDistance != value)
                    {
                        NotifyPropertyChanging("GoalDistance");
                        _goalDistance = value;
                        NotifyPropertyChanged("GoalDistance");
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            // Used to notify the page that a data context property changed
            private void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion

            #region INotifyPropertyChanging Members

            public event PropertyChangingEventHandler PropertyChanging;

            // Used to notify the data context that a data context property is about to change
            private void NotifyPropertyChanging(string propertyName)
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
                }
            }

            #endregion

        }

    }
}
