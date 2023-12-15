using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastPeriodMetadata
    {
    }
    [MetadataType(typeof(ForecastPeriodMetadata))]
    public partial class ForecastPeriod
    {
        public List<ForecastPeriodToolAndMethod> ToolAndMethods { get; set; }
        public bool IsSelect { get; set; }
    }
}