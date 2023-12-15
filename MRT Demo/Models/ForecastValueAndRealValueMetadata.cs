using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastValueAndRealValueMetadata
    {
    }
    [MetadataType(typeof(ForecastValueAndRealValueMetadata))]
    public partial class ForecastValueAndRealValue
    {
        public int UnitsID { get; set; }
    }
}