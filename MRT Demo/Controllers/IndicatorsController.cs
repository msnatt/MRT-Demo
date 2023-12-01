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
    public class IndicatorsController : Controller
    {
        private MRTEntities db = new MRTEntities();

        public ActionResult Index()
        {
            var indicators = db.Indicators.Include(i => i.IndicatorDetailStatus).Where(s => s.IsDelete == false);
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
            //ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status");
            Indicator indicator = new Indicator();
            indicator.CreateDate = DateTime.Now;
            indicator.UpdateDate = DateTime.Now;
            indicator.IsDelete = false;
            indicator.IsLastDelete = false;
            indicator.IsActive = false;

            List<IndicatorType> indicatorType = db.IndicatorTypes.ToList();
            indicator.IndicatorXIndicatorTypes = new List<IndicatorXIndicatorType>();
            foreach (var item in indicatorType)
            {
                IndicatorXIndicatorType indicatorXIndicatorType = new IndicatorXIndicatorType();
                indicatorXIndicatorType.IndicatorTypeID = item.ID;
                //indicatorXIndicatorType.IndicatorID = indicator.ID;
                indicatorXIndicatorType.CreateDate = DateTime.Now;
                indicatorXIndicatorType.UpdateDate = DateTime.Now;
                indicatorXIndicatorType.IsDelete = false;
                indicatorXIndicatorType.IsLastDelete = false;
                indicatorXIndicatorType.IndicatorType = item;
                indicator.IndicatorXIndicatorTypes.Add(indicatorXIndicatorType);
            }

            ViewBag.SelectListStatus = indicatorDetailStatuses();

            return View(indicator);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Indicator indicator)
        {
            var x = indicator.IndicatorXIndicatorTypes.ToList();
            indicator.IndicatorXIndicatorTypes = null;
            db.Indicators.Add(indicator);
            //db.SaveChanges();
            foreach (var temp in x)
            {
                temp.IndicatorType = null;
                temp.IndicatorID = indicator.ID;
                db.IndicatorXIndicatorTypes.Add(temp);
                db.SaveChanges();

            }
            return RedirectToAction("Index");

            //ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status", indicator.IndicatorDetailStatusID);
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
            //ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status", indicator.IndicatorDetailStatusID);
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View(indicator);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Indicator indicator)
        {
            var Owner = indicator.IndicatorOwners.ToList();
            var Unit = indicator.IndicatorUnits.ToList();
            var Type = indicator.IndicatorXIndicatorTypes.ToList();
            indicator.IndicatorOwners = null;
            indicator.IndicatorUnits = null;
            indicator.IndicatorXIndicatorTypes = null;
            indicator.UpdateDate = DateTime.Now;
            db.Entry(indicator).State = EntityState.Modified;
            foreach (var OwnerItem in Owner)
            {
                if (OwnerItem.ID == 0)
                {
                    OwnerItem.IndicatorID = indicator.ID;
                    db.IndicatorOwners.Add(OwnerItem);
                    db.SaveChanges();
                }
                else
                {
                    OwnerItem.UpdateDate = DateTime.Now;
                    db.Entry(OwnerItem).State = EntityState.Modified;
                }
            }
            foreach (var UnitItem in Unit)
            {
                if (UnitItem.ID == 0)
                {
                    UnitItem.IndicatorID = indicator.ID;
                    db.IndicatorUnits.Add(UnitItem);
                    db.SaveChanges();
                }
                else
                {
                    UnitItem.UpdateDate= DateTime.Now;
                    db.Entry(UnitItem).State = EntityState.Modified;
                }
            }
            foreach (var TypeItem in Type)
            {
                if (TypeItem.ID == 0)
                {

                    TypeItem.IndicatorType = null;
                    TypeItem.IndicatorID = indicator.ID;
                    db.IndicatorXIndicatorTypes.Add(TypeItem);
                    db.SaveChanges();
                }
                else
                {
                    TypeItem.UpdateDate = DateTime.Now;
                    TypeItem.IndicatorType = null;
                    db.Entry(TypeItem).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
            //ViewBag.IndicatorDetailStatusID = new SelectList(db.IndicatorDetailStatus, "ID", "Status", indicator.IndicatorDetailStatusID);
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
            //db.Indicators.Remove(indicator);
            indicator.IsDelete = true;
            db.SaveChanges();
            ViewBag.SelectListStatus = indicatorDetailStatuses();

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
        public SelectList indicatorDetailStatuses()
        {
            var x = db.IndicatorDetailStatus.Select(i => new SelectListItem
            {
                Text = i.Status,
                Value = i.ID.ToString()
            });
            SelectList selectListStatus = new SelectList(x, "Value", "Text");

            return selectListStatus;
        }

        // ========================= Owner ==========================
        public ActionResult AddIndicatorOwnerEdit(Indicator indicator)
        {
            VoidAddIndicatorOwner(indicator);
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("Edit", indicator);
        }
        public ActionResult AddIndicatorOwner(Indicator indicator)
        {
            VoidAddIndicatorOwner(indicator);
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("Create", indicator);
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
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            ModelState.Clear();
            return View("Create", indicator);
        }
        public ActionResult DelIndicatorOwnerEdit(Indicator indicator)
        {
            VoidDelIndicatorOwner(indicator);
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            ModelState.Clear();
            return View("Edit", indicator);
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

        // ======================= Unit ===========================
        public ActionResult AddIndicatorUnit(Indicator indicator)
        {
            VoidAddIndicatorUnit(indicator);
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("Create", indicator);
        }
        public ActionResult AddIndicatorUnitEdit(Indicator indicator)
        {
            VoidAddIndicatorUnit(indicator);
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("Edit", indicator);
        }
        private void VoidAddIndicatorUnit(Indicator indicator)
        {
            IndicatorUnit indicatorUnit = new IndicatorUnit();
            indicatorUnit.CreateDate = DateTime.Now;
            indicatorUnit.UpdateDate = DateTime.Now;
            indicatorUnit.IsDelete = false;
            indicatorUnit.IsLastDelete = false;
            indicator.IndicatorUnits.Add(indicatorUnit);
        }

        public ActionResult DelIndicatorUnit(Indicator indicator)
        {
            VoidDelIndicatorUnit(indicator);
            ModelState.Clear();
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("Create", indicator);
        }
        public ActionResult DelIndicatorUnitEdit(Indicator indicator)
        {
            VoidDelIndicatorUnit(indicator);
            ModelState.Clear();
            ViewBag.SelectListStatus = indicatorDetailStatuses();
            return View("Edit", indicator);
        }


        private void VoidDelIndicatorUnit(Indicator indicator)
        {
            foreach (var item in indicator.IndicatorUnits)
            {
                if (item.IsDelete == true)
                {
                    if (item.ID == 0)
                    {
                        var temp = indicator.IndicatorUnits.ToList();
                        temp.Remove(item);
                        indicator.IndicatorUnits = temp;
                    }
                }
            }
        }
    }
}
