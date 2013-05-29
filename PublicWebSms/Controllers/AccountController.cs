using PublicWebSms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Controllers
{
    
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return Redirect("Login");
        }

        public ActionResult Login()
        {
            // Jika sudah login, tidak usah menampilkan halaman login. Langsung dashboard!
            if (UserSession.IsLogin())
            {
                return Redirect("/Dashboard");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginData loginData)
        {
            if (ModelState.IsValid)
            {
                if (UserSession.DoLogin(loginData.LoginName, loginData.Password))
                {
                    return Redirect("/Dashboard");
                }

            }

            return Redirect("/Account/Login");
        }

        [PwsAuthorize]
        public ActionResult Logout()
        {
            // Lakukan proses logout
            UserSession.DoLogout();
            return Redirect("/Account/Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterData registerData)
        {
            if (ModelState.IsValid)
            {

            }

            return View("RegisterSuccess");
        }

        [PwsAuthorize]
        public ActionResult Manage()
        {
            return View();
        }

    }
}
