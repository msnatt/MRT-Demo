using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class StategyMetadata
    {
        [DisplayName("รหัส")]
        public int No { get; set; }

        [DisplayName("ยุทศาสตร์ (Strategies)")]
        [Required]
        public string Stategy1 { get; set; }
        public Nullable<int> StrategicObjectiveID { get; set; }

        [DisplayName("กลยุทธ์ (Tactics)")]
        public virtual ICollection<Tactic> Tactics { get; set; }
        
        [DisplayName("วันที่สร้าง")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DisplayName("วันที่ปรับปรุง")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

    }
    [MetadataType(typeof(StategyMetadata))]
    public partial class Stategy
    {
        public bool isAddHere { get; set; }
        public void Insert(MRTEntities db)
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsDelete = false;
            this.IsLastDelete = false;
            var last = db.Stategies.ToList().LastOrDefault();
            if (last == null)
            {
                last = new Stategy();
                last.No = 0;
            }
            this.No = last.No + 1;

        }

    }

}