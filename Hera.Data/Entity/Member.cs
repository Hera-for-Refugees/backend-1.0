//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hera.Data.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Member
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Member()
        {
            this.Member_Child = new HashSet<Member_Child>();
            this.Member_Child_Vaccinate = new HashSet<Member_Child_Vaccinate>();
            this.Member_Health_Record = new HashSet<Member_Health_Record>();
            this.Member_Notification_Log = new HashSet<Member_Notification_Log>();
            this.Member_Notification = new HashSet<Member_Notification>();
            this.Member_Pregnancy = new HashSet<Member_Pregnancy>();
            this.Member_Question_Answer = new HashSet<Member_Question_Answer>();
            this.CMS_Content_Detail_Log = new HashSet<CMS_Content_Detail_Log>();
        }
    
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public System.DateTime Birthdate { get; set; }
        public string Job { get; set; }
        public string Photo { get; set; }
        public string Address { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public bool IsApproved { get; set; }
        public System.Guid ApproveCode { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string Platform { get; set; }
        public string ForgotPassword { get; set; }
        public string NotificationUserId { get; set; }
        public string DeviceToken { get; set; }
        public string Language { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Child> Member_Child { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Child_Vaccinate> Member_Child_Vaccinate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Health_Record> Member_Health_Record { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Notification_Log> Member_Notification_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Notification> Member_Notification { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Pregnancy> Member_Pregnancy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member_Question_Answer> Member_Question_Answer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMS_Content_Detail_Log> CMS_Content_Detail_Log { get; set; }
    }
}