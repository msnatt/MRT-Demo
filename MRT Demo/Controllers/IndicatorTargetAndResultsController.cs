using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MRT_Demo.Models;

namespace MRT_Demo.Controllers
{
    public class IndicatorTargetAndResultsController : Controller
    {
        private MRTEntities db = new MRTEntities();
        public ActionResult Index(int? Year, string Division, string Indicator)
        {
            if (Division == "") { Division = null; }
            if (Indicator == "") { Indicator = null; }
            var indicators = db.Indicators.Where(s => s.IsDelete == false);
            foreach (var indicator in indicators) { indicator.indicatorYear = Year; };
            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }
            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Where(q => q.Division == Division).Count() > 0);
            }
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            return View(indicators.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }

        public ActionResult Create()
        {

            return View();
        }
        public ActionResult Create(Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                db.Indicators.Add(indicator);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status", indicator.IndicatorDetailStatusID);
            return View(indicator);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status", indicator.IndicatorDetailStatusID);
            return View(indicator);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Indicator indicator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(indicator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status", indicator.IndicatorDetailStatusID);
            return View(indicator);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indicator indicator = db.Indicators.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            return View(indicator);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            Indicator indicator = db.Indicators.Find(id);
            db.Indicators.Remove(indicator);
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
        public SelectList IndicatorYearSelectItem()
        {
            List<int> YearList = new List<int>();
            for (var i = DateTime.Now.Year - 10; i < DateTime.Now.Year + 10; i++)
            {
                YearList.Add(i);
            }

            var Year = YearList.Select(i => new SelectListItem
            {
                Text = i.ToString(),
                Value = i.ToString()
            });
            SelectList YearSelectList = new SelectList(Year, "Value", "Text");

            return YearSelectList;
        }
        public SelectList IndicatorOwnerSelectItem()
        {
            var Owner = db.DataOwner.Select(i => new SelectListItem
            {
                Text = i.Division,
                Value = i.Division
            });
            SelectList selectListOwners = new SelectList(Owner, "Value", "Text");

            return selectListOwners;
        }

        public ActionResult Target(int id, string Division)
        {
            var indicators = db.Indicators.Find(id);
            if (indicators.ImportantIndicatorTargetMeasurement.Count == 0)
            {
                foreach (var item in db.IndicatorXIndicatorTypes.Where(b =>b.IndicatorID == indicators.ID))
                {
                    var breaker = 0;
                    var important = new ImportantIndicatorTargetMeasurement();
                    important.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                    for (var i = 0; i < 5; i++)
                    {
                        var importantTemp = new ImportantIndicatorTargetMeasurement();
                        importantTemp.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                        importantTemp.IndicatorTypeID = item.IndicatorTypeID;
                        importantTemp.IsDelete = false;
                        importantTemp.IsLastDelete = false;
                        importantTemp.IndicatorID = indicators.ID;
                        importantTemp.CreateDate = DateTime.Now;
                        importantTemp.UpdateDate = DateTime.Now;
                        importantTemp.IndicatorLevel = i + 1;
                        importantTemp.SubTarget.Add(importantTemp);
                        if (breaker <= 4)
                        {
                            important.SubTarget.Add(importantTemp);
                            importantTemp.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                        }
                    }
                    indicators.ImportantIndicatorTargetMeasurement.Add(important);
                }
            }
            else
            {

            }

            var x = indicators.IndicatorXIndicatorTypes.ToList();
            var y = db.IndicatorTypes.ToList();
            foreach (var item in x)
            {
                foreach (var item2 in y)
                {
                    if (item.IndicatorTypeID == item2.ID)
                    {
                        item.IndicatorType = item2;
                    }
                }
            }

            var a = indicators.IndicatorUnits;
            ViewBag.Year = indicators.indicatorYear;
            ViewBag.Division = Division;
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");

            //var z = indicators.ImportantIndicatorTargetMeasurement.Where(b=>b.IndicatorTypeID == indicators.IndicatorXIndicatorTypes.First().IndicatorTypeID).ToList();
            return View("Target", indicators);
        }

        [HttpPost]
        public ActionResult Target(Indicator indicators)
        {
            var important = indicators.ImportantIndicatorTargetMeasurement.ToList();
            indicators.ImportantIndicatorTargetMeasurement = null;
            indicators.IndicatorXIndicatorTypes = null;
            indicators.IndicatorOwners = null;
            db.Entry(indicators).State = EntityState.Modified;
            foreach (var item in important)
            {
                if (item.ID == 0)
                {
                    foreach (var item2 in item.SubTarget)
                    {
                        if (item2.ID == 0)
                        {
                            item2.IndicatorUnitID = item.IndicatorUnitID;
                            db.ImportantIndicatorTargetMeasurements.Add(item2);
                        }
                        //else
                        //{
                        //    db.Entry(item2).State = EntityState.Modified;
                        //}
                    }
                }
                else
                {
                    foreach (var item2 in item.SubTarget)
                    {
                        if (item2.ID == 0)
                        {
                            item2.IndicatorUnitID = item.IndicatorUnitID;
                            db.ImportantIndicatorTargetMeasurements.Add(item2);
                        }
                        else
                        {
                            db.Entry(item2).State = EntityState.Modified;
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { id = indicators.indicatorYear });
        }

        // ======================== Add Owner =========================
        public ActionResult AddIndicatorOwner(Indicator indicator)
        {
            VoidAddIndicatorOwner(indicator);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            return View("Target", indicator);
        }
        private void VoidAddIndicatorOwner(Indicator indicator)
        {
            IndicatorOwner indicatorOwner = new IndicatorOwner();
            indicatorOwner.CreateDate = DateTime.Now;
            indicatorOwner.UpdateDate = DateTime.Now;
            indicatorOwner.IsDelete = false;
            indicatorOwner.IsLastDelete = false;
            indicator.IndicatorOwners.Add(indicatorOwner);
        }
        public ActionResult DelIndicatorOwner(Indicator indicator)
        {
            VoidDelIndicatorOwner(indicator);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ModelState.Clear();
            return View("Target", indicator);
        }
        private void VoidDelIndicatorOwner(Indicator indicator)
        {
            foreach (var item in indicator.IndicatorOwners)
            {
                if (item.IsDelete == true)
                {
                    if (item.ID == 0)
                    {
                        var temp = indicator.IndicatorOwners.ToList();
                        temp.Remove(item);
                        indicator.IndicatorOwners = temp;
                    }
                }
            }
        }

        public ActionResult AddXType(Indicator indicator)
        {
            IndicatorXIndicatorType indicatorXIndicatorType = new IndicatorXIndicatorType();
            indicatorXIndicatorType.CreateDate = DateTime.Now;
            indicatorXIndicatorType.UpdateDate = DateTime.Now;
            indicatorXIndicatorType.IsDelete = false;
            indicatorXIndicatorType.IsLastDelete = false;
            indicatorXIndicatorType.IndicatorID = indicator.ID;
            indicator.IndicatorXIndicatorTypes.Add(indicatorXIndicatorType);

            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicator.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            return View("Target", indicator);
        }
    }
}
