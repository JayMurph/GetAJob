using System.Diagnostics;

namespace ApplicationOrganizer;

/// <summary>
/// Main page of app. Shows all Job Applications and allows for opening or creating new ones.
/// </summary>
public partial class MainPage : ContentPage
{
    private string _dataPath = FileSystem.AppDataDirectory;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = App.AllApplications;
    }

    /// <summary>
    /// Load job application data
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ((AllApplications)BindingContext).LoadApplications(Dispatcher);
    }

    /// <summary>
    /// Prompts user for the name of a new job application then creates one and
    /// navigates to a page for editing it
    /// </summary>
    /// <param name="sender">UNUSED</param>
    /// <param name="e">UNUSED</param>
    private async void NewApplicationButtonClicked(object sender, EventArgs e)
    {
        // get name of new job applcation
        string result = await DisplayPromptAsync("New Job Application", "Title", maxLength: 24, accept: "Create");
        if (result != null)
        {
            string path = Path.Combine(_dataPath, result);
            if (!Directory.Exists(path))
            {
                JobApplication newJobApp = await CreateNewJobApplicationAsync(path);

                if (newJobApp != null)
                {
                    ((AllApplications)BindingContext).JobApplications.Add(newJobApp);
                    await Shell.Current.GoToAsync($"{nameof(ApplicationPage)}?{nameof(ApplicationPage.ApplicationTitle)}={newJobApp.Title}");
                }
                else
                {
                    await DisplayAlert("Error", "Something went wrong while creating the new job application", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Job application already exists", "OK");
            }
        }
    }

    /// <summary>
    /// Creates object and directory for a new JobApplication
    /// </summary>
    /// <param name="path">Location to create Job Application directory</param>
    /// <returns>null if JobApplication could not be created</returns>
    private static async Task<JobApplication> CreateNewJobApplicationAsync(string path)
    {
        return await Task.Run(() =>
        {
            try
            {
                Directory.CreateDirectory(path);
                return new JobApplication(path);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        });
    }

    /// <summary>
    /// Opens a page for editing the selected Job Application
    /// </summary>
    /// <param name="sender">UNUSED</param>
    /// <param name="e">Contains info about selection</param>
    private async void ApplicationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count != 0 )
        {
            var app = (JobApplication)e.CurrentSelection[0];
            await Shell.Current.GoToAsync($"{nameof(ApplicationPage)}?{nameof(ApplicationPage.ApplicationTitle)}={app.Title}");
        }
    }

    /// <summary>
    /// Deletes a selected Job Application
    /// </summary>
    /// <param name="sender">Contains JobApplication to delete as CommandParameter</param>
    /// <param name="e">Unused</param>
    private async void DeleteContextMenuItemClicked(object sender, EventArgs e)
    {
        var item = ((MenuFlyoutItem)sender).CommandParameter as JobApplication;
        string toDelete = Path.Combine(_dataPath, item.Title);

        if (Directory.Exists(toDelete))
        {
            if(await DisplayAlert("Confirm", $"Are you sure that you would like to delete {item.Title}", "OK", "Cancel", FlowDirection.LeftToRight) != false)
            {
                try
                {
                    Directory.Delete(toDelete, true);
                    App.AllApplications.JobApplications.Remove(item);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await DisplayAlert("Error", "An error occurred", "OK");
                }
            }
        }
    }
}