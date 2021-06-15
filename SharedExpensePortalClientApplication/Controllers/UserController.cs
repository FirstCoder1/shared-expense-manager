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
        private List<UserGroup> userGroupMembers = new List<UserGroup>();
        private List<UserGroupTransaction> groupUserTransaction = new List<UserGroupTransaction>();

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
                            Password = model.Password,
                            CreateDate = System.DateTime.Now,
                            LastUpdate = System.DateTime.Now
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
                            LogInId = userId,
                            CreateDate = System.DateTime.Now,
                            LastUpdate = System.DateTime.Now
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
            List<int> transactionAmount = new List<int>();

            //Split Each Group users separated by Comma
            List<string> groupUsers = expenseInformation.GroupMembers.Split(',').ToList();
            //Open Database Connection
            using (SharedExpenseDBEntities _db = new SharedExpenseDBEntities())
            {
                var sessionuserId = Convert.ToInt32(Session["UserId"]);
                var sessionUserInfo = _db.ApplicationUserInformations.Where(x => x.LogInId == sessionuserId).Select(x => x).FirstOrDefault();
                groupUsers.Add(sessionUserInfo.UserEmailId);
                //Create users missing from the database
                foreach (var user in groupUsers)
                {
                    groupUserTransaction.Add(new UserGroupTransaction { UserId = CreateMissingUsers(user, _db), 
                        IsPayer = false, TransactionAmount = -(expenseInformation.ExpenseAmount / (groupUsers.Count + 1))});
                }

                //Identify if Payer's user prfile exists in the system, if not insert new profile
                //Set IsPayer flag to true in UserGroupTransaction model and allocate amount other owe after sharing the bill equally
                groupUserTransaction.Add(new UserGroupTransaction { UserId = CreateMissingUsers(expenseInformation.EmailId, _db), 
                    TransactionAmount = expenseInformation.ExpenseAmount - expenseInformation.ExpenseAmount/ (groupUsers.Count + 1), IsPayer = true });

                //Check if Expense already exist or need to create a new record to show in dropdown list
                CreateExpense(_db, expenseInformation);

                //Create Group for Payer
                var groupId = CreateGroupForPayer(_db, expenseInformation, groupUserTransaction.Where(x => x.IsPayer == true).Select(x=>x).FirstOrDefault());

                //Adding all the users with user expenses to this group with transaction amount loaded into DB
                foreach (var user in groupUserTransaction)
                {
                    ////This will alway return value as we are creating users who are not present in the system right now and storing EmailId
                    //var appUserId = _db.ApplicationUserInformations.Where(x => x.UserEmailId == expenseInformation.EmailId)
                    //    .Select(x => x.UserId).FirstOrDefault();

                    ////Identify if both the users are already part of single Group
                    //var userGroupResult = _db.UserGroups.Where(x => x.UserId == appUserId && x.UserId == sessionUserInfo.UserId).GroupBy(x => x.GroupId).
                    //    Where(grp => grp.Count() == 2).Select(grp => grp.Key).FirstOrDefault();

                    //Only create groups with expenses for user who isn't a payer
                    if (!user.IsPayer)
                    {
                        userGroupMembers.Add(new UserGroup
                        {
                            GroupId = groupId,
                            UserId = user.UserId,
                            TransactionAmount = user.TransactionAmount,
                            IsPayer = false,
                            CreateDate = System.DateTime.Now,
                            LastUpdate = System.DateTime.Now
                        });
                    }

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
                    GroupId = groupId,
                    ExpenseAmount = expenseInformation.ExpenseAmount,
                    ExpenseStatusId = _db.Status.FirstOrDefault(x => x.Description == "ACTIVE").StatusId,
                    CreateDate = System.DateTime.Now,
                    LastUpdate = System.DateTime.Now,
                };
                _db.GroupExpenses.Add(storeData);
                _db.SaveChanges();

                return RedirectToAction("ViewDashboard");
            }
        }

        private static int CreateGroupForPayer(SharedExpenseDBEntities dbContext, ExpenseViewModel expenseInfo, UserGroupTransaction userTransaction)
        {
            ////User who paid the bill
            //var paidUserId = userData != null ? userData.UserId :
            //            dbContext.ApplicationUserInformations.Where(x => x.UserLogIn.UserName == expenseInfo.UserName.Trim()).Select(x => x.UserId).FirstOrDefault();

            //Create a Default group by user logging expenses
            userGroup = new UserGroup
            {
                UserId = userTransaction.UserId,
                TransactionAmount = userTransaction.TransactionAmount,
                Group = new Group
                {
                    //Build Group with User EmailIDs
                    GroupName = "Payer" + 
                    dbContext.ApplicationUserInformations.Where(x=>x.UserId == userTransaction.UserId).Select(x => x.UserFirstName).FirstOrDefault(),
                    GroupDescription = "AutoGenerated Group",
                    GroupExpense = expenseInfo.ExpenseAmount,
                    CreateDate = System.DateTime.Now,
                    LastUpdate = System.DateTime.Now
                },
                IsPayer = true,
                CreateDate = System.DateTime.Now,
                LastUpdate = System.DateTime.Now
            };

            dbContext.UserGroups.Add(userGroup);
            dbContext.SaveChanges();
            userTransaction.GroupId = userGroup.GroupId;
            return userGroup.GroupId;
        }

        private static void CreateExpense(SharedExpenseDBEntities dbContext, ExpenseViewModel expenseInfo)
        {

            //Check if Expense already exist or need to create a new record to show in dropdown list
            if (expenseInfo.Expense == null)
            {
                expenseType = new ExpenseType
                {
                    ExpenseTypeName = expenseInfo.Title,
                    StatusId = dbContext.Status.FirstOrDefault(x => x.Description == "ACTIVE").StatusId,
                    CreateDate = System.DateTime.Now,
                    LastUpdate = System.DateTime.Now
                };
                dbContext.ExpenseTypes.Add(expenseType);
                dbContext.SaveChanges();
            }

        }

        private static int CreateMissingUsers(string user, SharedExpenseDBEntities _db)
        {
            //Parse all the group members entered by the users to check if they exist in the system, if user doesn't exist in the system,
            //Add user and send email to user using emailID
            //Default UserName will be extracted from emailID, which user will be able to update
            //User Password will be set by default which will be included in email sent
            //Split UserName from EmailId

            var _user = user.Split('@')[0].Trim();
            //Check if user who paid is present in application
            var queryResult = _db.UserLogIns.FirstOrDefault(a => a.UserName == _user);

            _db.Configuration.ValidateOnSaveEnabled = false;

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
                        Password = GetHash.GetHashForString(Membership.GeneratePassword(12, 3)),
                        CreateDate = System.DateTime.Now,
                        LastUpdate = System.DateTime.Now
                    }
                };
                _db.ApplicationUserInformations.Add(userData);
                _db.SaveChanges();
            }

            return queryResult != null ? _db.ApplicationUserInformations.Where(x => x.LogInId == queryResult.LogInId).Select(x => x.UserId).FirstOrDefault()
                : userData.UserId;
        }

        //LogOut
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("LogIn");
        }
    }
}