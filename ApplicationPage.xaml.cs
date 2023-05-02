using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ApplicationOrganizer;

/// <summary>
/// Page that allows editing info for a Job Application
/// </summary>
[QueryProperty(nameof(JobApplication), nameof(JobApplication))]
public partial class ApplicationPage : ContentPage
{
    public ObservableCollection<ApplicationStatus> Statuses { get; set; } = new ObservableCollection<ApplicationStatus>(Enum.GetValues<ApplicationStatus>().ToList());

    /// <summary>
    /// Model for the page
    /// </summary>
    private JobApplication _jobApplication;

    public JobApplication JobApplication
    {
        get => _jobApplication;
        set
        {
            _jobApplication = value;
            BindingContext = _jobApplication;
        }
    }

    public ApplicationPage()
    {
        InitializeComponent();
        BindingContext = _jobApplication;
    }

    /// <summary>
    /// Initializes job application model
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (await _jobApplication.Load() == false)
        {
            await DisplayAlert("Error", "Unable to load Application data", "OK");
            await Shell.Current.GoToAsync(nameof(MainPage));
        }

        BindingContext = _jobApplication;
    }

    /// <summary>
    /// Writes job application changes to file
    /// </summary>
    /// <param name="args"></param>
    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        await _jobApplication.Write();
    }

    /// <summary>
    /// Opens the folder containing the job application's documents in the windows file explorer
    /// </summary>
    /// <param name="sender">UNUSED</param>
    /// <param name="e">UNUSED</param>
    private void OpenDocumentsButtonClicked(object sender, EventArgs e)
    {
        Process.Start("explorer.exe", new[] { _jobApplication.Path });
    }

    /// <summary>
    /// Deletes a document for the job application
    /// </summary>
    /// <param name="sender">MenuFlyoutItem of job application document UI element</param>
    /// <param name="e">UNUSED</param>
    private async void DeleteDocumentContextMenuItemClicked(object sender, EventArgs e)
    {
        var item = ((MenuFlyoutItem)sender).CommandParameter as string;
        string toDelete = Path.Combine(_jobApplication.Path, item);
        if (File.Exists(toDelete))
        {
            try
            {
                File.Delete(toDelete);
                _jobApplication.Documents.Remove(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await DisplayAlert("Error", "An error occurred", "OK");
            }
        }
        else
        {
            await DisplayAlert("File not found", "File could not be found", "OK");
        }
    }

    /// <summary>
    /// Opens a file picker allowing the user to select documents to add to the Job Application
    /// </summary>
    /// <param name="sender">UNUSED</param>
    /// <param name="e">UNUSED</param>
    private async void AddDocumentButtonClicked(object sender, EventArgs e)
    {
        DocumentsAddButton.IsEnabled = false;
        var result = await FilePicker.PickMultipleAsync();
        if (result.Any())
        {
            foreach (var item in result)
            {
                await CopyFileToJobApplication(item);
            }
        }
        DocumentsAddButton.IsEnabled = true;
    }

    /// <summary>
    /// Copies a file to the Job Application's documents folder
    /// </summary>
    /// <param name="item">File to copy</param>
    /// <returns>Task</returns>
    private async Task CopyFileToJobApplication(FileResult item)
    {
        await Task.Run(async () =>
        {
            try
            {
                File.Copy(item.FullPath, Path.Combine(_jobApplication.Path, item.FileName));
                _jobApplication.Documents.Add(item.FileName);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to copy {item.FileName}", "OK");
                Debug.WriteLine(ex);
            }
        });
    }

    /// <summary>
    /// Opens the double tapped document with the Windows File Explorer
    /// </summary>
    /// <param name="sender">Unused</param>
    /// <param name="e">Contains name of document to view as Parameter property</param>
    private void DocumentDoubleTapped(object sender, TappedEventArgs e)
    {
        Process.Start("explorer.exe", new[] { Path.Combine(_jobApplication.Path, e.Parameter as string) });
    }
}