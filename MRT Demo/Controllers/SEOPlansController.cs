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
    public class SEOPlansController : BaseController
    {
        // GET: SEOPlans
        public ActionResult Index()
        {
            var seoplans = db.SEOPlans.Where(s => s.IsDelete == false).ToList();
            return View(seoplans);
        }

        // GET: SEOPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SEOPlan sEOPlan = db.SEOPlans.Find(id);
            if (sEOPlan == null)
            {
                return HttpNotFound();
            }
            return View(sEOPlan);
        }


        // GET: SEOPlans/Create
        public ActionResult Create()
        {
            List<int> ListYear = new List<int>();
            for (var i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++) { ListYear.Add(i); }
            var x = ListYear.Select(i => new SelectListItem
            {
                Text = i.ToString(),
                Value = i.ToString()
            });
            SelectList selectListsYear = new SelectList(x,"Value","Text");
            ViewBag.StartYear = selectListsYear;

            return View();
        }

        // POST: SEOPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SEOPlan sEOPlan)
        {
            sEOPlan.CreateDate = DateTime.Now;
            sEOPlan.UpdateDate = DateTime.Now;
            sEOPlan.IsLastDelete = false;
            sEOPlan.IsDelete = false;
            db.SEOPlans.Add(sEOPlan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SEOPlan sEOPlan = db.SEOPlans.Find(id);
            if (sEOPlan == null)
            {
                return HttpNotFound();
            }
            List<int> ListYear = new List<int>();
            for (var i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++) { ListYear.Add(i); }
            var x = ListYear.Select(i => new SelectListItem
            {
                Text = i.ToString(),
                Value = i.ToString()
            });
            SelectList selectListsYear = new SelectList(x,"Value","Text");
            ViewBag.StartYear = selectListsYear;

            return View(sEOPlan);
        }

        // POST: SEOPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,StartYear,EndYear,CreateBy,UpdateBy,CreateDate,UpdateDate,IsDelete,IsLastDelete")] SEOPlan sEOPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sEOPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sEOPlan);
        }

        // GET: SEOPlans/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SEOPlan sEOPlan = db.SEOPlans.Find(id);
        //    if (sEOPlan == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sEOPlan);
        //}

        // POST: SEOPlans/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            SEOPlan sEOPlan = db.SEOPlans.Find(id);
            sEOPlan.IsDelete = true;
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

        public ActionResult RecycleBin()
        {
            var seoplans = db.SEOPlans.Where(s => s.IsDelete == true && s.IsLastDelete==false).ToList();

            return View(seoplans);
        }
        public ActionResult Revert(int id)
        {
            var seo = db.SEOPlans.Find(id);
            seo.IsDelete = false;

            db.Entry(seo).State = EntityState.Modified;

            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
        public ActionResult LastDelete(int id)
        {
            var seo = db.SEOPlans.Find(id);
            seo.IsLastDelete = true;

            db.Entry(seo).State = EntityState.Modified;

            db.SaveChanges();

            return RedirectToAction("RecycleBin");
        }
    }
}
