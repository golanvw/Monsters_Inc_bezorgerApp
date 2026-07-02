using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace Monsters_Inc_bezorgerApp;

public class DetailsPackageWrapper : INotifyPropertyChanged
{
    private bool isSelected;
    private string statusDisplay = "In bus";
    private bool isDelivered;
    private bool isFailed;

    public string TitleDisplay { get; set; } = string.Empty;
    public string CodeDisplay { get; set; } = string.Empty;

    public bool IsSelected
    {
        get => isSelected;
        set { isSelected = value; OnPropertyChanged(); }
    }
    public string StatusDisplay
    {
        get => statusDisplay;
        set { statusDisplay = value; OnPropertyChanged(); }
    }
    public bool IsDelivered
    {
        get => isDelivered;
        set { isDelivered = value; OnPropertyChanged(); }
    }
    public bool IsFailed
    {
        get => isFailed;
        set { isFailed = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public partial class DetailsStopsPage : ContentPage, INotifyPropertyChanged
{
    private readonly int stopNumber;
    private ObservableCollection<DetailsPackageWrapper> displayPackages = new();
    private DetailsPackageWrapper? selectedDisplayPackage;
    private DetailsPackageWrapper? scannedPackage;
    private string customFailureReason = string.Empty;
    private bool isProcessingScan;

    public DetailsStopsPage(int stopNumber) : this(stopNumber, new List<string>()) { }

    public DetailsStopsPage(int stopNumber, List<string> scannedCodes)
    {
        this.stopNumber = stopNumber;
        InitializeComponent();

        var stop = DeliverySession.GetStop(stopNumber)
            ?? throw new InvalidOperationException($"Stop {stopNumber} not found.");

        var stopPackagesList = stop.Packages.ToList();

        for (int i = 0; i < 3; i++)
        {
            string title = (stopPackagesList.Count > i) ? stopPackagesList[i].Title : $"Pakket {i + 1}";
            string actualCode = (scannedCodes != null && scannedCodes.Count > i) ? scannedCodes[i] : "8724934865376";

            displayPackages.Add(new DetailsPackageWrapper
            {
                TitleDisplay = $"{title}:",
                CodeDisplay = actualCode,
                StatusDisplay = "In bus"
            });
        }

        SelectedDisplayPackage = displayPackages.FirstOrDefault();
        OrderNumberDisplay = $"Order nummer: {stop.OrderNumber}";

        // STAP 1: Hardcoded de badge naar Stop 1/1 zetten
        StopBadgeText = "Stop 1/1";

        BindingContext = this;

        cameraBarcodeReaderView.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.Ean13,
            AutoRotate = true
        };

        FailureReasonsList.ItemsSource = DeliverySession.CommonFailureReasons;
    }

    public string OrderNumberDisplay { get; }
    public string StopBadgeText { get; }
    public string PopupPackageDisplay => ScannedPackage is null
        ? string.Empty
        : $"{ScannedPackage.TitleDisplay} {ScannedPackage.CodeDisplay}";

    public ObservableCollection<DetailsPackageWrapper> DisplayPackages => displayPackages;

    public DetailsPackageWrapper? SelectedDisplayPackage
    {
        get => selectedDisplayPackage;
        set
        {
            if (selectedDisplayPackage == value) return;
            selectedDisplayPackage = value;
            OnPropertyChangedCustom();
            UpdatePackageSelectionStates();
        }
    }

    public DetailsPackageWrapper? ScannedPackage
    {
        get => scannedPackage;
        set
        {
            if (scannedPackage == value) return;
            scannedPackage = value;
            OnPropertyChangedCustom();
            OnPropertyChangedCustom(nameof(PopupPackageDisplay));
        }
    }

    public string CustomFailureReason
    {
        get => customFailureReason;
        set
        {
            if (customFailureReason == value) return;
            customFailureReason = value;
            OnPropertyChangedCustom();
        }
    }

    private void OnScanTapped(object sender, TappedEventArgs e)
    {
        isProcessingScan = false;
        cameraBarcodeReaderView.IsDetecting = true;
        ScanPopupOverlay2.IsVisible = true;
    }

    private void OnPackageTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject bindable && bindable.BindingContext is DetailsPackageWrapper package)
        {
            SelectedDisplayPackage = package;
        }
    }

    private void OnPackageSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is DetailsPackageWrapper package)
        {
            SelectedDisplayPackage = package;
        }
    }

    private async void OnRouteTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RoutePage());
    }

    // STAP 2: Knop uitschakelen en een melding tonen dat de rit klaar is
    private async void OnNextStopTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Rit voltooid", "Er zijn geen volgende stops meer. Dit was de laatste stop!", "OK");
    }

    private void UpdatePackageSelectionStates()
    {
        foreach (var package in displayPackages)
        {
            package.IsSelected = package == SelectedDisplayPackage;
        }
    }

    protected void BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var first = e.Results?.FirstOrDefault();
        if (first is null || isProcessingScan) return;

        isProcessingScan = true;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            string scannedCode = first.Value;
            var package = displayPackages.FirstOrDefault(p => p.CodeDisplay == scannedCode);

            if (package == null)
            {
                await DisplayAlert("Fout", "Dit pakket hoort niet bij deze stop.", "OK");
                isProcessingScan = false;
                return;
            }

            cameraBarcodeReaderView.IsDetecting = false;

            ScannedPackage = package;
            SelectedDisplayPackage = package;
            ScanPopupOverlay2.IsVisible = false;
            ScanResultOverlay.IsVisible = true;
            isProcessingScan = false;
        });
    }

    private void OnDeliveredTapped(object sender, TappedEventArgs e)
    {
        if (ScannedPackage is null) return;
        ScannedPackage.StatusDisplay = "bezorgd";
        ScannedPackage.IsDelivered = true;
        CloseAllOverlays();
    }

    private void OnNotDeliveredTapped(object sender, TappedEventArgs e)
    {
        ScanResultOverlay.IsVisible = false;
        CustomFailureReason = string.Empty;
        FailureReasonOverlay.IsVisible = true;
    }

    private void OnFailureReasonSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not string reason || ScannedPackage is null) return;
        ApplyFailureStatus(reason);
        FailureReasonsList.SelectedItem = null;
    }

    private void OnSaveCustomReasonClicked(object sender, EventArgs e)
    {
        if (ScannedPackage is null) return;
        var reason = CustomFailureReason.Trim();
        if (string.IsNullOrWhiteSpace(reason))
        {
            DisplayAlert("Reden ontbreekt", "Vul een reden in.", "OK");
            return;
        }
        ApplyFailureStatus(reason);
    }

    private void ApplyFailureStatus(string reason)
    {
        if (ScannedPackage is null) return;
        ScannedPackage.StatusDisplay = $"niet bezorgd: {reason}";
        ScannedPackage.IsFailed = true;
        CloseAllOverlays();
    }

    private void CloseAllOverlays()
    {
        ScanResultOverlay.IsVisible = false;
        FailureReasonOverlay.IsVisible = false;
        ScanPopupOverlay2.IsVisible = false;
        cameraBarcodeReaderView.IsDetecting = false;
        ScannedPackage = null;
        CustomFailureReason = string.Empty;
    }

    private void OnCancelFailureReasonClicked(object sender, EventArgs e)
    {
        FailureReasonOverlay.IsVisible = false;
        ScanResultOverlay.IsVisible = true;
    }

    private void CancelScanBtn_Clicked(object sender, EventArgs e)
    {
        CloseAllOverlays();
        isProcessingScan = false;
    }

    protected void OnPropertyChangedCustom([CallerMemberName] string propertyName = "")
    {
        base.OnPropertyChanged(propertyName);
    }
}