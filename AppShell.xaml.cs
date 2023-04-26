namespace ApplicationOrganizer;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(ApplicationPage), typeof(ApplicationPage));
	}
}
