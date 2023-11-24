using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class GoalMetadata
    {
        [DisplayName("รหัส"),Required]
        public Nullable<int> Goal1 { get; set; }


    }
    [MetadataType(typeof(GoalMetadata))]
    public partial class Goal
    {
        public string CreateAndUpdate
        {
            get
            {
                var startdt = string.Format("{0:dd/MM/yyyy}", CreateDate);
                var enddt = string.Format("{0:dd/MM/yyyy}", UpdateDate);

                return startdt + " - " + enddt;
            }
        }
    }
}