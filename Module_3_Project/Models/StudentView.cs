namespace Module_3_Project.Models
{
    public class StudentView
    {
        public string stud_id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public int year_level { get; set; }
        public string course { get; set; }
        public int units_passed { get; set; }
        public int units_left { get; set; }

        public List<Grade> grades { get; set; }
        public List<Course> courses { get; set; }
        public List<Enrolled> enrollments { get; set; }
        public List<Terms> terms { get; set; }

     
    }
}

