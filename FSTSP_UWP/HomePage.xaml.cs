using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FSTSP_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        ViewModel viewModel = new ViewModel();
        Windows.Storage.StorageFile file = null;

        public HomePage()
        {
            this.InitializeComponent();
        }

        private async void RunFstsp(object sender, RoutedEventArgs e)
        {
            ToggleLoading(true);

            var result = string.Empty;
            this.outputPanel.Text = result;

            result = await viewModel.generateSpace((int)this.areaSize.Value);


            this.outputPanel.Text += result;

            result = viewModel.runFSTSP((int)this.areaSize.Value, (int)this.numberOfCustomers.Value);
            Log(result);
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

        private async void SelectFile(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".xml");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                this.file = file;
                string text = await Windows.Storage.FileIO.ReadTextAsync(file);
                fileSelectorTitle.Text = file.Name;
                return;
                //Log(text);
            }
            fileSelectorTitle.Text = "No file selected";
        }

        private void Log(string s)
        {
            this.outputPanel.Text += $"\n{s}";
        }
    }
}
