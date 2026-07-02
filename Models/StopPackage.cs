using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Monsters_Inc_bezorgerApp;

public sealed class StopPackage : INotifyPropertyChanged
{
	private string statusDisplay;
	private bool isSelected;

	public StopPackage(string title, string packageNumber, string status, int stopIndex)
	{
		Title = title;
		PackageNumber = packageNumber;
		StopIndex = stopIndex;
		statusDisplay = FormatStatus(status);
	}

	public string Title { get; }

	public string PackageNumber { get; }

	public int StopIndex { get; }

	public bool IsLoadedInVan { get; set; }

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
			OnPropertyChanged();
			OnPropertyChanged(nameof(IsDelivered));
			OnPropertyChanged(nameof(IsFailed));
			OnPropertyChanged(nameof(IsPending));
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
			OnPropertyChanged();
		}
	}

	public bool IsDelivered => StatusDisplay.Contains("bezorgd", StringComparison.OrdinalIgnoreCase)
		&& !StatusDisplay.Contains("niet bezorgd", StringComparison.OrdinalIgnoreCase);

	public bool IsFailed => StatusDisplay.Contains("niet bezorgd", StringComparison.OrdinalIgnoreCase);

	public bool IsPending => StatusDisplay.Contains("afwachting", StringComparison.OrdinalIgnoreCase);

	public static string FormatStatus(string status) =>
		status.StartsWith("Status :", StringComparison.OrdinalIgnoreCase)
			? status
			: $"Status : {status}";

	public event PropertyChangedEventHandler? PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
