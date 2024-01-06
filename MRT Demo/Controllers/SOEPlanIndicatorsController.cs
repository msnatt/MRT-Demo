using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MRT_Demo.Models;

namespace MRT_Demo.Controllers
{
    public class SOEPlanIndicatorsController : Controller
    {
        private MRTEntities db = new MRTEntities();
        public ActionResult Index()
        {
            var sOEPlanIndicator = db.SOEPlanIndicator.Include(s => s.Goal).Include(s => s.Indicator);
            return View(sOEPlanIndicator.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlanIndicator sOEPlanIndicator = db.SOEPlanIndicator.Find(id);
            if (sOEPlanIndicator == null)
            {
                return HttpNotFound();
            }
            return View(sOEPlanIndicator);
        }
        public ActionResult Create()
        {
            ViewBag.GoalID = new SelectList(db.Goals, "ID", "Goal1");
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Indicator1");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SOEPlanIndicator sOEPlanIndicator)
        {
            if (ModelState.IsValid)
            {
                db.SOEPlanIndicator.Add(sOEPlanIndicator);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GoalID = new SelectList(db.Goals, "ID", "Goal1", sOEPlanIndicator.GoalID);
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Indicator1", sOEPlanIndicator.IndicatorID);
            return View(sOEPlanIndicator);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlanIndicator sOEPlanIndicator = db.SOEPlanIndicator.Find(id);
            if (sOEPlanIndicator == null)
            {
                return HttpNotFound();
            }
            ViewBag.GoalID = new SelectList(db.Goals, "ID", "Goal1", sOEPlanIndicator.GoalID);
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Indicator1", sOEPlanIndicator.IndicatorID);
            return View(sOEPlanIndicator);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SOEPlanIndicator sOEPlanIndicator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sOEPlanIndicator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GoalID = new SelectList(db.Goals, "ID", "Goal1", sOEPlanIndicator.GoalID);
            ViewBag.IndicatorID = new SelectList(db.Indicators, "ID", "Indicator1", sOEPlanIndicator.IndicatorID);
            return View(sOEPlanIndicator);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlanIndicator sOEPlanIndicator = db.SOEPlanIndicator.Find(id);
            if (sOEPlanIndicator == null)
            {
                return HttpNotFound();
            }
            return View(sOEPlanIndicator);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SOEPlanIndicator sOEPlanIndicator = db.SOEPlanIndicator.Find(id);
            db.SOEPlanIndicator.Remove(sOEPlanIndicator);
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

        //---------------------------- Target -------------------------------
        public ActionResult Target(int? id)
        {
            var strategic = db.StrategicObjectives.Find(id);

            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    var indicator = db.Indicators.Find(item2.IndicatorID);
                    item2.SubTarget = db.ImportantIndicatorTargetMeasurements.Where(b => b.IndicatorID == item2.IndicatorID && b.IndicatorLevel == 0 && b.IndicatorUnitID == item2.IndicatorUnitID).ToList();
                    if (item2.SubTarget.Count == 0)
                    {
                        item2.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                        for (var i = strategic.SEOPlan.StartYear; i < strategic.SEOPlan.EndYear + 1; i++)
                        {
                            ImportantIndicatorTargetMeasurement targetMeasurement = new ImportantIndicatorTargetMeasurement();
                            targetMeasurement.CreateDate = DateTime.Now;
                            targetMeasurement.UpdateDate = DateTime.Now;
                            targetMeasurement.IsDelete = false;
                            targetMeasurement.IsLastDelete = false;
                            targetMeasurement.IndicatorLevel = 0;
                            targetMeasurement.IndicatorID = indicator.ID;
                            targetMeasurement.Indicator = indicator;
                            targetMeasurement.GoalID = item.ID;
                            targetMeasurement.Year = i;

                            item2.SubTarget.Add(targetMeasurement);
                        }
                    }
                    else
                    {
                        foreach (var itemtarget in item2.SubTarget)
                        {
                            itemtarget.GoalID = item.ID;
                            itemtarget.Indicator = indicator;
                            itemtarget.IndicatorID = indicator.ID;
                        }
                    }

                }
            }

            IndicatorBag(strategic);
            IndicatorUnitBag(strategic);
            return View(strategic);
        }
        [HttpPost]
        public ActionResult Target(StrategicObjective strategic)
        {
            var goal = strategic.Goals;
            foreach (var item in goal)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    item2.Indicator = db.Indicators.Find(item2.IndicatorID);
                    foreach (var item3 in item2.SubTarget)
                    {
                        if (item3.ID == 0)
                        {
                            item3.IndicatorUnitID = item2.IndicatorUnitID;
                            db.ImportantIndicatorTargetMeasurements.Add(item3);
                        }
                        else
                        {
                            item3.IndicatorUnitID = item2.IndicatorUnitID;
                            db.Entry(item3).State = EntityState.Modified;
                        }
                    }
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index", "StrategicObjectives", new { id = strategic.SEOPlanID });
        }
        private void IndicatorBag(StrategicObjective strategic)
        {
            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    item2.IndicatorBag = db.Indicators.Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Indicator1 });
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
