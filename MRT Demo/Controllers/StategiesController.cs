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
    public class StategiesController : BaseController
    {
        public ActionResult Index(int? id)
        {
            //ดึงจาก Database
            var strategic = db.StrategicObjectives.Find(id);

            //กรอกเอาเฉพาะตัวที่ IsDelete เป็น false
            strategic.Stategies = strategic.Stategies.Where(s => s.IsDelete == false).ToList();
            return View(strategic);
        }
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
        public ActionResult Create(int? id)
        {
            Stategy stategy = new Stategy();
            stategy.StrategicObjectiveID = id;
            stategy.Insert(db);

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
        }
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
        }
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
                tactic.StategyID = stategy.ID;
                var lastTac = db.Tactics.ToList().LastOrDefault();
                if (lastTac == null)
                {
                    lastTac = new Tactic();
                    lastTac.No = 0;
                }
                tactic.No = lastTac.No + stategy.Tactics.Count + 1;
                tactic.Insert(db);
                stategy.Tactics.Add(tactic);

            }

        }
        public ActionResult DelTacticEdit(Stategy stategy)
        {
            return View("Edit", stategy);
        }
        public ActionResult DelTactic(Stategy stategy)
        {
            return View("Create", stategy);
        }
        public ActionResult RecycleBin(int strategicID)
        {
            //กรอกเอาเฉพาะ IsDelete เป็น True และ IsLastDelete เป็น false
            var stategies = db.Stategies.Where(s => s.IsDelete == true && s.StrategicObjectiveID == strategicID && s.IsLastDelete == false).ToList();
            return View(stategies);
        }
        public ActionResult Revert(int id)
        {
            var stategy = db.Stategies.Find(id);

            db.Entry(stategy).State = EntityState.Modified;
            stategy.IsDelete = false;
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
