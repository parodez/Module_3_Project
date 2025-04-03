namespace Module_3_Project.Models
{
    public class AdminViewModel
    {
        public List<Terms> terms { get; set; }
        public List<Enrolled> enrolled { get; set; }
        public List<Student> students { get; set; }
        public string term_id { get; set; }
        public int current_year { get; set; }
        public int current_term {  get; set; }
    }
}
