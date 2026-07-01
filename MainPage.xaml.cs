namespace Monsters_Inc_bezorgerApp;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		ErrorLabel.IsVisible = false;
    }

	private async void OnLoginClicked(object? sender, EventArgs e)
	{
		if(UsernameInput.Text == "admin" && PasswordInput.Text == "admin")
		{
			ErrorLabel.IsVisible = false;
            await DisplayAlert("Login Gelukt", "Je bent ingelogd!", "OK");
			Application.Current!.Windows[0].Page = new AppShell();
        }
		else
		{
			ErrorLabel.IsVisible = true;
			UsernameInput.BackgroundColor = Color.FromArgb("#FF0000");
			UsernameInput.TextColor = Color.FromArgb("#FFFFFF");
			PasswordInput.BackgroundColor = Color.FromArgb("#FF0000");
			PasswordInput.TextColor = Color.FromArgb("#FFFFFF");
        }
    }

	private void OnForgotPasswordClicked(object? sender, EventArgs e)
	{
		DisplayAlert("Wachtwoord vergeten", "Neem contact op met de beheerder om je wachtwoord te resetten.", "OK");
    }

    private void LoginWithKeyPassClicked(object sender, EventArgs e)
    {
		DisplayAlert("Inloggen met KeyPass", "Deze functie is nog niet beschikbaar.", "OK");
    }
}
