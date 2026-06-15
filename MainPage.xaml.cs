namespace Monsters_Inc_bezorgerApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		ErrorLabel.IsVisible = false;
    }

	private void OnLoginClicked(object? sender, EventArgs e)
	{
		if(UsernameInput.Text == "admin" && PasswordInput.Text == "admin")
		{
			ErrorLabel.IsVisible = false;
            DisplayAlert("Login Gelukt", "Je bent ingelogd!", "OK");
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
}
