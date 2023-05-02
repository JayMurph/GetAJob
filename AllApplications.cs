using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ApplicationOrganizer
{
    /// <summary>
    /// Loads and collects all JobApplications for the program, for the app data directory
    /// </summary>
    public partial class AllApplications : ObservableObject
    {
        /// <summary>
        /// All JobApplications used in the program
        /// </summary>
        public ObservableCollection<JobApplication> JobApplications { get; set; } = new ObservableCollection<JobApplication>();

        [ObservableProperty]
        private int _applicationsCreated;

        [ObservableProperty]
        private int _applicationsSubmitted;

        [ObservableProperty]
        private int _applicationsRejected;

        [ObservableProperty]
        private int _applicationsInterviewed;

        /// <summary>
        /// Loads and initializes the JobApplications parameter
        /// </summary>
        /// <param name="dispatcher">For dispatching notification changes</param>
        /// <returns>Task</returns>
        public async Task<bool> LoadApplications()
        {
            JobApplications.Clear();

            try
            {
                var jobApps = Directory
                    .EnumerateDirectories(FileSystem.Current.AppDataDirectory)
                    .Select((path) => new JobApplication(path));

                foreach (var jobApp in jobApps)
                {
                    await jobApp.Load();
                    JobApplications.Add(jobApp);
                }
            }
            catch (Exception ex)
            {
                JobApplications.Clear();
                Debug.WriteLine(ex);
                return false;
            }

            UpdateStats();
            JobApplications.CollectionChanged += (a, b) => UpdateStats();

            return true;
        }

        private void UpdateStats()
        {
            ApplicationsCreated = JobApplications.Count;
            ApplicationsSubmitted = JobApplications.Where(app => app.Status != ApplicationStatus.New).Count();
            ApplicationsRejected = JobApplications.Where(app => app.Status == ApplicationStatus.Rejected).Count();
            ApplicationsInterviewed = JobApplications.Where(app => app.Status == ApplicationStatus.Interviewing).Count();
        }
    }
}