using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FSTSP_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public SettingsPage()
        {
            this.InitializeComponent();

            traffic.Value = (double)(localSettings.Values[SettingsKeys.TrafficKey] ?? Convert.ToDouble(0));
            deliveryInterval.IsChecked = (bool)(localSettings.Values[SettingsKeys.DeliveryIntervalKey] ?? false);
            temperature.Value = (double)(localSettings.Values[SettingsKeys.TemperatureKey] ?? Convert.ToDouble(0));
            percipitation.Value = (double)(localSettings.Values[SettingsKeys.PercipitationKey] ?? Convert.ToDouble(0));
            percipitationType.SelectedValue = (string)(localSettings.Values[SettingsKeys.PercipitationTypeKey] ?? string.Empty);
            wind.Value = (double)(localSettings.Values[SettingsKeys.WindKey] ?? Convert.ToDouble(0));
            geoIndex.Value = (double)(localSettings.Values[SettingsKeys.GeoIndexKey] ?? Convert.ToDouble(0));
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            localSettings.Values[SettingsKeys.TrafficKey] = traffic.Value;
            localSettings.Values[SettingsKeys.DeliveryIntervalKey] = deliveryInterval.IsChecked;
            localSettings.Values[SettingsKeys.TemperatureKey] = temperature.Value;
            localSettings.Values[SettingsKeys.PercipitationKey] = percipitation.Value;
            localSettings.Values[SettingsKeys.PercipitationTypeKey] = percipitationType.SelectedValue;
            localSettings.Values[SettingsKeys.WindKey] = wind.Value;
            localSettings.Values[SettingsKeys.GeoIndexKey] = geoIndex.Value;

            Settings.TrafficScore = (int)traffic.Value;
            Settings.DeliveryInterval = (bool)deliveryInterval.IsChecked;
            Settings.Temperature = (int)temperature.Value;
            Settings.PrecipitationVolume = (int)percipitation.Value;
            Settings.PrecipitationType = percipitationType.SelectedValue.ToString();
            Settings.Wind = (int)wind.Value;
            Settings.GAIndex = (int)geoIndex.Value;

        }
    }
}
