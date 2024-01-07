using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using MRT_Demo.Models;

namespace MRT_Demo.Controllers
{
    public class GoalsController : Controller
    {
        private MRTEntities db = new MRTEntities();

        public ActionResult Index()
        {
            var goals = db.Goals.Include(g => g.StrategicObjective);
            goals.Where(s => s.IsDelete == false);
            return View(goals.ToList());
        }
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
        public ActionResult Create()
        {
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1");
            return View();
        }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Goal goal)
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
        public ActionResult Update(StrategicObjective strategic)
        {
            UpdateDropdown(strategic);
            ModelState.Clear();
            return View("Manage", strategic);
        }
        private void UpdateDropdown(StrategicObjective strategic)
        {
            List<int> ListIndicatorIDTemp = new List<int>();
            List<int> ListIndicatorUnitIDTemp = new List<int>();

            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (!item2.IsChange)
                    {
                        if (item2.IndicatorID != null || item2.IndicatorUnitID != null)
                        {
                            ListIndicatorIDTemp.Add((int)item2.IndicatorID);
                            ListIndicatorUnitIDTemp.Add((int)item2.IndicatorUnitID);
                        }
                    }
                }
            }

            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (item2.IsChange)
                    {

                        item2.IndicatorUnitBag = db.IndicatorUnits.Where(l => l.IndicatorID == item2.IndicatorID).Select(g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Unit });
                        if (item2.IndicatorID != null)
                        {
                            for (int i = 0; i < ListIndicatorUnitIDTemp.Count; i++)
                            {
                                item2.IndicatorUnitBag = item2.IndicatorUnitBag.Where(b => int.Parse(b.Value) != ListIndicatorUnitIDTemp[i]);
                            }
                        }
                        item2.IndicatorBag = db.Indicators.Select(g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Indicator1 });
                        var x = item2.IndicatorBag.ToList();
                        foreach (var inlist in ListIndicatorIDTemp)
                        {
                            item2.IndicatorBag = item2.IndicatorBag.Where(b => b.Value != inlist.ToString());
                        }
                    }
                    else
                    {
                        item2.IndicatorBag = db.Indicators.Select(g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Indicator1 });
                        item2.IndicatorUnitBag = db.IndicatorUnits.Where(l => l.IndicatorID == item2.IndicatorID).Select(g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Unit });
                    }
                    item2.IsChange = false;
                }
            }
        }
        private void IndicatorUnitBagChanged(StrategicObjective strategic, int unitid)
        {
            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (item2.IndicatorBag != null)
                    {
                        item2.IndicatorUnitBag = db.IndicatorUnits.Where(b => b.IndicatorID == item2.IndicatorID).Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Unit });
                    }
                }
            }
        }
        private void IndicatorBagChanged(StrategicObjective strategic, int indicatorid)
        {
            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (item2.IndicatorID == null) { item2.IndicatorBag = db.Indicators.Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Indicator1 }); }
                    else { item2.IndicatorBag = item2.IndicatorBag.Where(m => int.Parse(m.Value) != indicatorid); }
                }
            }
        }
        public ActionResult Manage(int id)
        {
            var strategic = db.StrategicObjectives.Find(id);
            UpdateDropdown(strategic);

            return View(strategic);
        }
        [HttpPost]
        public ActionResult Manage(StrategicObjective strategic)
        {
            var goal = strategic.Goals;
            SaveGoalToDB(goal);
            db.SaveChanges();
            return RedirectToAction("Index", "StrategicObjectives", new { id = strategic.SEOPlanID });
        }
        private void SaveGoalToDB(ICollection<Goal> goals)
        {
            foreach (var goal in goals)
            {
                var SOEPlanIndicator = goal.SOEPlanIndicator;
                goal.SOEPlanIndicator = new List<SOEPlanIndicator>();
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

                foreach (var goalindicator in SOEPlanIndicator)
                {
                    if (goalindicator.ID == 0)
                    {
                        if (goalindicator.IsDelete == false)
                        {
                            goalindicator.GoalID = goal.ID;
                            db.SOEPlanIndicator.Add(goalindicator);
                        }
                    }
                    else
                    {
                        db.Entry(goalindicator).State = EntityState.Modified;
                    }
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
            goal.IsAddIndiacator = false;
            var last = db.Goals.ToList().LastOrDefault();
            if (last == null)
            {
                last.No = 0;
            }

            goal.No = last.No + 1;
            strategic.Goals.Add(goal);
            UpdateDropdown(strategic);

            ModelState.Clear();
            return View("Manage", strategic);
        }
        public ActionResult DeleteGoal(StrategicObjective strategic)
        {
            UpdateDropdown(strategic);

            return View("Manage", strategic);
        }
        public ActionResult AddIndicator(StrategicObjective strategic)
        {
            foreach (var goal in strategic.Goals)
            {
                if (goal.IsAddIndiacator)
                {
                    SOEPlanIndicator SOEPlanIndicator = new SOEPlanIndicator();
                    SOEPlanIndicator.CreateDate = DateTime.Now;
                    SOEPlanIndicator.UpdateDate = DateTime.Now;
                    SOEPlanIndicator.IsDelete = false;
                    SOEPlanIndicator.IsLastDelete = false;
                    SOEPlanIndicator.IsChange = true;
                    SOEPlanIndicator.GoalID = goal.ID;

                    var last = db.SOEPlanIndicator.ToList().LastOrDefault();
                    if (last == null)
                    {
                        last = new SOEPlanIndicator();
                        last.No = 0;
                    }
                    SOEPlanIndicator.No = last.No + 1;
                    goal.SOEPlanIndicator.Add(SOEPlanIndicator);
                    goal.IsAddIndiacator = false;
                }
            }

            UpdateDropdown(strategic);

            return View("Manage", strategic);
        }
        public ActionResult DelIndicator(StrategicObjective strategic)
        {
            UpdateDropdown(strategic);


            return View("Manage", strategic);
        }

        private void IndicatorBag(StrategicObjective strategic)
        {
            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (item2.IndicatorBag == null) { item2.IndicatorBag = db.Indicators.Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Indicator1 }); }
                    //else { item2.IndicatorBag = item2.IndicatorBag.Where(m=>m.Value != indicatorid)}
                }
            }
        }
        private void IndicatorUnitBag(StrategicObjective strategic)
        {
            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (item2.IndicatorBag != null)
                    {
                        item2.IndicatorUnitBag = db.IndicatorUnits.Where(b => b.IndicatorID == item2.IndicatorID).Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Unit });
                    }
                }
            }
        }
    }
}
