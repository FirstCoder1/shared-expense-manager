using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ApplicationDataAccess;

namespace SharedExpenseManager.Controllers
{
    public class UserLogInController : Controller
    {
        private SharedExpenseDBEntities db = new SharedExpenseDBEntities();

        // GET: UserLogIns
        public async Task<ActionResult> Index()
        {
            return View(await db.UserLogIns.ToListAsync());
        }

        // GET: UserLogIns/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLogIn userLogIn = await db.UserLogIns.FindAsync(id);
            if (userLogIn == null)
            {
                return HttpNotFound();
            }
            return View(userLogIn);
        }

        // GET: UserLogIns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserLogIns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LogInId,UserName,Password,CreateDate,LastUpdate,IsActive")] UserLogIn userLogIn)
        {
            if (ModelState.IsValid)
            {
                db.UserLogIns.Add(userLogIn);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(userLogIn);
        }

        // GET: UserLogIns/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLogIn userLogIn = await db.UserLogIns.FindAsync(id);
            if (userLogIn == null)
            {
                return HttpNotFound();
            }
            return View(userLogIn);
        }

        // POST: UserLogIns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LogInId,UserName,Password,CreateDate,LastUpdate,IsActive")] UserLogIn userLogIn)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userLogIn).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(userLogIn);
        }

        // GET: UserLogIns/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLogIn userLogIn = await db.UserLogIns.FindAsync(id);
            if (userLogIn == null)
            {
                return HttpNotFound();
            }
            return View(userLogIn);
        }

        // POST: UserLogIns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UserLogIn userLogIn = await db.UserLogIns.FindAsync(id);
            db.UserLogIns.Remove(userLogIn);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
