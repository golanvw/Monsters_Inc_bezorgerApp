namespace Monsters_Inc_bezorgerApp;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
    }

    private void RouteButtonClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new StopsPage());  
    }
}