using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ApplicationDataAccess;
using SharedExpensePortalClientApplication.Models;

namespace SharedExpensePortalClientApplication.Controllers
{
    public class UserController : Controller
    {
        //Get: Enter Expense
        [HttpGet]
        public ActionResult EnterExpense()
        {
            DataTable _dt = new DataTable();

            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var expenseList = _db.ExpenseTypes.Select(x => x.ExpenseTypeName).ToList();
                ViewBag.ExpenseList = expenseList;
            }

            return View();
        }

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

        //ViewDashboard
        // GET: UserGroupInformation
        [HttpGet]
        public ActionResult ViewDashboard()
        {
            return View();
        }

        // GET: UserGroupInformation
        [HttpGet]
        public ActionResult ViewScoreCard(RouteValueDictionary ReturnUrl)
        {
            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var userId = Convert.ToInt32(ReturnUrl["Id"]);
                var queryResult = _db.ApplicationUserInformations.FirstOrDefault(a => a.LogInId == userId);

                return View();
            }
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
                    return RedirectToAction("ViewUserDashboard");
                }
                else
                {
                    ViewBag.error = "Information already registred with User Name";
                    return View();
                }
            }
        }

        //POST: Expense Entry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnterUserExpenses(ExpenseViewModel expenseInformation, string ReturnUrl = "")
        {
            ApplicationUserInformation userData = null;
            ExpenseType expenseType = null;
            UserGroup userGroup = null;
            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var userId = Convert.ToInt32(Session["UserId"]);
                //Check if entered user is present in application
                var queryResult = _db.UserLogIns.FirstOrDefault(a => a.UserName == expenseInformation.UserName);
                _db.Configuration.ValidateOnSaveEnabled = false;

                if (queryResult == null)
                {
                    //Create User with UserName and EmailId and send email to user with provided EmailId
                    userData = new ApplicationUserInformation
                    {
                        UserFirstName = expenseInformation.UserName,
                        UserEmailId = expenseInformation.EmailId,
                        UserLogIn = new UserLogIn
                        {
                            UserName = expenseInformation.UserName,
                            //Generate Random Password and send this to user via email and have them reset upon logIn
                            Password = GetHash.GetHashForString(Membership.GeneratePassword(12, 3))
                        }
                    };

                    _db.ApplicationUserInformations.Add(userData);
                    _db.SaveChanges();
                }

                //Check if Expoense already exist or need to create a new record to show in dropdown list
                if (expenseInformation.Expense == null)
                {
                    expenseType = new ExpenseType
                    {
                        ExpenseTypeName = expenseInformation.Title,
                        ExpenseTypeStatus = _db.Status.FirstOrDefault(x => x.StatusDescription == "ACTIVE").StatusId
                    };
                    _db.ExpenseTypes.Add(expenseType);
                    _db.SaveChanges();
                }
                //TODO: Create User Group with current user and user entered with expense form to track One-To-One expense

                //Store Expense information in [dbo].[UserExpense] table
                var storeData = new UserExpense()
                {
                    Title = expenseInformation.Expense ?? expenseInformation.Title,
                    Description = expenseInformation.Description,
                    ExpenseDate = expenseInformation.ExpenseDate,
                    ExpenseTypeId = expenseType != null ? expenseType.ExpenseTypeId : _db.ExpenseTypes.Where(x => x.ExpenseTypeName == expenseInformation.Expense)
                                    .Select(x => x.ExpenseTypeId).FirstOrDefault(),
                    UserId = userData != null ? userData.UserId : 
                            _db.ApplicationUserInformations.Where(x => x.LogInId == queryResult.LogInId).Select(x => x.UserId).FirstOrDefault(),
                    ExpenseAmount = expenseInformation.ExpenseAmount,
                    ExpenseStatusId = _db.Status.FirstOrDefault(x => x.StatusDescription == "ACTIVE").StatusId
                };

                _db.UserExpenses.Add(storeData);
                _db.SaveChanges();
                return RedirectToAction("ViewDashboard");
            }
        }
    }
}