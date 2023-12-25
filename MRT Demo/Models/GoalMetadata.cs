using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Models
{
    public class GoalMetadata
    {
        [DisplayName("รหัส")]
        public Nullable<int> No { get; set; }

        [DisplayName("เป้าประสงค์")]
        [Required]
        public Nullable<int> Goal1 { get; set; }
        public Nullable<int> StrategicObjectiveID { get; set; }

        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }



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
        public bool IsAddIndiacator { get; set; }
    }
}