using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class StrategicObjectiveMetadata
    {
        [DisplayName("รหัส")]
        public Nullable<int> No { get; set; }

        [DisplayName("วัตถุประสงค์เชิงยุทธศาสตร์ (Strategic Objectives)")]
        [Required]
        public string StrategicObjective1 { get; set; }

        [DisplayName("เป้าประสงค์ (Goals)")]
        public virtual ICollection<Goal> Goals { get; set; }

        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }


    }
    [MetadataType(typeof(StrategicObjectiveMetadata))]
    public partial class StrategicObjective
    {
        public string SearchText { get; set; }
    }
}