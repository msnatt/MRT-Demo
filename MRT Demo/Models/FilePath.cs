//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MRT_Demo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FilePath
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FilePath()
        {
            this.ForecastAnalysisResultsFile = new HashSet<ForecastAnalysisResultsFile>();
            this.ForecastChangeActionPlanFile = new HashSet<ForecastChangeActionPlanFile>();
            this.ForecastPeriodDocFile = new HashSet<ForecastPeriodDocFile>();
        }
    
        public int ID { get; set; }
        public string NameFile { get; set; }
        public string PathFile { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public Nullable<int> UpdateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsLastDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastAnalysisResultsFile> ForecastAnalysisResultsFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastChangeActionPlanFile> ForecastChangeActionPlanFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForecastPeriodDocFile> ForecastPeriodDocFile { get; set; }
    }
}