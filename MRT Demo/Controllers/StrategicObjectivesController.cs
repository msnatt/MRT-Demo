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
    public class StrategicObjectivesController : Controller
    {
        private MRTEntities db = new MRTEntities();

        public ActionResult Index(int? id, string a)
        {
            var seoplans = db.SEOPlans.Find(id);
            ViewBag.SYandEY = seoplans.StartEndYear;
            ViewBag.SEOPlanID = id;

            var strategicObjectives = db.StrategicObjectives.Where(s => s.IsDelete != true && s.SEOPlanID == id);
            if (a != null)
            {
                strategicObjectives = strategicObjectives.Where(s => s.StrategicObjective1.Contains(a));
            }
            return View(strategicObjectives);
        }
        [HttpPost]
        public ActionResult SearchTextFunc(string isAcc, StrategicObjective strategic)
        {

            return RedirectToAction("Index", new { id = strategic.SEOPlanID, a = isAcc });
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }
            return View(strategicObjective);
        }

        public ActionResult Create(int SEOPlanID)
        {
            StrategicObjective strategicObjective = new StrategicObjective();
            strategicObjective.SEOPlanID = SEOPlanID;

            return View(strategicObjective);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StrategicObjective strategicObjective)
        {
            var last = db.StrategicObjectives.ToList().LastOrDefault();
            if (last == null)
            {
                last = new StrategicObjective();
                last.No = 0;
            }
            strategicObjective.IsDelete = false;
            strategicObjective.IsLastDelete = false;
            strategicObjective.CreateDate = DateTime.Now;
            strategicObjective.UpdateDate = DateTime.Now;
            strategicObjective.No = last.No + 1;
            db.StrategicObjectives.Add(strategicObjective);
            db.SaveChanges();
            ViewBag.SEOPlanID = new SelectList(db.SEOPlans, "ID", "ID", strategicObjective.SEOPlanID);

            return RedirectToAction("Index", new { id = strategicObjective.SEOPlanID, a = "" });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }
            ViewBag.SEOPlanID = new SelectList(db.SEOPlans, "ID", "ID", strategicObjective.SEOPlanID);
            return View(strategicObjective);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,No,StrategicObjective1,SEOPlanID,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsLastDelete")] StrategicObjective strategicObjective)
        {
            if (ModelState.IsValid)
            {
                db.Entry(strategicObjective).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = strategicObjective.SEOPlanID });
            }
            ViewBag.SEOPlanID = new SelectList(db.SEOPlans, "ID", "ID", strategicObjective.SEOPlanID);
            return View(strategicObjective);
        }
        public ActionResult DeleteConfirmed(int id)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            strategicObjective.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = strategicObjective.SEOPlanID, a = "" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult RecycleBin(int SEOPlanID)
        {
            var strategic = db.StrategicObjectives.Where(s => s.IsDelete == true && s.SEOPlanID == SEOPlanID && s.IsLastDelete == false).ToList();
            ViewBag.SEOPlanID = SEOPlanID;
            return View(strategic);
        }
        public ActionResult Revert(int id)
        {
            var strategic = db.StrategicObjectives.Find(id);
            strategic.IsDelete = false;

            db.Entry(strategic).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("RecycleBin", new { SEOPlanID = strategic.SEOPlanID });
        }
        public ActionResult LastDelete(int id)
        {
            var strategic = db.StrategicObjectives.Find(id);
            strategic.IsLastDelete = true;

            db.Entry(strategic).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("RecycleBin", new { SEOPlanID = strategic.SEOPlanID });
        }
    }
}
