using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastPeriodToolAndMethodMetadata
    {
    }
    [MetadataType(typeof(ForecastPeriodToolAndMethodMetadata))]
    public partial class ForecastPeriodToolAndMethod
    {
        public string TempMethod { get; set; }

        public void Insert(MRTEntities db)
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsLastDelete = false;
            this.IsDelete = false;
            this.TempMethod = "";
        }
    }
}