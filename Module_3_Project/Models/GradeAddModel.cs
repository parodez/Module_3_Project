namespace Module_3_Project.Models
{
    public class GradeAddModel
    {
        public List<Student> students {  get; set; }
        public List<Terms> terms { get; set; }
        public List<Course> courses { get; set; }
        public string message { get; set; }
    }
}
