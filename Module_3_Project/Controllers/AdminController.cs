using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Module_3_Project.Models;
using MySql.Data.MySqlClient;

namespace Module_3_Project.Controllers
{
    public class AdminController : Controller
    {
        private IConfiguration Configuration;
        public AdminController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        public IActionResult AdminView(string term_id = "current term", string message = "")
        {
            // Getting session data and conditional check
            string sessionname = HttpContext.Session.GetString("MySession");

            if (sessionname == null)
            {
                Debug.WriteLine("No Session data");
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Debug.WriteLine(sessionname);

            }

            List<Terms> terms = new List<Terms>();
            List<Enrolled> enrolled = new List<Enrolled>();
            List<Student> students = new List<Student>();

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            sql = "SELECT * FROM student_info ORDER BY stud_id DESC";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                students.Add(new Student
                                {
                                    stud_id = sdr["stud_id"].ToString(),
                                    name = sdr["name"].ToString(),
                                    age = Int32.Parse(sdr["age"].ToString()),
                                    year_level = Int32.Parse(sdr["year_level"].ToString()),
                                    course = sdr["course"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            int current_year = 0;
            int current_term = 0;
            sql = "SELECT * FROM terms ORDER BY term_id DESC";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                terms.Add(new Terms
                                {
                                    term_id = sdr["term_id"].ToString(),
                                    start_year = Int32.Parse(sdr["start_year"].ToString()),
                                    term = Int32.Parse(sdr["term"].ToString())
                                });
                                if (Int32.Parse(sdr["start_year"].ToString()) > current_year)
                                {
                                    current_year = Int32.Parse(sdr["start_year"].ToString());
                                    if (Int32.Parse(sdr["term"].ToString()) > current_term)
                                    {
                                        current_term = Int32.Parse(sdr["term"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    con.Close();
                }
            }

            sql = "SELECT * FROM enrollments";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                enrolled.Add(new Enrolled
                                {
                                    enrollment_id = Int32.Parse(sdr["enrollment_id"].ToString()),
                                    term_id = sdr["term_id"].ToString(),
                                    stud_id = sdr["stud_id"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            if (term_id == "current term")
            {
                term_id = current_year.ToString() + current_term.ToString();
            }
            AdminViewModel adminView = new AdminViewModel()
            {
                terms = terms,
                enrolled = enrolled,
                students = students,
                term_id = term_id,
                current_term = current_term,
                current_year = current_year,
                message = message
            };

            return View(adminView);
        }
        public IActionResult StudentInfo(string stud_id)
        {
            // Getting session data and conditional check (optional here, adjust as needed)
            string sessionname = HttpContext.Session.GetString("MySession");
            if (sessionname == null)
            {
                Debug.WriteLine("wala ng session data (AllStudentInfo)");
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Debug.WriteLine(sessionname + " (AllStudentInfo)");
            }

            StudentInfo studentInfo = new StudentInfo();
            studentInfo.student = new Student();
            studentInfo.grades = new List<Grade>();
            studentInfo.enrollments = new List<Enrolled>();
            studentInfo.terms = new List<Terms>();
            studentInfo.courses = new List<Course>();
            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            //Get Student Personal Info
            sql = "SELECT * FROM student_info WHERE stud_id = '" + stud_id + "';";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                studentInfo.student.stud_id = sdr["stud_id"].ToString();
                                studentInfo.student.name = sdr["name"].ToString();
                                studentInfo.student.age = Int32.Parse(sdr["age"].ToString());
                                studentInfo.student.year_level = Int32.Parse(sdr["year_level"].ToString());
                                studentInfo.student.course = sdr["course"].ToString();
                                studentInfo.student.units_passed = Int32.Parse(sdr["units_passed"].ToString());
                                studentInfo.student.units_left = Int32.Parse(sdr["units_left"].ToString());
                            }
                        }
                    }
                    con.Close();
                }
            }

            //Get Student Grades
            sql = "SELECT * FROM grades WHERE StudentID = '" + stud_id + "';";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                studentInfo.grades.Add(new Grade
                                {
                                    GradeID = Int32.Parse(sdr["GradeID"].ToString()),
                                    StudentID = sdr["StudentID"].ToString(),
                                    CourseID = Int32.Parse(sdr["CourseID"].ToString()),
                                    Term = Int32.Parse(sdr["Term"].ToString()),
                                    GradeValue = sdr["GradeValue"].ToString()
                                }
                                );
                            }
                        }
                    }
                    con.Close();
                }
            }

            //Get Student Enrollment Info
            sql = "SELECT * FROM enrollments WHERE stud_id = '" + stud_id + "';";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                studentInfo.enrollments.Add(new Enrolled
                                {
                                    enrollment_id = Int32.Parse(sdr["enrollment_id"].ToString()),
                                    term_id = sdr["term_id"].ToString(),
                                    stud_id = sdr["stud_id"].ToString(),
                                    CourseID = Int32.Parse(sdr["CourseID"].ToString())
                                }
                                );
                            }
                        }
                    }
                    con.Close();
                }
            }

            //Get Terms
            sql = "SELECT * FROM terms;";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                studentInfo.terms.Add(new Terms
                                {
                                    term_id = sdr["term_id"].ToString(),
                                    start_year = Int32.Parse(sdr["start_year"].ToString()),
                                    term = Int32.Parse(sdr["term"].ToString())
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            //Get Courses
            sql = "SELECT * FROM courses;";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                studentInfo.courses.Add(new Course
                                {
                                    CourseID = Int32.Parse(sdr["CourseID"].ToString()),
                                    CourseCode = sdr["CourseCode"].ToString(),
                                    CourseName = sdr["CourseName"].ToString(),
                                    UnitsWorth = Int32.Parse(sdr["UnitsWorth"].ToString())
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return View(studentInfo);
        }
        public IActionResult CourseView(string message = "")
        {
            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            CourseViewModel courseViewInfo = new CourseViewModel();
            courseViewInfo.courses = new List<Course>();
            courseViewInfo.message = message;

            //Get all Courses
            sql = "SELECT * FROM courses;";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();

                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                courseViewInfo.courses.Add(new Course
                                {
                                    CourseID = Int32.Parse(sdr["CourseID"].ToString()),
                                    CourseCode = sdr["CourseCode"].ToString(),
                                    CourseName = sdr["CourseName"].ToString(),
                                    UnitsWorth = Int32.Parse(sdr["UnitsWorth"].ToString())
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            //Get all Courses - END

            return View(courseViewInfo);
        }//CourseView - END
        public IActionResult CourseDML(string btn_course_dml, string CourseID = "")
        {
            if (btn_course_dml == "add")
            {
                return RedirectToAction("CourseAdd", "Admin");
            }
            else if (btn_course_dml == "edit")
            {
                return RedirectToAction("CourseEdit", "Admin", new { CourseID });
            }
            else
            {
                return RedirectToAction("CourseDelete", "Admin", new { CourseID });
            }
        }
        public IActionResult CourseAdd(string txtCourseCode = "", string txtCourseName = "", string txtUnitsWorth = "")
        {
            List<Course> courses = new List<Course>();
            CourseAddModel addCourseInfo = new CourseAddModel();
            addCourseInfo.message = "";

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            if (txtCourseCode != "")
            {
                //Check if Course to be addded exists
                sql = "SELECT * FROM courses;";
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        con.Open();

                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.HasRows)
                            {
                                while (sdr.Read())
                                {
                                    if (txtCourseCode == sdr["CourseCode"].ToString() ||
                                        txtCourseName == sdr["CourseName"].ToString())
                                    {
                                        addCourseInfo.message = "Course already exists";
                                    }
                                }
                            }
                        }
                        con.Close();
                    }
                }
                //Check if Course to be addded exists - END
                
                //Runs if Course to be added does not exist
                if (addCourseInfo.message == "")
                {
                    //Add Course to database
                    sql = "INSERT INTO courses VALUES (NULL,'" + txtCourseCode + "','" + txtCourseName + "','" + txtUnitsWorth + "');";
                    using (MySqlConnection con = new MySqlConnection(constr))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, con))
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    //Add Course to database - END

                    //Go back to Admin Dashboard with transaction feedback
                    return RedirectToAction("CourseView", "Admin", new { message = "Course Added Successfully" });
                    //Go back to Admin Dashboard with transaction feedback - END
                }
                //Runs if Course to be added does not exist - END
            }

            return View(addCourseInfo);
        }//CourseAdd - END
        public IActionResult CourseEdit(string CourseID, string txtCourseCode = "", string txtCourseName = "", string txtUnitsWorth = "")
        {
            List<Course> courses = new List<Course>();
            CourseAddModel courseInfo = new CourseAddModel();
            courseInfo.course = new Course();
            courseInfo.message = "";

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            //Runs if user pressed the edit button
            if (txtCourseCode != "")
            {
                //Check if Course to be edited exists
                sql = "SELECT * FROM courses;";
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        con.Open();

                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.HasRows)
                            {
                                while (sdr.Read())
                                {
                                    if (CourseID != sdr["CourseID"].ToString() && 
                                        (txtCourseCode == sdr["CourseCode"].ToString()) ||
                                        txtCourseName == sdr["CourseName"].ToString())
                                    {
                                        courseInfo.message = "Course already exists";
                                        break;
                                    }
                                }
                            }
                        }
                        con.Close();
                    }
                }
                //Checks if edited Course already exists - END

                //Runs if Course to be edited does not exist
                if (courseInfo.message == "")
                {
                    //Edits Course database
                    sql = "UPDATE courses SET CourseCode='" + txtCourseCode + "',CourseName='" + txtCourseName + "',UnitsWorth='" + txtUnitsWorth + "' WHERE CourseID='" + CourseID + "';";
                    using (MySqlConnection con = new MySqlConnection(constr))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, con))
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    courseInfo.message = "Course Edited Successfully";
                    //Edits Course database - END
                }
                //Runs if Course to be added does not exist - END

                return RedirectToAction("CourseView", "Admin", new { message = courseInfo.message });
            }
            //Runs if user pressed the edit button - END
            //Runs if first time loading
            else
            {
                //Gets Course data
                sql = "SELECT * FROM courses WHERE CourseID='" + CourseID + "';";
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        con.Open();

                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                courseInfo.course.CourseID = Int32.Parse(sdr["CourseID"].ToString());
                                courseInfo.course.CourseCode = sdr["CourseCode"].ToString();
                                courseInfo.course.CourseName = sdr["CourseName"].ToString();
                                courseInfo.course.UnitsWorth = Int32.Parse(sdr["UnitsWorth"].ToString());
                            }
                        }
                        con.Close();
                    }
                }
                //Gets Course data - END

                return View(courseInfo);
            }
            //Runs if first time loading - END
        }//CourseEdit - END
        public IActionResult CourseDelete(string CourseID)
        {
            string message = "";

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            sql = "DELETE FROM courses WHERE CourseID='" + CourseID + "';";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return RedirectToAction("CourseView", "Admin", new { message = "Course Deleted Successfully" });
        }//Course Delete - END
        public IActionResult AllStudentView(string message = "")
        {
            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            StudentViewModel studentViewInfo = new StudentViewModel();
            studentViewInfo.students = new List<Student>();
            studentViewInfo.message = message;

            //Get all Courses
            sql = "SELECT * FROM student_info;";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    using (MySqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                studentViewInfo.students.Add(new Student
                                {
                                    stud_id = sdr["stud_id"].ToString(),
                                    name = sdr["name"].ToString(),
                                    age = Int32.Parse(sdr["age"].ToString()),
                                    year_level = Int32.Parse(sdr["year_level"].ToString()),
                                    course = sdr["course"].ToString(),
                                    units_passed = Int32.Parse(sdr["units_passed"].ToString()),
                                    units_left = Int32.Parse(sdr["units_left"].ToString())
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            //Get all Courses - END

            return View(studentViewInfo);
        }//AllStudentView - END
        public IActionResult StudentDML(string btn_student_dml, string txtStudID)
        {
            if (btn_student_dml == "add")
            {
                return RedirectToAction("StudentAdd", "Admin");
            }
            else if (btn_student_dml == "edit")
            {
                return RedirectToAction("StudentEdit", "Admin", new { txtStudID });
            }
            else
            {
                return RedirectToAction("StudentDelete", "Admin", new { txtStudID });
            }
        }//StudentDML - END
        public IActionResult StudentAdd(string txtStudID = "", string txtName = "", string txtAge = "", string txtYearLevel = "", string txtCourse = "", string txtUnitsPassed = "", string txtUnitsLeft = "")
        {
            StudentAddModel studentAddInfo = new StudentAddModel();
            studentAddInfo.student = new Student();
            studentAddInfo.message = "";

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            if (txtStudID != "")
            {
                //Check if Student to be addded exists
                sql = "SELECT * FROM student_info WHERE stud_id='" + txtStudID + "';";
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.HasRows)
                            {
                                while (sdr.Read())
                                {
                                    studentAddInfo.message = "Course already exists";
                                    break;
                                }
                            }
                        }
                        con.Close();
                    }
                }
                //Check if Student to be addded exists - END

                //Runs if Student to be added does not exist
                if (studentAddInfo.message == "")
                {
                    //Add Student to database
                    sql = "INSERT INTO student_info VALUES ('" + txtStudID + "','password','" + txtName + "','" + txtAge + "','" + txtYearLevel + "','" + txtCourse + "','" + txtUnitsPassed + "','" + txtUnitsLeft + "','0');";
                    using (MySqlConnection con = new MySqlConnection(constr))
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, con))
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    //Add Student to database - END

                    //Go back to Admin Dashboard with transaction feedback
                    return RedirectToAction("AllStudentView", "Admin", new { message = "Student Added Successfully" });
                    //Go back to Admin Dashboard with transaction feedback - END
                }
                //Runs if Student to be added does not exist - END
            }
            return View(studentAddInfo);
        }//StudentADD - END
        public IActionResult StudentEdit(string txtStudID = "", string txtName = "", string txtAge = "", string txtYearLevel = "", string txtCourse = "", string txtUnitsPassed = "", string txtUnitsLeft = "")
        {
            StudentAddModel studentInfo = new StudentAddModel();
            studentInfo.student = new Student();
            studentInfo.message = "";

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            //Runs if user pressed the edit button
            if (txtName != "")
            {
                //Edits Student database
                sql = "UPDATE student_info SET name='" + txtName + "',age='" + txtAge + "',year_level='" + txtYearLevel + "',course='" + txtCourse + "',units_passed='" + txtUnitsPassed + "',units_left='" + txtUnitsLeft + "' WHERE stud_id='" + txtStudID + "';";
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                studentInfo.message = "Course Edited Successfully";
                //Edits Student database - END

                return RedirectToAction("AllStudentView", "Admin", new { message = studentInfo.message });
            }
            //Runs if user pressed the edit button - END
            //Runs if first time loading
            else
            {
                //Gets Student data
                sql = "SELECT * FROM student_info WHERE stud_id='" + txtStudID + "';";
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        con.Open();
                        using (MySqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                studentInfo.student.stud_id = sdr["stud_id"].ToString();
                                studentInfo.student.name = sdr["name"].ToString();
                                studentInfo.student.age = Int32.Parse(sdr["age"].ToString());
                                studentInfo.student.year_level = Int32.Parse(sdr["year_level"].ToString());
                                studentInfo.student.course = sdr["course"].ToString();
                                studentInfo.student.units_passed = Int32.Parse(sdr["units_passed"].ToString());
                                studentInfo.student.units_left = Int32.Parse(sdr["units_left"].ToString());                            }
                        }
                        con.Close();
                    }
                }
                //Gets Student data - END

                return View(studentInfo);
            }
            //Runs if first time loading - END
        }//StudentEdit - END
        public IActionResult StudentDelete(string txtStudID)
        {
            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            sql = "DELETE FROM student_info WHERE stud_id='" + txtStudID + "';";
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            return RedirectToAction("AllStudentView", "Admin", new { message = "Student Deleted Successfully" });
        }
    }
}
