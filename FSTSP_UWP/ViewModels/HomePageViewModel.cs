using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace FSTSP_UWP.ViewModels
{
    public class HomePageViewModel : ObservableObject
    {
        public ViewModel BusinessLogic = new ViewModel();

        public IAsyncRelayCommand RunFSTSP { get; set; }
        public IAsyncRelayCommand RunTSP { get; set; }

        private int _area = 0;

        public int Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

        private int _customers = 0;

        public int Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private StorageFile _selectedFile;

        public StorageFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                SetProperty(ref _selectedFile, value);
                OnPropertyChanged(nameof(SelectedFileTitle));
            }
        }
        public string SelectedFileTitle
        {
            get => SelectedFile != null ? SelectedFile.Name : "No file selected";
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
                OnPropertyChanged(nameof(IsLoadingValue));
            }
        }
        public Visibility IsLoadingValue
        {
            get => IsLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private string _log = "";
        public string Log
        {
            get => _log;
            set => SetProperty(ref _log, value);
        }


        public HomePageViewModel()
        {
            RunFSTSP = new AsyncRelayCommand(new Func<Task>(OnRunFstsp), new Func<bool>(OnCanRun));
            RunTSP = new AsyncRelayCommand(new Func<Task>(OnRunTsp), new Func<bool>(OnCanRun));
        }

        private async Task OnRunFstsp()
        {
            IsLoading = true;

            var result = string.Empty;
            ResetLog();

            result = await BusinessLogic.generateSpace(Area);
            LogResult(result);

            result = BusinessLogic.runFSTSP(Area, Customers);
            LogResult(result);

            IsLoading = false;
        }

        private async Task OnRunTsp()
        {
            IsLoading = true;

            var result = string.Empty;
            ResetLog();

            result = await BusinessLogic.generateSpace(Area);
            LogResult(result);

            result = BusinessLogic.runFSTSPnoDrones(Area, Customers);
            LogResult(result);

            IsLoading = false;
        }
        //private async Task OnRunTsp()
        //{
        //    IsLoading = true;

        //    var result = string.Empty;
        //    ResetLog();

        //    result = await BusinessLogic.generateSpace(Area);
        //    LogResult(result);

        //    result = BusinessLogic.runTSP(Area, Customers);
        //    LogResult(result);

        //    IsLoading = false;
        //}

        private bool OnCanRun()
        {
            return !IsLoading;
        }

        private void LogResult(string s)
        {
            Log += $"\n{s}";
        }

        private void ResetLog()
        {
            Log = "";
        }
    }
}
