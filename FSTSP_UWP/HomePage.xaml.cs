using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FSTSP_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        ViewModel viewModel = new ViewModel();

        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void RunFstsp(object sender, RoutedEventArgs e)
        {
            ToggleLoading(true);

            var result = string.Empty;


                result = await viewModel.generateSpace((int)this.areaSize.Value);


            this.outputPanel.Text += result;

            result = viewModel.runFSTSP((int)this.areaSize.Value, (int)this.numberOfCustomers.Value);
            this.outputPanel.Text += "\n" + result;

            ToggleLoading(false);
        }

        private void RunTsp(object sender, RoutedEventArgs e)
        {
            ToggleLoading(true);
            outputPanel.Text = $"{outputPanel.Text}\nnew lint";
            ToggleLoading(false);
        }

        private void ToggleLoading(bool value)
        {
            progressRing.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            runFstspBtn.IsEnabled = !value;
            runTspBtn.IsEnabled = !value;

        }
    }
}
