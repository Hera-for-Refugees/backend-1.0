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
    
    public partial class Translation
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public int LanguageId { get; set; }
        public int RecordId { get; set; }
        public string Translation1 { get; set; }
    
        public virtual SLanguage SLanguage { get; set; }
        public virtual SType_Table SType_Table { get; set; }
    }
}