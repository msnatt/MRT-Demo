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
        public bool IsSelect { get; set; }
        public bool IsAddCompetitor { get; set; }
        public void Insert(MRTEntities db)
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsLastDelete = false;
            this.IsDelete = false;
        }
    }
}