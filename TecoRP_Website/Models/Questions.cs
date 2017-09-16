//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TecoRP_Website.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Questions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Questions()
        {
            this.Answers = new HashSet<Answers>();
            this.QuestionToApplications = new HashSet<QuestionToApplications>();
        }
    
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public bool IsTextArea { get; set; }
        public string Selection_A { get; set; }
        public string Selection_B { get; set; }
        public string Selection_C { get; set; }
        public Nullable<byte> RightAnswer { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answers> Answers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuestionToApplications> QuestionToApplications { get; set; }
    }
}
