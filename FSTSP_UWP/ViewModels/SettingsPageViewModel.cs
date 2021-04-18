using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;
using Windows.Storage;

namespace FSTSP_UWP.ViewModels
{
    public class SettingsPageViewModel : ObservableObject
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

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
            Traffic = (int?)localSettings.Values[SettingsKeys.TrafficKey] ?? 0;
            DeliveryInterval = (bool?)localSettings.Values[SettingsKeys.DeliveryIntervalKey] ?? false;
            Temperature = (int?)localSettings.Values[SettingsKeys.TemperatureKey] ?? 0;
            Percipitation = (int?)localSettings.Values[SettingsKeys.PercipitationKey] ?? 0;
            PercipitationType = (string)localSettings.Values[SettingsKeys.PercipitationTypeKey] ?? string.Empty;
            Wind = (int?)localSettings.Values[SettingsKeys.WindKey] ?? 0;
            GeoIndex = (int?)localSettings.Values[SettingsKeys.GeoIndexKey] ?? 0;
        }

        public void OnSave()
        {
            localSettings.Values[SettingsKeys.TrafficKey] = Traffic;
            localSettings.Values[SettingsKeys.DeliveryIntervalKey] = DeliveryInterval;
            localSettings.Values[SettingsKeys.TemperatureKey] = Temperature;
            localSettings.Values[SettingsKeys.PercipitationKey] = Percipitation;
            localSettings.Values[SettingsKeys.PercipitationTypeKey] = PercipitationType;
            localSettings.Values[SettingsKeys.WindKey] = Wind;
            localSettings.Values[SettingsKeys.GeoIndexKey] = GeoIndex;
        }
    }
}
