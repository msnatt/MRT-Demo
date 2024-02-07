using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Models
{
    public class SOEPlanIndicatorMetadata
    {
    }
    [MetadataType(typeof(SOEPlanIndicatorMetadata))]
    public partial class SOEPlanIndicator
    {
        public IEnumerable<SelectListItem> IndicatorBag { get; set; }
        public IEnumerable<SelectListItem> IndicatorUnitBag { get; set; }
        public List<ImportantIndicatorTargetMeasurement> SubTarget { get; set; }
        public bool IsChange { get; set; }

        public void Insert(MRTEntities db)
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;
            this.IsLastDelete = false;
            this.IsChange = true;
            var last = db.SOEPlanIndicator.ToList().LastOrDefault();
            if (last == null)
            {
                last = new SOEPlanIndicator();
                last.No = 0;
            }
            this.No = last.No + 1;
        }

    }
}