﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ImportantIndicatorTargetMeasurementMetadata
    {
    }
    [MetadataType(typeof(ImportantIndicatorTargetMeasurement))]
    public partial class ImportantIndicatorTargetMeasurement
    {
        public int level { get; set; }
        
        public int GoalID { get; set; }
        public List<ImportantIndicatorTargetMeasurement> SubTarget { get; set;}
    
    }
}