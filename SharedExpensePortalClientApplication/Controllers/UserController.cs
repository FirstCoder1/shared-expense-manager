using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SharedExpenseApplicationDataAccess;
using SharedExpensePortalClientApplication.Models;

namespace SharedExpensePortalClientApplication.Controllers
{
    public class UserController : Controller
    {
        private static ApplicationUserInformation userData = null;
        private static ExpenseType expenseType = null;
        private static UserGroup userGroup = null;
        List<UserGroup> userGroupMembers = null;

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

        [HttpGet]
        public ActionResult Register()
        {
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
                    return RedirectToAction("ViewDashboard");
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

        // GET: EditUserInformation
        [HttpGet]
        public ActionResult EditUserInformation()
        {
            ApplicationUserInformation appUser = new ApplicationUserInformation();

            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var userId = Convert.ToInt32(Session["UserId"]);
                var queryResult = _db.ApplicationUserInformations.FirstOrDefault(a => a.UserLogIn.LogInId == userId);
                appUser.UserFirstName = queryResult.UserFirstName;
                appUser.UserLastName = queryResult.UserLastName;
                appUser.UserEmailId = queryResult.UserEmailId;
                appUser.UserPhoneNumber = queryResult.UserPhoneNumber;
                appUser.UserLogIn = new UserLogIn
                {
                    UserName = queryResult.UserLogIn.UserName
                };
            }
            return View(appUser);
        }

        //POST: UserRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserInformation(ApplicationUserInformation model)
        {
            if (ModelState.IsValid)
            {
                using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
                {
                    var userId = Convert.ToInt32(Session["UserId"]);

                    var queryResult = _db.ApplicationUserInformations.Where(x => x.UserLogIn.LogInId == userId).FirstOrDefault();
                    queryResult.UserFirstName = model.UserFirstName;
                    queryResult.UserLastName = model.UserLastName;
                    queryResult.UserEmailId = model.UserEmailId;
                    queryResult.UserPhoneNumber = model.UserPhoneNumber;
                    queryResult.UserLogIn.UserName = model.UserLogIn.UserName;
                    if (queryResult.UserLogIn.Password != null)
                    {
                        queryResult.UserLogIn.Password = GetHash.GetHashForString(model.UserLogIn.Password);
                    }
                    _db.SaveChanges();

                    return View("ViewDashboard");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        //POST: UserRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
                {
                    var userCheck = _db.UserLogIns.Where(x => x.UserName == model.UserName).FirstOrDefault();
                    UserLogIn userData = new UserLogIn();

                    if (userCheck == null)
                    {
                        model.Password = GetHash.GetHashForString(model.Password);
                        _db.Configuration.ValidateOnSaveEnabled = false;
                        var storeData = new UserLogIn()
                        {
                            UserName = model.UserName,
                            //TODO: Add EmailId to store with registration to send email link
                            Password = model.Password
                        };
                        _db.UserLogIns.Add(storeData);
                        _db.SaveChanges();
                        Session["UserId"] = storeData.LogInId;

                        return RedirectToAction("ViewUserInformation", new RouteValueDictionary(
                            new { controller = "User", action = "UserData", Id = storeData.LogInId }));
                    }
                    else
                    {
                        ViewBag.error = "Email already exists";
                        return View();
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //POST: UserInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewUserInformation(UserViewModel userInformation, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
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
                        return RedirectToAction("ViewDashboard");
                    }
                    else
                    {
                        ViewBag.error = "Information already registred with User Name";
                    }
                }
            }
            return View();
        }

        //POST: Expense Entry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnterUserExpenses(ExpenseViewModel expenseInformation, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                //Split Each Group users separated by Comma
                List<string> groupUsers = expenseInformation.GroupMembers.Split(',').ToList();
                groupUsers.Add(expenseInformation.UserName);

                //Open Database Connection
                using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
                {
                    var userId = Convert.ToInt32(Session["UserId"]);
                    var sessionUserInfo = _db.ApplicationUserInformations.Where(x => x.LogInId == userId).Select(x => x).FirstOrDefault();

                    //Create users missing from the database
                    CreateMissingUsers(_db, groupUsers);

                    //Check if Expense already exist or need to create a new record to show in dropdown list
                    CreateExpense(_db, expenseInformation);

                    CreateGroupForLogInUser(_db, expenseInformation, userId);

                    //Create User Group with current user and user entered with expense form to track One-To-One expense
                    foreach (var user in groupUsers)
                    {
                        //This will alway return value as we are creating users who are not present in the system right now and storing EmailId
                        var appUserId = _db.ApplicationUserInformations.Where(x => x.UserEmailId == expenseInformation.EmailId)
                            .Select(x => x.UserId).FirstOrDefault();

                        //If Group > 1 => Divide by GroupId

                        ////Identify if both the users are already part of single Group
                        //var userGroupResult = _db.UserGroups.Where(x => x.UserId == appUserId && x.UserId == sessionUserInfo.UserId).GroupBy(x => x.GroupId).
                        //    Where(grp => grp.Count() == 2).Select(grp => grp.Key).FirstOrDefault();

                        userGroupMembers.Add(new UserGroup
                        {
                            GroupId = userGroup.GroupId,
                            UserId = appUserId
                        });
                    }

                    _db.UserGroups.AddRange(userGroupMembers);
                    _db.SaveChanges();

                    //Store Group Expense information in [dbo].[UserExpense] table for future extension
                    var storeData = new GroupExpense()
                    {
                        Title = expenseInformation.Expense ?? expenseInformation.Title,
                        Description = expenseInformation.Description,
                        ExpenseDate = expenseInformation.ExpenseDate,
                        ExpenseTypeId = expenseType != null ? expenseType.ExpenseTypeId : _db.ExpenseTypes.Where(x => x.ExpenseTypeName == expenseInformation.Expense)
                                        .Select(x => x.ExpenseTypeId).FirstOrDefault(),
                        GroupId = userGroup.GroupId,
                        //PayerId
                        //ReceiverIdList = Excluding Payer
                        ExpenseAmount = expenseInformation.ExpenseAmount,
                        ExpenseStatusId = _db.Status.FirstOrDefault(x => x.Description == "ACTIVE").StatusId
                    };
                    _db.GroupExpenses.Add(storeData);
                    _db.SaveChanges();

                    return RedirectToAction("ViewDashboard");
                }
            }
            return View();
        }

        private static void CreateGroupForLogInUser(SharedExpenseDBEntities dbContext, ExpenseViewModel expenseInfo, int userId)
        {
            //User who paid the bill
            var paidUserId = userData != null ? userData.UserId :
                        dbContext.ApplicationUserInformations.Where(x => x.UserLogIn.UserName == expenseInfo.UserName.Trim()).Select(x => x.UserId).FirstOrDefault();

            //Create a Default group by user logging expenses
            userGroup = new UserGroup
            {
                UserId = userId,
                Group = new Group
                {
                    //Build Group with User EmailIDs
                    GroupName = expenseInfo.EmailId.Split('@')[0] + " " +
                        dbContext.ApplicationUserInformations.Where(x => x.UserLogIn.LogInId == userId).Select(x => x.UserEmailId)
                        .ToString().Split('@')[0],
                    GroupDescription = "AutoGenerated Group"
                }
            };

            dbContext.UserGroups.Add(userGroup);
            dbContext.SaveChanges();

        }

        private static void CreateExpense(SharedExpenseDBEntities dbContext, ExpenseViewModel expenseInfo)
        {

            //Check if Expense already exist or need to create a new record to show in dropdown list
            if (expenseInfo.Expense == null)
            {
                expenseType = new ExpenseType
                {
                    ExpenseTypeName = expenseInfo.Title,
                    StatusId = dbContext.Status.FirstOrDefault(x => x.Description == "ACTIVE").StatusId
                };
                dbContext.ExpenseTypes.Add(expenseType);
                dbContext.SaveChanges();
            }

        }

        private static void CreateMissingUsers(SharedExpenseDBEntities dbContext, List<string> groupUsers)
        {

            //Parse all the group members entered by the users to check if they exist in the system, if user doesn't exist in the system,
            //Add user and send email to user using emailID
            //Default UserName will be extracted from emailID, which user will be able to update
            //User Password will be set by default which will be included in email sent
            foreach (var user in groupUsers)
            {
                //Split UserName from EmailId
                var _user = user.Split('@').ToString().Trim();
                //Check if user who paid is present in application
                var queryResult = dbContext.UserLogIns.FirstOrDefault(a => a.UserName == _user);

                dbContext.Configuration.ValidateOnSaveEnabled = false;

                if (queryResult == null)
                {
                    //Create User with UserName and EmailId and send email to user with provided EmailId
                    userData = new ApplicationUserInformation
                    {
                        UserFirstName = _user,
                        UserEmailId = user,
                        UserLogIn = new UserLogIn
                        {
                            UserName = _user,
                            //Generate Random Password and send this to user via email and have them reset upon logIn
                            Password = GetHash.GetHashForString(Membership.GeneratePassword(12, 3))
                        }
                    };

                    dbContext.ApplicationUserInformations.Add(userData);
                    dbContext.SaveChanges();
                }
            }
        }

        //LogOut
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("LogIn");
        }
    }
}