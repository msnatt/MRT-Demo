using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class TacticMetadata
    {
        [DisplayName("รหัส")]
        public Nullable<int> No { get; set; }
        [DisplayName("กลยุทธ์ (Tactics)"),Required]
        public string Tactic1 { get; set; }
        public Nullable<int> StategyID { get; set; }

        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

    }
    [MetadataType(typeof(TacticMetadata))]
    public partial class Tactic
    {
         
    }
}