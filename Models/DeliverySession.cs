using System.Collections.ObjectModel;

namespace Monsters_Inc_bezorgerApp;

public static class DeliverySession
{
	private static readonly List<DeliveryStop> stops;
	private static readonly Dictionary<string, StopPackage> packagesByBarcode;

	static DeliverySession()
	{
		var allPackages = new List<StopPackage>
		{
			new("Pakket 1:", "8724934865376", "in afwachting", 1),
			new("Pakket 2:", "8743768229710", "in afwachting", 1),
			new("Pakket 1:", "8312000000001", "in afwachting", 2),
			new("Pakket 2:", "8312000000002", "in afwachting", 2),
			new("Pakket 3:", "8312000000003", "in afwachting", 2),
			new("Pakket 4:", "8312000000004", "in afwachting", 2),
			new("Pakket 5:", "8312000000005", "in afwachting", 2),
			new("Pakket 1:", "5089000000001", "in afwachting", 3),
			new("Pakket 2:", "5089000000002", "in afwachting", 3),
			new("Pakket 3:", "5089000000003", "in afwachting", 3)
		};

		packagesByBarcode = allPackages.ToDictionary(p => p.PackageNumber);

		stops =
		[
			new DeliveryStop(1, "6414 van Appelstreitweg 37", "746299BE",
				allPackages.Where(p => p.StopIndex == 1).ToList()),
			new DeliveryStop(2, "8312 Kernkraftweg 56", "837461CD",
				allPackages.Where(p => p.StopIndex == 2).ToList()),
			new DeliveryStop(3, "5089 Marketstraat 12", "952837FB",
				allPackages.Where(p => p.StopIndex == 3).ToList())
		];
	}

	public static IReadOnlyList<DeliveryStop> Stops => stops;

	public static int TotalStops => stops.Count;

	public static int TotalPackages => packagesByBarcode.Count;

	public static int LoadedInVanCount => packagesByBarcode.Values.Count(p => p.IsLoadedInVan);

	public static string VanProgressText => $"{LoadedInVanCount}/{TotalPackages} pakketten in bus";

	public static bool TryLoadIntoVan(string barcode, out StopPackage? package, out string message)
	{
		if (!packagesByBarcode.TryGetValue(barcode, out package))
		{
			message = "Onbekende barcode.";
			return false;
		}

		if (package.IsLoadedInVan)
		{
			message = $"{package.Title} staat al in de bus.";
			return false;
		}

		package.IsLoadedInVan = true;
		message = $"{package.Title} ingescand in de bus.";
		return true;
	}

	public static StopPackage? FindPackage(string barcode) =>
		packagesByBarcode.TryGetValue(barcode, out var package) ? package : null;

	public static DeliveryStop? GetStop(int stopNumber) =>
		stops.FirstOrDefault(s => s.StopNumber == stopNumber);

	public static ObservableCollection<StopPackage> GetLoadedPackagesForStop(int stopNumber)
	{
		var stop = GetStop(stopNumber);
		if (stop is null)
		{
			return [];
		}

		return new ObservableCollection<StopPackage>(
			stop.Packages.Where(p => p.IsLoadedInVan));
	}

	public static readonly IReadOnlyList<string> CommonFailureReasons =
	[
		"Niemand thuis",
		"Adres niet gevonden",
		"Pakket geweigerd",
		"Geen toegang tot locatie"
	];
}
