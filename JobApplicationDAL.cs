namespace ApplicationOrganizer
{
    /// <summary>
    /// Simplified Job Application for data persistence
    /// </summary>
    public class JobApplicationDAL
    {
        public string CompanyName { get; set; }
        public string ContactInfo { get; set; }
        public string OtherInfo { get; set; }
        public string Position { get; set; }
        public DateTime DateApplied { get; set; } = DateTime.Today.AddDays(-1);
        public DateTime Interview { get; set; } = DateTime.Today.AddDays(-1);
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);
        public ApplicationStatus Status { get; set; } = ApplicationStatus.New;
    }
}
