using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace RunWalkProV2.Helpers
{

    class RWParams
    {
        IsolatedStorageSettings _settings = IsolatedStorageSettings.ApplicationSettings;

        double _currentDistance;
        public double CurrentDistance
        {
            get
            {
                if (_settings.Contains("DistanceOfLastWorkout"))
                {

                    double num = 0.0;

                    if (double.TryParse(_settings["DistanceOfLastWorkout"].ToString(), out num))
                    {
                        this._currentDistance = (double)_settings["DistanceOfLastWorkout"];
                    }
                    else
                    {
                        this._currentDistance = 0.0;
                    }
                    
                }
                else
                {
                    this._currentDistance = 0.0;
                    _settings.Add("DistanceOfLastWorkout", Convert.ToDouble(_currentDistance));
                }
                return _currentDistance;
            }
            set
            {
                this._currentDistance = value;
                _settings["DistanceOfLastWorkout"] = Convert.ToDouble(value);
                _settings.Save();
            }
        }

        double _totalCurrentTimeInSeconds;
        public double TotalCurrentTimeInSeconds
        {
            get
            {
                if (_settings.Contains("TotalCurrentTimeInSeconds"))
                {
                    this._totalCurrentTimeInSeconds = (double)_settings["TotalCurrentTimeInSeconds"];
                }
                else
                {
                    _settings.Add("TotalCurrentTimeInSeconds", _totalCurrentTimeInSeconds);
                }
                return _totalCurrentTimeInSeconds;
            }
            set
            {
                this._totalCurrentTimeInSeconds = value;
                _settings["TotalCurrentTimeInSeconds"] = value;
                _settings.Save();
            }
        }

        int _totalCurrentCalories;
        public int TotalCurrentCalories
        {

            get
            {
                if (_settings.Contains("TotalCaloriesOfLastWorkout"))
                {

                    int num = 0;

                    if (int.TryParse(_settings["TotalCaloriesOfLastWorkout"].ToString(), out num))
                    {
                        this._totalCurrentCalories = (int)_settings["TotalCaloriesOfLastWorkout"];
                    }
                    else
                    {
                        this._totalCurrentCalories = 0;
                    }
                    
                }
                else
                {
                    _settings.Add("TotalCaloriesOfLastWorkout", _totalCurrentTimeInSeconds);
                }
                return _totalCurrentCalories;
            }
            set
            {
                int num;
                bool isNum = int.TryParse(value.ToString(), out num);

                if (isNum)
                {
                    this._totalCurrentCalories = value;
                    _settings["TotalCaloriesOfLastWorkout"] = value;
                }
                else
                {
                    this._totalCurrentCalories = 0;
                    _settings["TotalCaloriesOfLastWorkout"] = 0;
                }

                _settings.Save();
            }
        }

    }
}
