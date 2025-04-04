using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Module_3_Project.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System;

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

                    return RedirectToAction("AdminView", "Admin");
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

                    return RedirectToAction("StudentView", "Home", new { stud_id = txtUname });
                }
            }

            return View();
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

            StudentView studentView = new StudentView();

            studentView.grades = new List<Grade>();
            studentView.courses = new List<Course>();
            studentView.enrollments = new List<Enrolled>();
            studentView.terms = new List<Terms>();

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            // Fetch Student Personal Info
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
                                studentView.stud_id = sdr["stud_id"].ToString();
                                studentView.name = sdr["name"].ToString();
                                studentView.age = Int32.Parse(sdr["age"].ToString());
                                studentView.year_level = Int32.Parse(sdr["year_level"].ToString());
                                studentView.course = sdr["course"].ToString();
                                studentView.units_passed = Int32.Parse(sdr["units_passed"].ToString());
                                studentView.units_left = Int32.Parse(sdr["units_left"].ToString());
                            }
                        }
                    }
                    con.Close();
                }
            }

            // Fetch All Courses (remains the same)
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
                                studentView.courses.Add(new Course
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

            // Fetch Student Grades (remains the same)
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
                                studentView.grades.Add(new Grade
                                {
                                    GradeID = Int32.Parse(sdr["GradeID"].ToString()),
                                    StudentID = sdr["StudentID"].ToString(),
                                    CourseID = Int32.Parse(sdr["CourseID"].ToString()),
                                    Term = Int32.Parse(sdr["Term"].ToString()),
                                    GradeValue = sdr["GradeValue"].ToString()
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }

            return View(studentView);
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