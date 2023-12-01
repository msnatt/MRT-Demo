using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

    }
}