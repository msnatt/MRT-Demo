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
    public class GoalsController : BaseController
    {
        public ActionResult Index()
        {
            var goals = db.Goals.Include(g => g.StrategicObjective);
            goals.Where(s => s.IsDelete == false);
            return View(goals.ToList());
        }
        //public ActionResult Create()
        //{
        //    ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1");
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Goal goal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Goals.Add(goal);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", goal.StrategicObjectiveID);
        //    return View(goal);
        //}
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Goal goal = db.Goals.Find(id);
        //    if (goal == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", goal.StrategicObjectiveID);
        //    return View(goal);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Goal goal)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(goal).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.StrategicObjectiveID = new SelectList(db.StrategicObjectives, "ID", "StrategicObjective1", goal.StrategicObjectiveID);
        //    return View(goal);
        //}
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
            //อัพเดต Dropdown ของ Indicator
            UpdateDropdown(strategic);
            ModelState.Clear();
            return View("Manage", strategic);
        }
        private void UpdateDropdown(StrategicObjective strategic)
        {
            //Declure Variable
            List<int> ListAllIndicatorID = new List<int>();
            List<int> ListAllIndicatorUnitID = new List<int>();
            List<int> ListOnChangeIndicatorID = new List<int>();
            List<int> ListOnChangeIndicatorUnitID = new List<int>();
            List<int> ListIndicatorIDTemp = new List<int>();
            List<int> ListIndicatorUnitIDTemp = new List<int>();
            //==============================================================//

            //แยกข้อมูลเข้า List ที่สร้างไว้
            foreach (var item in db.IndicatorUnits)
            {
                ListAllIndicatorID.Add((int)item.IndicatorID);
                ListAllIndicatorUnitID.Add(item.ID);
                ListOnChangeIndicatorID.Add((int)item.IndicatorID);
                ListOnChangeIndicatorUnitID.Add(item.ID);
            }

            //เพิ่ม IndicatorID และ UnitID ไปยัง ListTemp
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


            //เช็คไม่ซ้ำกัน UnitID และ IndicatorID
            foreach (var item in ListAllIndicatorID)
            {
                foreach (var item2 in ListIndicatorIDTemp.ToList())
                {
                    if (item == item2)
                    {
                        ListOnChangeIndicatorID.Remove(item2);
                        ListIndicatorIDTemp.Remove(item2);
                    }
                }
            }
            foreach (var item in ListAllIndicatorUnitID)
            {
                foreach (var item2 in ListIndicatorUnitIDTemp)
                {
                    if (item == item2)
                    {
                        ListOnChangeIndicatorUnitID.Remove(item2);
                    }
                }
            }

            //นำ List ที่เก็บค่าไม่ซ้ำไปใช้งาน
            ListIndicatorIDTemp = ListOnChangeIndicatorID;
            ListIndicatorUnitIDTemp = ListOnChangeIndicatorUnitID;

            ListIndicatorIDTemp = ListIndicatorIDTemp.Distinct().ToList();


            // ============= End check unique indicator & Unit ===============


            foreach (var item in strategic.Goals)
            {
                foreach (var item2 in item.SOEPlanIndicator)
                {
                    if (item2.IsChange)
                    {

                        if (ListIndicatorUnitIDTemp.Count > 0)
                        {
                            List<SelectListItem> temp = new List<SelectListItem>();
                            if (item2.IndicatorID != null)
                            {
                                foreach (var item3 in ListIndicatorUnitIDTemp)
                                {
                                    foreach (var item4 in db.IndicatorUnits.Where(b => b.ID == item3 && b.IndicatorID == item2.IndicatorID).Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Unit }))
                                    {
                                        temp.Add(item4);
                                    }
                                }
                            }
                            item2.IndicatorUnitBag = temp;
                        }
                        else
                        {
                            item2.IndicatorUnitBag = db.IndicatorUnits.Where(l => l.IndicatorID == item2.IndicatorID).Select(g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Unit });
                        }
                        if (ListIndicatorIDTemp.Count > 0)
                        {
                            List<SelectListItem> temp = new List<SelectListItem>();
                            foreach (var item3 in ListIndicatorIDTemp)
                            {
                                foreach (var item4 in db.Indicators.Where(b => b.ID == item3).Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Indicator1 }))
                                {
                                    temp.Add(item4);
                                }
                            }
                            item2.IndicatorBag = temp;
                        }
                        else
                        {
                            item2.IndicatorBag = db.Indicators.Select(g => new SelectListItem() { Value = g.ID.ToString(), Text = g.Indicator1 });
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
            //วน goals เพื่อ อับเดตหรือเพิ่มตัวใหม่
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

                //วน SOEPlansIndicator
                foreach (var goalindicator in SOEPlanIndicator)
                {
                    //ถ้าเป็นตัวใหม่ให้เพิ่ม
                    if (goalindicator.ID == 0)
                    {
                        if (goalindicator.IsDelete == false)
                        {
                            goalindicator.GoalID = goal.ID;
                            db.SOEPlanIndicator.Add(goalindicator);
                        }
                    }
                    //ถ้าเป็นตัวเก่าให้ track อัพเดตไว้
                    else
                    {
                        db.Entry(goalindicator).State = EntityState.Modified;
                    }
                }
            }
        }
        public ActionResult AddGoal(StrategicObjective strategic)
        {
            //สร้าง Goal และ Insert
            Goal goal = new Goal();
            goal.StrategicObjectiveID = strategic.ID;
            goal.Insert(db);
            strategic.Goals.Add(goal);

            UpdateDropdown(strategic);

            ModelState.Clear();
            return View("Manage", strategic);
        }
        public ActionResult DeleteGoal(StrategicObjective strategicObjective)
        {
            //ลบ Goal และไล่ลบ SOEPlan ด้วย
            foreach (var item in strategicObjective.Goals)
            {
                db.Entry(item).State = EntityState.Modified;
                if (item.IsDelete == true)
                {
                    foreach (var item2 in item.SOEPlanIndicator)
                    {
                        item2.IsDelete = true;
                        db.Entry(item2).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
            UpdateDropdown(strategicObjective);

            return View("Manage", strategicObjective);
        }
        public ActionResult AddIndicator(StrategicObjective strategic)
        {
            foreach (var goal in strategic.Goals)
            {
                if (goal.IsAddIndiacator)
                {
                    SOEPlanIndicator SOEPlanIndicator = new SOEPlanIndicator();
                    SOEPlanIndicator.GoalID = goal.ID;
                    SOEPlanIndicator.Insert(db);
                    goal.SOEPlanIndicator.Add(SOEPlanIndicator);

                    goal.IsAddIndiacator = false;
                }
            }

            UpdateDropdown(strategic);
            ModelState.Clear();
            return View("Manage", strategic);
        }
        public ActionResult DelIndicator(StrategicObjective strategic)
        {
            UpdateDropdown(strategic);
            ModelState.Clear();
            return View("Manage", strategic);
        }

        //private void IndicatorBag(StrategicObjective strategic)
        //{
        //    foreach (var item in strategic.Goals)
        //    {
        //        foreach (var item2 in item.SOEPlanIndicator)
        //        {
        //            if (item2.IndicatorBag == null) { item2.IndicatorBag = db.Indicators.Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Indicator1 }); }
        //            //else { item2.IndicatorBag = item2.IndicatorBag.Where(m=>m.Value != indicatorid)}
        //        }
        //    }
        //}
        //private void IndicatorUnitBag(StrategicObjective strategic)
        //{
        //    foreach (var item in strategic.Goals)
        //    {
        //        foreach (var item2 in item.SOEPlanIndicator)
        //        {
        //            if (item2.IndicatorBag != null)
        //            {
        //                item2.IndicatorUnitBag = db.IndicatorUnits.Where(b => b.IndicatorID == item2.IndicatorID).Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Unit });
        //            }
        //        }
        //    }
        //}
    }
}
