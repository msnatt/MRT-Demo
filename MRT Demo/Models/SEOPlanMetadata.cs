using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class SEOPlanMetadata
    {
        [DisplayName("ระยะเวลาแผน")]
        public Nullable<int> StartYear { get; set; }

        [DisplayName("ระยะเวลาแผน")]
        public Nullable<int> EndYear { get; set; }

        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }



    }
    [MetadataType(typeof(SEOPlanMetadata))]
    public partial class SEOPlan
    {
        [DisplayName("ปี")]
        public string StartEndYear { get { return this.StartYear.ToString()+ " - " + this.EndYear.ToString(); } set { StartEndYear = value; } }
    }
}