namespace ApplicationOrganizer;

public partial class App : Application
{
	internal static AllApplications AllApplications = new AllApplications();
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
		window.Height = 800;
		window.Width = 900;
		return window;
    }
}
