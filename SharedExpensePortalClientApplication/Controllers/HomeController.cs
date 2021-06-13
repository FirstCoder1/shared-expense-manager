using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SharedExpenseApplicationDataAccess;
using SharedExpensePortalClientApplication.Models;

namespace SharedExpensePortalClientApplication.Controllers
{
    public class HomeController : Controller
    {
        private SharedExpenseDBEntities _dbContext = new SharedExpenseDBEntities();

        public ActionResult Index()
        {
            return View();
        }
        //GET: LogIn
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        //POST: UserLogIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LoginViewModel login, string ReturnUrl = "")
        {
            string message = "";
            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var data = _db.UserLogIns.Where(a => a.UserName == login.UserName).FirstOrDefault();
                if (data != null)
                {
                    if (string.Compare(GetHash.GetHashForString(login.Password), data.Password) == 0)
                    {
                        var ticket = new FormsAuthenticationTicket(login.UserName, true, 10);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(50);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["UserName"] = data.UserName;
                        Session["UserId"] = data.LogInId;
                        //TODO: Display Page with 3 buttons: Enter New Expense, View Score Card

                        var queryResult = _db.ApplicationUserInformations.FirstOrDefault(a => a.LogInId == data.LogInId);

                        if (queryResult != null)
                        {
                            return RedirectToAction("ViewDashboard", "User");
                        }
                        else
                        {
                            return RedirectToAction("ViewUserInformation", new RouteValueDictionary(
                                new { controller = "User", action = "UserData", Id = data.LogInId }));
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }
    }
}