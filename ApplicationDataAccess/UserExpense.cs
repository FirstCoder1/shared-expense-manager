//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ApplicationDataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserExpense
    {
        public int ExpenseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime ExpenseDate { get; set; }
        public int ExpenseTypeId { get; set; }
        public decimal ExpenseAmount { get; set; }
        public int ExpenseStatusId { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime LastUpdate { get; set; }
    
        public virtual ApplicationUserInformation ApplicationUserInformation { get; set; }
        public virtual ExpenseType ExpenseType { get; set; }
        public virtual Status Status { get; set; }
    }
}
