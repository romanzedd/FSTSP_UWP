using Windows.Storage;

namespace FSTSP_UWP
{
    public static class Settings
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static int TrafficScore
        {
            get { return (int?)localSettings.Values[SettingsKeys.TrafficKey] ?? 0; }
            set { localSettings.Values[SettingsKeys.TrafficKey] = value; }
        }

        public static bool DeliveryInterval
        {
            get { return (bool?)localSettings.Values[SettingsKeys.DeliveryIntervalKey] ?? false; }
            set { localSettings.Values[SettingsKeys.DeliveryIntervalKey] = value; }
        }

        public static int Temperature
        {
            get { return (int?)localSettings.Values[SettingsKeys.TemperatureKey] ?? 0; }
            set { localSettings.Values[SettingsKeys.TemperatureKey] = value; }
        }

        public static int PrecipitationVolume
        {
            get { return (int?)localSettings.Values[SettingsKeys.PrecipitationKey] ?? 0; }
            set { localSettings.Values[SettingsKeys.PrecipitationKey] = value; }
        }

        public static string PrecipitationType
        {
            get { return (string)localSettings.Values[SettingsKeys.PrecipitationTypeKey] ?? string.Empty; }
            set { localSettings.Values[SettingsKeys.PrecipitationTypeKey] = value; }
        }

        public static int Wind
        {
            get { return (int?)localSettings.Values[SettingsKeys.WindKey] ?? 0; }
            set { localSettings.Values[SettingsKeys.WindKey] = value; }
        }

        public static int GAIndex
        {
            get { return (int?)localSettings.Values[SettingsKeys.GeoIndexKey] ?? 0; }
            set { localSettings.Values[SettingsKeys.GeoIndexKey] = value; }
        }
    }
}
