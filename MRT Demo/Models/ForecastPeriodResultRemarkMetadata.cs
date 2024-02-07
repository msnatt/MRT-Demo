using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRT_Demo.Models
{
    public class ForecastPeriodResultRemarkMetadata
    {
        public int ID { get; set; }
        public Nullable<int> ForecastPeriodID { get; set; }
        public string HelpImproveIndicator { get; set; }
        public string ProblemAndCorrection { get; set; }
        public string ImprotantFactorsAndEvents { get; set; }
        [DisplayName("5. ในกรณีที่ทีการเปลี่ยนแปลงเครื่่องมือระหว่างปีโปรดระบุเหตุผล/สาเหตุการเปลี่ยนแปลงด้วย เช่น พบว่าค่าคาดการณ์มีความคลาดเคลื่อนหรือไม่แม่นยำ เป็นต้น")]
        public string ReasonForToolChange { get; set; }
        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดการความเสี่ยง ณ งวด")]
        public bool IsAnalysisResults { get; set; }
        [DisplayName("ผลการวิเคราะห์-ประเมิน-จัดการความเสี่ยง ณ งวด")]
        public string AnalysisResults { get; set; }
        [DisplayName("ทบทวน/ปรับปรุงและหรือจัดทำเพิ่มแผนปฎิบัติการ")]
        public bool IsChangeActionPlan { get; set; }
        [DisplayName("ทบทวน/ปรับปรุงและหรือจัดทำเพิ่มแผนปฎิบัติการ")]
        public string ChangeActionPlan { get; set; }
        [DisplayName("เร่งรัด/ปรับปรุงการดำเนินงาน")]
        public bool IsChangeOperation { get; set; }
        [DisplayName("เร่งรัด/ปรับปรุงการดำเนินงาน")]
        public string ChangeOperation { get; set; }
        [DisplayName("อื่นๆ โปรดระบุ")]
        public bool IsOther { get; set; }
        [DisplayName("อื่นๆ โปรดระบุ")]
        public string Other { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsLastDelete { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastAnalysisResultsFile> ForecastAnalysisResultsFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastChangeActionPlanFile> ForecastChangeActionPlanFile { get; set; }
        public virtual ForecastPeriod ForecastPeriod { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodDocFile> ForecastPeriodDocFile { get; set; }
    }
    [MetadataType(typeof(ForecastPeriodResultRemarkMetadata))]
    public partial class ForecastPeriodResultRemark
    {
        public List<HttpPostedFileBase> ListFileA { get; set; }
        public List<HttpPostedFileBase> ListFileB { get; set; }
        public List<HttpPostedFileBase> ListFileC { get; set; }
        public void Insert(MRTEntities db)
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.IsLastDelete = false;
            this.IsDelete = false;
        }

    }
}