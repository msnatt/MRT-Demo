using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            foreach (var indicator in indicators) { indicator.indicatorYear = Year; };
            if (Indicator != null)
            {
                indicators = indicators.Where(s => s.Indicator1.Contains(Indicator));
            }
            if (Division != null)
            {
                indicators = indicators.Where(s => s.IndicatorOwners.Where(q => q.Division == Division).Count() > 0);
            }
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
        private void ChangeForecastPeriod(Indicator indicators,int? MQYID)
        {
            var count = 0;
            var setvalue = 0;
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

        // ======================== Target ================================
        public ActionResult Target(int id, string Division)
        {
            var indicators = db.Indicators.Find(id);
            if (indicators.ImportantIndicatorTargetMeasurement.Count == 0)
            {
                foreach (var item in db.IndicatorXIndicatorTypes.Where(b => b.IndicatorID == indicators.ID))
                {
                    var breaker = 0;
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
                        importantTemp.level = 0;
                        importantTemp.CreateDate = DateTime.Now;
                        importantTemp.UpdateDate = DateTime.Now;
                        importantTemp.IndicatorLevel = i + 1;
                        importantTemp.SubTarget.Add(importantTemp);
                        if (breaker <= 4)
                        {
                            important.SubTarget.Add(importantTemp);
                            importantTemp.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                        }
                    }
                    important.Indicator = indicators;
                    important.level = 0;
                    indicators.ImportantIndicatorTargetMeasurement.Add(important);
                }
            }
            else
            {

                List<ImportantIndicatorTargetMeasurement> NewListImportant = indicators.ImportantIndicatorTargetMeasurement.ToList();
                List<ImportantIndicatorTargetMeasurement> DeleteListImportant = indicators.ImportantIndicatorTargetMeasurement.ToList();
                indicators.ImportantIndicatorTargetMeasurement.Clear();


                //foreach (var item in indicators.IndicatorXIndicatorTypes)
                //{
                //    var breaker = 0;
                //    var important = new ImportantIndicatorTargetMeasurement();
                //    important.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                //    foreach (var item2 in NewListImportant.Where(b => b.IndicatorTypeID == item.IndicatorTypeID))
                //    {
                //        item2.SubTarget = new List<ImportantIndicatorTargetMeasurement> { item2 };
                //        if (breaker <= 4)
                //        {
                //            important.SubTarget.Add(item2);
                //            item2.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                //            DeleteListImportant.Remove(item2);
                //        }
                //        important.IndicatorUnitID = item2.IndicatorUnitID;
                //    }
                //    indicators.ImportantIndicatorTargetMeasurement.Add(important);
                //}

                //NewListImportant = DeleteListImportant;

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
                    if (indicators.IndicatorXIndicatorTypes.ElementAt(i).level == 0)
                    {
                        foreach (var item2 in NewListImportant.Where(b => b.IndicatorTypeID == indicators.IndicatorXIndicatorTypes.ElementAt(i).IndicatorTypeID))
                        {
                            item2.SubTarget = new List<ImportantIndicatorTargetMeasurement> { item2 };
                            if (breaker <= 4)
                            {
                                important.SubTarget.Add(item2);
                                DeleteListImportant.Remove(item2);
                                item2.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                            }
                            important.IndicatorUnitID = item2.IndicatorUnitID;
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
                            important.level = item2.level;
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

            var a = indicators.IndicatorUnits;
            ViewBag.Year = indicators.indicatorYear;
            ViewBag.Division = Division;
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
        public ActionResult Result(int id)
        {
            var indicators = db.Indicators.Find(id);
            List<ImportantIndicatorTargetMeasurement> NewListImportant = indicators.ImportantIndicatorTargetMeasurement.ToList();
            List<ImportantIndicatorTargetMeasurement> DeleteListImportant = indicators.ImportantIndicatorTargetMeasurement.ToList();
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
                if (indicators.IndicatorXIndicatorTypes.ElementAt(i).level == 0)
                {
                    foreach (var item2 in NewListImportant.Where(b => b.IndicatorTypeID == indicators.IndicatorXIndicatorTypes.ElementAt(i).IndicatorTypeID))
                    {
                        item2.SubTarget = new List<ImportantIndicatorTargetMeasurement> { item2 };
                        if (breaker <= 4)
                        {
                            important.SubTarget.Add(item2);
                            DeleteListImportant.Remove(item2);
                            item2.SubTarget = new List<ImportantIndicatorTargetMeasurement>();
                        }
                        important.IndicatorUnitID = item2.IndicatorUnitID;
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
                        important.level = item2.level;
                        important.Indicator = indicators;
                        indicators.ImportantIndicatorTargetMeasurement.Add(important);
                    }
                }

            }

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
                forecastPeriod.ToolAndMethods = new List<ForecastPeriodToolAndMethod>();
                if (i == 0) { forecastPeriod.IsSelect = true; } else { forecastPeriod.IsSelect = false; }
                foreach (var item in db.ForecastTool)
                {
                    ForecastPeriodToolAndMethod toolAndMethod = new ForecastPeriodToolAndMethod();
                    toolAndMethod.CreateDate = DateTime.Now;
                    toolAndMethod.UpdateDate = DateTime.Now;
                    toolAndMethod.IsLastDelete = false;
                    toolAndMethod.IsDelete = false;
                    toolAndMethod.ForecastToolID = item.ID;
                    toolAndMethod.ForecastPeriodID = forecastPeriod.ID;
                    toolAndMethod.ForecastTool = item;
                    forecastPeriod.ForecastPeriodToolAndMethod.Add(toolAndMethod);
                    forecastPeriod.ToolAndMethods.Add(toolAndMethod);
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

            SetIndicatorImportant(indicators);
            ViewBag.Year = indicators.indicatorYear;
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
        public ActionResult ChangePeriod(Indicator indicators)
        {
            ChangeForecastPeriod(indicators,indicators.ImportantIndicatorResultMeasurement.First().PeriodMonthOrQuaterOrYearID);
            SetForecastTool(indicators);
            SetIndicatorImportant(indicators);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicators)
        {
            SetForecastTool(indicators);
            SetIndicatorImportant(indicators);
            ViewBag.YearBag = IndicatorYearSelectItem();
            ViewBag.DivisionBag = IndicatorOwnerSelectItem();
            ViewBag.Xunit = new SelectList(indicators.IndicatorUnits.Select(i => new SelectListItem { Value = i.ID.ToString(), Text = i.Unit }).ToList(), "Value", "Text");
            ViewBag.MQY = db.PeriodMonthOrQuaterOrYear.ToList();
            return View("Result", indicators);
        }
    }
}
