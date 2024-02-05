using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MRT_Demo.Models
{
    public class IndicatorXIndicatorTypeMetadata
    {
        [DataType(DataType.MultilineText)]
        public string Definition { get; set; }

    }
    [MetadataType(typeof(IndicatorXIndicatorTypeMetadata))]
    public partial class IndicatorXIndicatorType
    {
        public int level {  get; set; }
        public void Insert(MRTEntities db, Indicator indicator, IndicatorType indicatorType)
        {
            this.IndicatorTypeID = indicatorType.ID;
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;
            this.IsLastDelete = false;
            this.IndicatorType = indicatorType;
            indicator.IndicatorXIndicatorTypes.Add(this);

        }
    }
}