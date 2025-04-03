using Org.BouncyCastle.Asn1.X509;

namespace Module_3_Project.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int UnitsWorth { get; set; }
    }
}
