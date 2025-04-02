using System.Diagnostics;
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

        public IActionResult AdminLogin(string txtUname, string txtPass)
        {
            LoggedUser loggedUser = new LoggedUser();
            loggedUser.LoggedIn = false;
            loggedUser.UserName = "";

            string sql = "SELECT * FROM usercredentials WHERE uname = '" + txtUname + "' AND upassword = '" + txtPass + "'";
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
                            loggedUser.LoggedIn = true;
                        }
                    }
                    con.Close();
                }
            }
            if (loggedUser.LoggedIn == true)
            {
                return RedirectToAction("Privacy", "Home");
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
