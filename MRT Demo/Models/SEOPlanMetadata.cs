using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class SEOPlanMetadata
    {
    }
    [MetadataType(typeof(SEOPlanMetadata))]
    public partial class SEOPlan
    {
        public string StartEndYear { get { return this.StartYear.ToString()+ " - " + this.EndYear.ToString(); } set { StartEndYear = value; } }
    }
}