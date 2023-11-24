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
    public class StategiesController : Controller
    {
        private MRTEntities db = new MRTEntities();

        // GET: Stategies
        public ActionResult Index()
        {
            var stategies = db.Stategies.Include(s => s.StrategicObjective);
            return View(stategies.ToList());
        }

        // GET: Stategies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stategy stategy = db.Stategies.Find(id);
            if (stategy == null)
            {
                return HttpNotFound();
            }
            return View(stategy);
        }

        // GET: Stategies/Create
        public ActionResult Create()
        {
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1");
            return View();
        }

        // POST: Stategies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,No,Stategy1,StrategicObjectiveID,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsLastDelete")] Stategy stategy)
        {
            if (ModelState.IsValid)
            {
                db.Stategies.Add(stategy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", stategy.StrategicObjectiveID);
            return View(stategy);
        }

        // GET: Stategies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stategy stategy = db.Stategies.Find(id);
            if (stategy == null)
            {
                return HttpNotFound();
            }
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", stategy.StrategicObjectiveID);
            return View(stategy);
        }

        // POST: Stategies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,No,Stategy1,StrategicObjectiveID,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsLastDelete")] Stategy stategy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stategy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", stategy.StrategicObjectiveID);
            return View(stategy);
        }

        // GET: Stategies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stategy stategy = db.Stategies.Find(id);
            if (stategy == null)
            {
                return HttpNotFound();
            }
            return View(stategy);
        }

        // POST: Stategies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stategy stategy = db.Stategies.Find(id);
            db.Stategies.Remove(stategy);
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
