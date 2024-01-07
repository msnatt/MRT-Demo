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
    public class IndicatorTargetAndResultsController : Controller
    {
        private MRTEntities db = new MRTEntities();
        public ActionResult Index(int? Year, string Division, string Indicator)
        {
            if (Division == "") { Division = null; }
            if (Indicator == "") { Indicator = null; }
            var indicators = db.Indicators.Where(s => s.IsDelete == false);
            foreach (var indicator in indicators) { if (Year != null) { indicator.indicatorYear = (int)Year; } };
            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }
            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Where(q => q.Division == Division).Count() > 0);
            }
            ViewBag.NowYearBag = Year;
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
        private void ChangeForecastPeriod(Indicator indicators, int? MQYID, int? setvalue)
        {
            var count = 0;
            if (setvalue == null) { setvalue = 0; }
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
                var temp = db.ImportantIndicatorTargetMeasurements.Where(b => b.IndicatorID == indicator.ID && b.Year == Year);
                foreach (var item2 in temp) { item.SubTarget.Add(item2); }
            }
        }

        // ======================== Target ================================
        [HttpGet]
        public ActionResult Target(int? id, int SelectedYear)
        {
            var indicators = db.Indicators.Find(id);
            if (indicators.ImportantIndicatorTargetMeasurement.Count == 0)
            {
                var breaker = 0;
                foreach (var item in db.IndicatorXIndicatorTypes.Where(b => b.IndicatorID == indicators.ID))
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

                List<ImportantIndicatorTargetMeasurement> NewListImportant = indicators.ImportantIndicatorTargetMeasurement.Where(b => b.IndicatorLevel > 0).ToList();
                List<ImportantIndicatorTargetMeasurement> DeleteListImportant = indicators.ImportantIndicatorTargetMeasurement.Where(b => b.IndicatorLevel > 0).ToList();
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
                            indicators.ImportantIndicatorTargetMeasurement.Add(important);
                        }
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

                            indicators.ImportantIndicatorTargetMeasurement.Add(important);
                        }
                    }

                }
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

            var a = indicators.SOEPlanIndicator;
            indicators.indicatorYear = SelectedYear;
            SetTargetIndicator(SelectedYear, indicators);
            IndicatorBag(indicators);
            IndicatorUnitBag(indicators);
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
            return RedirectToAction("Index", new { id = indicators.indicatorYear });
        }
        // ---------------------------- Owner -----------------------------
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
        // ---------------------------- Type ------------------------------
        public ActionResult AddXType(Indicator indicator)
        {
            IndicatorXIndicatorType indicatorXIndicatorType = new IndicatorXIndicatorType();
            indicatorXIndicatorType.CreateDate = DateTime.Now;
            indicatorXIndicatorType.UpdateDate = DateTime.Now;
            indicatorXIndicatorType.IsDelete = false;
            indicatorXIndicatorType.IsLastDelete = false;
            indicatorXIndicatorType.IndicatorID = indicator.ID;
            indicatorXIndicatorType.level = indicator.IndicatorXIndicatorTypes.Count + 1;
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
        // ========================= Result ================================
        [HttpGet]
        public ActionResult Result(int id, int SelectedYear)
        {
            var indicators = db.Indicators.Find(id);
            List<ImportantIndicatorTargetMeasurement> NewListImportant = indicators.ImportantIndicatorTargetMeasurement.Where(b => b.IndicatorLevel > 0).ToList();
            List<ImportantIndicatorTargetMeasurement> DeleteListImportant = indicators.ImportantIndicatorTargetMeasurement.Where(b => b.IndicatorLevel > 0).ToList();
            indicators.ImportantIndicatorTargetMeasurement.Clear();

            for (int i = indicators.ImportantIndicatorTargetMeasurement.Count; i < NewListImportant.Count / 5; i++)
            {
                if (i < indicators.IndicatorXIndicatorTypes.Count)
                {
                    //var Xtype = indicators.IndicatorXIndicatorTypes.ElementAt(i);
                }
                else
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
                        indicators.ImportantIndicatorTargetMeasurement.Add(important);
                    }
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

                        indicators.ImportantIndicatorTargetMeasurement.Add(important);
                    }
                }
            }

            if (indicators.ImportantIndicatorResultMeasurement.Count == 0)
            {
                ImportantIndicatorResultMeasurement importantIndicatorResult = new ImportantIndicatorResultMeasurement();
                importantIndicatorResult.IndicatorID = indicators.ID;
                importantIndicatorResult.CreateDate = DateTime.Now;
                importantIndicatorResult.UpdateDate = DateTime.Now;
                importantIndicatorResult.Isdelete = false;
                importantIndicatorResult.IsLastDelete = false;
                importantIndicatorResult.Indicator = indicators;
                importantIndicatorResult.PeriodMonthOrQuaterOrYearID = 1;
                for (var i = 0; i < 18; i++)
                {
                    ForecastPeriod forecastPeriod = new ForecastPeriod();
                    forecastPeriod.CreateDate = DateTime.Now;
                    forecastPeriod.UpdateDate = DateTime.Now;
                    forecastPeriod.IsLastDelete = false;
                    forecastPeriod.IsDelete = false;
                    forecastPeriod.ImportantIndicatorResultMeasurement = importantIndicatorResult;
                    forecastPeriod.ImportantIndicatorResultMeasurementID = importantIndicatorResult.ID;
                    ForecastPeriodResultRemark resultRemark = new ForecastPeriodResultRemark();
                    resultRemark.ForecastPeriodID = forecastPeriod.ID;
                    resultRemark.CreateDate = DateTime.Now;
                    resultRemark.UpdateDate = DateTime.Now;
                    resultRemark.IsLastDelete = false;
                    resultRemark.IsDelete = false;
                    forecastPeriod.ForecastPeriodResultRemark.Add(resultRemark);
                    if (i == 0) { forecastPeriod.IsSelect = true; } else { forecastPeriod.IsSelect = false; }
                    foreach (var item in db.ForecastTool)
                    {
                        ForecastPeriodToolAndMethod toolAndMethod = new ForecastPeriodToolAndMethod();
                        toolAndMethod.CreateDate = DateTime.Now;
                        toolAndMethod.UpdateDate = DateTime.Now;
                        toolAndMethod.IsLastDelete = false;
                        toolAndMethod.IsDelete = false;
                        toolAndMethod.ForecastToolID = item.ID;
                        toolAndMethod.ForecastPeriod = forecastPeriod;
                        toolAndMethod.ForecastPeriodID = forecastPeriod.ID;
                        toolAndMethod.ForecastTool = item;
                        toolAndMethod.TempMethod = "";
                        forecastPeriod.ForecastPeriodToolAndMethod.Add(toolAndMethod);
                    }
                    foreach (var item in indicators.IndicatorUnits)
                    {
                        ForecastValueAndRealValue realValue = new ForecastValueAndRealValue();
                        realValue.CreateDate = DateTime.Now;
                        realValue.UpdateDate = DateTime.Now;
                        realValue.IsLastDelete = false;
                        realValue.IsDelete = false;
                        realValue.UnitsID = item.ID;
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

            ChangeForecastPeriod(indicators, indicators.ImportantIndicatorResultMeasurement.First().PeriodMonthOrQuaterOrYearID, null);
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
            var PeriodTemp = indicators.ImportantIndicatorResultMeasurement.First().ForecastPeriod;
            indicators.ImportantIndicatorResultMeasurement.First().ForecastPeriod = null;

            if (indicators.ImportantIndicatorResultMeasurement.First().ID == 0) { db.ImportantIndicatorResultMeasurement.Add(indicators.ImportantIndicatorResultMeasurement.First()); } else { db.Entry(indicators.ImportantIndicatorResultMeasurement.First()).State = EntityState.Modified; }
            foreach (var item in PeriodTemp)
            {
                var c = item.ForecastPeriodCompetitorValue;
                var a = item.ForecastPeriodToolAndMethod;
                var d = item.ForecastPeriodResultRemark;
                var b = item.ForecastValueAndRealValue;
                item.ForecastPeriodCompetitorValue = null;
                item.ForecastPeriodToolAndMethod = null;
                item.ForecastPeriodResultRemark = null;
                item.ForecastValueAndRealValue = null;
                if (item.ID == 0)
                {
                    item.ImportantIndicatorResultMeasurement = null;
                    item.ImportantIndicatorResultMeasurementID = indicators.ImportantIndicatorResultMeasurement.First().ID;
                    db.ForecastPeriod.Add(item);
                }
                else
                {
                    item.ImportantIndicatorResultMeasurement = null;
                    db.Entry(item).State = EntityState.Modified;
                }

                db.SaveChanges();
                foreach (var ToolItem in a)
                {
                    if (ToolItem.ID == 0) { ToolItem.ForecastPeriodID = item.ID; db.ForecastPeriodToolAndMethod.Add(ToolItem); } else { db.Entry(ToolItem).State = EntityState.Modified; }
                }
                foreach (var ValueItem in b)
                {
                    if (ValueItem.ID == 0) { ValueItem.ForecastPeriodID = item.ID; db.ForecastValueAndRealValue.Add(ValueItem); } else { db.Entry(ValueItem).State = EntityState.Modified; }
                }
                foreach (var CompeItem in c)
                {
                    if (CompeItem.ID == 0) { CompeItem.ForecastPeriodID = item.ID; db.ForecastPeriodCompetitorValue.Add(CompeItem); } else { db.Entry(CompeItem).State = EntityState.Modified; }
                }
                foreach (var Remarkitem in d)
                {
                    if (Remarkitem.ID == 0) { Remarkitem.ForecastPeriodID = item.ID; db.ForecastPeriodResultRemark.Add(Remarkitem); } else { db.Entry(Remarkitem).State = EntityState.Modified; }
                }
                if (d.First().ListFileA.First() != null) { ActionSaveFile(d.First().ListFileA, d.First().ID, 1); }
                if (d.First().ListFileB.First() != null) { ActionSaveFile(d.First().ListFileB, d.First().ID, 2); }
                if (d.First().ListFileC.First() != null) { ActionSaveFile(d.First().ListFileC, d.First().ID, 3); }

            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        private void ActionSaveFile(List<HttpPostedFileBase> listFile, int ID, int runnumber)
        {
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
            ChangeForecastPeriod(indicators, indicators.ImportantIndicatorResultMeasurement.First().PeriodMonthOrQuaterOrYearID, null);
            SetForecastTool(indicators);
            SetIndicatorImportant(indicators);
            SetForecastPeriod(indicators);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicators, int? IndexInList)
        {
            ChangeForecastPeriod(indicators, null, IndexInList);
            SetForecastTool(indicators);
            SetIndicatorImportant(indicators);
            SetForecastPeriod(indicators);
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
