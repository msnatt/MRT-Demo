using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MRT_Demo.Models;

namespace MRT_Demo.Controllers
{
    public class GoalsController : Controller
    {
        private MRTEntities db = new MRTEntities();

        // GET: Goals
        public ActionResult Index()
        {
            var goals = db.Goals.Include(g => g.StrategicObjective);
            goals.Where(s => s.IsDelete == false);
            return View(goals.ToList());
        }

        // GET: Goals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        // GET: Goals/Create
        public ActionResult Create()
        {
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1");
            return View();
        }

        // POST: Goals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Goal goal)
        {
            if (ModelState.IsValid)
            {
                db.Goals.Add(goal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", goal.StrategicObjectiveID);
            return View(goal);
        }

        // GET: Goals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", goal.StrategicObjectiveID);
            return View(goal);
        }

        // POST: Goals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,No,Goal1,StrategicObjectiveID,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsLastDelete")] Goal goal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(goal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", goal.StrategicObjectiveID);
            return View(goal);
        }

        // GET: Goals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goals.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Goal goal = db.Goals.Find(id);
            db.Goals.Remove(goal);
            db.SaveChanges();
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

        public ActionResult Manage(int id)
        {
            var strategicObjective = db.StrategicObjectives.Find(id);

            return View(strategicObjective);
        }
        [HttpPost]
        public ActionResult Manage(StrategicObjective strategic)
        {
            var goal = strategic.Goals;
            SaveGoalToDB(goal);
            db.SaveChanges();
            return RedirectToAction("Index", "StrategicObjectives", new { id = strategic.SEOPlanID});
        }

        private void SaveGoalToDB(ICollection<Goal> goals)
        {
            foreach (var goal in goals)
            {
                if (goal.ID == 0)
                {
                    if (goal.IsDelete == false)
                    {
                        db.Goals.Add(goal);
                    }
                }
                else
                {
                    db.Entry(goal).State = EntityState.Modified;
                }
            }
        }

        public ActionResult AddGoal(StrategicObjective strategic)
        {

            Goal goal = new Goal();
            goal.CreateDate = DateTime.Now;
            goal.UpdateDate = DateTime.Now;
            goal.IsDelete = false;
            goal.IsLastDelete = false;
            goal.StrategicObjectiveID = strategic.ID;
            var last = db.Goals.ToList().LastOrDefault();
            if (last == null)
            {
                last.No = 0;
            }

            goal.No = last.No + 1;
            strategic.Goals.Add(goal);
            ModelState.Clear();
            return View("Manage", strategic);
        }
        public ActionResult DeleteGoal(StrategicObjective strategic)
        {

            return View("Manage", strategic);
        }

    }
}
