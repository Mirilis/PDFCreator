//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PDFCreator.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DataPoint
    {
        public int ID { get; set; }
        public System.DateTime ApprovedDate { get; set; }
        public int ApproverID { get; set; }
        public string Key { get; set; }
        public Nullable<int> ProcessCardID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    
        public virtual Approver Approver { get; set; }
        public virtual ProcessCard ProcessCard { get; set; }
    }
}
