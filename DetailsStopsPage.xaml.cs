using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Monsters_Inc_bezorgerApp;

public partial class DetailsStopsPage : ContentPage, INotifyPropertyChanged
{
	private readonly ObservableCollection<StopPackage> packages;
	private StopPackage? selectedPackage;
	private bool isAwaitingSelected;
	private bool isDeliveredSelected;
	private bool isOtherSelected;
	private string statusNotes = string.Empty;

	public DetailsStopsPage()
	{
		InitializeComponent();
		packages = new ObservableCollection<StopPackage>
		{
			new StopPackage("Pakket 1:", "249348", "in afwachting"),
			new StopPackage("Pakket 2:", "437682", "in afwachting")
		};
		SelectedPackage = packages[0];
		BindingContext = this;
	}

	public string OrderNumberDisplay => "Order nummer: 24679PBE";

	public string StopBadgeText => "Stop 1/6";

	public string PopupPackageDisplay => SelectedPackage is null
		? string.Empty
		: $"{SelectedPackage.Title} {SelectedPackage.PackageNumber}";

	public ObservableCollection<StopPackage> Packages => packages;

	public StopPackage? SelectedPackage
	{
		get => selectedPackage;
		set
		{
			if (selectedPackage == value)
			{
				return;
			}

			selectedPackage = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(PopupPackageDisplay));
			ApplySelectionFromPackage();
			UpdatePackageSelectionStates();
		}
	}

	public bool IsAwaitingSelected
	{
		get => isAwaitingSelected;
		set
		{
			if (isAwaitingSelected == value)
			{
				return;
			}

			isAwaitingSelected = value;
			OnPropertyChanged();
		}
	}

	public bool IsDeliveredSelected
	{
		get => isDeliveredSelected;
		set
		{
			if (isDeliveredSelected == value)
			{
				return;
			}

			isDeliveredSelected = value;
			OnPropertyChanged();
		}
	}

	public bool IsOtherSelected
	{
		get => isOtherSelected;
		set
		{
			if (isOtherSelected == value)
			{
				return;
			}

			isOtherSelected = value;
			OnPropertyChanged();
		}
	}

	public string StatusNotes
	{
		get => statusNotes;
		set
		{
			if (statusNotes == value)
			{
				return;
			}

			statusNotes = value;
			OnPropertyChanged();
		}
	}

	private void OnScanTapped(object sender, TappedEventArgs e)
	{
		ApplySelectionFromPackage();
		ScanPopupOverlay.IsVisible = true;
	}

	private void OnPackageTapped(object sender, TappedEventArgs e)
	{
		if (sender is BindableObject bindable && bindable.BindingContext is StopPackage package)
		{
			SelectedPackage = package;
		}
	}

	private void OnPackageSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is StopPackage package)
		{
			SelectedPackage = package;
		}
	}

	private void OnStatusSelectionChanged(object sender, CheckedChangedEventArgs e)
	{
		if (!e.Value)
		{
			return;
		}

		if (sender is RadioButton radioButton)
		{
			IsOtherSelected = string.Equals(radioButton.Content?.ToString(), "Overig", StringComparison.OrdinalIgnoreCase);
			if (IsOtherSelected)
			{
				StatusNotes = string.Empty;
			}
		}
	}

	private async void OnSaveStatusClicked(object sender, EventArgs e)
	{
		if (SelectedPackage is not null)
		{
			SelectedPackage.StatusDisplay = IsDeliveredSelected
				? "Status : bezorgd"
				: IsOtherSelected
					? $"Status : {StatusNotes}".TrimEnd()
					: "Status : in afwachting";

			OnPropertyChanged(nameof(PopupPackageDisplay));
		}

		ScanPopupOverlay.IsVisible = false;
		await Task.CompletedTask;
	}

	private async void OnRouteTapped(object sender, TappedEventArgs e)
	{
		await Navigation.PushAsync(new RoutePage());
	}

	private void ApplySelectionFromPackage()
	{
		if (SelectedPackage is null)
		{
			IsAwaitingSelected = true;
			IsDeliveredSelected = false;
			IsOtherSelected = false;
			StatusNotes = string.Empty;
			return;
		}

		IsAwaitingSelected = SelectedPackage.StatusDisplay.Contains("afwachting", StringComparison.OrdinalIgnoreCase);
		IsDeliveredSelected = SelectedPackage.StatusDisplay.Contains("bezorgd", StringComparison.OrdinalIgnoreCase);
		IsOtherSelected = !IsAwaitingSelected && !IsDeliveredSelected;
		StatusNotes = IsOtherSelected
			? SelectedPackage.StatusDisplay.Replace("Status :", string.Empty).Trim()
			: string.Empty;
	}

	private void UpdatePackageSelectionStates()
	{
		foreach (var package in packages)
		{
			package.IsSelected = package == SelectedPackage;
		}
	}

	public new event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public sealed class StopPackage : INotifyPropertyChanged
	{
		private string statusDisplay;
		private bool isSelected;

		public StopPackage(string title, string packageNumber, string status)
		{
			Title = title;
			PackageNumber = packageNumber;
			statusDisplay = $"Status : {status}";
		}

		public string Title { get; }

		public string PackageNumber { get; }

		public string StatusDisplay
		{
			get => statusDisplay;
			set
			{
				if (statusDisplay == value)
				{
					return;
				}

				statusDisplay = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatusDisplay)));
			}
		}

		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (isSelected == value)
				{
					return;
				}

				isSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}

}