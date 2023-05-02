using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ApplicationOrganizer
{
    /// <summary>
    /// Encapsulates info related to a job application. Notifies of property changes
    /// </summary>
    public partial class JobApplication : ObservableObject
    {
        public const string APPLICATION_MANIFEST_FILENAME = "application_manifest.json";

        /// <summary>
        /// Path to directory containing application manifest and documents
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Documents related to the Job Application
        /// </summary>
        public ObservableCollection<string> Documents { get; set; } = new ObservableCollection<string>();

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _companyName;

        [ObservableProperty]
        private string _contactInfo;

        [ObservableProperty]
        private string _otherInfo;

        [ObservableProperty]
        private DateTime _dateApplied = DateTime.Today.AddDays(-1);

        [ObservableProperty]
        private DateTime _interview = DateTime.Today.AddDays(-1);

        [ObservableProperty]
        private DateTime _dueDate = DateTime.Today.AddDays(7);

        [ObservableProperty]
        private ApplicationStatus _status = ApplicationStatus.New;

        private DirectoryInfo _documentsDir;

        /// <summary>
        /// Creates a new JobApplication and initializes its Title
        /// </summary>
        /// <param name="folderPath">
        /// Path to folder containing job application
        /// manifest and other documents
        /// </param>
        public JobApplication(string folderPath)
        {
            Path = folderPath;
            _documentsDir = new DirectoryInfo(folderPath);
            if (_documentsDir.Exists)
            {
                Title = _documentsDir.Name;
            }
        }

        /// <summary>
        /// Loads data and initializes properties of Job Application
        /// </summary>
        /// <param name="dispatcher">For invoking property changes</param>
        /// <returns>Task</returns>
        public async Task Load()
        {
            var files = _documentsDir.GetFiles();
            var manifestFile = files.Where(info => info.Name == APPLICATION_MANIFEST_FILENAME).FirstOrDefault();
            if (manifestFile == null)
            {
                await CreateNewManifestFile();
            }
            else
            {
                await InitializeFromManifest();
            }

            Documents.Clear();
            var otherFiles = files.Where(file => file.Name != APPLICATION_MANIFEST_FILENAME);
            foreach (var file in otherFiles)
            {
                Documents.Add(file.Name);
            }
        }

        /// <summary>
        /// Loads data from the application's manifest file and initializes self
        /// </summary>
        /// <param name="dispatcher">For invoking property changes</param>
        /// <returns>Task</returns>
        private async Task InitializeFromManifest()
        {
            try
            {
                using (var stream = File.OpenRead(System.IO.Path.Combine(Path, APPLICATION_MANIFEST_FILENAME)))
                {
                    JobApplicationDAL dal = (JobApplicationDAL)await JsonSerializer.DeserializeAsync(stream, typeof(JobApplicationDAL));
                    CompanyName = dal.CompanyName;
                    ContactInfo = dal.ContactInfo;
                    OtherInfo = dal.OtherInfo;
                    DateApplied = dal.DateApplied;
                    Interview = dal.Interview;
                    Status = dal.Status;
                    DueDate = dal.DueDate;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new manifest file for the Job Application
        /// </summary>
        /// <returns>Task</returns>
        private async Task CreateNewManifestFile()
        {
            try
            {
                File.CreateText(System.IO.Path.Combine(Path, APPLICATION_MANIFEST_FILENAME)).Close();
                try
                {
                    using (var stream = File.OpenWrite(System.IO.Path.Combine(Path, APPLICATION_MANIFEST_FILENAME)))
                    {
                        await JsonSerializer.SerializeAsync<JobApplicationDAL>(stream, new JobApplicationDAL());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Writes the properties of the Job Application to its manifest file
        /// </summary>
        /// <returns>Indicates if changes were written successfully</returns>
        public async Task<bool> Write()
        {
            var dal = new JobApplicationDAL()
            {
                CompanyName = CompanyName,
                ContactInfo = ContactInfo,
                OtherInfo = OtherInfo,
                DateApplied = DateApplied,
                Interview = Interview,
                Status = Status,
                DueDate = DueDate,
            };

            try
            {
                using (var stream = File.Open(System.IO.Path.Combine(Path, APPLICATION_MANIFEST_FILENAME), FileMode.Truncate))
                {
                    await JsonSerializer.SerializeAsync<JobApplicationDAL>(stream, dal);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// Encapsulates the status of a Job Application
    /// </summary>
    public enum ApplicationStatus
    {
        New, Submitted, Rejected, Accepted, Interviewing
    }
}