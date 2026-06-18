namespace Monsters_Inc_bezorgerApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        ContentPage loginPage = new global::Monsters_Inc_bezorgerApp.LoginPage();
        return new Window(loginPage);
    }
}