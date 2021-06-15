using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedExpensePortalClientApplication.Models
{
    public class UserGroupTransaction
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int TransactionAmount { get; set; }
        public bool IsPayer { get; set; }
    }
}