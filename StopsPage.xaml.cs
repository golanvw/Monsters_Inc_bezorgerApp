namespace Monsters_Inc_bezorgerApp;

public partial class StopsPage : ContentPage
{
	public StopsPage()
	{
		InitializeComponent();
    }

	private void OnStopTapped(object sender, TappedEventArgs e)
	{
		Navigation.PushAsync(new DetailsStopsPage());
	}
}