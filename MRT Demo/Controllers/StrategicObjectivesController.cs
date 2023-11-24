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

        // GET: StrategicObjectives
        public ActionResult Index(int? id, string a)
        {
            var seoplans = db.SEOPlans.Find(id);
            
            ViewBag.SEOPlanID = id;

            var strategicObjectives = db.StrategicObjectives.Where(s => s.IsDelete != true);
            if (a != null)
            {
                strategicObjectives = strategicObjectives.Where(s => s.StrategicObjective1 == a);
            }
            return View(strategicObjectives);
        }

        // GET: StrategicObjectives/Details/5
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

        // GET: StrategicObjectives/Create
        public ActionResult Create(int SEOPlanID)
        {
            StrategicObjective strategicObjective = new StrategicObjective();
            strategicObjective.SEOPlanID = SEOPlanID;

            return View(strategicObjective);
        }

        // POST: StrategicObjectives/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StrategicObjective strategicObjective)
        {
            var last = db.StrategicObjectives.LastOrDefault();

            strategicObjective.IsDelete = false;
            strategicObjective.IsLastDelete = false;
            strategicObjective.CreateDate = DateTime.Now;
            strategicObjective.UpdateDate = DateTime.Now;
            strategicObjective.No = last.No+1;
            db.StrategicObjectives.Add(strategicObjective);
            db.SaveChanges();
            ViewBag.SEOPlanID = new SelectList(db.SEOPlans, "ID", "ID", strategicObjective.SEOPlanID);

            return RedirectToAction("Index", new { id = strategicObjective.SEOPlanID, a = "" });
        }

        // GET: StrategicObjectives/Edit/5
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

        // POST: StrategicObjectives/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,No,StrategicObjective1,SEOPlanID,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsLastDelete")] StrategicObjective strategicObjective)
        {
            if (ModelState.IsValid)
            {
                db.Entry(strategicObjective).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SEOPlanID = new SelectList(db.SEOPlans, "ID", "ID", strategicObjective.SEOPlanID);
            return View(strategicObjective);
        }

        // GET: StrategicObjectives/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
        //    if (strategicObjective == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(strategicObjective);
        //}

        // POST: StrategicObjectives/Delete/5
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StrategicObjective strategicObjective = db.StrategicObjectives.Find(id);
            //db.StrategicObjectives.Remove(strategicObjective);
            strategicObjective.IsDelete = true;
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
    }
}
