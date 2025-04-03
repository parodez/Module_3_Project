using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Module_3_Project.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System;
using static Mysqlx.Expect.Open.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Module_3_Project.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration;
        public HomeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        public IActionResult Login(string txtUname, string txtPass, string btnLogin)
        {
            // Getting cookie
            string cookiename = HttpContext.Request.Cookies["MyCookie"];
            Debug.WriteLine($"Cookie Value (in Login): {cookiename}");

            var loggedUser = new LoggedUser();
            if (btnLogin == "Admin")
            {
                loggedUser.LoggedIn = false;
                loggedUser.UserName = "";

                string sql = "SELECT * FROM admin_credentials WHERE uname = '" + txtUname + "' AND password = '" + txtPass + "'";
                string constr = this.Configuration.GetConnectionString("DefaultConnection");

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
                                    loggedUser.LoggedIn = true;
                                    loggedUser.UserName = sdr["name"].ToString();
                                    loggedUser.Role = "Admin";
                                }
                            }
                        }
                        con.Close();
                    }
                }
                if (loggedUser.LoggedIn == true)
                {
                    // Creation of cookie for Admin
                    HttpContext.Response.Cookies.Append("MyCookie", "AdminCookie", new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(1),
                        HttpOnly = true,
                        IsEssential = true
                    });

                    // Creation of session for Admin
                    HttpContext.Session.SetString("MySession", "AdminLoggedIn");

                    return RedirectToAction("AdminView", "Home", new { term_id = "current term" });
                }
            }
            else if (btnLogin == "Student")
            {
                loggedUser.LoggedIn = false;
                loggedUser.UserName = "";

                string sql = "SELECT * FROM student_info WHERE stud_id = '" + txtUname + "' AND password = '" + txtPass + "'";
                string constr = this.Configuration.GetConnectionString("DefaultConnection");

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
                                    loggedUser.LoggedIn = true;
                                    loggedUser.UserName = sdr["name"].ToString();
                                }
                            }
                        }
                        con.Close();
                    }
                }
                if (loggedUser.LoggedIn == true)
                {
                    // Creation of cookie for Student
                    HttpContext.Response.Cookies.Append("MyCookie", "StudentCookie", new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(1),
                        HttpOnly = true,
                        IsEssential = true
                    });

                    // Creation of session for Student
                    HttpContext.Session.SetString("MySession", "StudentLoggedIn");

                    // Storing stud_id in session
                    HttpContext.Session.SetString("LogedInUserId", txtUname);
                    return RedirectToAction("StudentView", "Home", new { stud_id = txtUname });
                }
            }

            return View();
        }
        public IActionResult AdminView(string term_id)
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
                current_year = current_year
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
        public IActionResult StudentView(string stud_id)
        {
            // Getting session data and conditional check (optional here, adjust as needed)
            string sessionname = HttpContext.Session.GetString("MySession");
            if (sessionname == null)
            {
                Debug.WriteLine("wala ng session data (StudentView)");
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Debug.WriteLine(sessionname + " (StudentView)");
            }

            Student student = new Student();


            student.grades = new List<Grade>();
            student.courses = new List<Course>();
            student.enrollments = new List<Enrolled>();
            student.terms = new List<Terms>();

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

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
                                student.stud_id = sdr["stud_id"].ToString();
                                student.name = sdr["name"].ToString();
                                student.age = Int32.Parse(sdr["age"].ToString());
                                student.year_level = Int32.Parse(sdr["year_level"].ToString());
                                student.course = sdr["course"].ToString();
                                student.units_passed = Int32.Parse(sdr["units_passed"].ToString());
                                student.units_left = Int32.Parse(sdr["units_left"].ToString());
                            }
                        }
                    }
                    con.Close();
                }
            }

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
                                student.courses.Add(new Course
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
                                student.grades.Add(new Grade
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
                                student.enrollments.Add(new Enrolled
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
                                student.terms.Add(new Terms
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

            return View(student);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult MainPage()
        {
            return View();
        }
    }
}