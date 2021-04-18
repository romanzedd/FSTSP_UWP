using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace FSTSP_UWP.ViewModels
{
    public class SettingsPageViewModel : ObservableObject
    {
        public ICommand SaveCommand { get; set; }

        private int _traffic = 0;

        public int Traffic
        {
            get => _traffic;
            set => SetProperty(ref _traffic, value);
        }

        private bool _deliveryInterval = false;

        public bool DeliveryInterval
        {
            get => _deliveryInterval;
            set
            {
                SetProperty(ref _deliveryInterval, value);
            }
        }

        private int _temperature = 0;

        public int Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private int _percipitation = 0;

        public int Percipitation
        {
            get => _percipitation;
            set => SetProperty(ref _percipitation, value);
        }

        private string _percipitationType = null;

        public string PercipitationType
        {
            get => _percipitationType;
            set => SetProperty(ref _percipitationType, value);
        }

        private int _wind = 0;

        public int Wind
        {
            get => _wind;
            set => SetProperty(ref _wind, value);
        }

        private int _geoIndex;

        public int GeoIndex
        {
            get => _geoIndex;
            set => SetProperty(ref _geoIndex, value);
        }

        public SettingsPageViewModel()
        {
            SaveCommand = new RelayCommand(new Action(OnSave));

            OnLoad();
        }

        public void OnLoad()
        {
            Traffic = Settings.TrafficScore;
            DeliveryInterval = Settings.DeliveryInterval;
            Temperature = Settings.Temperature;
            Percipitation = Settings.PrecipitationVolume;
            PercipitationType = Settings.PrecipitationType;
            Wind = Settings.Wind;
            GeoIndex = Settings.GAIndex;
        }

        public void OnSave()
        {
            Settings.TrafficScore = Traffic;
            Settings.DeliveryInterval = DeliveryInterval;
            Settings.Temperature = Temperature;
            Settings.PrecipitationVolume = Percipitation;
            Settings.PrecipitationType = PercipitationType;
            Settings.Wind = Wind;
            Settings.GAIndex = GeoIndex;
        }
    }
}
