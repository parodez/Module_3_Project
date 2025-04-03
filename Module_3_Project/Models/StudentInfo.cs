namespace Module_3_Project.Models
{
    public class StudentInfo
    {
        public Student student { get; set; }
        public List<Grade> grades { get; set; }
        public List<Enrolled> enrollments { get; set; }
        public List<Terms> terms {  get; set; }
        public List<Course> courses {  get; set; }
    }
}
