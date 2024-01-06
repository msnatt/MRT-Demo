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
        [DataType(DataType.MultilineText)]
        public string Indicator1 { get; set; }
        [DisplayName("กำหนดสูตรการคำนวน")]
        public string Formula { get; set; }
        [DisplayName("เก็บรหัสสถานะรายละเอียดตัวชี้วัด")]
        public Nullable<int> IndicatorDetailStatusID { get; set; }
        [DisplayName("สถานะ")]
        public Nullable<bool> IsActive { get; set; }

        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("ส่วนงานที่จัดทำรายงาน")]
        public virtual ICollection<IndicatorOwner> IndicatorOwners { get; set; }

        [DisplayName("สถานะตัวชี้วัด")]
        public virtual IndicatorDetailStatus IndicatorDetailStatus { get; set; }


    }
    [MetadataType(typeof(IndicatorMetadata))]
    public partial class Indicator
    {
        public string isActiveText
        {
            get { if (IsActive) { return "ใช้งาน"; } else { return "ไม่ใช้งาน"; } }
        }
        public int indicatorYear { get; set; }
        public string IndicatorOwnersText
        {
            get
            {
                var x = IndicatorOwners.ToList();
                int count = 1;
                string text = "";
                foreach (var item in x)
                {
                    if (count == 1)
                    {
                        text = item.Division;
                    }
                    else
                    {
                        text = text + " , " + item.Division;
                    }
                    count++;
                }
                return text;
            }
        }
    }
}