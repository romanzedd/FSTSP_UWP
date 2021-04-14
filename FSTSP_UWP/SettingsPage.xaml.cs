using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FSTSP_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage: Page
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

        }
    }
}
