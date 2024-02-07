using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ImportantIndicatorResultMeasurementMetadata
    {
    }
    [MetadataType(typeof(ImportantIndicatorResultMeasurementMetadata))]
    public partial class ImportantIndicatorResultMeasurement
    {
        public void Insert(MRTEntities db)
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.Isdelete = false;
            this.IsLastDelete = false;
            this.PeriodMonthOrQuaterOrYearID = 1;
        }
    }
}