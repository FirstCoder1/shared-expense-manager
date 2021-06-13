using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ApplicationDataAccess;
using SharedExpensePortalClientApplication.Models;

namespace SharedExpensePortalClientApplication.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        [HttpGet]
        public ActionResult ViewUserInformation(RouteValueDictionary ReturnUrl)
        {
            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var userId = Convert.ToInt32(ReturnUrl["Id"]);
                var queryResult = _db.ApplicationUserInformations.FirstOrDefault(a => a.LogInId == userId);
                if (queryResult == null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("ViewScoreCard");
                }
            }
        }

        // GET: UserGroupInformation
        [HttpGet]
        public ActionResult ViewScoreCard(RouteValueDictionary ReturnUrl)
        {

            return View();
        }

        //POST: UserInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewUserInformation(UserViewModel userInformation, string ReturnUrl = "")
        {
            ApplicationUserInformation userData = new ApplicationUserInformation();
            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var userId = Convert.ToInt32(Session["UserId"]);
                var queryResult = _db.ApplicationUserInformations.FirstOrDefault(a => a.LogInId == userId);

                ApplicationUserInformation appData = new ApplicationUserInformation();

                if (queryResult == null)
                {
                    _db.Configuration.ValidateOnSaveEnabled = false;
                    var storeData = new ApplicationUserInformation()
                    {
                        UserFirstName = userInformation.FirstName,
                        UserLastName = userInformation.LastName,
                        UserEmailId = userInformation.EmailID,
                        UserPhoneNumber = userInformation.PhoneNumber,
                        LogInId = userId
                    };

                    _db.ApplicationUserInformations.Add(storeData);
                    _db.SaveChanges();
                    return RedirectToAction("ViewUserInformation");
                }
                else
                {
                    ViewBag.error = "Information already registred with User Name";
                    return View();
                }
            }
        }
    }
}