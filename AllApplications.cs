using System.Collections.ObjectModel;

namespace ApplicationOrganizer
{
    /// <summary>
    /// Loads and collects all JobApplications for the program, for the app data directory
    /// </summary>
    internal class AllApplications
    {
        /// <summary>
        /// All JobApplications used in the program
        /// </summary>
        public ObservableCollection<JobApplication> JobApplications { get; set; } = new ObservableCollection<JobApplication>();

        /// <summary>
        /// Loads and initializes the JobApplications parameter
        /// </summary>
        /// <param name="dispatcher">For dispatching notification changes</param>
        /// <returns>Task</returns>
        public async Task LoadApplications(IDispatcher dispatcher)
        {
            JobApplications.Clear();

            var jobApps = Directory
                .EnumerateDirectories(FileSystem.Current.AppDataDirectory)
                .Select((path) => new JobApplication(path));

            foreach(var jobApp in jobApps)
            {
                await jobApp.Load(dispatcher);
                JobApplications.Add(jobApp);
            }
        }
    }
}
