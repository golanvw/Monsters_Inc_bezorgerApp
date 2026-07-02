namespace Monsters_Inc_bezorgerApp;

public sealed class DeliveryStop
{
	public DeliveryStop(int stopNumber, string address, string orderNumber, IReadOnlyList<StopPackage> packages)
	{
		StopNumber = stopNumber;
		Address = address;
		OrderNumber = orderNumber;
		Packages = packages;
	}

	public int StopNumber { get; }

	public string Address { get; }

	public string OrderNumber { get; }

	public IReadOnlyList<StopPackage> Packages { get; }

	public int PackageCount => Packages.Count;

	public string StopBadgeText(int totalStops) => $"Stop {StopNumber}/{totalStops}";
}
