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
        public ActionResult Index(int? id)
        {
            var strategic = db.StrategicObjectives.Find(id);
            strategic.Stategies = strategic.Stategies.Where(s => s.IsDelete == false).ToList();
            return View(strategic);
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
        public ActionResult Create(int? id)
        {

            //ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1");

            Stategy stategy = new Stategy();
            stategy.CreateDate = DateTime.Now;
            stategy.UpdateDate = DateTime.Now;
            stategy.IsDelete = false;
            stategy.IsLastDelete = false;
            var last = db.Stategies.ToList().LastOrDefault();
            if (last == null)
            {
                last = new Stategy();
                last.No = 0;
            }
            stategy.No = last.No + 1;
            stategy.StrategicObjectiveID = id;

            return View(stategy);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Stategy stategy)
        {
            foreach (var item in stategy.Tactics)
            {
                if ((bool)!item.IsDelete)
                {
                    db.Tactics.Add(item);
                }
            }
            stategy.Tactics = null;
            db.Stategies.Add(stategy);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = stategy.StrategicObjectiveID });

            //ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", stategy.StrategicObjectiveID);
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
            //ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", stategy.StrategicObjectiveID);

            stategy.Tactics = stategy.Tactics.Where(s => s.IsDelete == false).ToList();
            return View(stategy);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stategy stategy)
        {
            foreach (var item in stategy.Tactics)
            {
                if (item.ID == 0)
                {
                    db.Tactics.Add(item);
                }
                else
                {
                    db.Entry(item).State = EntityState.Modified;
                }
            }
            db.Entry(stategy).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = stategy.StrategicObjectiveID });
            //ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", stategy.StrategicObjectiveID);
        }

        // GET: Stategies/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Stategy stategy = db.Stategies.Find(id);
        //    if (stategy == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(stategy);
        //}

        // POST: Stategies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stategy stategy = db.Stategies.Find(id);
            //db.Stategies.Remove(stategy);
            stategy.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index", new { id = stategy.StrategicObjectiveID });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult AddTactic(Stategy stategy)
        {
            VoidAddTactic(stategy);
            return View("Create", stategy);
        }
        public ActionResult AddTacticEdit(Stategy stategy)
        {
            VoidAddTactic(stategy);
            return View("Edit", stategy);

        }
        private void VoidAddTactic(Stategy stategy)
        {
            if (stategy.isAddHere)
            {
                Tactic tactic = new Tactic();
                tactic.IsDelete = false;
                tactic.IsLastDelete = false;
                tactic.UpdateDate = DateTime.Now;
                tactic.CreateDate = DateTime.Now;
                tactic.StategyID = stategy.ID;
                var lastTac = db.Tactics.ToList().LastOrDefault();
                if (lastTac == null)
                {
                    lastTac = new Tactic();
                    lastTac.No = 0;
                }
                tactic.No = lastTac.No + stategy.Tactics.Count + 1;
                stategy.Tactics.Add(tactic);

            }

        }

        public ActionResult DelTacticEdit(int? id, Stategy stategy)
        {
            return View("Edit", stategy);
        }

        public ActionResult DelTactic(int? id, Stategy stategy)
        {
            return View("Create", stategy);
        }

        public ActionResult RecycleBin(int strategicID)
        {
            var stategies = db.Stategies.Where(s => s.IsDelete == true && s.StrategicObjectiveID == strategicID && s.IsLastDelete == false).ToList();
            //ViewBag.SEOPlanID = id;
            return View(stategies);
        }
        public ActionResult Revert(int id)
        {
            var stategy = db.Stategies.Find(id);
            stategy.IsDelete = false;

            db.Entry(stategy).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("RecycleBin", new { strategicID = stategy.StrategicObjectiveID });
        }
        public ActionResult LastDelete(int id)
        {
            var stategy = db.Stategies.Find(id);
            stategy.IsLastDelete = true;

            db.Entry(stategy).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("RecycleBin", new { strategicID = stategy.StrategicObjectiveID });
        }



    }
}
