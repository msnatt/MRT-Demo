using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class IndicatorMetadata
    {

        [DisplayName("ตัวชี้วัด/เกณฑ์วัดการดำเนินงาน")]
        public string Indicator1 { get; set; }
        [DisplayName("กำหนดสูตรการคำนวน")]
        public string Formula { get; set; }
        [DisplayName("เก็บรหัสสถานะรายละเอียดตัวชี้วัด")]
        public Nullable<int> IndicatorDetailStatusID { get; set; }
        [DisplayName("สถานะตัวชี้วัด")]
        public Nullable<bool> IsActive { get; set; }

        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("ส่วนงานที่จัดการ")]
        public virtual ICollection<IndicatorOwner> IndicatorOwners { get; set; }

        [DisplayName("สถานะ")]
        public virtual IndicatorDetailStatus IndicatorDetailStatus { get; set; }


    }
    [MetadataType(typeof(IndicatorMetadata))]
    public partial class Indicator
    {

    }
}