using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Module_3_Project.Models;
using MySql.Data.MySqlClient;

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
                    return RedirectToAction("AdminView", "Home", new {term_id = "20241"});
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
                    return RedirectToAction("StudentView", "Home", new {stud_id = txtUname});
                }
            }
            
            return View();
        }
        public IActionResult AdminView(string term_id)
        {
            List<Terms> terms = new List<Terms>();
            List<Enrolled> enrolled = new List<Enrolled>();
            List<Student> students = new List<Student>();
   
            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            sql = "SELECT * FROM student_info";
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

            sql = "SELECT * FROM terms";
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

            AdminViewModel adminView = new AdminViewModel()
            {
                terms = terms,
                enrolled = enrolled,
                students = students,
                term_id = term_id
            };

            return View(adminView);
        }
        public IActionResult AllStudentInfo()
        {
            List<Student> students = new List<Student>();

            string constr = this.Configuration.GetConnectionString("DefaultConnection");
            string sql;

            sql = "SELECT * FROM student_info";
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

            return View(students);
        }
        public IActionResult StudentView(string stud_id)
        {
            Student student = new Student();

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

            return View(student);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
