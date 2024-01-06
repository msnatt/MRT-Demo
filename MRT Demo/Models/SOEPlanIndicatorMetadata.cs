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

    }
}