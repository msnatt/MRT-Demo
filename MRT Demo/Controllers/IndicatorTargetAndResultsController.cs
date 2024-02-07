using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MRT_Demo.Models;

namespace MRT_Demo.Controllers
{
    public class IndicatorTargetAndResultsController : BaseController
    {
        public ActionResult Index(int? SelectedYear, string Division, string Indicator)
        {
            if (Division == "") { Division = null; }
            if (Indicator == "") { Indicator = null; }
            var indicators = db.Indicators.Where(s => s.IsDelete == false);
            foreach (var indicator in indicators) { if (SelectedYear != null) { indicator.indicatorYear = (int)SelectedYear; } };
            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }
            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Where(q => q.Division == Division).Count() > 0);
            }
            ViewBag.NowYearBag = SelectedYear;
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            return View(indicators.ToList());
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
        private void IndicatorBag(Indicator indicator)
        {
            foreach (var item2 in indicator.SOEPlanIndicator)
            {
                item2.IndicatorBag = db.Indicators.Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Indicator1 });
            }
        }
        private void IndicatorUnitBag(Indicator indicator)
        {
            foreach (var item2 in indicator.SOEPlanIndicator)
            {
                if (item2.IndicatorBag != null)
                {
                    item2.IndicatorUnitBag = db.IndicatorUnits.Where(b => b.IndicatorID == item2.IndicatorID).Select(i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Unit });
                }
            }
        }
        public void SetForecastTool(Indicator indicator)
        {
            foreach (var item in indicator.ImportantIndicatorResultMeasurement.First().ForecastPeriod)
            {
                foreach (var item2 in item.ForecastPeriodToolAndMethod)
                {
                    item2.ForecastTool = db.ForecastTool.Find(item2.ForecastToolID);
                }
            }

        }
        public void SetIndicatorImportant(Indicator indicator)
        {
            foreach (var item in indicator.ImportantIndicatorTargetMeasurement)
            {
                item.Indicator = indicator;
            }
            foreach (var item in indicator.ImportantIndicatorResultMeasurement)
            {
                item.Indicator = indicator;
                foreach (var item2 in item.ForecastPeriod)
                {
                    item2.ImportantIndicatorResultMeasurement.Indicator = indicator;

                }
            }
        }
        private void ChangeForecastPeriod(Indicator indicators, int? MQYID, int setvalue = 0)
        {
            var count = 0;
            var Result = indicators.ImportantIndicatorResultMeasurement.First();
            if (MQYID == 1) { setvalue = 0; }
            if (MQYID == 2) { setvalue = 12; }
            if (MQYID == 3) { setvalue = 16; }
            foreach (var item in Result.ForecastPeriod)
            {
                if (count == setvalue)
                {
                    item.IsSelect = true;
                }
                else
                {
                    item.IsSelect = false;
                }
                count++;
            }
        }
        private void SetTargetIndicator(int Year, Indicator indicator)
        {
            foreach (var item in indicator.SOEPlanIndicator)
            {
                item.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                foreach (var item2 in db.ImportantIndicatorTargetMeasurements.Where(b => b.IndicatorID == indicator.ID && b.Year == Year && b.IndicatorUnitID == item.IndicatorUnitID && b.IndicatorLevel == 0))
                {
                    item.SubTarget.Add(item2);
                }
            }
        }
        private void TargetMeasurementFilter(Indicator indicators, int SelectYear)
        {
            if (indicators.ImportantIndicatorTargetMeasurement.Count == 0)
            {
                var breaker = 0;
                foreach (var item in db.IndicatorXIndicatorTypes.Where(b => b.IndicatorID == indicators.ID && b.IsCheck == true))
                {
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
                        importantTemp.level = i;
                        importantTemp.Year = SelectYear;
                        importantTemp.CreateDate = DateTime.Now;
                        importantTemp.UpdateDate = DateTime.Now;
                        importantTemp.IndicatorLevel = i + 1;
                        importantTemp.SubTarget.Add(importantTemp);
                        importantTemp.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                        important.SubTarget.Add(importantTemp);
                    }
                    important.Indicator = indicators;
                    important.level = breaker;
                    indicators.ImportantIndicatorTargetMeasurement.Add(important);
                    breaker++;
                }
            }
            else
            {

                List<ImportantIndicatorTargetMeasurement> NewListImportant = indicators.ImportantIndicatorTargetMeasurement.ToList();
                List<ImportantIndicatorTargetMeasurement> DeleteListImportant = indicators.ImportantIndicatorTargetMeasurement.ToList();
                indicators.ImportantIndicatorTargetMeasurement.Clear();
                for (int i = 0; i < NewListImportant.Count / 5; i++)
                {
                    if (i >= indicators.IndicatorXIndicatorTypes.Count)
                    {
                        IndicatorXIndicatorType indicatorXIndicatorType = new IndicatorXIndicatorType();
                        indicatorXIndicatorType.CreateDate = DateTime.Now;
                        indicatorXIndicatorType.UpdateDate = DateTime.Now;
                        indicatorXIndicatorType.IsDelete = false;
                        indicatorXIndicatorType.IsLastDelete = false;
                        indicatorXIndicatorType.IndicatorID = indicators.ID;
                        indicatorXIndicatorType.level = indicators.IndicatorXIndicatorTypes.Count + 1;
                        indicators.IndicatorXIndicatorTypes.Add(indicatorXIndicatorType);
                    }

                    var breaker = 0;
                    var important = new ImportantIndicatorTargetMeasurement();
                    important.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                    var c = 0;
                    foreach (var itemList in NewListImportant) { itemList.level = c; c++; }
                    if (indicators.IndicatorXIndicatorTypes.ElementAt(i).level == 0)
                    {
                        foreach (var item2 in NewListImportant.Where(b => b.IndicatorTypeID == indicators.IndicatorXIndicatorTypes.ElementAt(i).IndicatorTypeID && b.level < 15))
                        {
                            item2.SubTarget = new List<ImportantIndicatorTargetMeasurement> { item2 };
                            if (breaker <= 4)
                            {
                                important.SubTarget.Add(item2);
                                DeleteListImportant.Remove(item2);
                                item2.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                            }
                            important.IndicatorUnitID = item2.IndicatorUnitID;
                            important.level = i;
                        }
                        indicators.ImportantIndicatorTargetMeasurement.Add(important);
                    }
                    else
                    {
                        foreach (var itemList in DeleteListImportant) { itemList.level = i + 1; }
                        foreach (var item2 in DeleteListImportant.Where(b => b.level == indicators.IndicatorXIndicatorTypes.ElementAt(i).level))
                        {
                            item2.SubTarget = new List<ImportantIndicatorTargetMeasurement> { item2 };
                            if (breaker <= 4)
                            {
                                important.SubTarget.Add(item2);
                                NewListImportant.Remove(item2);
                                item2.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                            }
                            important.IndicatorUnitID = item2.IndicatorUnitID;
                            important.level = i;
                            important.IndicatorTypeID = item2.IndicatorTypeID;
                            important.Indicator = indicators;

                        }
                        indicators.ImportantIndicatorTargetMeasurement.Add(important);
                    }

                }
            }

        }
        private void CheckUniqueSOEPlan(Indicator indicators)
        {
            List<int> ListTemp = new List<int>();
            List<SOEPlanIndicator> ListSOEPlan = new List<SOEPlanIndicator>();
            foreach (var item in indicators.SOEPlanIndicator) { ListTemp.Add((int)item.IndicatorUnitID); ListSOEPlan.Add(item); }
            ListTemp = ListTemp.Distinct().ToList();
            indicators.SOEPlanIndicator.Clear();
            foreach (var item in ListTemp)
            {
                foreach (var item2 in ListSOEPlan)
                {
                    if (item == item2.IndicatorUnitID)
                    {
                        indicators.SOEPlanIndicator.Add(item2);
                    }
                }
            }
        }
        [HttpGet]
        public ActionResult Target(int? id, int SelectedYear)
        {
            var indicators = db.Indicators.Find(id);

            var tempTargetList = indicators.ImportantIndicatorTargetMeasurement.Where(b => b.IsDelete != true).ToList();
            indicators.ImportantIndicatorTargetMeasurement.Clear();

            //กรองแยก IndicatorTargetMeasurement ออกจาก IndicatorTargetMeasurement ที่มาจาก Goal
            indicators.ImportantIndicatorTargetMeasurement = tempTargetList.Where(b => b.IndicatorLevel > 0 && b.Year == SelectedYear).ToList();
            indicators.ListTarget = tempTargetList.Where(b => b.Year == SelectedYear && b.IndicatorLevel == 0).ToList();

            //filter TargetMeasurement เพื่อแยกระหว่าง ตัวที่มาจาก Goal Indicator
            TargetMeasurementFilter(indicators, SelectedYear);


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

            indicators.indicatorYear = SelectedYear;
            IndicatorBag(indicators);
            IndicatorUnitBag(indicators);
            CheckUniqueSOEPlan(indicators);
            SetTargetIndicator(SelectedYear, indicators);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");

            return View("Target", indicators);
        }
        [HttpPost]
        public ActionResult Target(Indicator indicators)
        {
            var important = indicators.ImportantIndicatorTargetMeasurement.ToList();
            indicators.ImportantIndicatorTargetMeasurement = null;
            indicators.ImportantIndicatorResultMeasurement = null;
            indicators.IndicatorXIndicatorTypes = null;
            indicators.IndicatorOwners = null;
            db.Entry(indicators).State = EntityState.Modified;
            foreach (var item in important)
            {
                foreach (var item2 in item.SubTarget)
                {
                    if (item2.ID == 0)
                    {
                        //item2.IndicatorTypeID = item.IndicatorTypeID;
                        item2.IndicatorUnitID = item.IndicatorUnitID;
                        db.ImportantIndicatorTargetMeasurements.Add(item2);
                    }
                    else
                    {
                        item2.IndicatorUnitID = item.IndicatorUnitID;
                        db.Entry(item2).State = EntityState.Modified;
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { SelectedYear = indicators.indicatorYear });
        }
        public ActionResult AddIndicatorOwner(Indicator indicator)
        {
            VoidAddIndicatorOwner(indicator);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ModelState.Clear();
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
            indicatorXIndicatorType.IndicatorID = indicator.ID;
            indicatorXIndicatorType.level = indicator.IndicatorXIndicatorTypes.Count + 1;
            indicatorXIndicatorType.Insert(db);
            indicator.IndicatorXIndicatorTypes.Add(indicatorXIndicatorType);


            var breaker = 0;
            var important = new ImportantIndicatorTargetMeasurement();
            important.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
            for (var i = 0; i < 5; i++)
            {
                var importantTemp = new ImportantIndicatorTargetMeasurement();
                importantTemp.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                importantTemp.IsDelete = false;
                importantTemp.IsLastDelete = false;
                importantTemp.IndicatorID = indicator.ID;
                importantTemp.level = indicatorXIndicatorType.level;
                importantTemp.CreateDate = DateTime.Now;
                importantTemp.UpdateDate = DateTime.Now;
                importantTemp.IndicatorLevel = i + 1;
                importantTemp.SubTarget.Add(importantTemp);
                if (breaker <= 4)
                {
                    important.SubTarget.Add(importantTemp);
                    important.level = importantTemp.level;
                    importantTemp.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                }
            }
            important.level = indicatorXIndicatorType.level;
            indicator.ImportantIndicatorTargetMeasurement.Add(important);

            SetIndicatorImportant(indicator);

            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicator.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            return View("Target", indicator);
        }
        [HttpGet]
        public ActionResult Result(int id, int SelectedYear)
        {
            var indicators = db.Indicators.Find(id);

            var tempTargetList = indicators.ImportantIndicatorTargetMeasurement.Where(b => b.IsDelete != true).ToList();
            indicators.ImportantIndicatorTargetMeasurement.Clear();

            //กรองแยก IndicatorTargetMeasurement ออกจาก IndicatorTargetMeasurement ที่มาจาก Goal
            indicators.ImportantIndicatorTargetMeasurement = tempTargetList.Where(b => b.IndicatorLevel > 0 && b.Year == SelectedYear).ToList();
            indicators.ListTarget = tempTargetList.Where(b => b.Year == SelectedYear && b.IndicatorLevel == 0).ToList();

            //filter TargetMeasurement เพื่อแยกระหว่าง ตัวที่มาจาก Goal Indicator
            TargetMeasurementFilter(indicators, SelectedYear);
            //**************************************************************//


            //Result
            var TempResultList = indicators.ImportantIndicatorResultMeasurement.ToList();
            indicators.ImportantIndicatorResultMeasurement.Clear();
            indicators.ImportantIndicatorResultMeasurement = TempResultList.Where(b => b.Year == SelectedYear).ToList();

            if (indicators.ImportantIndicatorResultMeasurement.Count == 0)
            {
                ImportantIndicatorResultMeasurement importantIndicatorResult = new ImportantIndicatorResultMeasurement();
                importantIndicatorResult.IndicatorID = indicators.ID;
                importantIndicatorResult.Indicator = indicators;
                importantIndicatorResult.Year = SelectedYear;
                importantIndicatorResult.Insert(db);

                for (var i = 0; i < 18; i++)
                {
                    ForecastPeriod forecastPeriod = new ForecastPeriod();
                    forecastPeriod.ImportantIndicatorResultMeasurement = importantIndicatorResult;
                    forecastPeriod.ImportantIndicatorResultMeasurementID = importantIndicatorResult.ID;
                    forecastPeriod.Insert(db);

                    ForecastPeriodResultRemark resultRemark = new ForecastPeriodResultRemark();
                    resultRemark.ForecastPeriodID = forecastPeriod.ID;
                    resultRemark.Insert(db);
                    forecastPeriod.ForecastPeriodResultRemark.Add(resultRemark);

                    if (i == 0) { forecastPeriod.IsSelect = true; } else { forecastPeriod.IsSelect = false; }
                    foreach (var item in db.ForecastTool)
                    {
                        ForecastPeriodToolAndMethod toolAndMethod = new ForecastPeriodToolAndMethod();
                        toolAndMethod.ForecastToolID = item.ID;
                        toolAndMethod.ForecastPeriod = forecastPeriod;
                        toolAndMethod.ForecastPeriodID = forecastPeriod.ID;
                        toolAndMethod.ForecastTool = item;
                        toolAndMethod.Insert(db);
                        forecastPeriod.ForecastPeriodToolAndMethod.Add(toolAndMethod);
                    }
                    foreach (var item in indicators.IndicatorUnits)
                    {
                        ForecastValueAndRealValue realValue = new ForecastValueAndRealValue();
                        realValue.UnitsID = item.ID;
                        realValue.Insert(db);
                        forecastPeriod.ForecastValueAndRealValue.Add(realValue);
                    }
                    importantIndicatorResult.ForecastPeriod.Add(forecastPeriod);
                }
                indicators.ImportantIndicatorResultMeasurement.Add(importantIndicatorResult);
            }
            else
            {
                foreach (var item in indicators.ImportantIndicatorResultMeasurement.First().ForecastPeriod)
                {
                    item.ImportantIndicatorResultMeasurement = indicators.ImportantIndicatorResultMeasurement.First();
                    foreach (var item2 in item.ForecastPeriodToolAndMethod)
                    {
                        item2.ForecastPeriod = item;
                    }

                    for (int i = 0; i < indicators.IndicatorUnits.Count; i++)
                    {
                        item.ForecastValueAndRealValue.ElementAt(i).UnitsID = indicators.IndicatorUnits.ElementAt(i).ID;
                    }
                }
            }

            ChangeForecastPeriod(indicators, indicators.ImportantIndicatorResultMeasurement.First().PeriodMonthOrQuaterOrYearID);
            SetIndicatorImportant(indicators);
            SetTargetIndicator(SelectedYear, indicators);
            IndicatorBag(indicators);
            IndicatorUnitBag(indicators);
            indicators.indicatorYear = SelectedYear;
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
        [HttpPost]
        public ActionResult Result(Indicator indicators)
        {
            //ล้าง ForecastPeriod จาก Indicator.ImportantIndicatorResultMeasurement และนำไปเก็บใน PeriodTemp
            List<ForecastPeriod> PeriodTemp = indicators.ImportantIndicatorResultMeasurement.First().ForecastPeriod.ToList();
            indicators.ImportantIndicatorResultMeasurement.First().ForecastPeriod = null;

            //เช็ค ResultMeasurement ว่าเพิ่มเข้า Database หรือ อัพเดต
            if (indicators.ImportantIndicatorResultMeasurement.First().ID == 0) { db.ImportantIndicatorResultMeasurement.Add(indicators.ImportantIndicatorResultMeasurement.First()); } else { db.Entry(indicators.ImportantIndicatorResultMeasurement.First()).State = EntityState.Modified; }

            //PeriodTemp มาวนเช็ค ว่าเพิ่มเข้า Database หรือ อัพเดต
            foreach (var item in PeriodTemp)
            {
                var Compe = item.ForecastPeriodCompetitorValue;
                var Tool = item.ForecastPeriodToolAndMethod;
                var Remark = item.ForecastPeriodResultRemark;
                var Real = item.ForecastValueAndRealValue;
                item.ForecastPeriodCompetitorValue = null;
                item.ForecastPeriodToolAndMethod = null;
                item.ForecastPeriodResultRemark = null;
                item.ForecastValueAndRealValue = null;
                if (item.ID == 0)
                {
                    //เพิ่มลง Database
                    item.ImportantIndicatorResultMeasurement = null;
                    item.ImportantIndicatorResultMeasurementID = indicators.ImportantIndicatorResultMeasurement.First().ID;
                    db.ForecastPeriod.Add(item);
                }
                else
                {
                    //อัพเดต Database
                    item.ImportantIndicatorResultMeasurement = null;
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();

                //เช็ค Tool ว่าเพิ่มเข้า Database หรือ อัพเดต
                foreach (var ToolItem in Tool)
                {
                    if (ToolItem.ID == 0) { ToolItem.ForecastPeriodID = item.ID; db.ForecastPeriodToolAndMethod.Add(ToolItem); } else { db.Entry(ToolItem).State = EntityState.Modified; }
                }
                //เช็ค RealValue ว่าเพิ่มเข้า Database หรือ อัพเดต
                foreach (var ValueItem in Real)
                {
                    if (ValueItem.ID == 0) { ValueItem.ForecastPeriodID = item.ID; db.ForecastValueAndRealValue.Add(ValueItem); } else { db.Entry(ValueItem).State = EntityState.Modified; }
                }
                //เช็ค Competitor ว่าเพิ่มเข้า Database หรือ อัพเดต
                foreach (var CompeItem in Compe)
                {
                    if (CompeItem.ID == 0) { CompeItem.ForecastPeriodID = item.ID; db.ForecastPeriodCompetitorValue.Add(CompeItem); } else { db.Entry(CompeItem).State = EntityState.Modified; }
                }
                //เช็ค Remark ว่าเพิ่มเข้า Database หรือ อัพเดต
                foreach (var RemarkItem in Remark)
                {
                    if (RemarkItem.ID == 0) { RemarkItem.ForecastPeriodID = item.ID; db.ForecastPeriodResultRemark.Add(RemarkItem); } else { db.Entry(RemarkItem).State = EntityState.Modified; }
                }
                if (Remark.First().ListFileA.First() != null) { ActionSaveFile(Remark.First().ListFileA, Remark.First().ID, 1); }
                if (Remark.First().ListFileB.First() != null) { ActionSaveFile(Remark.First().ListFileB, Remark.First().ID, 2); }
                if (Remark.First().ListFileC.First() != null) { ActionSaveFile(Remark.First().ListFileC, Remark.First().ID, 3); }

            }
            db.SaveChanges();

            return RedirectToAction("Index", new { SelectedYear = indicators.indicatorYear });
        }
        private void ActionSaveFile(List<HttpPostedFileBase> listFile, int ID, int runnumber)
        {
            //วนเซฟไฟล์ทีละตัว
            foreach (var fileitem in listFile)
            {
                string FileName = DateTime.Now.ToString("HHmmss-ddMMyyyy") + "_" + Path.GetFileName(fileitem.FileName);
                string UploadPath = Path.Combine(Server.MapPath("~/UserFile"), FileName);
                if (Directory.Exists("~/UserFile")) { fileitem.SaveAs(UploadPath); } else { Directory.CreateDirectory(Server.MapPath("~/UserFile")); fileitem.SaveAs(UploadPath); }

                FilePath filePath = new FilePath();
                filePath.CreateDate = DateTime.Now;
                filePath.UpdateDate = DateTime.Now;
                filePath.IsDelete = false;
                filePath.IsLastDelete = false;
                filePath.NameFile = FileName;
                filePath.PathFile = "~/UserFile/";
                db.FilePath.Add(filePath);
                db.SaveChanges();

                if (runnumber == 1)
                {
                    ForecastPeriodDocFile docFile = new ForecastPeriodDocFile();
                    docFile.ForecastPeriodResultRemarkID = ID;
                    docFile.FilePathID = filePath.ID;
                    docFile.CreateDate = DateTime.Now;
                    docFile.UpdateDate = DateTime.Now;
                    docFile.IsDelete = false;
                    docFile.IsLastDelete = false;
                    db.ForecastPeriodDocFile.Add(docFile);

                }
                if (runnumber == 2)
                {
                    ForecastAnalysisResultsFile analysis = new ForecastAnalysisResultsFile();
                    analysis.ForecastPeriodResultRemarkID = ID;
                    analysis.FilePathID = filePath.ID;
                    analysis.CreateDate = DateTime.Now;
                    analysis.UpdateDate = DateTime.Now;
                    analysis.IsDelete = false;
                    analysis.IsLastDelete = false;
                    db.ForecastAnalysisResultsFile.Add(analysis);

                }
                if (runnumber == 3)
                {
                    ForecastChangeActionPlanFile changeAction = new ForecastChangeActionPlanFile();
                    changeAction.ForecastPeriodResultRemarkID = ID;
                    changeAction.FilePathID = filePath.ID;
                    changeAction.CreateDate = DateTime.Now;
                    changeAction.UpdateDate = DateTime.Now;
                    changeAction.IsDelete = false;
                    changeAction.IsLastDelete = false;
                    db.ForecastChangeActionPlanFile.Add(changeAction);

                }

            }
        }
        public ActionResult ChangePeriod(Indicator indicators)
        {
            ChangeForecastPeriod(indicators, indicators.ImportantIndicatorResultMeasurement.First().PeriodMonthOrQuaterOrYearID);
            SetForecastTool(indicators);
            SetIndicatorImportant(indicators);
            SetForecastPeriod(indicators);
            IndicatorBag(indicators);
            IndicatorUnitBag(indicators);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicators, int IndexInList)
        {
            ChangeForecastPeriod(indicators, null, IndexInList);
            SetForecastTool(indicators);
            SetIndicatorImportant(indicators);
            SetForecastPeriod(indicators);
            IndicatorBag(indicators);
            IndicatorUnitBag(indicators);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
        public ActionResult DelCompetitorValue(Indicator indicator)
        {
            foreach (var item in indicator.ImportantIndicatorResultMeasurement.First().ForecastPeriod)
            {
                foreach (var item2 in item.ForecastPeriodCompetitorValue)
                {
                    if (item2.IsDelete)
                    {
                        //item.ForecastPeriodCompetitorValue.Remove(item2);
                    }
                }
            }
            SetForecastTool(indicator);
            SetIndicatorImportant(indicator);
            SetForecastPeriod(indicator);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicator.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicator);
        }
        public ActionResult AddCompetitorValue(Indicator indicator)
        {
            foreach (var item in indicator.ImportantIndicatorResultMeasurement.First().ForecastPeriod)
            {
                if (item.IsAddCompetitor)
                {
                    ForecastPeriodCompetitorValue competitorValue = new ForecastPeriodCompetitorValue();
                    competitorValue.CreateDate = DateTime.Now;
                    competitorValue.UpdateDate = DateTime.Now;
                    competitorValue.IsDelete = false;
                    competitorValue.IsLastDelete = false;
                    competitorValue.ForecastPeriodID = item.ID;
                    competitorValue.ForecastPeriod = item;
                    item.ForecastPeriodCompetitorValue.Add(competitorValue);
                }

            }
            SetForecastTool(indicator);
            SetIndicatorImportant(indicator);
            SetForecastPeriod(indicator);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicator.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            ModelState.Clear();
            return View("Result", indicator);
        }
        public void SetForecastPeriod(Indicator indicators)
        {
            foreach (var item in indicators.ImportantIndicatorResultMeasurement.First().ForecastPeriod)
            {
                item.ImportantIndicatorResultMeasurement = indicators.ImportantIndicatorResultMeasurement.First();
                foreach (var item2 in item.ForecastPeriodToolAndMethod)
                {
                    item2.ForecastPeriod = item;
                }

            }
        }
    }
}
