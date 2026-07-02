using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace Monsters_Inc_bezorgerApp;

public class ScannedPackageItem : INotifyPropertyChanged
{
    private bool isInBus;
    public string Code { get; set; } = string.Empty;

    public bool IsInBus
    {
        get => isInBus;
        set
        {
            isInBus = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(StatusColor));
        }
    }

    public string StatusText => IsInBus ? "In bus" : "Toegevoegd";
    public Color StatusColor => IsInBus ? Colors.LightGreen : Colors.Khaki;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class StopsPage : ContentPage, INotifyPropertyChanged
{
    private bool isProcessingScan;
    private bool isContinueButtonVisible;

    public ObservableCollection<ScannedPackageItem> ScannedPackagesList { get; set; } = new();

    public bool IsContinueButtonVisible
    {
        get => isContinueButtonVisible;
        set
        {
            if (isContinueButtonVisible == value) return;
            isContinueButtonVisible = value;
            OnPropertyChangedCustom();
            OnPropertyChangedCustom(nameof(IsScanButtonVisible));
        }
    }

    public bool IsScanButtonVisible => !IsContinueButtonVisible;

    public StopsPage()
    {
        InitializeComponent();
        PackagesListView.ItemsSource = ScannedPackagesList;
        BindingContext = this;

        cameraBarcodeReaderView.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.Ean13,
            AutoRotate = true
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateVanProgress();
    }

    private void UpdateVanProgress()
    {
        int inBusCount = ScannedPackagesList.Count(p => p.IsInBus);
        VanProgressLabel.Text = $"{inBusCount} pakketten in bus";
        IsContinueButtonVisible = inBusCount >= 3;
    }

    private void OnScanVanTapped(object sender, TappedEventArgs e)
    {
        isProcessingScan = false;
        ScanPopupOverlay.IsVisible = true;
    }

    private void CancelScanBtn_Clicked(object sender, EventArgs e)
    {
        ScanPopupOverlay.IsVisible = false;
        isProcessingScan = false;
    }

    // Nu alleen nog maar Stop 1!
    private async void OnStop1Tapped(object sender, TappedEventArgs e) => await NavigateToStop(1);

    protected void BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var first = e.Results?.FirstOrDefault();
        if (first is null || isProcessingScan) return;

        isProcessingScan = true;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string scannedCode = first.Value;
            var existingItem = ScannedPackagesList.FirstOrDefault(p => p.Code == scannedCode);

            if (existingItem == null)
            {
                ScannedPackagesList.Add(new ScannedPackageItem { Code = scannedCode, IsInBus = false });
                UpdateVanProgress();
                await DisplayAlert("Gevonden", $"Pakket {scannedCode} toegevoegd. Scan nogmaals voor de bus.", "OK");
            }
            else if (!existingItem.IsInBus)
            {
                existingItem.IsInBus = true;
                ScanPopupOverlay.IsVisible = false;
                UpdateVanProgress();

                var loadedCodes = ScannedPackagesList.Where(p => p.IsInBus).Select(p => p.Code).ToList();
                if (loadedCodes.Count >= 3)
                {
                    await DisplayAlert("Bus is vol!", "Alle 3 pakketten zijn ingeladen. We kunnen starten.", "OK");
                }
                else
                {
                    await DisplayAlert("In bus", $"Pakket {scannedCode} geladen!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Let op", "Dit pakket zit al in de bus!", "OK");
            }

            await Task.Delay(1500);
            isProcessingScan = false;
        });
    }

    private async Task NavigateToStop(int stopNumber)
    {
        var loadedCodes = ScannedPackagesList.Where(p => p.IsInBus).Select(p => p.Code).ToList();

        if (loadedCodes.Count < 3)
        {
            await DisplayAlert("Niet compleet", "Scan eerst alle 3 pakketten in de bus.", "OK");
            return;
        }

        await Navigation.PushAsync(new DetailsStopsPage(stopNumber, loadedCodes));
    }

    protected void OnPropertyChangedCustom([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
    }
}